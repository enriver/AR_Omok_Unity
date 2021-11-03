using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;


public class IndicatorScripts : MonoBehaviour
{
    public bool isSet;
    

    private ARRaycastManager raycastManager;
    private GameObject Indicator;
    public GameObject GuideText;
    

    public GameObject OmokBoardPrefab;

    public Vector3 boardSize;

    // Start is called before the first frame update
    void Start()
    {
        this.raycastManager = FindObjectOfType<ARRaycastManager>();
        this.Indicator = transform.GetChild(0).gameObject;
   
        this.isSet = false;
        
        this.GuideText = GameObject.Find("GuideText");

        this.boardSize = OmokBoardPrefab.GetComponent<MeshRenderer>().bounds.size;
        Indicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // �������� �����Ǿ����� Ȯ��
        if (!isSet)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes); // ȭ���� �߾ӿ� ARray�� ����

            // ��ü���� �浹 �ν� �� (ȭ���� ���������� �ν����� ���)
            if (hits.Count > 0)
            {
                
                this.GuideText.GetComponent<Text>().text = "��ġ�Ͽ� �������� �������ּ���";

                Pose hitPose = hits[0].pose;

                transform.position = hitPose.position;
                transform.rotation = hitPose.rotation;

                if (!Indicator.activeInHierarchy) Indicator.SetActive(true); // Indicator �� �ش� �����ǿ� active��

                if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) // ��ġ �̺�Ʈ�� �߻�������, �հ����� �� �� ����
                {
                    // ������ ���� -> Indicator ��ȯ ����
                    this.isSet = true;
                    Destroy(Indicator);

                    hitPose.position.y = Mathf.Round(hitPose.position.y * 1000) * 0.001f;
                    hitPose.rotation = Quaternion.Euler(-90, hitPose.rotation.y, hitPose.rotation.z);

                    Instantiate(OmokBoardPrefab, hitPose.position, hitPose.rotation);
                    Debug.Log("������ Position : (" + hitPose.position.x+","+hitPose.position.y+","+hitPose.position.z+")");
                    Debug.Log("������ ũ��"+boardSize);
                    this.GuideText.GetComponent<Text>().text = "���� ������ ���۵Ǿ����ϴ�.";
                    
                }                
            }
        }
    }
}

