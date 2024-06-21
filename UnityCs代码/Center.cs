using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Awaitable;


public class Center : MonoBehaviour
{
    // Define an enumeration for different types of centers

    Queue<DataRecord> datarecordQueue = new Queue<DataRecord>();
    Queue<DataRecord> ReciveQueue = new Queue<DataRecord>();
    [HideInInspector]
    public List<Guid> ReciveUid = new List<Guid>();
    public GameObject DataBase;
    private DataBase database;

    List<string> cityObjectNames = new List<string>();
    List<string> stringLine = new List<string>();

    List<CityDataTimeStructure> cityDataTimeStructures = new List<CityDataTimeStructure>();
    List<Guid> finishUID = new List<Guid>();

    private string CityName;
    private StreamWriter writer;
    string fullPath_communication;

    [HideInInspector]
    public List<GameObject> childStates = new List<GameObject>();
    private Dictionary<Guid, (string, DateTime)> latencyTestData = new Dictionary<Guid, (string, DateTime)>();
    private WebSocketController websocketController;
    void Start()
    {
        websocketController = GetComponent<WebSocketController>();
        database = DataBase.GetComponent<DataBase>();
        // Ensure a Collider component is attached and its isTrigger is set to true
        string title = "Time,Los Angeles,Tokyo,Calcutta,Beijing,Shanghai,Buenos Aires,London,Cairo,Washington,Wellington,Boca Chica";
        stringLine.Add(title);
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("Center script requires a Collider component.");
        }
        else if (!collider.isTrigger)
        {
            Debug.LogWarning("Collider's isTrigger property is not set. Setting it now.");
            collider.isTrigger = true; // Ensure the Collider is a trigger for safety
        }
        GameObject[] cityObjects = GameObject.FindGameObjectsWithTag("CityObject");
        foreach (GameObject cityObject in cityObjects)
        {
            if(cityObject != gameObject)
            {
                cityObjectNames.Add(cityObject.name);
            }
        }

        CityName = gameObject.name;
        ReadyToWrite();
        InvokeRepeating("SendLatencyTest", 0f, 60f);
        InvokeRepeating("WrightData", 60f, 60f);
    }

    public void SendMessage(string message)
    {
        foreach (string name in cityObjectNames)
        {
            Debug.Log($"{name}");
            try
            {
                DataRecord kaka = new DataRecord(DataType.Communication, name, CityName, message);
                Debug.Log(message);
                datarecordQueue.Enqueue(kaka);
            }
            catch (Exception e)
            {
                Debug.Log("Caught exception: " + e.ToString());
            }
        }
    }

    void SendLatencyTest()
    {
        string TargetLine = $"{DateTime.UtcNow}";
        List<string> strings = new List<string>(new string[11]);
        CityDataTimeStructure cdts = new CityDataTimeStructure(TargetLine);
        foreach (string name in cityObjectNames)
        {
            Guid DataUid = Guid.NewGuid();
            DataRecord dataRecord = new DataRecord(DataType.LatencyTest, name,gameObject.name, DataUid.ToString());
            datarecordQueue.Enqueue(dataRecord);
            latencyTestData.Add(DataUid, (name, dataRecord.CreationTime));
            cdts.AddCity(new CityDataStructure(name , DataUid.ToString()));
            //"Los Angeles,Tokyo,Calcutta,Beijing,Shanghai,Buenos Aires,London,Cairo,Washington,Wellington,Boca Chica"
            switch (name)
            {
                case "LosAngeles":
                    strings[0] = DataUid.ToString();
                    break;
                case "Tokyo":
                    strings[1] = DataUid.ToString();
                    break;
                case "Calcutta":
                    strings[2] = DataUid.ToString();
                    break;
                case "Beijing":
                    strings[3] = DataUid.ToString();
                    break;
                case "Shanghai":
                    strings[4] = DataUid.ToString();
                    break;
                case "BuenosAires":
                    strings[5] = DataUid.ToString();
                    break;
                case "London":
                    strings[6] = DataUid.ToString();
                    break;
                case "Cairo":
                    strings[7] = DataUid.ToString();
                    break;
                case "Washington":
                    strings[8] = DataUid.ToString();
                    break;
                case "Wellington":
                    strings[9] = DataUid.ToString();
                    break;
                case "BocaChica":
                    strings[10] = DataUid.ToString();
                    break;
            }
        }
        for (int i = 0; i < strings.Count; i++)
        {
            TargetLine = TargetLine + "," + strings[i];
        }
        stringLine.Add(TargetLine);
        //database.sendMessage(gameObject.name, "CityData", JsonUtility.ToJson(cdts));
        cityDataTimeStructures.Add(cdts);
    }

    void SendRelay(string target,string data)
    {
        DataRecord dataRecord = new DataRecord(DataType.Reply, target, gameObject.name, data);
        //Debug.Log(dataRecord.ToString());
        datarecordQueue.Enqueue(dataRecord);
    }

    void WrightData()
    {
        //StreamWriter Writer_Communication = new StreamWriter(fullPath_communication, false);
        //for(int i = 0; i < stringLine.Count; i++)
        //{
        //    Writer_Communication.WriteLine(stringLine[i]);
        //}
        //Writer_Communication.Flush();
        for(int i = 0; i < cityDataTimeStructures.Count; i++)
        {
            database.sendMessage(gameObject.name, "CityData", JsonUtility.ToJson(cityDataTimeStructures[i]));
        }
    }

    // Update is called once per frame
    void Update()
    {
        while(ReciveQueue.Count > 0)
        {
            DataRecord dataRecord = ReciveQueue.Dequeue();
            finishUID.Add(dataRecord.UID);
            if (dataRecord.Datatype == DataType.LatencyTest)
            {
                SendRelay(dataRecord.Source,dataRecord.Data);
            }
            if(dataRecord.Datatype == DataType.Reply)
            {

                Guid.TryParse(dataRecord.Data, out Guid DataUidConverted);
                var (targetname, currenttime) = latencyTestData[DataUidConverted];
                //cityDataTimeStructures
                DateTime CreationTime = DateTime.UtcNow;
                TimeSpan timeDifference = CreationTime.Subtract(currenttime);
                double Milliseconds = timeDifference.TotalMilliseconds / 2;
                //Debug.Log($"城市:{targetname}||时间差:{Milliseconds}毫秒");
                latencyTestData.Remove(DataUidConverted);
                for (int i = 0; i < cityDataTimeStructures.Count; i++)
                {
                    for(int j = 0; j < cityDataTimeStructures[i].cityData.Count; j++)
                    {
                        if (dataRecord.Data == cityDataTimeStructures[i].cityData[j].TimeOrUid)
                        {
                            cityDataTimeStructures[i].cityData[j].TimeOrUid = Milliseconds.ToString();
                            break;
                        }
                    }
                }
                //for (int i = 0; i < stringLine.Count; i++)
                //{
                //    string originalString = stringLine[i];
                //    string modifiedString = originalString.Replace(dataRecord.Data, Milliseconds.ToString());

                //    // Check if the string was modified
                //    if (!originalString.Equals(modifiedString))
                //    {
                //        stringLine[i] = modifiedString;
                //        Debug.Log(stringLine[i]);
                //    }
                //}
            }

            if(dataRecord.Datatype == DataType.Communication)
            {
                websocketController.ReciveMessage(dataRecord);
            }
        }
    }

    void ReadyToWrite()
    {
        // 获取用户的文档目录路径
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        // 创建目标文件夹路径
        string targetPath = Path.Combine(documentsPath, "LoveDataLoL");

        string cityPath = Path.Combine(targetPath, "City");
        string communicationPath = Path.Combine(targetPath, "Communication");

        Directory.CreateDirectory(cityPath);

        Directory.CreateDirectory(communicationPath);

        // 创建文件名
        string startTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"{CityName}_{startTime}.csv";
        string fullPath_city = Path.Combine(cityPath, filename);
        fullPath_communication = Path.Combine(communicationPath, filename);

        // 创建StreamWriter实例
        writer = new StreamWriter(fullPath_city, true);
        writer.WriteLine("Time,Communication");
        writer.Flush();
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpaceObject"))
        {
            SatelliteOrbit satellite = other.GetComponent<SatelliteOrbit>();
            if (satellite != null && satellite.dataRecordDictionary.Count != 0)
            {
                // 创建一个临时列表来存储需要移除的键
                var keysToRemove = new List<Guid>();

                foreach (var kvp in satellite.dataRecordDictionary)
                {
                    var uid = kvp.Key;
                    var record = kvp.Value;

                    if (record.Target == gameObject.name)
                    {
                        if (!ReciveUid.Contains(uid))
                        {
                            ReciveQueue.Enqueue(record);
                            ReciveUid.Add(uid);
                        }
                        keysToRemove.Add(uid);
                    }
                }

                // 从字典中移除所有已处理的记录
                foreach (var key in keysToRemove)
                {
                    satellite.dataRecordDictionary.Remove(key);
                }
            }

            //if (satellite != null && satellite.dataRecords.Count != 0)
            //{
            //    for (int i = 0; i < satellite.dataRecords.Count; i++)
            //    {
            //        if(satellite.dataRecords[i].Target == gameObject.name)
            //        {
            //            if (!ReciveUid.Contains(satellite.dataRecords[i].UID))
            //            {
            //                ReciveQueue.Enqueue(satellite.dataRecords[i]);
            //                ReciveUid.Add(satellite.dataRecords[i].UID);
            //            }
            //            satellite.dataRecords.RemoveAt(i);
            //            i--;
            //        }
            //    }
            //}
            //if (satellite != null && datarecordQueue.Count != 0)
            //{
            //    while(datarecordQueue.Count > 0)
            //    {
            //        satellite.dataRecords.Add(datarecordQueue.Dequeue());
                    
            //    }
            //}

            if (satellite != null && datarecordQueue.Count != 0)
            {
                while (datarecordQueue.Count > 0)
                {
                    var record = datarecordQueue.Dequeue();
                    if (!satellite.dataRecordDictionary.ContainsKey(record.UID))
                    {
                        satellite.dataRecordDictionary.Add(record.UID, record);
                    }
                }
            }
            WebAgentNew webAgentNew = satellite.gameObject.GetComponent<WebAgentNew>();
            webAgentNew.comunication = true;
            webAgentNew.comunication_city = gameObject;
            //if (childStates.Contains(satellite.gameObject))
            //{
            //    WebAgentNew webAgentNew = satellite.gameObject.GetComponent<WebAgentNew>();
            //    webAgentNew.comunication = true;
            //}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SpaceObject"))
        {
            SatelliteOrbit satellite = other.GetComponent<SatelliteOrbit>();
            WebAgentNew webAgentNew = satellite.gameObject.GetComponent<WebAgentNew>();
            webAgentNew.comunication = false;
            webAgentNew.comunication_city = null;
            //if (childStates.Contains(satellite.gameObject))
            //{
            //    WebAgentNew webAgentNew = satellite.gameObject.GetComponent<WebAgentNew>();
            //    webAgentNew.comunication = false;
            //}
        }
    }



    void LogData()
    {
        string logTime = DateTime.Now.ToString("HH:mm:ss.fff");
        writer.WriteLine($"{logTime},{1}");
        writer.Flush();
    }
}
