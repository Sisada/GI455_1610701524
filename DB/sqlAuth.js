const sqlite = require('sqlite3').verbose();
const app = require('express')();
const server = require('http').Server(app);
const websocket = require('ws');
const wss = new websocket.Server({server});

server.listen(process.env.PORT || 8080, ()=>{
    console.log("Server start at port "+server.address().port);
});

var roomList = [];
var loginsuc = false;

wss.on("connection", (ws)=>
{
    console.log("client connected");
    
    ws.on("message", (data)=>
    {
        let db = new sqlite.Database('./db_folder/authDB.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>
        {
            if (err) throw err;
        
            console.log('Connected to database.');

            var dataLoginFromClient =
            {
                eventName:"",
                data:""
            }

            dataLoginFromClient = JSON.parse(data);

            var dataRegisterFromClient =
            {
                eventName:"",
                data:""
            }

            dataRegisterFromClient = JSON.parse(data);

            if(dataLoginFromClient.eventName == "Login")
            {
                var splitStrLogin = dataLoginFromClient.data.split('#');
        
                var usernameid_Login = splitStrLogin[0];
                var password_Login = splitStrLogin[1];
                
                if (usernameid_Login == "" || password_Login == "")
                {
                    var callbackMsg =
                    {
                        eventName:"Login",
                        data:"notinput"
                    }
                    var dataLoginFromClient = JSON.stringify(callbackMsg);
                    ws.send(dataLoginFromClient);

                    console.log("[Login]" + dataLoginFromClient);
                }
                else if ((usernameid_Login != "" || password_Login != ""))
                {
                    var sqlLogin = "SELECT * FROM AuthData WHERE UsernameID= '"+usernameid_Login+"' AND Password= '"+password_Login+"' ";
        
                db.all(sqlLogin, (err,rows)=>
                {
                    if(err)
                    {
                         console.log(err);
                    }
                    else
                    {
                        if(rows.length > 0)
                        {
                            var callbackMsg =
                            {
                                eventName:"Login",
                                data:"success",
                                Name:rows[0].Name
                            }
        
                            var dataLoginFromClient = JSON.stringify(callbackMsg);
                            ws.send(dataLoginFromClient);
                            loginsuc = true;

                            if(loginsuc == true)
                            {
                                var toJsonObj = { 
                                    roomName:"",
                                    data:""
                                }
                                toJsonObj = JSON.parse(data);
                                
                                if(toJsonObj.eventName == "CreateRoom")//CreateRoom
                                {
                                    //============= Find room with roomName from Client =========
                                    var isFoundRoom = false;
                                    for(var i = 0; i < roomList.length; i++)
                                    {
                                        if(roomList[i].roomName == toJsonObj.data)
                                        {
                                            isFoundRoom = true;
                                            break;
                                        }
                                    }
                                    //===========================================================
                        
                                    if(isFoundRoom == true)// Found room
                                    {
                                        //Can't create room because roomName is exist.
                                        //========== Send callback message to Client ============
                        
                                        //ws.send("CreateRoomFail"); 
                        
                                        //I will change to json string like a client side. Please see below
                                        var callbackMsg = {
                                            eventName:"CreateRoom",
                                            data:"fail"
                                        }
                                        var toJsonStr = JSON.stringify(callbackMsg);
                                        ws.send(toJsonStr);
                                        //=======================================================
                        
                                        console.log("client create room fail.");
                                    }
                                    else
                                    {
                                        //============ Create room and Add to roomList ==========
                                        var newRoom = {
                                            roomName: toJsonObj.data,
                                            wsList: []
                                        }
                        
                                        newRoom.wsList.push(ws);
                        
                                        roomList.push(newRoom);
                                        //=======================================================
                        
                                        //========== Send callback message to Client ============
                        
                                        //ws.send("CreateRoomSuccess");
                        
                                        //I need to send roomName into client too. I will change to json string like a client side. Please see below
                                        var callbackMsg = {
                                            eventName:"CreateRoom",
                                            data:toJsonObj.data
                                        }
                                        var toJsonStr = JSON.stringify(callbackMsg);
                                        ws.send(toJsonStr);
                                        //=======================================================
                                        console.log("client create room success.");
                                    }
                        
                                    //console.log("client request CreateRoom ["+toJsonObj.data+"]");
                                    
                                }
                                else if(toJsonObj.eventName == "JoinRoom")//JoinRoom
                                {
                                    //============= Home work ================
                                    // Implementation JoinRoom event when have request from client.
                                    
                                    //================= Hint =================
                                    //roomList[i].wsList.push(ws);
                                    for(var i = 0; i < roomList.length; i++)
                                    {
                                        if(roomList[i].roomName == toJsonObj.data)
                                        {
                                            roomList[i].wsList.push(ws);
                        
                                            var callbackMsg = {
                                                eventName:"JoinRoom",
                                                data:toJsonObj.data
                                            }
                                            var toJsonStr = JSON.stringify(callbackMsg);
                                            ws.send(toJsonStr);
                                            
                                            console.log("client join room success.")
                                            break;
                                        }
                                        if(roomList[i].roomName != toJsonObj.data)
                                        {
                                            var callbackMsg = {
                                                eventName:"JoinRoom",
                                                data:"fail"
                                            }
                        
                                            var toJsonStr = JSON.stringify(callbackMsg);
                                            ws.send(toJsonStr);
                        
                                            console.log("client join room fail")
                                            break;
                                        }
                                    }
                        
                                    //============= Find room with roomName from Client =========
                        
                                    console.log("client request JoinRoom");
                                    //========================================
                                }
                                else if(toJsonObj.eventName == "LeaveRoom")//LeaveRoom
                                {
                                    //============ Find client in room for remove client out of room ================
                                    var isLeaveSuccess = false;//Set false to default.
                                    for(var i = 0; i < roomList.length; i++)//Loop in roomList
                                    {
                                        for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
                                        {
                                            if(ws == roomList[i].wsList[j])//If founded client.
                                            {
                                                roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.
                        
                                                if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                                                {
                                                    roomList.splice(i, 1);//Remove at index one time. When room is no one left.
                                                }
                                                isLeaveSuccess = true;
                                                break;
                                            }
                                        }
                                    }
                                    //===============================================================================
                        
                                    if(isLeaveSuccess)
                                    {
                                        //========== Send callback message to Client ============
                        
                                        //ws.send("LeaveRoomSuccess");
                        
                                        //I will change to json string like a client side. Please see below
                                        var callbackMsg = {
                                            eventName:"LeaveRoom",
                                            data:"success"
                                        }
                                        var toJsonStr = JSON.stringify(callbackMsg);
                                        ws.send(toJsonStr);
                                        //=======================================================
                        
                                        console.log("leave room success");
                                    }
                                    else
                                    {
                                        //========== Send callback message to Client ============
                        
                                        //ws.send("LeaveRoomFail");
                        
                                        //I will change to json string like a client side. Please see below
                                        var callbackMsg = {
                                            eventName:"LeaveRoom",
                                            data:"fail"
                                        }
                                        var toJsonStr = JSON.stringify(callbackMsg);
                                        ws.send(toJsonStr);
                                        //=======================================================
                        
                                        console.log("leave room fail");
                                    }
                                }
                            }

                            console.log("[Login]" + dataLoginFromClient);
                        }
                        else
                        {
                            var callbackMsg =
                            {
                                eventName:"Login",
                                data:"fail"
                            }
                            var dataLoginFromClient = JSON.stringify(callbackMsg);
                            ws.send(dataLoginFromClient);

                            console.log("[Login]" + dataLoginFromClient);
                        }
                    }
                });
                }
            }

            if(dataRegisterFromClient.eventName == "Register")
            {
                var splitStrRegister = dataRegisterFromClient.data.split('#');
        
                var name_Reg = splitStrRegister[0];
                var usernameID_Reg = splitStrRegister[1];
                var password_Reg = splitStrRegister[2];
                var passwordAgain_Reg = splitStrRegister[3];
                
                if(name_Reg == "" || usernameID_Reg == "" || password_Reg == "" || passwordAgain_Reg == "")
                {
                    var callbackMsg =
                    {
                        eventName:"Register",
                        data:"notinput"
                    }
                    var dataRegisterFromClient = JSON.stringify(callbackMsg);
                    ws.send(dataRegisterFromClient);

                    console.log("[Register]" + dataRegisterFromClient);

                    return;
                }

                else if (password_Reg != passwordAgain_Reg)
                {
                    var callbackMsg =
                    {
                        eventName:"Register",
                        data:"notmatch"
                    }
                    var dataRegisterFromClient = JSON.stringify(callbackMsg);
                    ws.send(dataRegisterFromClient);

                    console.log("[Register]" + dataRegisterFromClient);

                    return;
                }

                else if (password_Reg == passwordAgain_Reg )
                {
                    var sqlRegister = "INSERT INTO AuthData (UsernameID, Name, Password) VALUES('"+usernameID_Reg+"', '"+name_Reg+"', '"+password_Reg+"')";
        
                    db.all(sqlRegister, (err,rows)=>
                    {
                        if(err)
                        {
                            var callbackMsg =
                            {
                                eventName:"Register",
                                data:"fail"
                            }
                            var dataRegisterFromClient = JSON.stringify(callbackMsg);
                            ws.send(dataRegisterFromClient);
    
                            console.log("[Register]" + dataRegisterFromClient);
                        }
                        else
                        {
                            if(rows.length >= 0)
                            {
                                var callbackMsg =
                                {
                                    eventName:"Register",
                                    data:"success",
                                }
            
                                var dataRegisterFromClient = JSON.stringify(callbackMsg);
                                ws.send(dataRegisterFromClient);
    
                                console.log("[Register]" + dataRegisterFromClient);
                            }
                            else
                            {
                                var callbackMsg =
                                {
                                    eventName:"Register",
                                    data:"fail"
                                }
                                var dataRegisterFromClient = JSON.stringify(callbackMsg);
                                ws.send(dataRegisterFromClient);
    
                                console.log("[Register]" + dataRegisterFromClient);
                            }
                        }
                    });
                }
            }
        });
    });
});