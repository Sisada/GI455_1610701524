using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

namespace RoomWebSocket
{
    public class LoginManager : MonoBehaviour
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

        [SerializeField]
        private Text username_Login, password_Login;

        [SerializeField]
        private Text name_Reg, username_Reg, password_Reg , passwordAgain_Reg;

        [SerializeField]
        private Text ip_Connection, port_Connection;

        public Canvas mainMenuLoginRegCanvas;
        public Canvas mainMenuRegisterRegCanvas;
        public Canvas errorInputCanvas;        
        public Canvas errorLoginCanvas;
        public Canvas errorRegCanvas;
        public Canvas connectCanvas;

        public WebSocket ws;
        private string tempMessageString;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnLogin;
        public DelegateHandle OnRegister;

        public void Update()
        {
            UpdateNotifyMessage();
        }
        private void Awake()
        {
            errorInputCanvas.enabled = false;
            errorLoginCanvas.enabled = false;
            errorRegCanvas.enabled = false;
            mainMenuRegisterRegCanvas.enabled = false;
            mainMenuLoginRegCanvas.enabled = false;
            connectCanvas.enabled = true;
        }
        public void Connect()
        {
            if (ip_Connection.text == "127.0.0.1" && port_Connection.text == "8080")
            {
                string url = "ws://127.0.0.1:8080/";

                ws = new WebSocket(url);
    
                ws.OnMessage += OnMessage;

                ws.Connect();
                CloseBackToLogin();
            }    
        }

        public void AuthLogin(string loginManager)
        {
            loginManager = username_Login.text + "#" + password_Login.text;

            SocketEvent socketEvent = new SocketEvent("Login", loginManager);

            string dataLoginFromClient = JsonUtility.ToJson(socketEvent);

            ws.Send(dataLoginFromClient);
        }
        public void AuthRegister(string registerManager)
        {
            registerManager = name_Reg.text + "#" + username_Reg.text + "#" + password_Reg.text + "#" + passwordAgain_Reg.text;

            SocketEvent socketEvent = new SocketEvent("Register", registerManager);

            string dataRegisterFromClient = JsonUtility.ToJson(socketEvent);

            ws.Send(dataRegisterFromClient);
        }
        public void CloseErrorWindow()
        {
            errorInputCanvas.enabled = false;
            errorLoginCanvas.enabled = false;
            errorRegCanvas.enabled = false;
        }
        public void CloseBackToLogin()
        {
            errorInputCanvas.enabled = false;
            errorLoginCanvas.enabled = false;
            errorRegCanvas.enabled = false;
            mainMenuRegisterRegCanvas.enabled = false;
            mainMenuLoginRegCanvas.enabled = true;
        }
        public void GoToRegister()
        {
            errorInputCanvas.enabled = false;
            errorLoginCanvas.enabled = false;
            errorRegCanvas.enabled = false;
            mainMenuRegisterRegCanvas.enabled = true;
            mainMenuLoginRegCanvas.enabled = false;
        }

        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (receiveMessageData.eventName == "Login")
                {
                    if (OnLogin != null)
                    {
                        OnLogin(receiveMessageData);
                    }
                    else if (receiveMessageData.data == "success")
                    {
                        SceneManager.LoadScene(1);
                    }
                    else if (receiveMessageData.data == "fail")
                    {
                        errorLoginCanvas.enabled = true;
                    }
                    else if (receiveMessageData.data == "notinput")
                    {
                        errorInputCanvas.enabled = true;
                    }
                }
                else if (receiveMessageData.eventName == "Register")
                {
                    if (OnRegister != null)
                    {
                        OnRegister(receiveMessageData);
                    }
                    else if (receiveMessageData.data == "success")
                    {
                        CloseBackToLogin();
                    }
                    else if (receiveMessageData.data == "fail")
                    {
                        errorRegCanvas.enabled = true;
                    }
                    else if (receiveMessageData.data == "notmatch")
                    {
                        errorRegCanvas.enabled = true;
                    }
                    else if (receiveMessageData.data == "notinput")
                    {
                        errorInputCanvas.enabled = true;
                    }
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