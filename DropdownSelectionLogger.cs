using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class UpdateTrackingTarget : MonoBehaviour
{
    public TMP_Dropdown dropdown; // �������˵�����ϵ�����
    public TMP_Text SatelliteInfo;
    public Button GoTobutton; // ����ť����ϵ�����
    public Button ChangeAngleButton;
    public Button ChangeSpeedButton;
    public Button AdjustOrbitButton;
    public Button BackButton;

    public TMP_InputField InputAngleField;
    public TMP_InputField InputSpeedField;

    private TrackingSatellites trackingSatellitesScript; // ���ڴ洢 TrackingSatellites �ű�������
    private GameObject SelectTarget;
    void Start()
    {
        // ���Ҳ��洢 Main Camera �µ� TrackingSatellites �ű�
        trackingSatellitesScript = Camera.main.GetComponentInChildren<TrackingSatellites>();

        // ȷ���ҵ��˽ű�
        if (trackingSatellitesScript == null)
        {
            Debug.LogError("TrackingSatellites script not found on Main Camera!");
            return;
        }

        // ���ð�ť�ĵ���¼�����
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
        // ��ȡ��ǰѡ�е������˵��������
        int selectedIndex = dropdown.value;
        // ʹ��ѡ�е�������ȡ��Ӧ��ѡ������
        string selectedName = dropdown.options[selectedIndex].text;
        // ���� TrackingSatellites �ű��� TargetObjectName �ֶ�
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

        // ��ѡ�����������̨
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

                // ����ɹ�ת����parsedValue�Ͱ�����ת����ĸ�����
                //Debug.Log("����ֵת��Ϊ�������ɹ�: " + parsedValue);
            }
            else
            {
                // ���ת��ʧ�ܣ���ӡ������Ϣ
                Debug.Log("�����ֵ������Ч�ĸ�����: " + inputValue);
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
                // ���ת��ʧ�ܣ���ӡ������Ϣ
                Debug.Log("�����ֵ������Ч�ĸ�����: " + inputValue);
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
