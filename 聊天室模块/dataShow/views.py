import json

from django.apps import apps
from django.core.cache import cache
from django.http import HttpResponse, JsonResponse
from django.shortcuts import render
from django.views.decorators.csrf import csrf_exempt
from django.views.decorators.http import require_http_methods

from .apps import DatashowConfig
from .models import Message, ReadyToSend
from dataShow.Tool.DataBaseName import GetData
from dataShow.Tool.DrawPic import plot_avg_velocity, plot_avg_distance, plot_3d_trajectory
from dataShow.Tool.GetData import GetDataBack


@require_http_methods(["POST"])  # 仅允许POST请求
def getData(request):
    data = json.loads(request.body)  # 解析接收到的JSON数据
    role = data['role']
    if role == 'admin':
        messages = Message.objects.all()
        messages_list = [
            {'role': message.role, 'content': message.content, 'date': message.date, 'whocansee': message.whocansee} for
            message in messages]
        return JsonResponse({"status": "success", "message": messages_list})

    print(role)
    messages = Message.objects.filter(whocansee=role)
    messages_list = [
        {'role': message.role, 'content': message.content, 'date': message.date, 'whocansee': message.whocansee} for
        message in messages]

    return JsonResponse({"status": "success", "message": messages_list})

def sendMessage(request):
    if request.method == 'POST':
        try:
            data = json.loads(request.body)  # 解析接收到的JSON数据
            role = data.get('role')
            message_content = data.get('message')

            message = Message(content=message_content, role=role, whocansee=role)
            message.save()
            readytosend = ReadyToSend(content=message_content, role=role)
            readytosend.save()

            messages = Message.objects.filter(whocansee=role)

            messages_list = [
                {'role': message.role, 'content': message.content, 'date': message.date, 'whocansee': message.whocansee}
                for
                message in messages]

            return JsonResponse({"status": "success", "message": messages_list})
        except json.JSONDecodeError:
            return JsonResponse({"status": "error", "message": "解析错误"}, status=400)
        except Exception as e:
            return JsonResponse({"status": "error", "message": str(e)}, status=500)
    else:
        return JsonResponse({"status": "error", "message": "仅支持POST请求"}, status=405)


def home(request):
    return render(request, 'index.html')


def DataBaseName(request):
    data = GetData()  # 假设GetData()返回一个列表
    return JsonResponse(data, safe=False)  # 设置safe=False允许非字典类型的数据


@require_http_methods(["POST"])  # 仅允许POST请求
def BackData(request):
    try:
        data = json.loads(request.body)  # 解析接收到的JSON数据
        print(data)
        selected_id = data['id']
        # 根据接收到的ID进行处理，例如查询数据库、记录日志等操作
        print("Received ID:", selected_id)
        a = GetDataBack(selected_id)
        # Call the function with the data
    except json.JSONDecodeError:
        return JsonResponse({'status': 'error', 'message': 'Invalid JSON'}, status=400)
    except KeyError:
        return JsonResponse({'status': 'error', 'message': 'Missing key id'}, status=400)
    except Exception as e:
        return JsonResponse({'status': 'error', 'message': str(e)}, status=500)
    plot_avg_velocity(a)
    plot_avg_distance(a)
    plot_3d_trajectory(a)
    # 响应可以根据处理结果自定义
    return JsonResponse({'status': 'success', 'message': a})


def get_global_instance():
    aasa = apps.get_app_config('dataShow')
    return aasa
