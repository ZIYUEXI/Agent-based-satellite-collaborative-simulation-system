using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float rotationSpeed = 10.0f;  // ��������ת�ٶȱ�����������Unity�༭���е���

    // Start is called before the first frame update
    void Start()
    {
        // ��ʼ���ÿ�����������У�������Start������Ϊ��
    }

    // Update is called once per frame
    void Update()
    {
        // ÿ֡��ת����ʹ��Time.deltaTimeȷ����ת�ٶȲ�������֡��
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
