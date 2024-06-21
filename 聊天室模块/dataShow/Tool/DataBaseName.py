import re
from datetime import datetime
from pymongo import MongoClient


def GetData():
    # MongoDB 连接字符串
    uri = "mongodb://mongoadmin:secret@localhost:27017/"
    client = MongoClient(uri)

    # 获取所有数据库名称
    databases = client.list_database_names()

    # 定义一个正则表达式来匹配时间戳
    time_regex = re.compile(r"(\d{8}_\d{6})")

    # 从数据库名中提取时间戳，并存储对应关系
    time_to_db = {}
    for db in databases:
        match = time_regex.search(db)
        if match:
            timestamp = match.group(0)
            formatted_time = datetime.strptime(timestamp, "%Y%m%d_%H%M%S").strftime("%Y-%m-%d %H:%M:%S")
            time_to_db[formatted_time] = db

    return convert_data(time_to_db)


def convert_data(original_data):
    return [{'name': key, 'id': value} for key, value in original_data.items()]
