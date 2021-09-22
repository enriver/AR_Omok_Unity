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
        // Setting Scene을 불러왔을 경우, 초기 상태를 로컬DB에서 꺼내와서 설정해주어야함
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

    // 뒤로가기 이미지 클릭시 연결되는 onClick function
    public void MoveToMain()
    {
        SceneManager.LoadScene("MainScene");
    }

    // 로컬DB에 설정해 놓은 Timer 값을 불러와서 Toggle에 표시
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

    // 로컬DB에서 설정해 놓은 Timer Sound 값을 불러와서 Slider에 표시
    void setTimerSound()
    {
        this.timerSound = PlayerPrefs.GetInt("TimerSound");
        sld_timerSound.value = this.timerSound;

    }

    // 로컬DB에서 설정해 놓은 오목 Sound 값을 불러와서 Slider에 표시
    void setOmokSound()
    {
        this.omokSound = PlayerPrefs.GetInt("OmokSound");
        sld_omokSound.value = this.omokSound;

    }

    //로컬 DB에서 설ㅈ어해 놓은 오목 Vibe 값을 불러와서 Slider에 표시
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
