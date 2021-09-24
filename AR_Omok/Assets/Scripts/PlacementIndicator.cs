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
        // �������� �����Ǿ����� Ȯ��
        if (!isSet)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes); // ȭ���� �߾ӿ� ARray�� ����

            // ��ü���� �浹 �ν� �� (ȭ���� ���������� �ν����� ���)
            if (hits.Count > 0)
            {
                Pose hitPose = hits[0].pose;

                transform.position = hitPose.position;
                transform.rotation = hitPose.rotation;

                if (!indicator.activeInHierarchy) indicator.SetActive(true); // Indicator �� �ش� �����ǿ� active��

                if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) // ��ġ �̺�Ʈ�� �߻�������, �հ����� �� �� ����
                {
                    // ������ ����
                    hitPose.rotation = Quaternion.Euler(-90, hitPose.rotation.y, hitPose.rotation.z);
                    this.isSet = true;
                    indicator.SetActive(false); // Indicator ����
                    StartCoroutine(SetOmokBoard(hitPose));
                    
                }                
            }
        }
    }

    IEnumerator SetOmokBoard(Pose hitPose)
    {
        Instantiate(OmokBoard, hitPose.position, hitPose.rotation); // �������� ����
        Debug.Log("������ Position : (" + hitPose.position.x.ToString("F4") + "," + hitPose.position.y.ToString("F4") + "," + hitPose.position.z.ToString("F4") + ")");
        yield return new WaitForSecondsRealtime(0.5f);
        
    }
}

