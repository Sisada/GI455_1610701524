using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RoomWebSocket
{
    public class LoginRoomConnection : MonoBehaviour
    {
        [SerializeField]
        private Text ipConnection;
        [SerializeField]
        private Text portConnection;

        private WebSocket webSocket;

        public void ConnectToServerRoom()
        {
            if (ipConnection.text == "127.0.0.1" && portConnection.text == "8080")
            {
                SceneManager.LoadScene("LobbyRoom");
            }
        }
    }
}