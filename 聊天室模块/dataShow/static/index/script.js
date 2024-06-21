function sendMessage() {
    var message = document.getElementById('message').value;
    var role = document.getElementById('role').value;

    // Check if the role is 'admin' and prevent sending messages
    if (role === 'admin') {
        alert("管理员无法发送消息。");
        return; // Early return to stop execution
    }

    // Update UI to show the new message
    var messages = document.getElementById('messages');
    var messageDiv = document.createElement('div');
    messageDiv.textContent = role + ": " + message;  // Adding role information to message text
    messages.appendChild(messageDiv);
    document.getElementById('message').value = ''; // Clear the input after sending

    // Sending data to Django backend
    fetch('/sendMessage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-CSRFToken': getCookie('csrftoken')  // Getting CSRF token from cookies
        },
        body: JSON.stringify({ role: role, message: message })
    })
    .then(response => response.json())
    .then(data => {
    clearMessages();
    AddMessages(data.message)
    })
    .catch((error) => {
        console.error('Error sending message:', error);
    });
}

function clearMessages() {
    document.getElementById('messages').innerHTML = ''; // Clear the contents of the messages div
}

function AddMessages(data) {
    var messagesContainer = document.getElementById('messages');

    data.forEach(function(message) {
        var messageDiv = document.createElement('div'); // 创建一个新的 div 元素来显示消息
        var date = new Date(message.date).toLocaleString(); // 将日期字符串转换为更易读的格式

        // 设置 div 的内容，包括角色名、消息内容和日期
        messageDiv.textContent = message.role + ": " + message.content + " (" + date + ")";
        messageDiv.className = 'message'; // 可以添加一个类名用于CSS样式设计

        messagesContainer.appendChild(messageDiv);
    });
}



document.addEventListener('DOMContentLoaded', function() {
    var roleSelect = document.getElementById('role');
    var selectedRole = roleSelect.value;  // 初始选中的角色

    // 监听角色选择变化
    roleSelect.addEventListener('change', function() {
        selectedRole = this.value;  // 更新selectedRole的值
        sendData(selectedRole);  // 角色变化时立即发送数据
    });

});

setInterval(function() {
    var roleSelect = document.getElementById('role');
    var selectedRole = roleSelect.value;
    sendData(selectedRole);
}, 500);  // 时间间隔设为500毫秒

function sendData(role) {
    fetch('/GetData', {
        method: 'POST', // 或者 'GET'，具体取决于你的后端配置
        headers: {
            'Content-Type': 'application/json',
            'X-CSRFToken': getCookie('csrftoken')  // 从 cookie 中获取 Django CSRF token，仅当使用 POST 请求时需要
        },
        body: JSON.stringify({ role: role })
    })
    .then(response => response.json())
    .then(data => {
    clearMessages();
    AddMessages(data.message)
    })
}

// 用于获取 Django CSRF token 的函数
function getCookie(name) {
    let cookieValue = null;
    if (document.cookie && document.cookie !== '') {
        const cookies = document.cookie.split(';');
        for (let i = 0; i < cookies.length; i++) {
            const cookie = cookies[i].trim();
            // Does this cookie string begin with the name we want?
            if (cookie.substring(0, name.length + 1) === (name + '=')) {
                cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                break;
            }
        }
    }
    return cookieValue;
}
