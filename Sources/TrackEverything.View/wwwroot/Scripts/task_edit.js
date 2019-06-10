var uri = "tasks/" + GetURLParameter("id");
window.onload = function () {
    loadData();
};

function loadData() {
    $("document").ready(function () {
        $.getJSON("projects").done(function (projects) {
                setProjectSelect(projects);
                $.getJSON("workers").done(function (workers) {
                        setWorkersSelect(workers);
                        $.getJSON(uri).done(function (result) {
                                if (result != null) {
                                    $("#taskName").val(result.name.trim());
                                    $("#taskTime").val(result.estimation);
                                    $("#taskStartAt").val(correctDate(result.startAt));
                                    $("#taskEndAt").val(correctDate(result.endAt));
                                    $("#taskStatus").val(result.status);
                                    $("#taskProjectSelect option[value='" + result.projectId + "']").prop('selected', true);
                                } else
                                    alert("Can't load resourse, please reload page");
                            })
                            .fail(function (jqxhr, textStatus, error) {
                                alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
                            });
                        selectWorkers();
                    })
                    .fail(function (jqxhr, textStatus, error) {
                        alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
                    });
            })
            .fail(function (jqxhr, textStatus, error) {
                alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
            })
    });

    var correctDate = function (item) {
        if (item != null) {
            return datestring = new Date(item).toISOString().replace('T', ' ').substr(0, 10);
        } else
            return "";
    }
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

function setWorkersSelect(workers) {
    $.each(workers, function (i, item) {
        $("#taskExecutorsSelect").append(new Option(item.name, item.id));
    })
}

function setProjectSelect(projects) {
    $.each(projects, function (i, item) {
        $("#taskProjectSelect").append(new Option(item.name, item.id));
    })
}

function selectWorkers() {
    const workersUri = uri + "/workers";
    $.getJSON(workersUri).done(function (result) {
            $.each(result, function (i, item) {
                $("#taskExecutorsSelect option[value='" + item.id + "']").prop("selected", true);
            })
        })
        .fail(function (jqxhr, textStatus, error) {
            alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
        });
}

async function editTask(e) {
    e.preventDefault();
    const executors = await GetExecutors($("#taskExecutorsSelect").val());
    const project = await GetProject($("#taskProjectSelect").val());

    const item = {
        id: GetURLParameter("id"),
        name: $("#taskName").val(),
        estimation: $("#taskTime").val(),
        startAt: $("#taskStartAt").val(),
        endAt: $("#taskEndAt").val(),
        status: setStatus($("#taskStatus option:selected").text()),
        projectId: $("#taskProjectSelect").val(),
        executors,
        project
    };
    $.ajax({
        url: uri,
        contentType: "application/json",
        method: "PUT",
        data: JSON.stringify(item),
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(jqXHR);
            if (jqXHR.status == 400) {
                alert('Internal error: ' + jqXHR.responseText);
            } else {
                alert('Unexpected error.');
            }

        },
        success: function (result, textStatus, jqXHR) {
            var location = jqXHR.getResponseHeader('Location').split("/");
            window.location = "details_task.html?id=" + location[location.length - 1];;
        },
    });
}

async function GetExecutors(workersId) {
    if (workersId != null) {
        try {
            const uri = "workers/"
            const reqests = workersId.map(function (Id) {
                return $.getJSON(uri + Id)
                    .fail(function (jqxhr, textStatus, error) {
                        alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
                        return;
                    });
            })

            const responses = await Promise.all(reqests);

            return responses.map((result) => {
                return {
                    id: result.id,
                    name: result.name,
                    surname: result.surname,
                    middleName: result.middleName,
                    position: result.position,
                }
            })
        } catch (error) {
            console.log(error);
            alert('Internal error: ' + error.message + "\n try to reload page");
        }
    }

};

async function GetProject(projId) {
    if (projId != 0) {
        const uri = "projects/"
        try {
            var getJson = function (projId) {
                return $.getJSON(uri + projId)
                    .fail(function (jqxhr, textStatus, error) {
                        alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
                    });
            };
            const response = await getJson(projId);
            return response;
        } catch (error) {
            console.log(error);
            alert('Internal error: ' + error.message + "\n try to reload page");
        }
    }
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