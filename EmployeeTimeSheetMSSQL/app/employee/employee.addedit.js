var EmployeeManageModule = function () {
    var modeOfWork;

    var registerEvents = function () {
        $("#RankID").on("change", function () {
            toggleRank();
        });

        $("#Username").on("change", function () {
            var username = $(this).val();
            var data = {
                Username: username
            };

            if (username !== null && username !== "") {
                $.ajax({
                    url: "/Employee/CheckIfUniqueUsername/",
                    type: "POST",
                    data: JSON.stringify(data),
                    dataType: "json",
                    contentType: "application/json; charset=utf-8"
                }).done(function (data, status) {
                    if (!data.Success) {
                        $("#username-validation").html("There already is a user with that username.");
                    } else {
                        $("#username-validation").html("");
                    }
                }).fail(function () {
                    toastr.error("There was an error processing your request.");
                });
            } else {
                $("#username-validation").html("");
            }
        });

        $('#employeeForm').validate({
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
                'FirstName': {
                    required: true
                },
                'LastName': {
                    required: true
                },
                'Rank': {
                    required: true
                },
                'Supervisor:': {
                    required: $("RankID").val() === "employee"
                },
                'Username': {
                    required: modeOfWork === "Add",
                    minlength: 6,
                    maxlength: 15
                },
                'Email': {
                    required: modeOfWork === "Add",
                    email: true
                },
                'Password': {
                    required: modeOfWork === "Add",
                    minlength: 8,
                    maxlength: 25
                },
                'ConfirmPassword': {
                    required: modeOfWork === "Add",
                    equalTo: '#Password'
                }
            },
            messages: {
                'FirstName': 'First name is required',
                'LastName': 'Last name is required',
                'Rank': 'Rank is required',
                'Supervisor': 'Supervisor is required for employees',
                'Email': 'Invalid email address',
                'Username': {
                    required: 'Username is required',
                    minlength: 'Username must be at least 6 characters long',
                    maxlength: 'Username cannot be longer than 15 characters'
                },
                'Password': {
                    required: 'Password is required',
                    minlength: 'Password must be at least 8 characters long',
                    maxlength: 'Password cannot be longer than 25 characters'
                },
                'ConfirmPassword': {
                    required: 'Password is required',
                    minlength: 'Password must be at least 8 characters long',
                    maxlength: 'Password cannot be longer than 25 characters',
                    equalTo: 'Please enter the same password as above'
                }
            }
        });
       
    };

    var initializeElements = function () {
        toggleRank();

        if (modeOfWork === "Edit") {
            $("#new-user-form").addClass("d-none");
        } else {
            $("#new-user-form").removeClass("d-none");
        }

    };

    var toggleRank = function () {
        var rankID = $("#RankID").val();
        if (rankID === "supervisor" || rankID === "admin") {
            $("#supervisor-group").addClass("d-none");
            $("#supervisor-label").removeClass("required-label");
        } else {
            $("#supervisor-group").removeClass("d-none");
            $("#supervisor-label").addClass("required-label");
        }
    };

    return {
        Init: function (modeOfWorkVal) {
            modeOfWork = modeOfWorkVal;

            registerEvents();
            initializeElements();
        }
    };

}();