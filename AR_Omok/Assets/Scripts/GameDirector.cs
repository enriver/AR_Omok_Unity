using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class GameDirector : MonoBehaviour
{
    public GameObject GuideText;
    public GameObject TimerText;
    float TurnTime;
    
    GameObject Indicator;
    GameObject OmokGameObj;
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
        this.GuideText = GameObject.Find("GuideText");
        this.TimerText = GameObject.Find("Time");
        this.Indicator = GameObject.Find("PlaceIndicator");
        this.OmokGameObj = GameObject.Find("OmokGame");

        this.TurnTime = PlayerPrefs.GetFloat("TurnTime");

        this.isBoardSet = false;
        this.isGameStart = false;
        this.setStone = false;

        this.raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    // Update is called once per frame
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
                this.GuideText.GetComponent<Text>().text = "오목 게임이 시작되었습니다.";
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
                        if (OmokGame.getCurrentTurn() == 0) this.GuideText.GetComponent<Text>().text = "백돌이 패배하였습니다.";
                        else this.GuideText.GetComponent<Text>().text = "흑돌이 패배하였습니다.";
                    }
                }

                if (Input.touchCount == 0) return;

                Touch touch = Input.GetTouch(0);

                if(touch.phase == TouchPhase.Ended)
                {
                    Ray ray;
                    RaycastHit hitObj;

                    ray = arCamera.ScreenPointToRay(touch.position);

                    if(Physics.Raycast(ray,out hitObj))
                    {
                        if (hitObj.collider == null)
                        {
                            Debug.Log("아무것도 없넹");
                            return;
                        }
                        else
                        {
                            string OmokIndex=hitObj.collider.tag;
                            Debug.Log("충돌 TAG : "+OmokIndex);
                            if (raycastManager.Raycast(touch.position, hits, TrackableType.All))
                            {
                                int x = int.Parse(OmokIndex[5].ToString());
                                int y = int.Parse(OmokIndex[6].ToString());
                                Debug.Log("X :" + x + ", Y:" + y);

                                if (OmokGame.board[x, y] == 0)
                                {
                                    this.setStone = false;
                                    int turn = OmokGame.getCurrentTurn();

                                    PlaceStone(hits[0].pose, turn);
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

    void PlaceStone(Pose hitPose, int currentTurn)
    {
        hitPose.position.y = Mathf.Round(hitPose.position.y * 1000) * 0.001f;

        if (currentTurn == 1) Instantiate(BlackStone, hitPose.position, hitPose.rotation);
        else Instantiate(WhiteStone, hitPose.position, hitPose.rotation);

        Debug.Log("오목알 Position : (" + hitPose.position.x + "," + hitPose.position.y + "," + hitPose.position.z + ")");
        setTimer();
        this.setStone = false;
    }
}
