using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using Unity.VisualScripting;
using UnityEngine;

public class MyAcademy : MonoBehaviour
{
    private float resetParameter;
    private SatelliteGenerator satelliteGenerator;  // ���� SatelliteGenerator
    private bool firstReset = true;  // �����������������Ƿ�Ϊ��һ������

    private void Start()
    {
        satelliteGenerator = this.GetComponent<SatelliteGenerator>();
        Time.timeScale = 1f;  // Start simulation at real-time speed
        AdjustTimeScale();
    }

    public void Awake()
    {
        if (!firstReset)
        {
            Academy.Instance.OnEnvironmentReset += EnvironmentReset;
        }
        AdjustTimeScale();
    }

    private void Update()
    {
        // Check if there are no objects with the "SpaceObject" tag remaining
        if (GameObject.FindGameObjectsWithTag("SpaceObject").Length == 0)
        {
            EnvironmentReset();
            firstReset = false;
        }
    }

    void EnvironmentReset()
    {
        Debug.Log("������");
        resetParameter = Academy.Instance.EnvironmentParameters.GetWithDefault("reset_param", 0.1f);
        ResetScene();
    }

    void ResetScene()
    {
        // �������оɵ����Ǵ����������ǵ�һ������
        if (satelliteGenerator != null && !firstReset)
        {
            satelliteGenerator.DestroyAgents();
        }

        // �����µ����Ǵ���
        if (satelliteGenerator != null)
        {
            satelliteGenerator.GenerateAgents();
            firstReset = false;
        }
    }

    void AdjustTimeScale()
    {
        float timeScale = Academy.Instance.EnvironmentParameters.GetWithDefault("time_scale", 1.0f);
        Time.timeScale = timeScale;
    }
}
