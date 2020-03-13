using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;
using System;

public class Network : MonoBehaviour
{

    static SocketIOComponent socket;
    public GameObject playerPrefab;
    public GameObject localPlayer;
    public GameObject scoreBoxPrefab;
    public GameObject scoreContainer;
    Dictionary<string, GameObject> players;
    Dictionary<string, GameObject> scores;


    // Start is called before the first frame update
    void Start()
    {
        socket = GetComponent<SocketIOComponent>();
        //creating server events 
        socket.On("open", OnConnected);
        socket.On("spawn", OnSpawned);
        socket.On("move", OnMoved);
        socket.On("disconnected", OnDisconnected);
        socket.On("requestPosition", OnRequestPosition);
        socket.On("updatePosition", OnUpdatePosition);

        socket.On("updateScore", OnUpdateScore);

        players = new Dictionary<string, GameObject>();
        scores = new Dictionary<string, GameObject>();
    }

    private void OnUpdateScore(SocketIOEvent e)
    {
        var id = e.data["id"].ToString();
        var score = scores[id];
        var nscore = e.data["score"];
        //var nScore = 

    }
    public void changeScore(int score)
    {
        JSONObject data = new JSONObject();
        data.AddField("score", score);
        socket.Emit("updateScore", data);
    }
    private void OnUpdatePosition(SocketIOEvent e)
    {
        var id = e.data["id"].ToString();
        var player = players[id];
        var pos = new Vector3(GetFloatFromJson(e.data, "x"), 0, GetFloatFromJson(e.data, "z"));
        player.transform.position = pos;
    }

    private void OnRequestPosition(SocketIOEvent e)
    {
        //sends local players position to server to update on login
        socket.Emit("updatePosition", new JSONObject(VectorToJson(localPlayer.transform.position)));
    }

    void OnSpawned(SocketIOEvent e)
    {
        var player = Instantiate(playerPrefab);
        var scorebox = Instantiate(scoreBoxPrefab) as GameObject;
        scorebox.transform.parent = scoreContainer.transform;
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

    private void OnDisconnected(SocketIOEvent e)
    {
        var player = players[e.data["id"].ToString()];
        var score = scores[e.data["id"].ToString()];
        Destroy(player);
        Destroy(score);
        scores.Remove(e.data["id"].ToString());
        players.Remove(e.data["id"].ToString());
        
    }
    //helper functions
    float GetFloatFromJson(JSONObject data, string key)
    {
        return float.Parse(data[key].ToString().Replace("\"",""));
    }

    public static string VectorToJson(Vector3 vector)
    {
        return string.Format(@"{{""x"":""{0}"",""z"":""{1}""}}", vector.x, vector.z);
    }



}
