# Chinese Introduction
这是一个在 Unity 上使用 PPO 算法训练的卫星协同环境，本科毕业设计，使用了 C# 和 Python 进行制作，运用了 Unity ML-Agents 框架。
视频: [https://www.bilibili.com/video/BV1UJ4m1g7KG/?share_source=copy_web&vd_source=6c4e915aa198750e04254d2b74ce1dd1](https://www.bilibili.com/video/BV1UJ4m1g7KG/?share_source=copy_web&vd_source=6c4e915aa198750e04254d2b74ce1dd1)

## 开发部署
- 使用的 Unity 版本：2023.2.19f1
- 使用的 Unity ML-Agents 框架版本：Release 21

## 如果只想运行 Demo
1. 下载并解压 Demo
2. 运行前需要先打开聊天室模块 (Django)，确保 12346 端口没有被占用
3. 如果需要保存数据，需要额外准备一个 MongoDB 并打开数据输出模块，同时确保 12345 端口没有被占用

## 观测值
| 序号 | 观测值描述                                                       | 数据类型 |
|------|------------------------------------------------------------------|----------|
| 1    | 距离（distance）                                                 | float    |
| 2    | XZ平面角度（XZAngle）                                            | float    |
| 3    | XY平面角度（XYAngle）                                            | float    |
| 4    | 速度（velocity）                                                 | float    |
| 5    | 标准化速度XZ角度（velocitynormalizedXZAngle）                    | float    |
| 6    | 标准化速度向量（normalizedVelocity）                             | Vector3  |
| 7    | 平均通信距离（satelliteOrbit.CommunicateDistanceAverage）         | float    |
| 8    | 通信数量（satelliteOrbit.CommunicateNumber）                     | float    |
| 9    | 目标城市距离（TargetCity_distance）                              | float    |
| 10   | 目标城市与当前物体位置的距离（Vector3.Distance）                | float    |
| 11   | 目标城市位置（TargetCityObjectList[CityNumber].transform.position）| Vector3  |
| 12   | 当前物体位置（gameObject.transform.position）                    | Vector3  |

## 动作空间
| 序号 | 动作描述                        | 动作范围          |
|------|--------------------------------|-------------------|
| 1    | 速度控制（speedControl）        | 0: 无动作，1: 加速，2: 减速 |
| 2    | 角度控制（angleControl）        | 0: 无动作，1: 顺时针调整角度，2: 逆时针调整角度 |
| 3    | 轨道调整标志（adjustOrbitFlag） | 0: 不调整，1: 调整 |

如果有其他需要补充的信息，请告诉我。

## 下载地址
- Unity 项目下载地址：链接：[https://pan.baidu.com/s/1tZimGfWiRtwy375W_sc9lg](https://pan.baidu.com/s/1tZimGfWiRtwy375W_sc9lg) 提取码：gzs4

# English Introduction
This is a satellite collaboration environment trained using the PPO algorithm on Unity. It is an undergraduate graduation project developed using C# and Python, leveraging the Unity ML-Agents framework.
Video: [https://www.bilibili.com/video/BV1UJ4m1g7KG/?share_source=copy_web&vd_source=6c4e915aa198750e04254d2b74ce1dd1](https://www.bilibili.com/video/BV1UJ4m1g7KG/?share_source=copy_web&vd_source=6c4e915aa198750e04254d2b74ce1dd1)

## Development and Deployment
- Unity version used: 2023.2.19f1
- Unity ML-Agents framework version: Release 21

## If You Just Want to Run the Demo
1. Download and unzip the demo.
2. Before running, open the chatroom module (Django) and ensure that port 12346 is not occupied.
3. If you need to save data, prepare a MongoDB and open the data output module, ensuring that port 12345 is not occupied.

## Download Links
- Unity project download link: [https://pan.baidu.com/s/1tZimGfWiRtwy375W_sc9lg](https://pan.baidu.com/s/1tZimGfWiRtwy375W_sc9lg) Code: gzs4
- Alternative download link: [Google Drive](https://drive.google.com/file/d/1GQ1sJq62SajrnwDkU-QsH73EboySfX2K/view?usp=sharing)
