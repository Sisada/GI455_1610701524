using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;

namespace ProgramChat
{
    public class ChatConnection : MonoBehaviour
    {
        [SerializeField]
        private InputField textInput = null;
        [SerializeField]
        private Text textPublic = null;

        private WebSocket webSocket;

        public void Start()
        {
            webSocket = new WebSocket("ws://127.0.0.1:5500/");
            webSocket.Connect();

            webSocket.OnMessage += OnMessage;
        }

        public void SendChatButton()
        {
            if (webSocket.ReadyState == WebSocketState.Open)
            {
                webSocket.Send(textInput.text);
                textInput.text = null;
            }
        }

        private void OnDestroy()
        {
            if (webSocket != null)
            {
                webSocket.Close();
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            textPublic.text += messageEventArgs.Data + "\n";            
            Debug.Log("Receive Msg :" + messageEventArgs.Data);
        }
    }
}

