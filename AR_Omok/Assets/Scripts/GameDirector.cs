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


    bool isBoardSet; // ������ ���� ����
    bool isGameStart; // ���� ���� ����
    bool setStone; // ����� ���� ����
    bool isEnd; // ������ �������� ����
    bool isForbidden; // �ݼ�����
    bool isUnderFive; // 5�� �̳� �ΰ�

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

    // ȭ�� ���� ����
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }

        if (!this.isEnd)
        {
            // �������� �����ߴ��� üũ
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
                    if (this.setStone) // ������� ���� �Ͽ��°�
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

                            this.UnderFive.GetComponent<AudioSource>().Stop();

                            // CurrentTurn �� �޾Ƽ� ���� ���� Ȯ��
                            if (OmokGame.getCurrentTurn() == 0) this.panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "�鵹 �¸��Դϴ�";
                            else this.panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "�浹 �¸��Դϴ�";
                        }
                    }

                    if (Input.touchCount == 0) return; // ������ ���ൿ�� ��ġ�� ������ return

                    Touch touch = Input.GetTouch(0);

                    if(touch.phase == TouchPhase.Ended) // �հ����� ���� �̺�Ʈ �߻���
                    {
                        Ray ray;
                        RaycastHit hitObj;

                        ray = arCamera.ScreenPointToRay(touch.position); // �հ��� ���������� ���� ȭ������ raycast

                        if(Physics.Raycast(ray,out hitObj)) // Object �浹��
                        {
                            if (hitObj.collider == null || hitObj.collider.tag == "Untagged") // collider �� �浹 �Ͽ��°�
                            {
                                Debug.Log("�浹�� Collider �� �����ϴ�");
                                return;
                            }
                            else
                            {
                                string OmokIndex=hitObj.collider.tag;

                                if (isContain(OmokIndex)) return;
                                Debug.Log("�浹 TAG : "+OmokIndex);

                                if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes)) // ���� arRaycastManager �� �ʿ��Ѱ�
                                {
                                    int x = int.Parse(OmokIndex[5].ToString());
                                    int y = int.Parse(OmokIndex[6].ToString());
                                    //Debug.Log("X :" + x + ", Y:" + y);

                                    if (OmokGame.board[x, y] == 0) // �� �ε����ΰ�
                                    {
                                        int turn = OmokGame.getCurrentTurn();

          
                                        if(turn == 1)
                                        {
                                            if (OmokGame.isForbidden(x, y, turn))
                                            {
                                                
                                                this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "�ݼ��Դϴ�";
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


                                            // �����ΰ�
                                            if (OmokGame.isGameOver(x, y, turn))
                                            {
                                                this.isEnd = true;
                                                this.panel.SetActive(true);
                                                
                                                this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "";
                                                this.TimerText.GetComponent<Text>().text = "";
                                                this.TurnText.GetComponent<Text>().text = "";
                                                this.UnderFive.GetComponent<AudioSource>().Stop();

                                                if (turn == 1)
                                                {

                                                    this.panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "�浹 �¸��Դϴ�";
                                                }

                                                else
                                                {
                                                    this.panel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "�鵹 �¸��Դϴ�";
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
            this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "�鵹 �����Դϴ�";
        }
        else
        {
            Instantiate(WhiteStone, hitPose.position, hitPose.rotation);
            this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "�浹 �����Դϴ�";
        }
        Debug.Log("����� Position : (" + hitPose.position.x + "," + hitPose.position.y + "," + hitPose.position.z + ")");
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
            //this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "�� �鵹 ���� �Դϴ�";
            this.TurnText.GetComponent<Text>().text = "�� �鵹 ����";

        }
        else
        {
            Instantiate(WhiteStone, hitPosition, hitRotation);
            //this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "�� �浹 ���� �Դϴ�";
            this.TurnText.GetComponent<Text>().text = "�� �浹 ����";
        }
        
        this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "���ѽð� �ȿ� �����Ͽ� �ּ���";
        this.isUnderFive = false;
        this.UnderFive.GetComponent<AudioSource>().Stop();
        // if (OmokVibe == 1) Handheld.Vibrate();

        Debug.Log("����� Position : (" + hitPosition.x + "," + hitPosition.y + "," + hitPosition.z + ")");
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
