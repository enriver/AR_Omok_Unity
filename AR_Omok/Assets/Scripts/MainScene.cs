using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour
{
    public int clickCount = 0;

    void Start()
    {
        // 어플리케이션에 처음 접근이라면
        if (!PlayerPrefs.HasKey("FirstConnect")){

            PlayerPrefs.SetInt("FirstConnect", 1); // 로컬 저장소에 처음 접근이 아니라는 것을 설정
            PlayerPrefs.SetFloat("Time", 60.0f); // 초기 타이머 카운트를 60.0f 로 설정
            PlayerPrefs.SetInt("TimerSound", 1); // 초기 타이머 사운드를 On 으로 설정
            PlayerPrefs.SetInt("OmokSound", 1); // 초기 오목알 사운드를 On 으로 설정
            PlayerPrefs.SetInt("OmokVibe", 1); // 초기 오목알 사운드를 On 으로 설정


            Debug.Log("처음 어플리케이션을 실행하였습니다.");
        }
        else
        {
            Debug.Log("어플리케이션을 실행했던 기록이 있습니다.");
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.clickCount += 1;

            if (!IsInvoking("DoubleClick")) Invoke("DoubleClick", 1.0f);
        }else if(this.clickCount == 2)
        {
            CancelInvoke("DoubleClick");
            Application.Quit();
        }
    }

    void DoubleClick()
    {
        this.clickCount = 0;
    }

    public void MoveGameScene()
    {
        SceneManager.LoadScene("ArScene");
    }
    public void MoveRuleScene()
    {
        SceneManager.LoadScene("RuleScene");
    }
    public void MoveSettingScene()
    {
        SceneManager.LoadScene("SettingScene");
    }
        
}

