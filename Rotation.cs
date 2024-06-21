using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float rotationSpeed = 10.0f;  // 公开的自转速度变量，可以在Unity编辑器中调整

    // Start is called before the first frame update
    void Start()
    {
        // 初始设置可以在这里进行，但现在Start函数体为空
    }

    // Update is called once per frame
    void Update()
    {
        // 每帧旋转地球。使用Time.deltaTime确保旋转速度不依赖于帧率
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
