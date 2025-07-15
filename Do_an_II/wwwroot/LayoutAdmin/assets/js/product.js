var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    // Initialize DataTable with AJAX source
    dataTable = $('#productTable').DataTable({

        "ajax": {
            url: '/admin/product/getall',

        },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'description', "width": "25%" },
            { data: 'price', "width": "25%" },
            { data: 'quantity', "width": "25%" },
            { data: 'size', "width": "25%" },
            { data: 'color', "width": "25%" },
            { data: 'material', "width": "25%" },
            { data: 'category.name', "width": "25%" },
            {
                data: 'id', "width": "25%", render: function
                    (data) {
                    return `<div class="w-75 btn-group" role="group" >
                    <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i>Sửa </a>
                    <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger mx-2"><i class="bi bi-trash"></i>Xóa </a>

                    </div>`
                }
            }

        ]
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