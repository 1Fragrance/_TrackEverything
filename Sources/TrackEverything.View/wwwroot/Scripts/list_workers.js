const uri = "workers";
$(document).ready(function () {
    loadData();
});

function loadData() {
    $("document").ready(function () {
        $("#workers").html(" ");
        $.getJSON(uri).done(function (result) {
                $.each(result, function (i, item) {
                    $("#workers").append(row(item));
                })
            })
            .fail(function (jqxhr, textStatus, error) {
                console.log(jqxhr);
                alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
            });
        var row = function (item) {
            return "<tr data-rowid='" + item.id + "'>" +
                "<td>" + item.id + "</td>" +
                "<td>" + item.surname + "</td>" +
                "<td>" + item.name + "</td>" +
                "<td>" + item.middleName + "</td>" +
                "<td>" + item.position + "</td>" +
                "<td><a class='editLink' data-id='" + item.id + "'>Edit</a> | " +
                "<a class='detailsLink' data-id='" + item.id + "'>Details</a> | " +
                "<a class='deleteLink' data-id='" + item.id + "'>Delete</a></td></tr>";
        }
    })
}

function DeleteWorker(id) {
    $.ajax({
        url: uri + "/" + id,
        type: 'DELETE',
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            loadData();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status == 400) {
                alert('Internal error: ' + jqXHR.responseText);
            } else {
                alert('Unexpected error.');
            }
        }
    });
}


$("body").on("click", ".deleteLink", function () {
    var id = $(this).data("id");
    DeleteWorker(id);
});
$("body").on("click", ".editLink", function () {
    var id = $(this).data("id");
    window.location = ("edit_worker.html?id=" + id);
})
$("body").on("click", ".detailsLink", function () {
    var id = $(this).data("id");
    window.location = ("details_worker.html?id=" + id);
});