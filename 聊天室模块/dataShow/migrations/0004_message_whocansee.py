# Generated by Django 4.2.13 on 2024-06-11 13:01

from django.db import migrations, models


class Migration(migrations.Migration):
    dependencies = [
        ("dataShow", "0003_readytosend"),
    ]

    operations = [
        migrations.AddField(
            model_name="message",
            name="whocansee",
            field=models.CharField(default=111, max_length=100),
            preserve_default=False,
        ),
    ]
