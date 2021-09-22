using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class PlacementIndicator : MonoBehaviour
{
    public bool isSet;
    private ARRaycastManager raycastManager;
    private GameObject indicator;

    public GameObject OmokBoard;
    GameObject spawnOmokBoard;


    // Start is called before the first frame update
    void Start()
    {
        this.raycastManager = FindObjectOfType<ARRaycastManager>();
        this.indicator = transform.GetChild(0).gameObject;
        this.isSet = false;


        indicator.SetActive(false);
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
                Pose hitPose = hits[0].pose;

                transform.position = hitPose.position;
                transform.rotation = hitPose.rotation;

                if (!indicator.activeInHierarchy) indicator.SetActive(true); // Indicator 를 해당 포지션에 active함

                if(Input.touchCount > 0) // 터치 이벤트가 발생했을 경우
                {
                    // 오목판 생성
                    hitPose.rotation = Quaternion.Euler(-90, hitPose.rotation.y, hitPose.rotation.z);
                    
                    spawnOmokBoard = Instantiate(OmokBoard, hitPose.position, hitPose.rotation); // 오목판을 생성
                    Debug.Log("오목판이 생성된 위치 "+hitPose.position);
                    indicator.SetActive(false); // Indicator 삭제

                    this.isSet = true;
                }
                
            }
        }
    }
}

