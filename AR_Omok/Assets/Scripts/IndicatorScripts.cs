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
        // 오목판이 생성되었는지 확인
        if (!isSet)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes); // 화면의 중앙에 ARray를 보냄

            // 물체와의 충돌 인식 시 (화면의 지형지물을 인식했을 경우)
            if (hits.Count > 0)
            {

                this.GuideText.GetComponent<Text>().text = "터치하여 오목판을 생성해주세요";

                Pose hitPose = hits[0].pose;

                transform.position = hitPose.position;
                transform.rotation = hitPose.rotation;

                if (!Indicator.activeInHierarchy) Indicator.SetActive(true); // Indicator 를 해당 포지션에 active함

                if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) // 터치 이벤트가 발생했으며, 손가락을 뗄 때 동작
                {
                    // 오목판 생성 -> Indicator 순환 방지
                    this.isSet = true;
                    Destroy(Indicator);

                    hitPose.position.y = Mathf.Round(hitPose.position.y * 1000) * 0.001f;
                    hitPose.rotation = Quaternion.Euler(-90, hitPose.rotation.y, hitPose.rotation.z);

                    Instantiate(OmokBoardPrefab, hitPose.position, hitPose.rotation);
                    Debug.Log("오목판 Position : (" + hitPose.position.x+","+hitPose.position.y+","+hitPose.position.z+")");
                    Debug.Log("오목판 크기"+boardSize);
                    this.GuideText.GetComponent<Text>().text = "오목 게임이 시작되었습니다.";
                }                
            }
        }
    }
}

