using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class SatelliteGenerator : MonoBehaviour
{
    // Public variable to hold the Earth GameObject
    public GameObject Earth;

    // Public variable to hold the Satellite prefab
    public GameObject SatellitePrefab;

    public GameObject DataBase;

    // Public variable to control the number of satellites
    public int NumberOfSatellites; // 默认生成5个卫星，你可以在Unity编辑器中调整这个数量

    private List<GameObject> satellites = new List<GameObject>();

    private List<GameObject> City = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("NumberState"))
        {
            int Number = 0;
            if (int.TryParse(PlayerPrefs.GetString("NumberState"), out Number))
            {
                NumberOfSatellites = Number;
                PlayerPrefs.SetString("NumberState", "None");
            }
        }
        AddAllCityObjects();
    }

    void AddAllCityObjects()
    {
        GameObject[] cityObjects = GameObject.FindGameObjectsWithTag("CityObject");
        foreach (GameObject cityObject in cityObjects)
        {
            City.Add(cityObject);
        }
    }
    private Dictionary<string, (Vector3 coordinates, float xzAngle, float xyAngle, float distance)> cityData = new Dictionary<string, (Vector3, float, float, float)>()
{
    {"洛杉矶 (Los Angeles)", (new Vector3(250.77f, -5.95f, 371.57f), 127.4236f, 64.05233f, 380f)},
    {"东京 (Tokyo)", (new Vector3(349.51f, -8.12f, -189.42f), 149.9566f, 115.5607f, 380f)},
    {"加尔各答 (Calcutta)", (new Vector3(-50.96f, -5.65f, -380.55f), 90.34596f, 82.36844f, 380f)},
    {"北京 (Beijing)", (new Vector3(165.03f, 39.06f, -399.97f), 126.9178f, 110.7548f, 380f)},
    {"上海 (Shanghai)", (new Vector3(150.60f, 2.67f, -438.42f), 98.07912f, 108.8094f, 380f)},
    {"布宜诺斯艾利斯 (Buenos Aires)", (new Vector3(-170.72f, -331.82f, 259.09f), 92.33385f, 125.8465f, 380f)},
    {"伦敦 (London)", (new Vector3(-80.59f, 348.84f, 148.11f), 69.3007f, 111.3167f, 380f)},
    {"开罗 (Cairo)", (new Vector3(-308.03f, 219.88f, 284.01f), 151.951f, 68.05673f, 380f)},
    {"华盛顿 (Washington)", (new Vector3(285.37f, 124.09f, -276.64f), 111.9783f, 137.8896f, 380f)},
    {"惠灵顿 (Wellington)", (new Vector3(-17.61f, -386.86f, 28.30f), 92.4777f, 151.9431f, 380f)},
    {"博卡奇卡 (Boca Chica)", (new Vector3(-404.77f, 5.68f, -235.93f), 165.0641f, 77.49395f, 380f)}
};

    private (Vector3, float, float, float) GetRandomCityData()
    {
        List<string> keys = new List<string>(cityData.Keys);
        string randomKey = keys[UnityEngine.Random.Range(0, keys.Count)];
        var (coordinates, xzAngle, xyAngle, distance) = cityData[randomKey];
        Debug.Log($"Random City: {randomKey}, Coordinates: {coordinates}, XZ Angle: {xzAngle}, XY Angle: {xyAngle}, Distance: {distance}");
        return (coordinates, xzAngle, xyAngle, distance);
    }

    private List<(Vector3, float, float, float)> GetTargetCityGroup(int NumberSatellite)
    {
        List<(Vector3, float, float, float)> values = new List<(Vector3, float, float, float)> ();
        List<string> keys = new List<string>(cityData.Keys);
        int GroupEach = NumberSatellite / keys.Count;
        int AndNumber = NumberSatellite % keys.Count;

        foreach (KeyValuePair<string, (Vector3 coordinates, float xzAngle, float xyAngle, float distance)> kvp in cityData)
        {
            for (int i = 0; i < GroupEach; i++)
            {
                values.Add(kvp.Value);
            }
        }
        for(int i = 0;i < AndNumber; i++)
        {
            string randomKey = keys[UnityEngine.Random.Range(0, keys.Count)];
            values.Add(cityData[randomKey]);
        }
        return values;
    }

    private List<GameObject> GetGameObjectsCity(int NumberSatellite)
    {
        List<GameObject> values = new List<GameObject>();
        int GroupEach = NumberSatellite / City.Count;
        int AndNumber = NumberSatellite % City.Count;

        foreach(GameObject gameObject in City)
        {
            for (int i = 0; i < GroupEach; i++)
            {
                values.Add(gameObject);
            }
        }
        for (int i = 0; i < AndNumber; i++)
        {
            int randomKey = UnityEngine.Random.Range(0, City.Count);
            values.Add(City[randomKey]);
        }
        return values;
    }
    public void GenerateAgents()
    {
        //List<GameObject> values = GetGameObjectsCity(NumberOfSatellites);
        //System.Random random = new System.Random();

        // 生成一个在0到99之间的随机整数（包括0，不包括100）
        //int randomNumber = random.Next(0, NumberOfSatellites);
        for (int i = 0; i < NumberOfSatellites; i++)
        {
            //正常跑
            List<GameObject> cityList = new List<GameObject>(City);
            cityList = Shuffle(cityList);
            //训练
            //List<GameObject> cityList = City;
            //cityList = Shuffle(cityList);


            // Generate a random distance between 400f and 600f
            float distance = UnityEngine.Random.Range(500f, 600f);

            // Generate a random direction from the Earth
            Vector3 randomDirection = UnityEngine.Random.onUnitSphere;

            // Calculate the position to place the satellite
            Vector3 satellitePosition = Earth.transform.position + randomDirection * distance;

            GameObject satelliteInstance = Instantiate(SatellitePrefab, satellitePosition, Quaternion.identity);

            // Initialize values on the new satellite instance
            satelliteInstance.name = "Satellite_" + i;  // Set a unique name for each satellite
            SatelliteOrbit satelliteScript = satelliteInstance.GetComponent<SatelliteOrbit>();  // Assume there's a Satellite component
            WebAgentNew webAgent = satelliteInstance.GetComponent<WebAgentNew>();
            // Example of initializing some properties or methods
            if (satelliteScript != null)
            {
                satelliteScript.ID = i;  // Custom initialization method
                satelliteScript.DataBase = DataBase;
            }
            if (webAgent != null)
            {
                webAgent.TargetCityObjectList = cityList;
                //Debug.Log(cityList.ToString());
            }
            satellites.Add(satelliteInstance);
        }
    }

    public void DestroyAgents()
    {
        // 检查卫星列表是否非空且包含元素
        if (satellites != null && satellites.Count > 0)
        {
            foreach (GameObject satellite in satellites)
            {
                Destroy(satellite);  // 销毁每个卫星对象
            }
            satellites.Clear();  // 清空列表
        }
    }

    List<T> Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random(); // 创建随机数生成器
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

}
