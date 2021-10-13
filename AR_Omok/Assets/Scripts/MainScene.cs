using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour
{
    public int clickCount = 0;

    void Start()
    {
        // ���ø����̼ǿ� ó�� �����̶��
        if (!PlayerPrefs.HasKey("FirstConnect")){

            PlayerPrefs.SetInt("FirstConnect", 1); // ���� ����ҿ� ó�� ������ �ƴ϶�� ���� ����
            PlayerPrefs.SetFloat("TurnTime", 60.0f); // �ʱ� Ÿ�̸� ī��Ʈ�� 60.0f �� ����
            PlayerPrefs.SetInt("TimerSound", 1); // �ʱ� Ÿ�̸� ���带 On ���� ����
            PlayerPrefs.SetInt("OmokSound", 1); // �ʱ� ����� ���带 On ���� ����
            PlayerPrefs.SetInt("OmokVibe", 1); // �ʱ� ����� ���带 On ���� ����


            Debug.Log("ó�� ���ø����̼��� �����Ͽ����ϴ�.");
        }
        else
        {
            Debug.Log("���ø����̼��� �����ߴ� ����� �ֽ��ϴ�.");
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
        SceneManager.LoadScene("ModeScene");
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

