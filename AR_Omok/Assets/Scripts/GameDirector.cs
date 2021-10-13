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


    bool isBoardSet; // ������ ���� ����
    bool isGameStart; // ���� ���� ����
    bool setStone; // ����� ���� ����

    public GameObject BlackStone;
    public GameObject WhiteStone;

    List<string> tagNames = new List<string>();
    
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

    // ȭ�� ���� ����
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("ModeScene");
        }

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
                    }
                    else
                    {
                        // CurrentTurn �� �޾Ƽ� ���� ���� Ȯ��
                        if (OmokGame.getCurrentTurn() == 0) this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "�鵹�� �й��Ͽ����ϴ�.";
                        else this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "�浹�� �й��Ͽ����ϴ�.";
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
                                    this.setStone = false;
                                    int turn = OmokGame.getCurrentTurn();

          
                                    if(turn == 1)
                                    {
                                        if (OmokGame.isForbidden(x, y, turn))
                                        {
                                            this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "�ݼ��Դϴ�";
                                            return;
                                        }
                                    }
                                    


                                    PlaceStone(hitObj.collider.transform.position, hitObj.collider.transform.rotation, turn);
                                    tagNames.Add(OmokIndex);
                                    //PlaceStone(hits[0].pose, turn);
                                    OmokGame.setStone(x, y, turn);


                                    // �����ΰ�
                                    if (OmokGame.isGameOver(x, y,turn))
                                    {
                                        if(turn == 1) this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "�浹 �¸��Դϴ�";
                                        else this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "�鵹 �¸��Դϴ�";

                                        // ���� �˾�â �߰� ���

                                        return;
                                    }

                                    OmokGame.setCurrentTurn(turn);
                                    //OmokGame.showBoard();
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
            this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "�鵹 �����Դϴ�";
        }
        else
        {
            Instantiate(WhiteStone, hitPosition, hitRotation);
            this.Indicator.GetComponent<IndicatorScripts>().GuideText.GetComponent<Text>().text = "�浹 �����Դϴ�";
        }

        if (OmokVibe == 1) Handheld.Vibrate();

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
}
