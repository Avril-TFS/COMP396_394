using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PhontonMenu : MonoBehaviour
{
    [Tooltip(" Attach PausePanel in UI ")]
    public GameObject pausePanel;
    //public GameObject instructionPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
        //instructionPanel.SetActive(true);
        //StartCoroutine(HideInstructionPanelAfterDelay(2f));
    }
    // Coroutine to hide the instructionPanel after a delay
    //private IEnumerator HideInstructionPanelAfterDelay(float delay)
    //{
    //    // Wait for the specified delay
    //    yield return new WaitForSeconds(delay);

    //    // Hide the instructionPanel
    //    //instructionPanel.SetActive(false);
    //}

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

    }

    public void LOAD_SCENE(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Go_To_MainMenu()
    {
        
        LeaveRoomAndDisconnect();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QUIT_GAME()
    {
        Application.Quit();
    }

    public void LeaveRoomAndDisconnect()
    {
        PhotonNetwork.LeaveRoom(); // Leave the current room
        PhotonNetwork.Disconnect(); // Disconnect from the server
        Debug.Log("LeaveRoom and Disconnect from the server");
        //Initializing Region to empty
        //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
        Debug.Log("isConnected? " + PhotonNetwork.IsConnected);
        Debug.Log("isConnectedAndReady?" + PhotonNetwork.IsConnectedAndReady);
        Debug.Log("isMasterClient?" + PhotonNetwork.IsMasterClient);
        Debug.Log($"Current Server: {PhotonNetwork.ServerAddress}");
        Debug.Log($"Current Region: {PhotonNetwork.CloudRegion}");
    }

}
