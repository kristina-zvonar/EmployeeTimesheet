var WorkItemManageModule = function () {
    var isAdmin;

    var registerEvents = function () {
        $("#Name").on("change", function () {
            var name = $("#Name").val();
            var data = {
                Name: name
            };

            if (name !== null && name !== "") {
                $.ajax({
                    url: "/WorkItem/CheckIfUnique/",
                    type: "POST",
                    data: JSON.stringify(data),
                    dataType: "json",
                    contentType: "application/json; charset=utf-8"
                }).done(function (data, status) {
                    if (!data.Success) {
                        $("#name-validation").html("There already is a work item with that name.");
                    } else {
                        $("#name-validation").html("");
                    }
                }).fail(function () {
                    toastr.error("There was an error processing your request.");
                });
            } 
        });

        $('#workItemForm').validate({
            errorClass: 'help-block has-error',
            errorElement: 'div',
            errorPlacement: function (error, e) {
                error.appendTo(e.parent('.form-group'));
            },
            highlight: function (e) {
                $(e).closest('.form-group').removeClass('has-success has-error').addClass('has-error');
                $(e).closest('.help-block').remove();
            },
            success: function (e) {
                e.closest('.form-group').removeClass('has-success has-error');
                e.closest('.help-block').remove();
            },
            rules: {
                'Name': {
                    required: true
                },
                'Estimate': {
                    required: true,
                    number: true,
                },
                'Supervisor:': {
                    required: isAdmin
                }                
            },
            messages: {
                'Name': 'Name is required',
                'Estimate': {
                    required: "Estimate is required",
                    number: "Estimate is a number of hours that takes to complete the work item"
                },                
                'Supervisor': 'Supervisor should be stated explicitly when admin creates a work item',                
            }
        });

        $("#TypeID").on("change", function () {
            var type = $("#TypeID").val();
            if (type === "2") {
                $("#parent-div").removeClass("d-none");
            } else {
                $("#parent-div").val("");
                $("#parent-div").addClass("d-none");
            }
        });
    };

    return {
        Init: function (isAdminVal) {
            isAdmin = isAdminVal;

            registerEvents();
        }
    };

}();