var uri = "projects/" + GetURLParameter("id");

$("document").ready(function () {
    $.getJSON(uri + "/tasks").done(function (result) {
            $.each(result, function (i, item) {
                if (i > 0) {
                    $("#projectTasks").append("<span>, " + item.name + " </span>");
                } else {
                    $("#projectTasks").append("<span>" + item.name + " </span>");
                }
            });

            $.getJSON(uri).done(function (result) {
                    if (result != null) {
                        $("#projectName").text(result.name.trim());
                        $("#projectShortname").text(result.shortname.trim());
                        $("#projectCreationDate").text(correctDate(result.creationDate));
                        $("#projectStatus").text(selectStatus(result.status));
                        $("#projectDescription").text(result.description.trim());
                    } else
                        alert("Can't load resourse, please reload page");
                })
                .fail(function (jqxhr, textStatus, error) {
                    alert('Internal error: ' + jqXHR.responseText + "\n try to reload page");
                });;
        })
        .fail(function (jqxhr, textStatus, error) {
            alert('Internal error: ' + jqXHR.responseText + "\n try to reload page");
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
    } else {
        return "---";
    }
}