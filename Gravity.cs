
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float gravityConstant = 6.674f;  // 引力常数
    private Rigidbody earthRigidbody;  // 地球的Rigidbody
    public float minimumDistance = 1.0f;  // 设置一个最小距离阈值，防止数值计算问题

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
                // 使用 Mathf.Max 确保距离不小于设定的最小值
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
            //Destroy(other.gameObject);  // 销毁碰撞的物体
        }
    }
}