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
        Debug.Log("NetWork Mode �߰� ����");
    }

    public void StartAIMode()
    {
        Debug.Log("AI Mode �߰� ����");
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

            // ���̽� �ҽ� �ڵ� �� �Լ��� �ҷ�����
            var getReturnValue = vScope.GetVariable<Func<string>>("testPythonFunc");
            Debug.Log(getReturnValue());

        }catch(Exception ex)
        {
            Debug.Log(ex.Message.ToString());
        }
    }*/
}
