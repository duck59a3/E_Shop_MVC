var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    // Initialize DataTable with AJAX source
    dataTable = $('#productTable').DataTable({
        responsive: true,
        autoWidth: false,
        "ajax": {
            url: '/admin/product/getall',

        },
        "columns": [
            { data: 'name', "width": "10%" },
            //{ data: 'description', "width": "10%" },
            { data: 'price', "width": "15%" },
            { data: 'quantity', "width": "10%" },
            //{ data: 'size', "width": "10%" },
            //{ data: 'color', "width": "10%" },
            //{ data: 'material', "width": "10%" },
            { data: 'categoryName', "width": "5%" },
            {
                data: 'isFlashSale',
                width: "10%",
                render: function (data) {
                    if (data) {
                        return `<span class="badge bg-danger">FLASH SALE</span>`;
                    }
                    return `<span class="badge bg-secondary">---</span>`;
                }
            },
            {
                data: 'id', "width": "25%", render: function
                    (data) {
                    return `<div class="w-75 btn-group" role="group" >
                    <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i>Sửa </a>
                    <a href="/admin/product/flashsale?id=${data}" 
                           class="btn btn-sm btn-warning mx-1">
                           Flash Sale
                        </a>
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