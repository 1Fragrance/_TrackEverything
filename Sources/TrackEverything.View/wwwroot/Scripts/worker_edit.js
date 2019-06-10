var uri = "workers/" + GetURLParameter("id");
window.onload = function () {
    loadData();
};

function loadData() {
    $("document").ready(function () {
        $.getJSON("tasks").done(function (result) {
                $.each(result, function (i, item) {
                    $("#selectList").append(new Option(item.name, item.id));
                })
                $.getJSON(uri).done(function (result) {
                        if (result != null) {
                            $("#workerName").val(result.name.trim());
                            $("#workerSurname").val(result.surname.trim());
                            $("#workerMiddlename").val(result.middleName.trim());
                            $("#workerPosition").val(result.position.trim());
                        } else
                            alert("Can't load resourse, please reload page");
                    })
                    .fail(function (jqxhr, textStatus, error) {
                        alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
                    })
                selectTasks();
            })
            .fail(function (jqxhr, textStatus, error) {
                alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
            });
    })
}

function selectTasks() {
    const tasksUri = uri + "/tasks";
    $.getJSON(tasksUri).done(function (result) {
            $.each(result, function (i, item) {
                console.log(item.id);
                $("#selectList option[value='" + item.id + "']").prop("selected", true);
            })
        })
        .fail(function (jqxhr, textStatus, error) {
            alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
        });
}

async function editWorker(e) {
    e.preventDefault();
    const tasks = await GetTasks($("#selectList").val());

    const item = {
        id: GetURLParameter("id"),
        name: $("#workerName").val(),
        surname: $("#workerSurname").val(),
        middleName: $("#workerMiddlename").val() || null,
        position: $("#workerPosition").val(),
        tasks
    };

    $.ajax({
        url: uri,
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
            window.location = "details_worker.html?id=" + location[location.length - 1];;
        },
    })
}


async function GetTasks(tasksId) {
    try {
        const uri = "tasks/"
        const reqests = tasksId.map(function (Id) {
            return $.getJSON(uri + Id)
            .fail(function (jqxhr, textStatus, error) {
                alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
                return;
            });;
        })

        const responses = await Promise.all(reqests);

        return responses.map((result) => {
            return {
                id: result.id,
                name: result.name.trim(),
                estimation: result.estimation,
                status: result.status,
                startAt: result.startAt,
                endAt: result.endAt,
                creationDate: result.creationDate,
                projectid: result.projectid,
                project: result.project
            }
        })
    } catch (error) {
        alert('Internal error: ' + error.message + "\n try to reload page");
    }
};


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