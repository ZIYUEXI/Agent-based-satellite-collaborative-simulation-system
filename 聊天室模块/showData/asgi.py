# asgi.py
import os
from django.core.asgi import get_asgi_application
from channels.auth import AuthMiddlewareStack
from channels.routing import ProtocolTypeRouter, URLRouter
import dataShow.routing

os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'showData.settings')

application = ProtocolTypeRouter({
    "http": get_asgi_application(),
    "websocket": AuthMiddlewareStack(
        URLRouter(
            dataShow.routing.websocket_urlpatterns
        )
    ),
})
