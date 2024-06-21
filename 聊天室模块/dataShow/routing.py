# routing.py
from django.urls import path
from .consumers import DataShowConsumer

websocket_urlpatterns = [
    path('ws/dataShow/', DataShowConsumer.as_asgi()),  # 确保使用了.as_asgi()
]
