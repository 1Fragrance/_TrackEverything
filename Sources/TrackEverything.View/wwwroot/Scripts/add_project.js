function addProject(e) {
    e.preventDefault();

    const item = {
        name: $("#projectName").val(),
        shortname: $("#projectShortname").val(),
        description: $("#projectDescription").val()
    };

    $.ajax({
        type: "POST",
        accepts: "application/json",
        url: "projects",
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
            window.location = "details_project.html?id=" + location[location.length - 1];;
        },
    });

}