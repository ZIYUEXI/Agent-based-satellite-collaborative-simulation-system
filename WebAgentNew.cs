using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class WebAgentNew : Agent
{
    SatelliteOrbit satelliteOrbit;
    Rigidbody satelliteRb;
    private bool Aject = false;
    private bool AddSpead = false;
    private bool RemoveSpead = false;
    private bool AddAngle = false;
    private bool RemoveAngle = false;
    //TargetCity_xzAngle, TargetCity_xyAngle, TargetCity_distance

    private float lastDistanceError = float.MaxValue;

    private float TargetCity_distance = 380f;

    private int stepCount = 0; // 添加步数计数器
    [HideInInspector]
    public bool comunication;
    [HideInInspector]
    public GameObject comunication_city;
    [HideInInspector]
    public List<GameObject> TargetCityObjectList;
    [HideInInspector]
    public float reward;
    [HideInInspector]
    public float targetDistance = 0;
    [HideInInspector]
    public float targetNormalizedVelocity = 0;
    float distance;
    float velocity;
    float XZAngle;
    float XYAngle;
    float velocitynormalizedXZAngle;
    Vector3 normalizedVelocity;
    int CityNumber = 0;

    Vector3 lastPos = Vector3.zero;
    void Start()
    {
        satelliteOrbit = GetComponent<SatelliteOrbit>();
        satelliteRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        reward = GetCumulativeReward();
        getMyinfor();
        if (distance > 1000f || distance < 315f)
        {
            SetReward(-400f);
            EndEpisode();
            Destroy(gameObject);
            Debug.Log("超出范围");
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            Aject = true;
        }

        if (Input.GetKeyUp(KeyCode.UpArrow)) { AddSpead = true; }
        if (Input.GetKeyUp(KeyCode.DownArrow)) { RemoveSpead = true; }
        if (Input.GetKeyUp(KeyCode.A)) { AddAngle = true; }
        if (Input.GetKeyUp(KeyCode.D)) { RemoveAngle = true; }


    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        int speedControl = actionBuffers.DiscreteActions[0];
        int angleControl = actionBuffers.DiscreteActions[1];
        int adjustOrbitFlag = actionBuffers.DiscreteActions[2];
        satelliteOrbit.TargetCity = TargetCityObjectList[CityNumber].name;


        switch (speedControl)
        {
            case 1:
                satelliteOrbit.ChangeSpeed(+1f);
                //satelliteOrbit.Engry = satelliteOrbit.Engry - 1f;
                AddReward(-0.001f);//修改了减少数值
                break;
            case 2:
                satelliteOrbit.ChangeSpeed(-1f);
                //satelliteOrbit.Engry = satelliteOrbit.Engry - 1f;
                AddReward(-0.001f);
                break;
            default:
                AddReward(0.000001f);
                break;
        }
        switch (angleControl)
        {
            case 1:
                satelliteOrbit.AdjustOrbitDirectionWithQuaternion(+1f);
                //satelliteOrbit.Engry = satelliteOrbit.Engry - 1f;
                AddReward(-0.001f);
                break;
            case 2:
                satelliteOrbit.AdjustOrbitDirectionWithQuaternion(-1f);
                //satelliteOrbit.Engry = satelliteOrbit.Engry - 1f;
                AddReward(-0.001f);
                break;
            default:
                AddReward(0.000001f);
                break;
        }
        if (adjustOrbitFlag == 1)
        {
            satelliteOrbit.AdjustVelocityToBePerpendicular();
            //satelliteOrbit.Engry = satelliteOrbit.Engry - 10f;
            AddReward(-0.01f);
        }
        else
        {
            AddReward(0.00001f);
        }

        //if (stepCount >= 20000)
        //{
        //    Debug.Log("时间到了");
        //    EndEpisode();
        //    Destroy(gameObject);
        //}
        getMyinfor();
        //基础分数
        float currentDistanceError = Mathf.Abs(distance - TargetCity_distance);
        if (TargetCityObjectList != null)
        {
            float distanceTotarget = Vector3.Distance(TargetCityObjectList[CityNumber].transform.position, gameObject.transform.position);
            if (currentDistanceError < lastDistanceError || currentDistanceError < 20f)
            {
                //AddReward(0.0002f);//运行时撤销
                float rewardForDistance = 1 / (currentDistanceError + 1f);  // +1 是为了避免log(0)的情况
                AddReward(rewardForDistance * 1f);
                lastDistanceError = currentDistanceError;
            }
            else
            {
                AddReward(-0.001f);
            }

            //if(distanceTotarget < lastdistanceTotarget || distanceTotarget < 50f)
            //{
            //    float rewardForDistance = 1 / (distanceTotarget + 1f);  // +1 是为了避免log(0)的情况
            //    AddReward(rewardForDistance * 0.5f);
            //    lastdistanceTotarget = distanceTotarget;
            //}
            
            //新训练添加
            //if (Vector3.Distance(lastPos, gameObject.transform.position) < 0.5f) 
            //{
            //    AddReward(-0.5f);
            //}
            //lastPos = gameObject.transform.position;


            if (comunication == true && currentDistanceError < 20f && TargetCityObjectList[CityNumber] == comunication_city)
            {
                //Debug.Log("good");
                AddReward(0.011f);
                if (satelliteOrbit.CommunicateNumber >= 1 && satelliteOrbit.CommunicateNumber < 5)
                {
                    AddReward(0.001f * satelliteOrbit.CommunicateNumber);
                    AddReward(satelliteOrbit.CommunicateDistanceAverage * 0.005f);
                }
                else
                {
                    AddReward(-0.01f);
                }
            }
            else
            {
                AddReward(-0.001f);
            }
        }
        stepCount++;
        if (stepCount % 3000 == 0 && stepCount > 0)
        {
            CityNumber++;  // 增加城市编号
            CityNumber %= TargetCityObjectList.Count;  // 确保CityNumber不超过列表长度
        }

    }


    public override void CollectObservations(VectorSensor sensor)
    {//试一下盲区
        getMyinfor();
        if (satelliteOrbit.Leader != true)
        {
            sensor.AddObservation(distance);//1
            sensor.AddObservation(XZAngle);//1
            sensor.AddObservation(XYAngle);//1
            sensor.AddObservation(velocity);//1
            sensor.AddObservation(velocitynormalizedXZAngle);//1
            sensor.AddObservation(normalizedVelocity);//3
            sensor.AddObservation(satelliteOrbit.CommunicateDistanceAverage);//1

            sensor.AddObservation(satelliteOrbit.CommunicateNumber);//1
            sensor.AddObservation(TargetCity_distance);//1

            sensor.AddObservation(Vector3.Distance(TargetCityObjectList[CityNumber].transform.position, gameObject.transform.position));//1
            sensor.AddObservation(TargetCityObjectList[CityNumber].transform.position);//3
            sensor.AddObservation(gameObject.transform.position);//3
        }
    }

    void getMyinfor()
    {
        var (id, distance, velocity, Angle, XYAngle, Engry, velocitynormalized, normalizedVelocity) = satelliteOrbit.GetInfo();
        this.distance = distance;
        this.velocity = velocity;
        this.XZAngle = Angle;
        this.XYAngle = XYAngle;
        //float velocitynormalizedXZAngle;
        //float normalizedVelocity;
        this.velocitynormalizedXZAngle = velocitynormalized;
        this.normalizedVelocity = normalizedVelocity;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (satelliteOrbit.Contrel == true)
        {
            if (Aject)
            {
                discreteActionsOut[2] = 1;
                Aject = false;
            }
            else
            {
                discreteActionsOut[2] = 0;
            }
            if (AddAngle)
            {
                discreteActionsOut[1] = 1;
                AddAngle = false;
            }
            else if (RemoveAngle)
            {
                discreteActionsOut[1] = 2;
                RemoveAngle = false;
            }
            else
            {
                discreteActionsOut[1] = 0;

            }
            if (AddSpead)
            {
                discreteActionsOut[0] = 1;
                AddSpead = false;
            }
            else if (RemoveSpead)
            {
                discreteActionsOut[0] = 2;
                RemoveSpead = false;
            }
            else
            {
                discreteActionsOut[0] = 0;
            }

        }
        else
        {
            discreteActionsOut[2] = 0;
            discreteActionsOut[1] = 0;
            discreteActionsOut[0] = 0;

        }
    }



}
