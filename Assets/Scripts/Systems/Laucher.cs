using System;
using Photon.Pun;
using UnityEngine;

namespace Systems
{
    public class Laucher : MonoBehaviourPunCallbacks
    {
        public void Awake() //0
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            Connect();
        }
        public override void OnConnectedToMaster() //2
        {
            Debug.Log("CONNECTED");
            Join(); //b
            
            base.OnConnectedToMaster(); //a
            
        }
        public override void OnJoinedRoom() //4
        {
            Debug.Log("OnJoinedRoom.....");
            StartGame();
            
            base.OnJoinedRoom();
        }
        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed.....");
            Create();
            
            base.OnJoinRandomFailed(returnCode, message);
        }
        public void Connect()//1
        {
            Debug.Log("Try Connecting.....");
            PhotonNetwork.GameVersion = "0.0.0";
            PhotonNetwork.ConnectUsingSettings();
        }
        
        public void Join() //3
        {
            PhotonNetwork.JoinRandomRoom();
        }
        
        public void Create()
        {
            PhotonNetwork.CreateRoom("");
        }

        public void StartGame() //5
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.LoadLevel(1);
            }
        }
    }
}