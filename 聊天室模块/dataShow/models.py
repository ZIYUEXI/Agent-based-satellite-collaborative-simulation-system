from django.db import models


class Message(models.Model):
    content = models.TextField()
    role = models.CharField(max_length=100)
    date = models.DateTimeField(auto_now_add=True)  # 自动设置为对象创建的时间
    whocansee = models.CharField(max_length=100)

    def __str__(self):
        return f"{self.role}: {self.content} on {self.date} witch {self.whocansee} can see"


class ReadyToSend(models.Model):
    content = models.TextField()
    role = models.CharField(max_length=100)
