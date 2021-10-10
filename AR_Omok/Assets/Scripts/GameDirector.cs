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
    float TurnTime;
    int OmokSound;
    int OmokVibe;
    
    GameObject Indicator;
    GameObject OmokGameObj;
    AudioSource audioSource;

    Omok OmokGame;
    private ARRaycastManager raycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField] private Camera arCamera;


    bool isBoardSet; // 오목판 생성 여부
    bool isGameStart; // 게임 시작 여부
    bool setStone; // 오목알 착수 여부

    public GameObject BlackStone;
    public GameObject WhiteStone;
    
    // Start is called before the first frame update
    void Start()
    {
        this.TimerText = GameObject.Find("TimerText");
        this.Indicator = GameObject.Find("PlaceIndicator");
        this.OmokGameObj = GameObject.Find("OmokGame");
        this.audioSource = GetComponent<AudioSource>(); 

        this.TurnTime = PlayerPrefs.GetFloat("TurnTime");
        this.OmokSound = PlayerPrefs.GetInt("OmokSound");
        this.OmokVibe = PlayerPrefs.GetInt("OmokVibe");

        this.isBoardSet = false;
        this.isGameStart = false;
        this.setStone = false;

        this.raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    // 화면 떨림 방지
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }

        // 오목판을 생성했는지 체크
        if (isBoardSet)
        {
            if (!isGameStart)
            {
                OmokGame = this.OmokGameObj.GetComponent<OmokGameController>().OmokGame;
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
                    }
                    else
                    {
                        // CurrentTurn 을 받아서 승패 여부 확인
                        if (OmokGame.getCurrentTurn() == 0) this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "백돌이 패배하였습니다.";
                        else this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "흑돌이 패배하였습니다.";
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
                            Debug.Log("충돌 TAG : "+OmokIndex);
                            if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes)) // 굳이 arRaycastManager 가 필요한가
                            {
                                int x = int.Parse(OmokIndex[5].ToString());
                                int y = int.Parse(OmokIndex[6].ToString());
                                Debug.Log("X :" + x + ", Y:" + y);

                                if (OmokGame.board[x, y] == 0) // 빈 인덱스인가
                                {
                                    this.setStone = false;
                                    int turn = OmokGame.getCurrentTurn();

                                    PlaceStone(hitObj.collider.transform.position, hitObj.collider.transform.rotation, turn);
                                    //PlaceStone(hits[0].pose, turn);
                                    OmokGame.board[x, y] = turn;

                                    OmokGame.setCurrentTurn(turn);
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
            this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "백돌 차례입니다";
        }
        else
        {
            Instantiate(WhiteStone, hitPosition, hitRotation);
            this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "흑돌 차례입니다";
        }

        if (OmokVibe == 1) Handheld.Vibrate();
        Debug.Log("오목알 Position : (" + hitPosition.x + "," + hitPosition.y + "," + hitPosition.z + ")");
        setTimer();
        this.setStone = true;
    }
}
