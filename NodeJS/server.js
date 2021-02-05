const { CONNECTING } = require('ws');
var websocket = require('ws');

var callbackinitServer = () => 
{
    console.log("Server is running.");
}

var wss = new websocket.Server({port:5500}, callbackinitServer);

var wsList = []; //เก็บ client ไปที่ server    

wss.on("connection", (ws)=>
{    
    wsList.push(ws); //ใช้ connect กับ client []  

    for (var j = 0; j < wsList.length; j++)
    {
    if (wsList[j] == ws)
         {    
            console.log("client " + j + " connected");
            break;
         }
    } 
    
    ws.on("message", (data) =>
    {
        for (var j = 0; j < wsList.length; j++)        
        {
        if(wsList[j] != ws)
        {
           wsList[j].send(data + "                                             ");
           continue;
        }

        if(wsList[j] == ws)
        {
           wsList[j].send(data);         
           console.log("send from client " + j + ": " + data);
           continue;
        }
    }
    });

    ws.on("close", ()=> 
    {
        console.log("client " + j + " disconnect.");
        wsList = ArrayRemove(wsList, ws)
    });
});

function ArrayRemove(arr,value)
{
    return arr.filter((element) =>
    {
        return element != value;
    });
}

//function test(a, callback)
//{
//    callback();
//}
//var callbacktest = ()=>{
//}
//test(a, callbacktest);
