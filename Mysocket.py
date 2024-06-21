import queue
import socket
import json
import threading

from pymongo import MongoClient


def handle_database(queue):
    client = MongoClient('mongodb://mongoadmin:secret@localhost:27017/')
    while True:
        data = queue.get()
        if data is None:  # 用于停止数据库线程
            break
        #print(data)
        db = client["data_" + data['dateTime']]
        if data['Type'] == 'SatelliteData':
            collection = db[data['FromWho']]
            print(data)
            collection.insert_one(json.loads(data['Data'].strip()))
        if data['Type'] == 'CityData':
            collection = db[data['FromWho']]
            citydata = json.loads(data['Data'].strip())
            collection.update_one(
                {"Time": citydata['Time']},  # 匹配条件，假设Time是唯一的
                {"$set": citydata},
                upsert=True  # 如果不存在匹配的文档，将插入一个新文档
            )
        print("PicShow inserted to MongoDB:", data)


def start_server(que, host='127.0.0.1', port=12345):
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind((host, port))
    server_socket.listen(5)  # 可以增加监听队列的大小
    print(f"Listening on {host}:{port}")

    try:
        while True:
            client_socket, addr = server_socket.accept()
            print('Connected by', addr)
            threading.Thread(target=handle_client, args=(client_socket, que)).start()
    finally:
        server_socket.close()


def handle_client(client_socket, queue):
    try:
        with client_socket.makefile('r') as client_file:
            while True:
                data = client_file.readline()
                data_structure = json.loads(data.strip())
                queue.put(data_structure)
    except Exception as e:
        print(f"Error handling client: {e}")
    finally:
        client_socket.close()
        print("Connection closed")


data_queue = queue.Queue()
db_thread = threading.Thread(target=handle_database, args=(data_queue,))
db_thread.start()
start_server(que=data_queue)