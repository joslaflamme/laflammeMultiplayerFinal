using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class Network : MonoBehaviour
{

    static SocketIOComponent socket;
    public GameObject playerPrefab;


    // Start is called before the first frame update
    void Start()
    {
        socket = GetComponent<SocketIOComponent>();
        //creating server events 
        socket.On("open", OnConnected);
        socket.On("spawn", OnSpawned);
        socket.On("move", OnMoved);
    }

    void OnSpawned(SocketIOEvent e)
    {
        Instantiate(playerPrefab);
    }   

    void OnConnected(SocketIOEvent e)
    {
        Debug.Log("connected to server");
        //sending generic json data to server
        JSONObject data = new JSONObject();
        data.AddField("msg", "Hello Yall");
        socket.Emit("yolo", data);
    }

    void OnMoved(SocketIOEvent e)
    {
        Debug.Log("Network player is moving" + e.data);
    }

   
}
