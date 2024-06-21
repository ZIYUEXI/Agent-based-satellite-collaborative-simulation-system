import json
from channels.generic.websocket import AsyncWebsocketConsumer
import logging

# 配置日志
logger = logging.getLogger(__name__)

class DataShowConsumer(AsyncWebsocketConsumer):
    async def connect(self):
        await self.accept()
        # 当有新的连接时打印或记录日志
        logger.info("New WebSocket connection established.")
        print("New WebSocket connection established.")  # 或使用print在控制台输出

    async def disconnect(self, close_code):
        # 当连接断开时打印或记录日志
        logger.info(f"WebSocket connection closed with close code: {close_code}")
        print(f"WebSocket connection closed with close code: {close_code}")

    async def receive(self, text_data=None, bytes_data=None):
        text_data_json = json.loads(text_data)
        message = text_data_json.get('message', '')

        # 将接收到的消息回传给客户端
        await self.send(text_data=json.dumps({
            'message': message
        }))
        # 打印或记录接收到的消息
        logger.info(f"Received message: {message}")
        print(f"Received message: {message}")
