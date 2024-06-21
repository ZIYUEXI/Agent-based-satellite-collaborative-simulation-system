@echo off
call conda activate myenv

echo Checking for existing processes on port 8000...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :8000') do (
    echo Found process %%a on port 8000, attempting to kill...
    taskkill /PID %%a /F
    echo Process %%a has been terminated.
)

echo Starting Uvicorn server...
uvicorn showData.asgi:application --host 0.0.0.0 --port 8000 --reload

pause
