const uri = "tasks";
$(document).ready(function () {
    loadData();
});

function loadData() {
    $("document").ready(function () {
        $("#tasks").html(" ");
        $.getJSON(uri).done(function (result) {
                $.each(result, function (i, item) {
                    $("#tasks").append(row(item));
                    GetProject(item.id);
                });
            })
            .fail(function (jqxhr, textStatus, error) {
                alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
            });

        var row = function (item) {
            return "<tr data-rowid='" + item.id + "'>" +
                "<td>" + item.id + "</td>" +
                "<td>" + item.name + "</td>" +
                "<td></td>" +
                "<td>" + correctDate(item.startAt) + "</td>" +
                "<td>" + correctDate(item.endAt) + "</td>" +
                "<td>" + correctDate(item.creationDate) + "</td>" +
                "<td>" + item.estimation + "</td>" +
                "<td>" + selectStatus(item.status) + "</td>" +
                "<td><a class='editLink' data-id='" + item.id + "'>Edit</a> | " +
                "<a  class='detailsLink' data-id='" + item.id + "'>Details</a> | " +
                "<a  class='deleteLink' data-id='" + item.id + "'>Delete</a></td></tr>";
        }

        var correctDate = function (item) {
            if (item != null) {
                return new Date(item).toDateString();
            } else
                return "---";
        }
    })
}

function selectStatus(status) {
    let result;
    switch (status) {
        case 1:
            result = "Not Started";
            break;
        case 2:
            result = "In Process";
            break;
        case 3:
            result = "Completed";
            break;
        case 4:
            result = "Delayed";
            break;
        default:
            result = 1;
    }
    return result;
}

function DeleteTask(id) {
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

async function GetProject(item_id) {
    const proj_uri = "tasks/" + item_id + "/project";
    $.getJSON(proj_uri)
        .done(function (result) {
            if (result == null || result.name == null) {
                $("#tasks tr[data-rowid='" + item_id + "'] td:nth-child(3)").text("---");
            } else {
                $("#tasks tr[data-rowid='" + item_id + "'] td:nth-child(3)").text(result.name);
            }
        })
        .fail(function () {
            $("#tasks tr[data-rowid='" + item_id + "'] td:nth-child(3)").text("---");
        })
}


$("body").on("click", ".deleteLink", function () {
    var id = $(this).data("id");
    DeleteTask(id);
});
$("body").on("click", ".editLink", function () {
    var id = $(this).data("id");
    window.location = ("edit_task.html?id=" + id);
})
$("body").on("click", ".detailsLink", function () {
    var id = $(this).data("id");
    window.location = ("details_task.html?id=" + id);
});