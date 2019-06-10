var uri = "projects/" + GetURLParameter("id");
$("document").ready(function () {
    $.getJSON(uri).done(function (result) {
        if (result != null) {
            $("#projectName").val(result.name.trim());
            $("#projectShortname").val(result.shortname.trim());
            $("#projectDescription").val(result.description.trim());
            $("#projectStatusSelect").val(result.status);
        } else
            alert("Can't load resourse, please reload page");
    }).fail(function (jqxhr, textStatus, error) {
        alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
    })
});

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

function setStatus(status) {
    let result;
    switch (status) {
        case "Not Started":
            result = 1;
            break;
        case "In Process":
            result = 2;
            break;
        case "Completed":
            result = 3;
            break;
        case "Delayed":
            result = 4;
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

function editProject(e) {
    e.preventDefault();
    const item = {
        name: $("#projectName").val(),
        shortname: $("#projectShortname").val(),
        description: $("#projectDescription").val(),
        status: setStatus($("#projectStatusSelect option:selected").text())
    };

    $.ajax({
        url: uri,
        accepts: "application/json",
        contentType: "application/json",
        method: "PUT",
        data: JSON.stringify(item),
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status == 400) {
                alert('Internal error: ' + jqXHR.responseText);
            } else {
                alert('Unexpected error.');
            }
        },
        success: function (result, textStatus, jqXHR) {
            var location = jqXHR.getResponseHeader('Location').split("/");
            window.location = "details_project.html?id=" + location[location.length - 1];;
        },
    })
}