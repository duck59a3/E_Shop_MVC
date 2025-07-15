var dataTable;  

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    // Initialize DataTable with AJAX source
    dataTable = $('#myTable').DataTable({

        "ajax": {
            url: '/admin/category/getall',

        },
        "columns": [
            { data: 'displayOrder', "width": "15%" },
            { data: 'name', "width": "25%" },
            {
                data: 'description', "width": "35%",
                render: function (data, type, row) {
                    if (type !== 'display') return data;

                    if (!data) return '';
                    if (data.length > 100) {
                        return `<span title="${data}">${data.substring(0, 100)}...</span>`;
                    }
                    return data;
                }
            },


            {
                data: 'id', "width": "25%", render: function
                    (data) {
                    return `<div class="w-75 btn-group" role="group" >
                    <a href="/admin/category/edit?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i>Sửa </a>
                    <a onClick=Delete('/admin/category/delete/${data}') class="btn btn-danger mx-2"><i class="bi bi-trash"></i>Xóa </a>

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