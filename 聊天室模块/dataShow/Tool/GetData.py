from datetime import datetime

from pymongo import MongoClient


def GetDataBack(datetime_str):
    # Step 1: Parse the original datetime string
    uri = "mongodb://mongoadmin:secret@localhost:27017/"
    client = MongoClient(uri)

    # 获取所有数据库名称
    databases = client.list_database_names()
    dt = datetime.strptime(datetime_str, "%Y-%m-%d %H:%M:%S")

    # Step 2: Format the datetime object into the new format
    formatted_dt = dt.strftime("data_%Y%m%d_%H%M%S")
    print(client[formatted_dt].list_collection_names())
    backData = {"NumberOFSatellite": 0, "NumberOFCity": 0}
    Satellites = []
    for i in client[formatted_dt].list_collection_names():
        if "Satellite" in i:
            backData["NumberOFSatellite"] = backData["NumberOFSatellite"] + 1
            Satellites.append(i)
        else:
            backData["NumberOFCity"] = backData["NumberOFCity"] + 1
    pipeline = [
        {
            "$group": {
                "_id": None,  # 对所有文档进行分组
                "AverageCommunicationNumber": {"$avg": "$CommunicationNumber"},
                "AverageDistance": {"$avg": "$Distance"},
                "AverageVelocity": {"$avg": "$Velocity"},
                "AllXValues": {"$push": "$X"},  # 收集所有 X 字段的值到一个列表中
                "AllYValues": {"$push": "$Y"},  # 收集所有 X 字段的值到一个列表中
                "AllZValues": {"$push": "$Z"}  # 收集所有 X 字段的值到一个列表中
            }
        }
    ]
    for i in Satellites:
        results = list(client[formatted_dt][i].aggregate(pipeline))

        a = [{"avg_CommunicationNumber": str(results[0]['AverageCommunicationNumber'])},
             {"avg_Distance": str(results[0]['AverageDistance'])},
             {"avg_Velocity": results[0]['AverageVelocity']},
             {"AllXValues": results[0]["AllXValues"]},
             {"AllYValues": results[0]["AllYValues"]},
             {"AllZValues": results[0]["AllZValues"]},
             ]
        backData[i] = a
    return backData
