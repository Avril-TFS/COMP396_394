using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;


public class ConnectToServer : MonoBehaviourPunCallbacks
{

    private void Start()
    {
        //Initializing Region to empty
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";

        PhotonNetwork.ConnectUsingSettings();
        //Debug.Log("Connecting to Photon server...");
    }

    //This function will be called when we successuflly connected to our server
    public override void OnConnectedToMaster()
    {
        //Debug.Log("Connected to master server");
        PhotonNetwork.JoinLobby();
       
    }

    //When we joined Lobby, go to Lobby scene
    public override void OnJoinedLobby()
    {
       // Debug.Log("Joined Lobby");
        SceneManager.LoadScene("Lobby");
    }

}
