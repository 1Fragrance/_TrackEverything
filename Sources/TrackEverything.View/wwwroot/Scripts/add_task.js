$("document").ready(function () {
    $.getJSON("projects").done(function (result) {
            $.each(result, function (i, item) {
                $("#taskProjectSelect").append(new Option(item.name, item.id));
            })
        })
        .fail(function (jqxhr, textStatus, error) {
            alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
        });;
    $.getJSON("workers").done(function (result) {
            $.each(result, function (i, item) {
                $("#taskExecutorsSelect").append(new Option(item.name, item.id));
            })
        })
        .fail(function (jqxhr, textStatus, error) {
            alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
        });;
})

async function addTask(e) {

    e.preventDefault();
    const executors = await GetExecutors($("#taskExecutorsSelect").val());
    const project = await GetProject($("#taskProjectSelect").val());

    const item = {
        name: $("#taskName").val(),
        estimation: $("#taskTime").val(),
        startAt: $("#taskStartAt").val(),
        endAt: $("#taskEndAt").val(),
        projectId: $("#taskProjectSelect").val(),
        executors,
        project
    };
    $.ajax({
        type: "POST",
        url: "tasks",
        contentType: "application/json",
        data: JSON.stringify(item),
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(jqXHR);
            debugger;
            if (jqXHR.status == 400) {
                alert('Internal error: ' + jqXHR.responseText);
            } else {
                alert('Unexpected error.');
            }
        },
        success: function (result, textStatus, jqXHR) {
            var location = jqXHR.getResponseHeader('Location').split("/");
            window.location = "details_task.html?id=" + location[location.length - 1];;
        }
    });
}

function datesValidation(start, end) {
    var startDate = new Date(start);
    var endDate = new Date(end);
    if (startDate > endDate) {
        document.querySelector("#taskStartAt").setCustomValidity('Start date cannot be more than End date');
        return false;
    } else {
        return true;
    }
}



async function GetExecutors(workersId) {
    if (workersId != null) {
        const uri = "workers/";
        try {
            const reqests = workersId.map(function (Id) {
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
                    name: result.name,
                    surname: result.surname,
                    middlename: result.middleName,
                    position: result.position,
                }
            });
        } catch (error) {
            alert('Exec Internal error: ' + error.message + "\n try to reload page");
        }
    }
};

async function GetProject(projectId) {

    if (projectId != 0) {
        const uri = "projects/"
        try {
            var getJson = function (projectId) {
                return $.getJSON(uri + projectId)
                    .fail(function (jqxhr, textStatus, error) {
                        alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
                    });
            };
            const response = await getJson(projectId);
            return response
        } catch (error) {
            alert('Proj Internal error: ' + error.message + "\n try to reload page");
        }
    }
}