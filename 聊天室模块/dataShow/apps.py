import json

from django.apps import AppConfig
from django.core.cache import cache
from websocket_server import WebsocketServer
import threading



class DatashowConfig(AppConfig):
    default_auto_field = "django.db.models.BigAutoField"
    name = "dataShow"
    websocket_server_instance = None
    clients = {}

    def ready(self):
        from .models import Message
        from dataShow.models import ReadyToSend
        Message.objects.all().delete()  # 启动时删除所有Message记录
        ReadyToSend.objects.all().delete()  # 启动时删除所有Message记录

        self.websocket_server_instance = self.initialize_websocket_server()

        # 启动WebSocket服务器的线程
        server_thread = threading.Thread(target=self.run_websocket_server)
        server_thread.start()

    def initialize_websocket_server(self):
        server = WebsocketServer(host='127.0.0.1', port=12346)
        server.set_fn_new_client(self.new_client)
        server.set_fn_client_left(self.client_left)
        server.set_fn_message_received(self.message_received)
        return server

    def run_websocket_server(self):
        self.websocket_server_instance.run_forever()

    @staticmethod
    def new_client(client, server):
        print(DatashowConfig.clients)

    @staticmethod
    def client_left(client, server):
        custom_id = client['id']
        print(f"Client {client['id']} disconnected")


    @staticmethod
    def message_received(client, server, message):
        try:
            # 尝试解析收到的消息为JSON
            data = json.loads(message)
            if data.get('type') == 'init':
                # 处理初始化类型的消息
                custom_id = data.get('cityName')
                client['custom_id'] = custom_id
                DatashowConfig.clients[client['custom_id']] = client
            if data.get('type') == 'heartbeat':
                role = data.get('cityName')
                from dataShow.models import ReadyToSend
                back = ReadyToSend.objects.filter(role=role)
                if back.count() != 0:
                    for i in back:
                        server.send_message(client, i.content)
                    back.delete()
            if data.get('type') == 'GotoTalk':
                role = data.get('role')
                whocansee = data.get('whocansee')
                content = data.get('content')
                from .models import Message
                message = Message(content=content, role=role, whocansee=whocansee)
                message.save()
                print(message)
            else:
                pass
        except json.JSONDecodeError:
            # 如果消息不是有效的JSON，打印错误
            print("Received non-JSON message:", message)
            server.send_message(client, "Error: Please send valid JSON")
