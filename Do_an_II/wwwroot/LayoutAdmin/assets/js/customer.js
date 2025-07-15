var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    // Initialize DataTable with AJAX source
    dataTable = $('#tablecustomer').DataTable({

        "ajax": {
            url: '/admin/user/getallcustomer',

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
                        return`
                        <div class="text-center d-flex gap-2 justify-content-center">
                        <a onclick=LockUnlock('${data.id}') class ="btn btn-danger text-white" style="cursor:pointer; width:150px">
                           <i class="bi bi-lock-fill"></i> Khóa
                           </a>
                           <a onClick=Delete('/admin/user/delete/${data.id}') class ="btn btn-danger text-white" style="cursor:pointer; width:150px">
                           <i class="bi bi-trash-fill"></i> Xóa
                           </a>
                    

                    </div>
                `

                    }
                    else {
                        return`<div class="text-center d-flex gap-2 justify-content-center">
                           <a onclick=LockUnlock('${data.id}') class ="btn btn-success text-white" style="cursor:pointer; width:150px">
                           <i class="bi bi-unlock-fill"></i> Mở khóa
                           </a>
                           <a onClick=Delete('/admin/user/delete/${data.id}') class ="btn btn-danger text-white" style="cursor:pointer; width:150px">
                           <i class="bi bi-trash-fill"></i> Xóa
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
        url: '/admin/user/lockunlock',
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
function Delete(url) {
    Swal.fire({
        title: "Bạn có chắc chắn muốn xóa",
        text: "Bạn sẽ không thể quay ngược",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Xóa ngay!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {

                    dataTable.ajax.reload();
                    Swal.fire(
                        "Deleted!",
                        data.message,
                        "success"
                    );
                }


            })
        }
    });
}
