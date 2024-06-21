from django.urls import path
from .views import home, DataBaseName, BackData,getData,sendMessage

urlpatterns = [
    path('', home, name='home'),
    path('GetDataBase',DataBaseName,name='DataBaseApi'),
    path('sendId',BackData,name='BackData'),
    path('GetData',getData,name='BackData'),
    path('sendMessage',sendMessage,name='sendMessage'),
]
