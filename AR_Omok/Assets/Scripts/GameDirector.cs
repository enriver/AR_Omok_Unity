using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class GameDirector : MonoBehaviour
{
    public GameObject TimerText;
    public GameObject TurnText;

    float TurnTime;
    int TimerSound;
    int OmokSound;
    int OmokVibe;
    
    GameObject Indicator;
    AudioSource audioSource;

    public GameObject UnderFive;

    Omok OmokGame;
    private ARRaycastManager raycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField] private Camera arCamera;


    bool isBoardSet; // 오목판 생성 여부
    bool isGameStart; // 게임 시작 여부
    bool setStone; // 오목알 착수 여부
    bool isEnd; // 게임이 끝났는지 여부
    bool isForbidden; // 금수여부
    bool isUnderFive; // 5초 이내 인가

    public GameObject BlackStone;
    public GameObject WhiteStone;

    List<string> tagNames = new List<string>();

    public GameObject panel;
    
    // Start is called before the first frame update
    void Start()
    {
        this.Indicator = GameObject.Find("PlaceIndicator");
        this.TimerText = GameObject.Find("TimerText");
        this.TurnText = GameObject.Find("TurnText");
        
        this.audioSource = GetComponent<AudioSource>();
        this.UnderFive = GameObject.Find("UnderFive");

        this.TurnTime = PlayerPrefs.GetFloat("TurnTime");
        this.TimerSound = PlayerPrefs.GetInt("TimerSound");
        this.OmokSound = PlayerPrefs.GetInt("OmokSound");
        this.OmokVibe = PlayerPrefs.GetInt("OmokVibe");

        this.isBoardSet = false;
        this.isGameStart = false;
        this.setStone = false;
        this.isEnd = false;
        this.isForbidden = false;
        this.isUnderFive = false;

        this.raycastManager = FindObjectOfType<ARRaycastManager>();

        this.panel = GameObject.Find("Panel");
        this.panel.SetActive(false);
    }

    // 화면 떨림 방지
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }

        if (!this.isEnd)
        {
            // 오목판을 생성했는지 체크
            if (isBoardSet)
            {
                if (!isGameStart)
                {
                    OmokGame = new Omok();
                    //OmokGame = this.OmokGameObj.GetComponent<OmokGameController>().OmokGame;
                    isGameStart = true;
                }
                else
                {
                    if (this.setStone) // 오목알을 착수 하였는가
                    {
                        if(this.TurnTime >= 0.0f)
                        {
                            this.TurnTime -= Time.deltaTime;
                            this.TimerText.GetComponent<Text>().text = this.TurnTime.ToString("F1");

                            if (this.TimerSound==1)
                            {
                                if (this.isUnderFive == false && this.TurnTime <= 5.0f)
                                {
                                    this.isUnderFive = true;
                                    this.UnderFive.GetComponent<AudioSource>().Play();

                                }
                            }   
                        }
                        else
                        {
                            this.panel.SetActive(true);
                            this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "";
                            this.TimerText.GetComponent<Text>().text = "";
                            this.TurnText.GetComponent<Text>().text = "";
                            
                            

                            // CurrentTurn 을 받아서 승패 여부 확인
                            if (OmokGame.getCurrentTurn() == 0) this.panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "백돌 승리입니다";
                            else this.panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "흑돌 승리입니다";
                        }
                    }

                    if (Input.touchCount == 0) return; // 프레임 진행동안 터치가 없으면 return

                    Touch touch = Input.GetTouch(0);

                    if(touch.phase == TouchPhase.Ended) // 손가락을 떼는 이벤트 발생시
                    {
                        Ray ray;
                        RaycastHit hitObj;

                        ray = arCamera.ScreenPointToRay(touch.position); // 손가락 포지션으로 부터 화면으로 raycast

                        if(Physics.Raycast(ray,out hitObj)) // Object 충돌시
                        {
                            if (hitObj.collider == null || hitObj.collider.tag == "Untagged") // collider 와 충돌 하였는가
                            {
                                Debug.Log("충돌한 Collider 가 없습니다");
                                return;
                            }
                            else
                            {
                                string OmokIndex=hitObj.collider.tag;

                                if (isContain(OmokIndex)) return;
                                Debug.Log("충돌 TAG : "+OmokIndex);

                                if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes)) // 굳이 arRaycastManager 가 필요한가
                                {
                                    int x = int.Parse(OmokIndex[5].ToString());
                                    int y = int.Parse(OmokIndex[6].ToString());
                                    //Debug.Log("X :" + x + ", Y:" + y);

                                    if (OmokGame.board[x, y] == 0) // 빈 인덱스인가
                                    {
                                        int turn = OmokGame.getCurrentTurn();

          
                                        if(turn == 1)
                                        {
                                            if (OmokGame.isForbidden(x, y, turn))
                                            {
                                                
                                                this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "금수입니다";
                                                this.isForbidden = true;
                                            }
                                        }

                                        if (!this.isForbidden)
                                        {
                                            this.setStone = false;
                                            PlaceStone(hitObj.collider.transform.position, hitObj.collider.transform.rotation, turn);
                                            tagNames.Add(OmokIndex);
                                            //PlaceStone(hits[0].pose, turn);
                                            OmokGame.setStone(x, y, turn);


                                            // 오목인가
                                            if (OmokGame.isGameOver(x, y, turn))
                                            {
                                                this.isEnd = true;
                                                this.panel.SetActive(true);
                                                
                                                this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "";
                                                this.TimerText.GetComponent<Text>().text = "";
                                                this.TurnText.GetComponent<Text>().text = "";

                                                if (turn == 1)
                                                {

                                                    this.panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "흑돌 승리입니다";
                                                }

                                                else
                                                {
                                                    this.panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "백돌 승리입니다";
                                                }

                                                return;
                                            }
                                            else
                                            {
                                                OmokGame.setCurrentTurn(turn);
                                            }
                                            //OmokGame.showBoard();
                                        }
                                        else
                                        {
                                            this.isForbidden = false;
                                        }
                                    }
                                    else
                                    {
                                        return;
                                    }
                                
                                }
                            }
                        }
                    }
                

                }
            }
            else
            {
                isBoardSet = this.Indicator.GetComponent<IndicatorScripts>().isSet;
            }
        }

    }

    void setTimer()
    {
        this.TurnTime = PlayerPrefs.GetFloat("TurnTime");
    }

    /*
    void PlaceStone(Pose hitPose, int currentTurn)
    {
        hitPose.position.y = Mathf.Round(hitPose.position.y * 1000) * 0.001f;

        if (currentTurn == 1)
        {
            Instantiate(BlackStone, hitPose.position, hitPose.rotation);
            this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "백돌 차례입니다";
        }
        else
        {
            Instantiate(WhiteStone, hitPose.position, hitPose.rotation);
            this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "흑돌 차례입니다";
        }
        Debug.Log("오목알 Position : (" + hitPose.position.x + "," + hitPose.position.y + "," + hitPose.position.z + ")");
        setTimer();
        this.setStone = false;
    }
    */

    void PlaceStone(Vector3 hitPosition, Quaternion hitRotation, int currentTurn)
    {
        hitPosition.y = Mathf.Round(hitPosition.y * 1000) * 0.001f;

        if (OmokSound == 1) audioSource.Play();

        if (currentTurn == 1)
        { 
            Instantiate(BlackStone, hitPosition, hitRotation);
            //this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "○ 백돌 차례 입니다";
            this.TurnText.GetComponent<Text>().text = "○ 백돌 차례";

        }
        else
        {
            Instantiate(WhiteStone, hitPosition, hitRotation);
            //this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "● 흑돌 차례 입니다";
            this.TurnText.GetComponent<Text>().text = "● 흑돌 차례";
        }
        
        this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "제한시간 안에 착수하여 주세요";
        this.isUnderFive = false;
        this.UnderFive.GetComponent<AudioSource>().Stop();
        // if (OmokVibe == 1) Handheld.Vibrate();

        Debug.Log("오목알 Position : (" + hitPosition.x + "," + hitPosition.y + "," + hitPosition.z + ")");
        setTimer();
        this.setStone = true;
    }

    bool isContain(string tagName)
    {
        foreach(string tag in tagNames)
        {
            if (tag == tagName) return true;
        }
        return false;
    }

    public void moveMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
