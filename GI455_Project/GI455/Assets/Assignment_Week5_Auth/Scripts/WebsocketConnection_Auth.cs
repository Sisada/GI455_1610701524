using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace RoomWebSocket
{
    public class WebsocketConnection_Auth : MonoBehaviour
    {

        public struct SocketEventRoom
        {
            public string eventName;
            public string data;

            public SocketEventRoom(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }

        private WebSocket ws;

        private string tempMessageString;

        public delegate void DelegateHandle(SocketEventRoom result);
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

            mainMenuCanvas.enabled = true;
            createRoomCanvas.enabled = false;
            joinRoomCanvas.enabled = false;
            roomRoomCanvas.enabled = false;
        }

        private void Update()
        {
            UpdateNotifyMessage();
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

            SocketEventRoom socketEventRoom = new SocketEventRoom("CreateRoom", roomName);

            string toJsonStr = JsonUtility.ToJson(socketEventRoom);

            ws.Send(toJsonStr);

            roomNameServer.text = "Room : " + roomName;
        }

        public void JoinRoom(string roomName)
        {
            roomName = inputJoinRoom.text;

            SocketEventRoom socketEvent = new SocketEventRoom("JoinRoom", roomName);

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
            SocketEventRoom socketEvent = new SocketEventRoom("LeaveRoom", "");

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
                SocketEventRoom receiveMessageData = JsonUtility.FromJson<SocketEventRoom>(tempMessageString);

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


