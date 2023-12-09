using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;

    private void Start()
    {
        //Initializing Region to empty
        //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
        //Debug.Log("isConnected? " + PhotonNetwork.IsConnected);
        //Debug.Log("isConnectedAndReady?"+PhotonNetwork.IsConnectedAndReady);
        //Debug.Log("isMasterClient?" + PhotonNetwork.IsMasterClient);
        //Debug.Log($"Current Server: {PhotonNetwork.ServerAddress}");
        //Debug.Log($"Current Region: {PhotonNetwork.CloudRegion}");

    }

  

    //when you create a room, you automatically join that room
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(joinInput.text);
        }
        else
        {
            Debug.LogError("Not connected to the master server yet.");
        }

    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("PhontonLevelOne");
    }
}
