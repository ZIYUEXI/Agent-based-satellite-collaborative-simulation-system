using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using System.Data;

public class WebSocketController : MonoBehaviour
{
    private WebSocket ws;
    string CityName;
    private Center center;

    void Start()
    {
        center = GetComponent<Center>();
        // 连接到本地 Django WebSocket 服务器
        ws = new WebSocket("ws://127.0.0.1:12346");
        ws.OnOpen += (sender, e) =>
        {
            CityName = this.gameObject.name;
            var message = JsonConvert.SerializeObject(new { type = "init", cityName = CityName });
            Debug.Log("WebSocket connected!");
            ws.Send(message);
        };

        ws.OnMessage += (sender, e) =>
        {
            center.SendMessage(e.Data);
            Debug.Log($"{e.Data}");
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket closed: " + e.Reason);
        };

        ws.Connect();
        InvokeRepeating("HearBeat", 3f, 0.8f);
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.CloseAsync();
            ws = null;
        }
    }

    void HearBeat()
    {
        var message = JsonConvert.SerializeObject(new { type = "heartbeat", cityName = CityName });
        ws.Send(message);
    }

    public void ReciveMessage(DataRecord dataRecord)
    {
        var message = JsonConvert.SerializeObject(new { type = "GotoTalk", content = dataRecord.Data,role = dataRecord.Source, whocansee = dataRecord.Target });
        ws.Send(message);
    }
}
