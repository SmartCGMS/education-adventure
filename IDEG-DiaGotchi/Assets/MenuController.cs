using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void NewGameButtonPressed()
    {
        SceneManager.LoadScene("Scenes/GameScene", LoadSceneMode.Single);
    }

    public void ContinueGameButtonPressed()
    {
        //
    }

    public void OptionsButtonPressed()
    {
        //
    }

    public void ExitButtonPressed()
    {
        Application.Quit(0);
    }
}
