using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class UpdateTrackingTarget : MonoBehaviour
{
    public TMP_Dropdown dropdown; // 将下拉菜单组件拖到这里
    public TMP_Text SatelliteInfo;
    public Button GoTobutton; // 将按钮组件拖到这里
    public Button ChangeAngleButton;
    public Button ChangeSpeedButton;
    public Button AdjustOrbitButton;
    public Button BackButton;

    public TMP_InputField InputAngleField;
    public TMP_InputField InputSpeedField;

    private TrackingSatellites trackingSatellitesScript; // 用于存储 TrackingSatellites 脚本的引用
    private GameObject SelectTarget;
    void Start()
    {
        // 查找并存储 Main Camera 下的 TrackingSatellites 脚本
        trackingSatellitesScript = Camera.main.GetComponentInChildren<TrackingSatellites>();

        // 确保找到了脚本
        if (trackingSatellitesScript == null)
        {
            Debug.LogError("TrackingSatellites script not found on Main Camera!");
            return;
        }

        // 设置按钮的点击事件监听
        GoTobutton.onClick.AddListener(UpdateTargetObjectName);
        ChangeAngleButton.onClick.AddListener(ChangeAngle);
        ChangeSpeedButton.onClick.AddListener(ChangeSpeed);
        AdjustOrbitButton.onClick.AddListener(AdjustOrbit);
        BackButton.onClick.AddListener(BackToStart);
    }

    private void Update()
    {
        if (SelectTarget != null)
        {
            SatelliteOrbit satelliteOrbit = SelectTarget.GetComponent<SatelliteOrbit>();
            //MyNewAgent myNewAgent = SelectTarget.GetComponent<MyNewAgent>();
            //WebAgent myNewAgent = SelectTarget.GetComponent<WebAgent>();
            WebAgentNew myNewAgent = SelectTarget.GetComponent<WebAgentNew>();
            if (satelliteOrbit != null && myNewAgent != null)
            {
                var (id, distance, velocity, Angle, XYAngle, Engry, AngleWithvelocitynormalized, velocitynormalized) = satelliteOrbit.GetInfo();

                // Building the messages string from the dictionary
                //var (Nearid, Neardistance, Nearvelocity, NearnormalizedVelocity, Nearmessages) = satelliteAgent.nearestSatellite.GetComponent<SatelliteOrbit>().GetInfo();
                //// Set the text for SatelliteInfo
                //SatelliteInfo.text = $"ID: {id}\nDistance: {distance} km\nVelocity: {velocity} km/s\nNormalized Velocity: {normalizedVelocity}\n" +
                //    $"Near Distance:{ Neardistance}\nNear Velocity: {Nearvelocity} km/s\nNear Normalized Velocity: {NearnormalizedVelocity}\n";
                // Set the text for SatelliteInfo
                SatelliteInfo.text = $"ID: {id}\nDistance: {distance} km\nVelocity: {velocity}km/s" +
                    $"\nXZAngle: {Angle}\nXYAngle: {XYAngle}\nEngry:{Engry}\nAngle With velocitynormalized:{AngleWithvelocitynormalized}\n velocitynormalized:{velocitynormalized}\nTargetCity:" +
                    $"{satelliteOrbit.TargetCity}\nTargetAngle:" +
                    $"{myNewAgent.targetNormalizedVelocity}\nReward:{myNewAgent.reward}";
            }
            else
            {
                SatelliteInfo.text = "No satellite orbit data available.";
            }
        }
        else
        {
            SatelliteInfo.text = "Select a satellite to display information.";
        }

        if(trackingSatellitesScript.ContrulMode == true)
        {
            InputAngleField.interactable = true;
            InputSpeedField.interactable = true;
        }
        else
        {
            InputAngleField.interactable = false;
            InputSpeedField.interactable = false;
        }

    }

    void UpdateTargetObjectName()
    {
        // 获取当前选中的下拉菜单项的索引
        int selectedIndex = dropdown.value;
        // 使用选中的索引获取对应的选项名称
        string selectedName = dropdown.options[selectedIndex].text;
        // 更新 TrackingSatellites 脚本的 TargetObjectName 字段
        trackingSatellitesScript.TargetObjectName = selectedName;

        GameObject Target = GameObject.Find(selectedName);
        if (Target != null)
        {
            SelectTarget = Target;
        }
        else
        {
            SelectTarget = null;
            Debug.Log("Wrong!");
        }

        // 可选：输出到控制台
        Debug.Log("TrackingSatellites Target Updated to: " + selectedName);
    }

    void BackToStart()
    {
        trackingSatellitesScript.TargetObjectName = "Main";
        SelectTarget = null;
    }
    
    void ChangeAngle()
    {
        
        if (SelectTarget != null)
        {
            SatelliteOrbit satelliteOrbit = SelectTarget.GetComponent<SatelliteOrbit>();
            string inputValue = InputAngleField.text;

            float parsedValue;
            if (float.TryParse(inputValue, out parsedValue))
            {
                //satelliteOrbit.AdjustOrbitDirection(parsedValue);
                //satelliteOrbit.AdjustOrbitDirection(parsedValue);
                satelliteOrbit.AdjustOrbitDirectionWithQuaternion(parsedValue);
                InputAngleField.text = "";

                // 如果成功转换，parsedValue就包含了转换后的浮点数
                //Debug.Log("输入值转换为浮点数成功: " + parsedValue);
            }
            else
            {
                // 如果转换失败，打印错误消息
                Debug.Log("输入的值不是有效的浮点数: " + inputValue);
                InputAngleField.text = "";
            }
        }

        else
        {
            Debug.Log("Wrong!");
        }
    }

    void ChangeSpeed()
    {
        if (SelectTarget != null)
        {
            SatelliteOrbit satelliteOrbit = SelectTarget.GetComponent<SatelliteOrbit>();
            string inputValue = InputSpeedField.text;

            float parsedValue;
            if (float.TryParse(inputValue, out parsedValue))
            {
                satelliteOrbit.ChangeSpeed(parsedValue);
                InputSpeedField.text = "";
            }
            else
            {
                // 如果转换失败，打印错误消息
                Debug.Log("输入的值不是有效的浮点数: " + inputValue);
                InputSpeedField.text = "";
            }
        }

        else
        {
            Debug.Log("Wrong!");
        }
    }

    void AdjustOrbit()
    {
        if (SelectTarget != null)
        {
            SatelliteOrbit satelliteOrbit = SelectTarget.GetComponent<SatelliteOrbit>();
            satelliteOrbit.AdjustVelocityToBePerpendicular();
        }
    }
}
