using UnityEngine;
using System.Collections;

namespace KopyKat
{
    public class Matchmaker : MonoBehaviour
    {
        private const string ROOM_NAME = "MainRoom";
        private static int playerNum = -1;

        public static int GetPlayerNum()
        {
            return playerNum;
        }

        // Use this for initialization
        void Start()
        {
            //wizard's already set up settings to connect to a server;
            //unfortunately, that's localhost for now.
            Debug.Log("Joining server...");
            PhotonNetwork.ConnectUsingSettings("0.1");
            //PhotonNetwork.player.ID
        }

        // Update is called once per frame
        void Update()
        {
        }

        void OnJoinedLobby()
        {
            Debug.Log("Joined server, attempting to join game room");
            PhotonNetwork.JoinRoom(ROOM_NAME);
        }

        void OnJoinedRoom()
        {
            Debug.Log("Joined room.");
            playerNum = PhotonNetwork.player.ID;
        }

        void OnPhotonJoinRoomFailed()
        {
            Debug.Log("Failed to join room! Attempting to create room...");
            PhotonNetwork.CreateRoom(ROOM_NAME);
        }

        void OnPhotonCreateRoomFailed()
        {
            Debug.Log("Failed to create room! Disconnecting.");
            PhotonNetwork.Disconnect();
        }

        void OnDisconnectedFromPhoton()
        {
            Debug.Log("Disconnected from server.");
            playerNum = -1;
        }
    }
}