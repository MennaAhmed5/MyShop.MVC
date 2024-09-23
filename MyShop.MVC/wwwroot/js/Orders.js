var dtble;
$(document).ready(function () {
    loadData();
});
function loadData() {
    dtble = $("#myTable").DataTable({
        "ajax": {
            "url": "/Admin/Order/GetData"
        },
        "columns": [
            { "data": "id" },
            { "data": "name" },
            { "data": "phone" },
            { "data": "applicationUser.email" },
            { "data": "orderStatus" },
            { "data": "totalPrice" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-warning">Details</a>
                     `;
                }
            }
        ]
    });
}