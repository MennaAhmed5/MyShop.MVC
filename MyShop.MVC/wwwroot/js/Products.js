var dtble;

$(document).ready(function () {
    loadData();
});

function loadData() {
    dtble = $("#myTable").DataTable({
        "ajax": {
            "url": "/Admin/Product/GetData"
        },
        "columns": [
            { "data":"name"},
            { "data": "description" },
            { "data": "price" },
            { "data": "category.name" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <a href="Admin/Product/Edit/${data}" class="btn btn-success">Edit</a>
                            <a href="Admin/Product/Delete/${data}" class="btn btn-danger">Delete</a>

                            `
                }

            }
        ]
       })
}