//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeScripts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //StartPython();   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    public void StartBasicMode()
    {
        SceneManager.LoadScene("ArScene");
    }

    public void StartNetworkMode()
    {
        Debug.Log("NetWork Mode 추가 예정");
    }

    public void StartAIMode()
    {
        SceneManager.LoadScene("AIScene");
        Debug.Log("AI Mode 추가 예정");
    }

    /*
    void StartPython()
    {
        var pyEngine = Python.CreateEngine();
        var vScope = pyEngine.CreateScope();

        try
        {
            var vSource = pyEngine.CreateScriptSourceFromFile("pythonScripts/test.py");
            vSource.Execute(vScope);

            // 파이썬 소스 코드 안 함수들 불러오기
            var getReturnValue = vScope.GetVariable<Func<string>>("testPythonFunc");
            Debug.Log(getReturnValue());

        }catch(Exception ex)
        {
            Debug.Log(ex.Message.ToString());
        }
    }*/
}
