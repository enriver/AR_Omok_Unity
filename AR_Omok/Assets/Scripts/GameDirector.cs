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


    bool isBoardSet; // ������ ���� ����
    bool isGameStart; // ���� ���� ����
    bool setStone; // ����� ���� ����

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

        // �������� �����ߴ��� üũ
        if (isBoardSet)
        {
            if (!isGameStart)
            {
                this.GuideText.GetComponent<Text>().text = "���� ������ ���۵Ǿ����ϴ�.";
                OmokGame = this.OmokGameObj.GetComponent<OmokGameController>().OmokGame;
                isGameStart = true;
            }
            else
            {
                if (this.setStone) // ������� ���� �Ͽ��°�
                {
                    if(this.TurnTime >= 0.0f)
                    {
                        this.TurnTime -= Time.deltaTime;
                        this.TimerText.GetComponent<Text>().text = this.TurnTime.ToString("F1");
                    }
                    else
                    {
                        // CurrentTurn �� �޾Ƽ� ���� ���� Ȯ��
                        if (OmokGame.getCurrentTurn() == 0) this.GuideText.GetComponent<Text>().text = "�鵹�� �й��Ͽ����ϴ�.";
                        else this.GuideText.GetComponent<Text>().text = "�浹�� �й��Ͽ����ϴ�.";
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
                            Debug.Log("�ƹ��͵� ����");
                            return;
                        }
                        else
                        {
                            string OmokIndex=hitObj.collider.tag;
                            Debug.Log("�浹 TAG : "+OmokIndex);
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

        Debug.Log("����� Position : (" + hitPose.position.x + "," + hitPose.position.y + "," + hitPose.position.z + ")");
        setTimer();
        this.setStone = false;
    }
}
