
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float gravityConstant = 6.674f;  // ��������
    private Rigidbody earthRigidbody;  // �����Rigidbody
    public float minimumDistance = 1.0f;  // ����һ����С������ֵ����ֹ��ֵ��������

    void Start()
    {
        earthRigidbody = GetComponent<Rigidbody>();
        if (earthRigidbody == null)
        {
            Debug.LogError("Gravity script requires a Rigidbody component on the earth object.");
        }
    }

    void Update()
    {
        GameObject[] spaceObjects = GameObject.FindGameObjectsWithTag("SpaceObject");
        foreach (GameObject obj in spaceObjects)
        {
            Rigidbody objRigidbody = obj.GetComponent<Rigidbody>();
            //SatelliteMacroMotion satellite = obj.GetComponentInChildren<SatelliteMacroMotion>();
            if (objRigidbody != null && earthRigidbody != null)
            {
                Vector3 direction = transform.position - obj.transform.position;
                float distance = direction.magnitude;
                //Debug.Log(spaceObjects.Length);
                // ʹ�� Mathf.Max ȷ�����벻С���趨����Сֵ
                distance = Mathf.Max(distance, minimumDistance);

                float forceMagnitude = gravityConstant * (earthRigidbody.mass * objRigidbody.mass) / (distance * distance);
                Vector3 force = direction.normalized * forceMagnitude;
                //satellite.GiveCurrentForce(force);
                objRigidbody.AddForce(force);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SpaceObject"))
        {
            //Destroy(other.gameObject);  // ������ײ������
        }
    }
}