using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class Network : MonoBehaviour
{

    static SocketIOComponent socket;
    public GameObject playerPrefab;
    Dictionary<string, GameObject> players;


    // Start is called before the first frame update
    void Start()
    {
        socket = GetComponent<SocketIOComponent>();
        //creating server events 
        socket.On("open", OnConnected);
        socket.On("spawn", OnSpawned);
        socket.On("move", OnMoved);
        players = new Dictionary<string, GameObject>();
    }

    void OnSpawned(SocketIOEvent e)
    {

        var player = Instantiate(playerPrefab);
        Debug.Log("Spawned" + e.data);
        players.Add(e.data["id"].ToString(), player);
        Debug.Log(players.Count);
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
        
        var id = e.data["id"].ToString();
        var player = players[id];
        Debug.Log(player.name);
        var pos = new Vector3(GetFloatFromJson(e.data,"x"), 0, GetFloatFromJson(e.data, "z"));
        var navigatePos = player.GetComponent<NavigatePos>();
        navigatePos.NavigateTo(pos);
        Debug.Log("Network player is moving" + e.data);
    }

    float GetFloatFromJson(JSONObject data, string key)
    {
        return float.Parse(data[key].ToString().Replace("\"",""));
    }

   
}
