var uri = "workers/" + GetURLParameter("id");

$("document").ready(function () {
    $.getJSON(uri + "/tasks").done(function (result) {
            $.each(result, function (i, item) {
                if (i > 0) {
                    $("#workerTasks").append("<span>, " + item.name + " </span>");
                } else {
                    $("#workerTasks").append("<span>" + item.name + " </span>");
                }
            })
            $.getJSON(uri).done(function (result) {
                    if (result != null) {
                        $("#workerName").text(result.name.trim());
                        $("#workerSurname").text(result.surname.trim());
                        $("#workerMiddlename").text(result.middleName.trim());
                        $("#workerPosition").text(result.position.trim());
                    } else
                        alert("Can't load resourse, please reload page");
                })
                .fail(function (jqxhr, textStatus, error) {
                    alert('Internal error: ' + jqXHR.responseText + "\n try to reload page");
                })
        })
        .fail(function (jqxhr, textStatus, error) {
            alert('Internal error: ' + jqXHR.responseText + "\n try to reload page");
        });
})





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