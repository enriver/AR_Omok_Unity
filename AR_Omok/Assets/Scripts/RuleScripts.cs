using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuleScripts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    public void MoveToMain()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void SeekRenjuRule()
    {
        Application.OpenURL("https://namu.wiki/w/%EB%A0%8C%EC%A3%BC");
    }
}
