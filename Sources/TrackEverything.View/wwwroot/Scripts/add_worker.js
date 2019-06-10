$("document").ready(function () {
    $.getJSON("tasks").done(function (result) {
            $.each(result, function (i, item) {
                $("#selectList").append(new Option(item.name, item.id));
            })
        })
        .fail(function (jqxhr, textStatus, error) {
            alert('Internal error: ' + jqxhr.responseText + "\n try to reload page");
        });;

});

async function addWorker(e) {
    e.preventDefault();
    const tasks = await GetTasks($("#selectList").val())

    const item = {
        name: $("#workerName").val(),
        surname: $("#workerSurname").val(),
        middleName: $("#workerMiddlename").val(),
        position: $("#workerPosition").val(),
        tasks,
    };

    $.ajax({
        type: "POST",
        url: "workers",
        contentType: "application/json",
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
        }
    });
}

async function GetTasks(tasksId) {
    if (tasksId != null) {
        const uri = "tasks/";
        try {
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
                    project: result.project,
                    projectId: result.projectId
                }
            })
        } catch (error) {
            alert('Internal error: ' + error.message + "\n try to reload page");
        }
    }

};