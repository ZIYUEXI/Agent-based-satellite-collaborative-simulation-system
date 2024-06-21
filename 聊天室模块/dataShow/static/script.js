//function updateTime() {
//    const now = new Date();
//    document.getElementById('time').textContent = now.toLocaleTimeString();
//}
//setInterval(updateTime, 1000); // 每秒更新时间
//updateTime();

function fetchData() {
    fetch('/GetDataBase') // 假设你的后端API路径为 '/api/data'
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const selectBox = document.querySelector('.select-box');
            // 清空现有的选项
            selectBox.innerHTML = '';

            // 假设后端返回的data是数组形式，每个元素都有id和name
            data.forEach(item => {
                const option = document.createElement('option');
                option.textContent = item.name; // name是将要显示的文本
                selectBox.appendChild(option);
            });
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function sendSelectedId() {
    const selectBox = document.querySelector('.select-box');
    const selectedId = selectBox.value; // 获取选中的option的value
    fetch('/sendId', { // 假设你的后端接收ID的API路径为 '/sendId'
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ id: selectedId })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        document.getElementById('NumberOfSatellites').textContent = data.message.NumberOFSatellite;
        document.getElementById('NumberOfCity').textContent = data.message.NumberOFCity;
        console.log('Success:', data); // 处理返回的数据
    })
    .catch(error => {
        console.error('Error:', error);
    });
}

document.addEventListener('DOMContentLoaded', function() {
    fetchData(); // 调用函数获取数据
    const selectBox = document.querySelector('.select-box');
    selectBox.addEventListener('change', sendSelectedId); // 添加监听事件
});