using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ProgramChat
{
    public class LoginConnection : MonoBehaviour
    {
        [SerializeField]
        private Text ipConnection;
        [SerializeField]
        private Text portConnection;

        private WebSocket webSocket;

        public void ConnectToServerChat()
        {
            if(ipConnection.text == "127.0.0.1" && portConnection.text == "5500")
            {
                SceneManager.LoadScene("Chat");
            }
        }
        
        //void Update()
        //{
        //    if(Input.GetKeyDown(KeyCode.S))
        //    {
        //        if (webSocket.ReadyState == WebSocketState.Open)
        //        {
        //            webSocket.Send("" + Random.Range(0, 999999));
        //        }
        //    }
        //}

        //private void OnDestroy()
        //{
        //    if(webSocket != null)
        //    {
        //        webSocket.Close();
        //    }
        //}

        //private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        //{
        //    Debug.Log("Receive Msg :" + messageEventArgs.Data);
        //}
    }   
}

