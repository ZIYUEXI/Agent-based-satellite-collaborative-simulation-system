using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    public bool DataBaseAlive = false;
    private TcpClient clientSocket;
    private TcpClient clientToRoomSocket;
    private StreamWriter writer;
    private StreamWriter writer_ToRoom;
    private DateTime dateTime;

    void Start()
    {
        try
        {
            dateTime = DateTime.Now;
            clientSocket = new TcpClient("127.0.0.1", 12345);
            NetworkStream stream = clientSocket.GetStream();
            writer = new StreamWriter(stream);
            DataBaseAlive = true;
            //clientToRoomSocket = new TcpClient("127.0.0.1", 12346);
            //NetworkStream streamToRoom = clientToRoomSocket.GetStream();
            //writer_ToRoom = new StreamWriter(streamToRoom);
        }
        catch (Exception e)
        {
            Debug.LogError("Unable to connect to server: " + e.Message);
            DataBaseAlive = false;
        }
    }
    public void sendMessage(string FromWho, string Type, string Data)
    {
        if (DataBaseAlive)
        {
            DataStructure data = new DataStructure(FromWho, Type, Data, dateTime);
            string jsonData = JsonUtility.ToJson(data);
            SendData(jsonData);
        }
    }

    void SendData(string message)
    {
        try
        {
            writer.WriteLine(message);
            writer.Flush();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send data: " + e.Message);
            // If send fails, you might want to handle it (e.g., retry or close the connection)
            DataBaseAlive = false;
        }
    }

    public void sendMessageToRoom(string FromWho, string Type, string Data)
    {
        if (DataBaseAlive)
        {
            DataStructure data = new DataStructure(FromWho, Type, Data, dateTime);
            string jsonData = JsonUtility.ToJson(data);
            SendDataToRoom(jsonData);
        }
    }

    void SendDataToRoom(string message)
    {
        try
        {
            writer.WriteLine(message);
            writer.Flush();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send data: " + e.Message);
            // If send fails, you might want to handle it (e.g., retry or close the connection)
            DataBaseAlive = false;
        }
    }

    void OnApplicationQuit()
    {
        writer.Close();
        clientSocket.Close();
    }
}


[System.Serializable]
public class DataStructure
{
    public string FromWho;
    public string Type;
    public string Data;
    public string dateTime;

    public DataStructure(string FromWho,string Type, string Data, DateTime dateTime)
    {
        this.FromWho = FromWho;
        this.Type = Type;
        this.Data = Data;
        this.dateTime = dateTime.ToString("yyyyMMdd_HHmmss");
    }
}

