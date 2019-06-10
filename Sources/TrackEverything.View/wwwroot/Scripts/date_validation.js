$(document).ready(function () {
    $("#taskStartAt").change(function () {
        dateValidation();
    });
    $("#taskEndAt").change(function () {
        dateValidation();
    });

    $("#createForm").submit(function (e) {
        if (dateValidation() == true) {
            addTask(e);
        } else {
            e.preventDefault();
        }
    });

    $("#editForm").submit(function (e) {
        if (dateValidation() == true) {
            editTask(e);
        } else {
            e.preventDefault();
        }
    });
});

function dateValidation() {
    var startDate = new Date($("#taskStartAt").val());
    var endDate = new Date($("#taskEndAt").val());
    if (startDate > endDate  || (!startDate.isValid() && endDate.isValid())) {
        document.querySelector("#taskStartAt").setCustomValidity('Start date cannot be more than end date');
        $("#taskStartAt").blur();
        return false;
    } else {
        document.querySelector("#taskStartAt").setCustomValidity('');
        return true;
    }
}

Date.prototype.isValid = function () {
    return this.getTime() === this.getTime();
};  