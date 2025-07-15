

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
                { data: 'name', "width":"35%" },
                { data: 'description', "width": "35%" }

            ]
        });
}

