using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using Unity.VisualScripting;
using UnityEngine;

public class MyAcademy : MonoBehaviour
{
    private float resetParameter;
    private SatelliteGenerator satelliteGenerator;  // 引用 SatelliteGenerator
    private bool firstReset = true;  // 新增布尔变量跟踪是否为第一次重置

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
        Debug.Log("重置了");
        resetParameter = Academy.Instance.EnvironmentParameters.GetWithDefault("reset_param", 0.1f);
        ResetScene();
    }

    void ResetScene()
    {
        // 销毁所有旧的卫星代理，除非这是第一次重置
        if (satelliteGenerator != null && !firstReset)
        {
            satelliteGenerator.DestroyAgents();
        }

        // 生成新的卫星代理
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
