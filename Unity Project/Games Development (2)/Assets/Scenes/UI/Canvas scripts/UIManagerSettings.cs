using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerSettings : MonoBehaviour
{


    // Start is called before the first frame update
    private void Start()
    {   
        //hideAllUIPanels();
    }

    

    public void pauseTime()
    {   //freeze the game so player
        Time.timeScale = 0f;
    }

    public void resumeTime()
    {   //unfreeze the game
        Time.timeScale = 1f;
    }

    public void quitGame()
    {
        Application.Quit();
    }

}
