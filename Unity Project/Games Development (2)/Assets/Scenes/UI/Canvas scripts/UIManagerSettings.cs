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

    // Update is called once per frame
/*        private void pauseGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        pausePanel.SetActive(true);
    }



        public void hideAllUIPanels()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
    }
    
*/

    public void pauseTime()
    {
        Time.timeScale = 0f;
    }

    public void resumeTime()
    {
        Time.timeScale = 1f;
    }

    public void quitGame()
    {
        Application.Quit();
    }

}
