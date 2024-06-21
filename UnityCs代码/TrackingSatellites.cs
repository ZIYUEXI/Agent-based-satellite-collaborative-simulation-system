using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingSatellites : MonoBehaviour
{
    [HideInInspector]
    public string TargetObjectName = "Main";  // 目标物体的名称
    private Transform target;        // 上一帧所跟踪的目标物体的Transform组件
    private string currentTargetName; // 当前跟踪的目标名称

    private float mouseSensitivity = 200.0f;  // 鼠标灵敏度
    private float clampAngle = 80.0f;         // 镜头的最大垂直偏转角度
    private float offsetDistance = 10.0f;      // 摄像机与目标的偏移距离
    private float minDistance = 10.0f;          // 摄像机与目标的最小距离
    private float maxDistance = 100.0f;         // 摄像机与目标的最大距离
    GameObject targetObj;
    private float rotY = 0.0f; // x轴旋转量（在这里是垂直方向）
    private float rotX = 0.0f; // y轴旋转量（在这里是水平方向）

    public float movementSpeed;

    [HideInInspector]
    public bool ContrulMode = false;

    private bool contrulEnabled = false;

    private Vector3 initialPosition;           // 初始位置
    private Quaternion initialRotation;        // 初始旋转

    void Start()
    {
        currentTargetName = TargetObjectName;
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        initialPosition = transform.position; // 保存初始位置
        initialRotation = transform.rotation; // 保存初始旋转
    }

    void Update()
    {
        if (currentTargetName != TargetObjectName)
        {
            if(targetObj != null)
            {
                SatelliteOrbit satelliteOrbit = targetObj.GetComponent<SatelliteOrbit>();
                satelliteOrbit.Contrel = false;
            }
            currentTargetName = TargetObjectName;
            contrulEnabled = false;
            targetObj = GameObject.Find(TargetObjectName);
            if (targetObj != null)
            {
                target = targetObj.transform;
            }
            else
            {
                target = null;  // 如果未找到物体，则重置target
                Debug.LogWarning("没有找到名为 '" + TargetObjectName + "' 的对象。");
            }
        }

        if (currentTargetName == "Main" && contrulEnabled == false)
        {
            transform.position = initialPosition;
            targetObj = null;
            transform.rotation = initialRotation;
            contrulEnabled = true;
            return;  // 不进行进一步的控制
        }

        if(contrulEnabled == true)
        {
            // 使用WASD键控制摄像机移动
            float horizontal = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
            float vertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
            transform.Translate(horizontal, 0, vertical);

            // 使用鼠标控制视角
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            rotY += mouseX;
            rotX += mouseY;
            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            transform.rotation = localRotation;
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            ContrulMode = !ContrulMode;
        }

        if (!ContrulMode)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // 如果找到了目标物体，则使用鼠标输入更新摄像机的旋转
        if (target != null && targetObj != null)
        {
            SatelliteOrbit satelliteOrbit = targetObj.GetComponent<SatelliteOrbit>();
            if (!ContrulMode)
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = -Input.GetAxis("Mouse Y");

                rotY += mouseX * mouseSensitivity * Time.deltaTime;
                rotX += mouseY * mouseSensitivity * Time.deltaTime;

                rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

                Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
                transform.rotation = localRotation;
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                offsetDistance -= scroll * 10.0f; // 滚轮值乘以一个因子以控制滚动速度
                offsetDistance = Mathf.Clamp(offsetDistance, minDistance, maxDistance);
                satelliteOrbit.Contrel = true;
            }
            else
            {
                satelliteOrbit.Contrel = false;
            }

            // 计算基于目标的相对位置
            transform.position = target.position - transform.forward * offsetDistance;
        }
    }
}
