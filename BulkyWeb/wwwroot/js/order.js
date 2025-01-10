$(document).ready(function () {
    var url = window.location.search.toLowerCase(); // تحويل النص إلى أحرف صغيرة لضمان التوافق
    var status = "all"; // القيمة الافتراضية

    if (url.includes("inprocess")) {
        status = "inprocess";
    } else if (url.includes("completed")) {
        status = "completed";
    } else if (url.includes("pending")) {
        status = "pending";
    } else if (url.includes("approved")) {
        status = "approved";
    }

    console.log("Selected Status:", status); // تتبع الحالة الحالية
    loadDataTable(status);
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall?status='+ status },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "20%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/details?orderid=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> </a>               
                    </div>`
                },
                "width": "15%"
            }
        ]
    });
}

