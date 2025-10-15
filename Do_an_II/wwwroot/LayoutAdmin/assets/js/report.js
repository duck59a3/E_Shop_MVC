const connection = new signalR.HubConnectionBuilder()
    .withUrl("/DashboardHub")
    .build();

connection.start().then(() => console.log("Connected to dashboard hub"));

connection.on("UpdateDashboard", data => {
    document.getElementById("revenue").innerText = `Doanh thu: ${data.revenue}₫`;
    document.getElementById("orders").innerText = `Đơn hàng: ${data.orders}`;
    

    // Cập nhật biểu đồ doanh thu
    addRevenueData(data.revenue);
});

const ctx = document.getElementById("revenueChart");
const chart = new Chart(ctx, {
    type: "line",
    data: {
        labels: [],
        datasets: [{ label: "Doanh thu (₫)", data: [] }]
    }
});

function addRevenueData(revenue) {
    const now = new Date().toLocaleTimeString();
    chart.data.labels.push(now);
    chart.data.datasets[0].data.push(revenue);
    chart.update();
}
