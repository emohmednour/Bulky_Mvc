var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { data: 'name', "width": "20%" },
            { data: 'email', "width": "20%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'company.name', "width": "10%" },
            { data: 'role', "width": "10%" },
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

