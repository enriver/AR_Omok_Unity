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
        // �������� �����ߴ��� üũ
        isSet = this.indicator.GetComponent<PlacementIndicator>().isSet;

        if (isSet)
        {
            if (!isGameStart)
            {
                this.guideText.GetComponent<Text>().text = "";
                game = new Omok();
                Debug.Log("��������� �����Ǿ����ϴ�");
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
                            this.guideText.GetComponent<Text>().text = "�鵹�� �����ϴ�.";
                        }
                        else
                        {
                            this.guideText.GetComponent<Text>().text = "�浹�� �����ϴ�.";
                        }
                    }
                }

                if(Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    
                    List<ARRaycastHit> hits = new List<ARRaycastHit>();

                    if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes)) // ��ġ�� ���� object ���������� ���� ��
                    {
                        if (touch.phase == TouchPhase.Ended) // �հ����� ����
                        {
                            this.setOmokStone = false;
                            int turn = game.getCurrentTurn();
                            PlaceOmokStone(hits[0].pose, turn); // ������� ����
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
        if (currentTurn == 1) // ���� ������ ����̱� ������, �������� �� ���� ���
        {
            Instantiate(OmokStoneBlack, hitPose.position, hitPose.rotation); // ������ ����
        }
        else
        {
            Instantiate(OmokStoneWhite, hitPose.position, hitPose.rotation); // �� ����
        }
        Debug.Log("����� Position : ("+hitPose.position.x.ToString("F4")+ ","+hitPose.position.y.ToString("F4")+","+hitPose.position.z.ToString("F4")+")");
        setTimer();
        this.setOmokStone = true;
    }
}
