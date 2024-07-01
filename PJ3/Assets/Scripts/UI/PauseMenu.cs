using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Managers;

    private UIManager uIManager;

    private PlayerandCameraHolders playerandCamera;

    public void start(){
        uIManager = Managers.GetComponent<UIManager>();
        playerandCamera = Managers.GetComponent<PlayerandCameraHolders>(); 
    }

    public void ExitPause(){
        Debug.Log("Exit Pause");

        uIManager.HidePause();
        playerandCamera.PlayerCanMove(true);
    }

   public void ExitGame (){
        Debug.Log("Exit Game");
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
