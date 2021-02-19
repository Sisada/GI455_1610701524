using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace RoomWebSocket
{
    public class WebsocketConnectionRoom : MonoBehaviour
    {

        public struct SocketEvent
        {
            public string eventName;
            public string data;

            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }

        private WebSocket ws;

        private string tempMessageString;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;

        [SerializeField]
        Text inputCreateRoom = null;
        [SerializeField]
        Text inputJoinRoom = null;

        public Canvas mainMenuCanvas;
        public Canvas createRoomCanvas;
        public Canvas joinRoomCanvas;
        public Canvas roomRoomCanvas;

        public Canvas ErrorMessageCanvas;

        public Text roomNameServer;

        private void Awake()
        {
            Connect();

            mainMenuCanvas.enabled = true;
            createRoomCanvas.enabled = false;
            joinRoomCanvas.enabled = false;
            roomRoomCanvas.enabled = false;
        }

        private void Update()
        {
            UpdateNotifyMessage();
        }

        public void Connect()
        {
            string url = "ws://127.0.0.1:8080/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();
        }

        public void EnableCreateCanvas()
        {
            mainMenuCanvas.enabled = false;
            createRoomCanvas.enabled = true;
            joinRoomCanvas.enabled = false;
            roomRoomCanvas.enabled = false;

            ErrorMessageCanvas.enabled = false;
        }

        public void EnableJoinCanvas()
        {
            mainMenuCanvas.enabled = false;
            createRoomCanvas.enabled = false;
            joinRoomCanvas.enabled = true;

            ErrorMessageCanvas.enabled = false;
        }

        public void EnableLeaveCanvas()
        {
            mainMenuCanvas.enabled = true;
            createRoomCanvas.enabled = false;
            joinRoomCanvas.enabled = false;
            roomRoomCanvas.enabled = false;

            ErrorMessageCanvas.enabled = false;

            LeaveRoom();
        }

        public void CreateRoom(string roomName)
        {
            roomName = inputCreateRoom.text;

            SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            roomNameServer.text = "Room : " + roomName;
        }

        public void JoinRoom(string roomName)
        {
            roomName = inputJoinRoom.text;

            SocketEvent socketEvent = new SocketEvent("JoinRoom", roomName);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            roomNameServer.text = "Room : " + roomName;
        }

        public void LeaveCanvas()
        {
            ErrorMessageCanvas.enabled = false;
        }

        public void LeaveRoom()
        {
            SocketEvent socketEvent = new SocketEvent("LeaveRoom", "");

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            roomNameServer.text = null;
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }

        public void SendMessage(string message)
        {

        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (receiveMessageData.eventName == "CreateRoom")
                {
                    if (OnCreateRoom != null)
                    {
                        OnCreateRoom(receiveMessageData);
                    }
                    else if (receiveMessageData.data != "fail")
                    {
                        roomRoomCanvas.enabled = true;
                    }
                    else if (receiveMessageData.data == "fail")
                    {
                        ErrorMessageCanvas.enabled = true;
                    }
                }
                else if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (OnJoinRoom != null)
                    {
                        OnJoinRoom(receiveMessageData);
                    }
                    else if (receiveMessageData.data != "fail")
                    {
                        roomRoomCanvas.enabled = true;
                    }
                    else if (receiveMessageData.data == "fail")
                    {
                        ErrorMessageCanvas.enabled = true;
                    }
                }
                else if (receiveMessageData.eventName == "LeaveRoom")
                {
                    if (OnLeaveRoom != null)
                        OnLeaveRoom(receiveMessageData);
                }

                tempMessageString = "";
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log(messageEventArgs.Data);

            tempMessageString = messageEventArgs.Data;
        }
    }
}


