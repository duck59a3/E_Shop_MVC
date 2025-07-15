var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    // Initialize DataTable with AJAX source
    dataTable = $('#tableemployee').DataTable({

        "ajax": {
            url: '/admin/employee/getallemployee',

        },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'address', "width": "10%" },
            { data: 'email', "width": "20%" },
            { data: 'city', "width": "10%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" }, "width": "35%",
                render: function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        return `
                        <div class="text-center d-flex gap-2 justify-content-center">
                        <a onclick=LockUnlock('${data.id}') class ="btn btn-danger text-white" style="cursor:pointer; width:150px">
                           <i class="bi bi-lock-fill"></i> Khóa
                           </a>
                           <a class ="btn btn-danger text-white" style="cursor:pointer; width:150px">
                           <i class="bi bi-pencil-square"></i>Đổi quyền
                           </a>
                    

                    </div>
                    `

                    }
                    else {
                        return `<div class="text-center d-flex gap-2 justify-content-center">
                           <a onclick=LockUnlock('${data.id}') class ="btn btn-success text-white" style="cursor:pointer; width:150px">
                           <i class="bi bi-unlock-fill"></i> Mở khóa
                           </a>
                           <a class ="btn btn-danger text-white" style="cursor:pointer; width:150px">
                           <i class="bi bi-pencil-square"></i> Đổi quyền
                           </a>
                    

                    </div>
                `
                    }
                }
            }

        ]
    });

    

}
function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/admin/employee/lockunlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else {
                toastr.error(response.message);
            }
        },


    });
}

