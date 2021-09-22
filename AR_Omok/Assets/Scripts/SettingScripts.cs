using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingScripts : MonoBehaviour
{
    public Toggle second30;
    public Toggle second45;
    public Toggle second60;

    public Slider sld_timerSound;
    public Slider sld_omokSound;
    public Slider sld_omokVibe;

    float time;
    int timerSound;
    int omokSound;
    int omokVibe;

    // Start is called before the first frame update
    void Start()
    {
        // Setting Scene�� �ҷ����� ���, �ʱ� ���¸� ����DB���� �����ͼ� �������־����
        setTimer();
        setTimerSound();
        setOmokSound();
        setOmokVibe();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }

        saveTime();

        sld_timerSound.onValueChanged.AddListener(delegate { saveTimerSound(sld_timerSound.value); });
        sld_omokSound.onValueChanged.AddListener(delegate { saveOmokSound(sld_omokSound.value); });
        sld_omokVibe.onValueChanged.AddListener(delegate { saveOmokVibe(sld_omokVibe.value); });
    }

    // �ڷΰ��� �̹��� Ŭ���� ����Ǵ� onClick function
    public void MoveToMain()
    {
        SceneManager.LoadScene("MainScene");
    }

    // ����DB�� ������ ���� Timer ���� �ҷ��ͼ� Toggle�� ǥ��
    void setTimer()
    {
        this.time = PlayerPrefs.GetFloat("Time");

        if(this.time == 30.0f)
        {
            second30.isOn = true;
        }else if(this.time == 45.0f)
        {
            second45.isOn = true;
        }
        else
        {
            second60.isOn = true;
        }
    }

    // ����DB���� ������ ���� Timer Sound ���� �ҷ��ͼ� Slider�� ǥ��
    void setTimerSound()
    {
        this.timerSound = PlayerPrefs.GetInt("TimerSound");
        sld_timerSound.value = this.timerSound;

    }

    // ����DB���� ������ ���� ���� Sound ���� �ҷ��ͼ� Slider�� ǥ��
    void setOmokSound()
    {
        this.omokSound = PlayerPrefs.GetInt("OmokSound");
        sld_omokSound.value = this.omokSound;

    }

    //���� DB���� �������� ���� ���� Vibe ���� �ҷ��ͼ� Slider�� ǥ��
    void setOmokVibe()
    {
        this.omokVibe = PlayerPrefs.GetInt("OmokVibe");
        sld_omokVibe.value = this.omokVibe;

    }

    void saveTime()
    {
        if (second30.isOn)
        {
            this.time = 30.0f;
        }else if (second45.isOn)
        {
            this.time = 45.0f;
        }
        else
        {
            this.time = 60.0f;
        }

        PlayerPrefs.SetFloat("Time", this.time);
    }

    public void saveTimerSound(float slider_value)
    {
        if(slider_value >= 0.5)
        {
            sld_timerSound.value = 1;
            this.timerSound = 1;
        }
        else
        {
            sld_timerSound.value = 0;
            this.timerSound = 0;
        }

        PlayerPrefs.SetInt("TimerSound", this.timerSound);
    }

    public void saveOmokSound(float slider_value)
    {
        if(slider_value >= 0.5)
        {
            sld_omokSound.value = 1;
            this.omokSound = 1;
        }
        else
        {
            sld_omokSound.value = 0;
            this.omokSound = 0;
        }

        PlayerPrefs.SetInt("OmokSound", this.omokSound);
    }

    public void saveOmokVibe(float slider_value)
    {
        if(slider_value >= 0.5)
        {
            sld_omokVibe.value = 1;
            this.omokVibe = 1;
        }
        else
        {
            sld_omokVibe.value = 0;
            this.omokVibe = 0;
        }

        PlayerPrefs.SetInt("OmokVibe", this.omokVibe);
    }
}
