var uri = "tasks/" + GetURLParameter("id");

$("document").ready(function () {
    $.getJSON(uri + "/workers").done(function (result) {
            $.each(result, function (i, item) {
                if (i > 0) {
                    $("#taskExecutors").append("<span>, " + item.name + " </span>");
                } else {
                    $("#taskExecutors").append("<span>" + item.name + " </span>");
                }
            })
            GetProject();
            $.getJSON(uri).done(function (result) {
                    if (result != null) {
                        $("#taskName").text(result.name.trim());
                        $("#taskTime").text(result.estimation);
                        $("#taskCreationDate").text(correctDate(result.creationDate));
                        $("#taskStartAt").text(correctDate(result.startAt));
                        $("#taskEndAt").text(correctDate(result.endAt));
                        $("#taskStatus").text(selectStatus(result.status));
                    } else
                        alert("Can't load resourse, please reload page");
                })
                .fail(function (jqxhr, textStatus, error) {
                    alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
                });
        })
        .fail(function (jqxhr, textStatus, error) {
            alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
        });
})

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

function GetURLParameter(sParam) {
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) {
            return sParameterName[1];
        }
    }
}

function correctDate(item) {
    if (item != null) {
        return new Date(item).toDateString();
    } else
        return "---";
}

function GetProject() {
    const proj_uri = uri + "/project";
    $.getJSON(proj_uri).done(function (result) {
            if (result != null) {
                if (result.name == null) {
                    $("#taskProject").text("*Undepended*");
                } else {
                    $("#taskProject").text(result.name);
                }
            }
        })
        .fail(function (jqxhr, textStatus, error) {
            alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
        });;
}