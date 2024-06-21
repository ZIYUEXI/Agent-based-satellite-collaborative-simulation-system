using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingSatellites : MonoBehaviour
{
    [HideInInspector]
    public string TargetObjectName = "Main";  // Ŀ�����������
    private Transform target;        // ��һ֡�����ٵ�Ŀ�������Transform���
    private string currentTargetName; // ��ǰ���ٵ�Ŀ������

    private float mouseSensitivity = 200.0f;  // ���������
    private float clampAngle = 80.0f;         // ��ͷ�����ֱƫת�Ƕ�
    private float offsetDistance = 10.0f;      // �������Ŀ���ƫ�ƾ���
    private float minDistance = 10.0f;          // �������Ŀ�����С����
    private float maxDistance = 100.0f;         // �������Ŀ���������
    GameObject targetObj;
    private float rotY = 0.0f; // x����ת�����������Ǵ�ֱ����
    private float rotX = 0.0f; // y����ת������������ˮƽ����

    public float movementSpeed;

    [HideInInspector]
    public bool ContrulMode = false;

    private bool contrulEnabled = false;

    private Vector3 initialPosition;           // ��ʼλ��
    private Quaternion initialRotation;        // ��ʼ��ת

    void Start()
    {
        currentTargetName = TargetObjectName;
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        initialPosition = transform.position; // �����ʼλ��
        initialRotation = transform.rotation; // �����ʼ��ת
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
                target = null;  // ���δ�ҵ����壬������target
                Debug.LogWarning("û���ҵ���Ϊ '" + TargetObjectName + "' �Ķ���");
            }
        }

        if (currentTargetName == "Main" && contrulEnabled == false)
        {
            transform.position = initialPosition;
            targetObj = null;
            transform.rotation = initialRotation;
            contrulEnabled = true;
            return;  // �����н�һ���Ŀ���
        }

        if(contrulEnabled == true)
        {
            // ʹ��WASD������������ƶ�
            float horizontal = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
            float vertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
            transform.Translate(horizontal, 0, vertical);

            // ʹ���������ӽ�
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

        // ����ҵ���Ŀ�����壬��ʹ���������������������ת
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
                offsetDistance -= scroll * 10.0f; // ����ֵ����һ�������Կ��ƹ����ٶ�
                offsetDistance = Mathf.Clamp(offsetDistance, minDistance, maxDistance);
                satelliteOrbit.Contrel = true;
            }
            else
            {
                satelliteOrbit.Contrel = false;
            }

            // �������Ŀ������λ��
            transform.position = target.position - transform.forward * offsetDistance;
        }
    }
}
