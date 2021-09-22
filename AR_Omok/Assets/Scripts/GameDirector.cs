using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Omok {
    bool[,] board;
    int currentTurn;

        public Omok()
    {
        this.currentTurn = 1;
        this.board = new bool[9,9] {{ false, false, false, false, false, false, false, false, false },
                                    { false, false, false, false, false, false, false, false, false },
                                    { false, false, false, false, false, false, false, false, false },
                                    { false, false, false, false, false, false, false, false, false },
                                    { false, false, false, false, false, false, false, false, false },
                                    { false, false, false, false, false, false, false, false, false },
                                    { false, false, false, false, false, false, false, false, false },
                                    { false, false, false, false, false, false, false, false, false },
                                    { false, false, false, false, false, false, false, false, false }};
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
            StartCoroutine(giveDelay(1.5f));
            if (!isGameStart)
            {
                this.guideText.GetComponent<Text>().text = "";
                game = new Omok();
                Debug.Log("오목게임이 생성되었습니다");
                isGameStart = true;
            }
            else
            {
                PlaceOmokStone(game.getCurrentTurn());
                StartCoroutine(giveDelay(1.5f));

            }
        }

    }

    IEnumerator giveDelay(float delayTime)
    {
        float countTime = 0.0f;

        while (countTime < delayTime)
        {
            countTime += Time.deltaTime;
        }

        yield return null;
    }
    
    void setTimer(int currentTurn)
    {
        this.time = PlayerPrefs.GetFloat("Time");
        if (this.time >= 0)
        {
            this.time -= Time.deltaTime;
            this.timerText.GetComponent<Text>().text = this.time.ToString("F1");
        }
        else
        {
            if (currentTurn == 1)
            {
                this.guideText.GetComponent<Text>().text = "흑돌이 졌습니다.";
            }
            else
            {
                this.guideText.GetComponent<Text>().text = "백돌이 졌습니다.";
            }
        }
    }
    void PlaceOmokStone(int currentTurn)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Ended)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes))
                {
                    Pose hitPose = hits[0].pose;

                    if (currentTurn == 1) // 현재 오목돌이 흰색이기 때문에, 검정색을 둘 차례 명시
                    {
                        Instantiate(OmokStoneBlack, hitPose.position, hitPose.rotation); // 검은돌 착수
                    }
                    else
                    {
                        Instantiate(OmokStoneWhite, hitPose.position, hitPose.rotation); // 흰돌 착수
                    }
                    Debug.Log("오목알이 생성된 위치 " + hitPose.position);
                    setTimer(currentTurn);
                    game.setCurrentTurn(currentTurn);
                }
            }   
        }
    }
}
