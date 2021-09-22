using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Omok {
    int[,] board;
    int currentTurn;

        public Omok()
    {
        this.currentTurn = 1;
        this.board = new int[9,9] { {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0}};
    }
    
    public int getCurrentTurn()
    {
        return currentTurn;
    }
    public void setCurrentTurn(int turn)
    {
        if(turn == 1)
        {
            this.currentTurn = 0;
        }
        else
        {
            this.currentTurn = 1;
        }
    }

}
public class GameDirector : MonoBehaviour
{
    public GameObject guideText;
    public GameObject timerText;
    
    GameObject indicator;

    float time;
    bool isSet;
    bool isGameStart;
    bool setOmokStone;

    private ARRaycastManager raycastManager;
    public GameObject OmokStoneWhite;
    public GameObject OmokStoneBlack;

    Omok game;

    // Start is called before the first frame update
    void Start()
    {
        this.guideText = GameObject.Find("GuideText");
        this.timerText = GameObject.Find("Time");
        this.indicator = GameObject.Find("PlaceIndicator");

        this.time = PlayerPrefs.GetFloat("Time");
        this.isSet = false;
        this.isGameStart = false;
        this.setOmokStone = false;
 
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
        isSet = this.indicator.GetComponent<PlacementIndicator>().isSet;

        if (isSet)
        {
            if (!isGameStart)
            {
                this.guideText.GetComponent<Text>().text = "";
                game = new Omok();
                Debug.Log("오목게임이 생성되었습니다");
                isGameStart = true;
            }
            else
            {
                if (this.setOmokStone)
                {
                    if(this.time >= 0.0f)
                    {
                        this.time -= Time.deltaTime;
                        this.timerText.GetComponent<Text>().text = this.time.ToString("F1");
                    }
                    else
                    {
                        if (game.getCurrentTurn() == 0)
                        {
                            this.guideText.GetComponent<Text>().text = "백돌이 졌습니다.";
                        }
                        else
                        {
                            this.guideText.GetComponent<Text>().text = "흑돌이 졌습니다.";
                        }
                    }
                }

                if(Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    
                    List<ARRaycastHit> hits = new List<ARRaycastHit>();

                    if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes)) // 터치를 통해 object 생성반응이 있을 때
                    {
                        if (touch.phase == TouchPhase.Ended) // 손가락을 떼면
                        {
                            this.setOmokStone = false;
                            int turn = game.getCurrentTurn();
                            PlaceOmokStone(hits[0].pose, turn); // 오목알을 생성
                            game.setCurrentTurn(turn);
                        }
                    }
                }
            }
        }

    }
    
    void setTimer()
    {
        this.time = PlayerPrefs.GetFloat("Time");
    }
    void PlaceOmokStone(Pose hitPose, int currentTurn)
    {
        if (currentTurn == 1) // 현재 오목돌이 흰색이기 때문에, 검정색을 둘 차례 명시
        {
            Instantiate(OmokStoneBlack, hitPose.position, hitPose.rotation); // 검은돌 착수
        }
        else
        {
            Instantiate(OmokStoneWhite, hitPose.position, hitPose.rotation); // 흰돌 착수
        }
        Debug.Log("오목알 Position : ("+hitPose.position.x.ToString("F4")+ ","+hitPose.position.y.ToString("F4")+","+hitPose.position.z.ToString("F4")+")");
        setTimer();
        this.setOmokStone = true;
    }
}
