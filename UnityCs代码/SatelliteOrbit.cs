using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using Unity.MLAgents.Policies;
using Unity.MLAgents;
using System.IO.Abstractions;
using System.Linq;


public class SatelliteOrbit : MonoBehaviour
{
    private Dictionary<int, DateTime> encounteredSatellites = new Dictionary<int, DateTime>();

    public Dictionary<DateTime, String> encounteredMessage = new Dictionary<DateTime, String>();

    private Dictionary<int, GameObject> communicationSatellite = new Dictionary<int, GameObject>();
    private GameObject Earth; // �����Transform
    private Rigidbody satelliteRigidbody; // ���ǵ�Rigidbody���
    LineRenderer lineRenderer;

    private float Distance = 90000;
    public bool Leader = true;

    // ������������
    private float gravitationalConstant = 6.6743f; // ע�����Ϊ��ʵ������λ

    private float communicationRange = 80f;  // ͨ�ž����趨Ϊ29��λ

    public int ID;
    private StreamWriter writer;
    [HideInInspector]
    public GameObject DataBase;
    public bool linereaderOpen = false;
    private DataBase database;
    [HideInInspector]
    public bool Alive = true;
    [HideInInspector]
    public bool Communication = false;
    [HideInInspector]
    public bool Communication_Center = false;
    [HideInInspector]
    public float Engry = 100f;
    [HideInInspector]
    public bool Contrel = false;
    [HideInInspector]
    public string TargetCity;
    public float targetInclination;
    [HideInInspector]
    public List<DataRecord> dataRecords = new List<DataRecord>();
    [HideInInspector]
    public Dictionary<Guid, DataRecord> dataRecordDictionary = new Dictionary<Guid, DataRecord>();
    [HideInInspector]
    public int CommunicateNumber = 0;
    [HideInInspector]
    public float CommunicateDistanceAverage = 0;
    public Dictionary<int, DateTime> BackSatellitesDictionary()
    {
        return encounteredSatellites;
    }

    enum RotationDirection
    {
        Clockwise,
        Counterclockwise,
        Undefined  // When the vectors are parallel or one is zero
    }

    void Start()
    {
        database = DataBase.GetComponent<DataBase>();
        Earth = GameObject.Find("Earth"); // �����Transform
        satelliteRigidbody = GetComponent<Rigidbody>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (Earth == null || satelliteRigidbody == null)
        {
            Debug.LogError("SatelliteOrbit: Missing components.");
            return;
        }

        //if (Leader == true)
        //{
        //    WebAgent webAgent = GetComponent<WebAgent>();
        //    BehaviorParameters behaviorParameters = GetComponent<BehaviorParameters>();
        //    DecisionRequester decisionRequester = GetComponent<DecisionRequester>();
        //    decisionRequester.enabled = false;
        //    behaviorParameters.enabled = false;
        //    webAgent.enabled = false;


        //}



        // �������ٶ�
        Vector3 toEarth = Earth.transform.position - transform.position;
        float distance = toEarth.magnitude;
        float orbitalSpeed = Mathf.Sqrt(gravitationalConstant * Earth.GetComponent<Rigidbody>().mass / distance);

        // ����һ���������
        Vector3 randomVector = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        Vector3 perpendicularVector = Vector3.Cross(toEarth, randomVector).normalized;

        // ȷ����������������
        if (perpendicularVector == Vector3.zero)
        {
            perpendicularVector = Vector3.up;
        }

        // ��������Ĺ���ٶ�ת��Ϊ���ٶ�����
        satelliteRigidbody.velocity = perpendicularVector * orbitalSpeed;

        if (PlayerPrefs.HasKey("NumberState"))
        {
            if(PlayerPrefs.GetInt("DataLogging") == 1)
            {
                ReadyToWrite();
            }
        }
        InvokeRepeating("UpdateLinreader", 0.5f, 0.07f);
        InvokeRepeating("exChangeInfor", 10f, 0.1f);
    }

    void FixedUpdate()
    {
        Distance = Vector3.Distance(Earth.transform.position,
    transform.position);
        if (Earth != null)
        {
            ApplyGravitationalForce();
        }

        CommunicateWithOtherSatellites();
        Communication = false;

        AlignWithVelocity();

        if (Alive == false)
        {

        }
        Engry = Engry + 0.5f;
        Engry = Mathf.Min(Engry, 100f);

    }

    float CalculateAngleWithXYPlane()
    {
        Vector3 toEarth = Earth.transform.position - transform.position;
        Vector3 velocity = satelliteRigidbody.velocity;

        // Calculate the normal to the plane formed by toEarth and velocity
        Vector3 planeNormal = Vector3.Cross(toEarth, velocity).normalized;

        // The normal to the x-z plane is the y-axis
        Vector3 xyPlaneNormal = Vector3.forward;

        // Calculate the angle between the two normals
        float angle = Vector3.Angle(planeNormal, xyPlaneNormal);
        angle = Mathf.Repeat(angle, 180);
        return angle;
    }


    float CalculateAngleWithXZPlane()
    {
        Vector3 toEarth = Earth.transform.position - transform.position;
        Vector3 velocity = satelliteRigidbody.velocity;

        // Calculate the normal to the plane formed by toEarth and velocity
        Vector3 planeNormal = Vector3.Cross(toEarth, velocity).normalized;

        // The normal to the x-z plane is the y-axis
        Vector3 xzPlaneNormal = Vector3.up;

        // Calculate the angle between the two normals
        float angle = Vector3.Angle(planeNormal, xzPlaneNormal);
        angle = Mathf.Repeat(angle, 180);
        return angle;
    }
    public (int, float, float, float, float, float,float,Vector3) GetInfo()
    {
        return (ID, Distance, satelliteRigidbody.velocity.magnitude,
            CalculateAngleWithXZPlane(),CalculateAngleWithXYPlane(),
            Engry, CalculateAngleWithXZPlane(satelliteRigidbody.velocity.normalized),satelliteRigidbody.velocity.normalized);
    }
    private void AlignWithVelocity()
    {
        if (satelliteRigidbody.velocity != Vector3.zero)
        {
            transform.forward = satelliteRigidbody.velocity.normalized;  // �����ǵ�ǰ�������ٶ�����
        }
    }
    void ReadyToWrite()
    {
        // ��ȡ�û����ĵ�Ŀ¼·��
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        // ����Ŀ���ļ���·��
        string targetPath = Path.Combine(documentsPath, "LoveDataLoL");

        // ȷ��Ŀ���ļ��д���
        Directory.CreateDirectory(targetPath);

        // �����ļ���
        string startTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"{ID}_{startTime}.csv";
        string fullPath = Path.Combine(targetPath, filename);

        // ����StreamWriterʵ��
        //writer = new StreamWriter(fullPath, true);
        //writer.WriteLine("Time,X,Y,Z,Velocity,VelocityDirectionX,VelocityDirectionY,VelocityDirectionZ");
        //writer.WriteLine("Time,X,Y,Z,Velocity,Distance,CommunicationNumber");
        //writer.Flush();
        //database.sendMessage(gameObject.name, "SatelliteData",  "Time,X,Y,Z,Velocity,Distance,CommunicationNumber");

        // ��ʼ��¼���ݣ�ÿ0.5���¼һ��
        InvokeRepeating("LogData", 0.5f, 0.5f);
    }
    private void ApplyGravitationalForce()
    {
        Vector3 toEarth = Earth.transform.position - transform.position;
        float distance = toEarth.magnitude;
        float forceMagnitude = gravitationalConstant * (satelliteRigidbody.mass * Earth.GetComponent<Rigidbody>().mass) / (distance * distance);
        Vector3 force = toEarth.normalized * forceMagnitude;

        satelliteRigidbody.AddForce(force);
    }
    void LogData()
    {
        Vector3 position = transform.position;
        Vector3 velocity = satelliteRigidbody.velocity;
        string logTime = DateTime.Now.ToString("HH:mm:ss.fff");
        //writer.WriteLine($"{logTime},{position.x},{position.y},{position.z},{velocity.magnitude},{Distance},{CommunicateNumber}");
        //writer.Flush();
        //database.sendMessage(gameObject.name, "SatelliteData", $"{logTime},{position.x},{position.y},{position.z},{velocity.magnitude},{Distance},{CommunicateNumber}");
        database.sendMessage(gameObject.name, "SatelliteData", JsonUtility.ToJson(new SatelliteDataStructure(logTime, position.x, position.y, position.z, velocity.magnitude, Distance, CommunicateNumber)));
    }
    void CommunicateWithOtherSatellites()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, communicationRange);
        List<int> IDs = new List<int>();

        foreach (var hitCollider in hitColliders)
        {

            if (hitCollider.gameObject != gameObject && hitCollider.gameObject.CompareTag("SpaceObject"))
            {

                SatelliteOrbit otherSatellite = hitCollider.gameObject.GetComponent<SatelliteOrbit>();
                if (!communicationSatellite.ContainsKey(otherSatellite.ID))
                {
                    communicationSatellite.Add(otherSatellite.ID, hitCollider.gameObject);
                }
                IDs.Add(otherSatellite.ID);
                if (otherSatellite != null)
                {
                    int otherID = otherSatellite.ID;
                    // ���ҵ���������ʱ����¼ͨ�ţ����Ҽ�¼����������
                    if (this.encounteredMessage.Count > 0)
                    {
                        foreach (var entry in this.encounteredMessage)
                        {
                            // ���Է��ֵ����Ƿ��Ѵ�����ͬ�ļ�
                            if (!otherSatellite.encounteredMessage.ContainsKey(entry.Key))
                            {
                                otherSatellite.encounteredMessage.Add(entry.Key, entry.Value);
                            }
                        }
                    }

                    if (encounteredSatellites.ContainsKey(otherID))
                    {
                        // ����Ѿ�����������������ʱ��
                        encounteredSatellites[otherID] = DateTime.Now;
                        // Debug.Log($"Updated communication time with Satellite ID: {otherID} at {DateTime.Now}");
                    }
                    else
                    {
                        // ������״���������ӵ��ֵ���
                        encounteredSatellites.Add(otherID, DateTime.Now);
                        // Debug.Log($"Communicating with Satellite ID: {otherID} at {DateTime.Now}");
                    }
                    Communication = true;
                    //�ϲ������б�ȥ�أ�
                    //MergeDataRecords(this.dataRecords,otherSatellite.dataRecords);

                }
            }
        }
        CleanDictionary(communicationSatellite, IDs);

    }

    public void UpdateLinreader()
    {
        if(linereaderOpen == false)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
        }
        float totalDistance = 0f;
        lineRenderer.positionCount = communicationSatellite.Count + 1;
        lineRenderer.SetPosition(0, transform.position);
        int a = 1;
        foreach (GameObject gameObject in communicationSatellite.Values)
        {
            lineRenderer.SetPosition(a, gameObject.transform.position);
            a++;
            totalDistance += Vector3.Distance(transform.position, gameObject.transform.position);
        }
        CommunicateNumber = a - 1;
        CommunicateDistanceAverage = (CommunicateNumber > 0) ? totalDistance / CommunicateNumber : 0f;
    }
    public void MergeDictionaries(Dictionary<Guid, DataRecord> targetDictionary, Dictionary<Guid, DataRecord> sourceDictionary)
    {
        foreach (var kvp in sourceDictionary)
        {
            if (!targetDictionary.ContainsKey(kvp.Key))
            {
                targetDictionary.Add(kvp.Key, kvp.Value);
            }
        }
    }
    public void AdjustOrbitDirection(float angle)
    {
        Vector3 currentPosition = transform.position;
        Vector3 currentVelocityDir = satelliteRigidbody.velocity.normalized;
        Vector3 toEarth = Earth.transform.position - currentPosition;
        float currentEquatorAngle = CalculateAngleWithXZPlane();
        angle = Mathf.Repeat(angle, 360);
        Vector3 rotationAxis = toEarth.normalized;
        float angleRad = angle * Mathf.Deg2Rad;
        Vector3 rotatedVelocity = currentVelocityDir * Mathf.Cos(angleRad) +
                                  Vector3.Cross(rotationAxis, currentVelocityDir) * Mathf.Sin(angleRad) +
                                  rotationAxis * Vector3.Dot(rotationAxis, currentVelocityDir) * (1 - Mathf.Cos(angleRad));
        satelliteRigidbody.velocity = rotatedVelocity * satelliteRigidbody.velocity.magnitude;
    }

    public void AdjustOrbitDirectionWithQuaternion(float angle)
    {
        Vector3 currentPosition = transform.position;
        Vector3 currentVelocityDir = satelliteRigidbody.velocity.normalized;
        Vector3 toEarth = Earth.transform.position - currentPosition;

        float angleWithXZPlane = CalculateAngleWithXZPlane(currentVelocityDir);

        //Debug.Log(angleWithXZPlane);

        // ʹ����Ԫ������һ����toEarth������תangleRad���ȵ���ת
        Quaternion rotation = Quaternion.AngleAxis(angle, toEarth.normalized);

        // ʹ����Ԫ����ת��ǰ�ٶ�����
        Vector3 rotatedVelocity = rotation * currentVelocityDir;

        // Ӧ���µ��ٶ�������������ԭ�����ٶȴ�С
        satelliteRigidbody.velocity = rotatedVelocity * satelliteRigidbody.velocity.magnitude;
    }

    public void ChangeSpeed(float speed)
    {
        float NewSpeed = satelliteRigidbody.velocity.magnitude + speed;

        NewSpeed = Mathf.Clamp(NewSpeed, -100, 100);
        satelliteRigidbody.velocity = satelliteRigidbody.velocity.normalized * NewSpeed;
    }


    public void AdjustVelocityToBePerpendicular()
    {
        // ���� toEarth ����
        Vector3 toEarth = Earth.transform.position - transform.position;
        float distance = toEarth.magnitude;

        Vector3 currentVelocity = satelliteRigidbody.velocity.normalized;
        // ����������֮��ļн�
        float angleBetween = Mathf.Acos(Vector3.Dot(currentVelocity, toEarth) / (currentVelocity.magnitude * toEarth.magnitude));
        float angleToRotate = 90 - angleBetween * Mathf.Rad2Deg;

        // ������ת��
        Vector3 rotationAxis = Vector3.Cross(currentVelocity, toEarth).normalized;

        // ������ת
        Quaternion rotation = Quaternion.AngleAxis(-angleToRotate, rotationAxis);

        // Ӧ����ת
        Vector3 newVelocity = rotation * currentVelocity;

        float orbitalSpeed = Mathf.Sqrt(gravitationalConstant * Earth.GetComponent<Rigidbody>().mass / distance);
        try
        {
            satelliteRigidbody.velocity = newVelocity * orbitalSpeed; 
        }
        catch
        {
            return;
        }
    }

    public void AdjustOrbit()
    {
        Vector3 toEarth = Earth.transform.position - transform.position;
        float distance = toEarth.magnitude;
        float orbitalSpeed = Mathf.Sqrt(gravitationalConstant * Earth.GetComponent<Rigidbody>().mass / distance);
        Vector3 perpendicularVector = Vector3.Cross(toEarth, satelliteRigidbody.velocity.normalized).normalized;
        satelliteRigidbody.velocity = perpendicularVector * orbitalSpeed;
    }

    void exChangeInfor()
    {
        foreach(var key in communicationSatellite.Keys)
        {
            try
            {
                SatelliteOrbit otherSatellite = communicationSatellite[key].gameObject.GetComponent<SatelliteOrbit>();
                MergeDictionaries(this.dataRecordDictionary, otherSatellite.dataRecordDictionary);
            }
            catch (Exception e)
            {
                continue;
            }
        }
    }

    void CleanDictionary(Dictionary<int, GameObject> dict, List<int> keysToKeep)
    {
        List<int> keysToRemove = new List<int>();

        // ȷ��Ҫɾ���ļ�
        foreach (var key in dict.Keys)
        {
            if (!keysToKeep.Contains(key))
            {
                keysToRemove.Add(key);
            }
        }

        // ɾ��δ�������ļ�
        foreach (var key in keysToRemove)
        {
            dict.Remove(key);
        }
    }

    private float CalculateAngleWithXZPlane(Vector3 velocity)
    {
        // �� xz ƽ���ϵ�ͶӰ
        Vector3 velocityXZ = new Vector3(velocity.x, 0, velocity.z);

        // ����ͶӰ��ԭ�ٶ������ļн�
        float angle = Vector3.Angle(velocity, velocityXZ);

        return angle;
    }
}
