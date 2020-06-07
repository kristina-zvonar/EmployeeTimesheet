var TimeLogModule = function () {
    var isEmployee;
    var timeLogDataSource;
    var dataGrid;
    
    var rebindEvents = function () {
        $('#timeLogEntryForm').validate({
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
                'Hours': {
                    required: true,
                    number: true
                },
                'WorkItemID:': {
                    required: true
                }
            },
            messages: {                
                'Hours': {
                    required: "Hours field is required",
                    number: "Hours field is a number of hours spent working on the working item."
                },
                'WorkItemID': 'Work item field is required'
            }
        });
    };

    var loadData = function () {
        timeLogDataSource = new DevExpress.data.DataSource({
            load: function (loadOptions) {
                var d = new $.Deferred();

                var params = {
                    Skip: loadOptions.skip,
                    Take: loadOptions.take
                }; 

                $.ajax({
                    url: "/TimeLog/Get/",
                    type: "POST",
                    data: JSON.stringify(params),
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (data, textStatus, jqXHR) {
                        d.resolve(data.data, { totalCount: data.totalCount });
                    }
                });
                return d.promise();
            }
        });

        return initTable();
    };

    var initTable = function () {
        dataGrid = $("#time_logs").dxDataGrid({
            dataSource: timeLogDataSource,
            showColumnLines: true,
            rowAlternationEnabled: true,
            wordWrapEnabled: true,
            onContentReady: function () {
                rebindEvents();
            },
            filterRow: {
                visible: true,
                applyFilter: "auto"
            },
            headerFilter: {
                visible: true
            },
            selection: {
                mode: 'multiple'
            },
            export: {
                enabled: true,
                fileName: "TimeLogsExport",
                allowExportSelectedData: true
            },
            paging: {
                pageSize: 10
            },
            pager: {
                enabled: true,
                showPageSizeSelector: true,
                allowedPageSizes: [10, 20, 50],
                showNavigationButtons: true,
                showInfo: true,
                infoText: "Page #{0}. Total: {1} ({2} items)"
            },
            remoteOperations: {
                sorting: true,
                paging: true,
                filtering: false
            },
            columns: [  
                {
                    dataField: "ID",
                    caption: "#",
                    width: 60,
                    cellTemplate: function (container, options) {
                        var data = options.row.data;
                        var timeLogDate = new Date(data.EntryDate);
                        var currentDate = new Date();
                        if (timeLogDate.getDate() === currentDate.getDate() &&
                            timeLogDate.getMonth() === currentDate.getMonth() &&
                            timeLogDate.getFullYear() === currentDate.getFullYear()
                            && data.IsWorkItemActive
                            && isEmployee) {
                            $("<a href='/TimeLog/Edit/" + data.ID + "' data-toggle='tooltip' data-placement='top' title='Edit' class='btn btn-xs btn-primary btn-theme-info'><i class='fa fa-edit'></i></a><span>&nbsp;</span>").appendTo(container);                            
                        }
                    },
                    allowHeaderFiltering: false,
                    allowFiltering: false,
                    visible: isEmployee
                },
                {
                    dataField: "EntryDate",
                    caption: "Date",
                    dataType: "date",
                    format: "dd/MM/yyyy",
                    allowHeaderFiltering: false
                },
                {
                    dataField: "Hours",
                    allowHeaderFiltering: false
                },
                {
                    dataField: "Employee"
                },
                {
                    dataField: "WorkItem"
                }              

            ],
            onToolbarPreparing: function (e) {
                var dataGrid = e.component;
                e.toolbarOptions.items.unshift(
                    {
                        location: "before",
                        widget: "dxButton",
                        visible: isEmployee,
                        options: {
                            icon: "add",
                            onClick: function () {
                                window.location = "/TimeLog/Add/";
                            }
                        }
                    },                    
                    {
                        location: "after",
                        widget: "dxButton",
                        options: {
                            icon: "refresh",
                            onClick: function () {
                                dataGrid.refresh();
                            }
                        }
                    });
            },
        }).dxDataGrid("instance");
    };

    return {
        Init: function (isEmployeeVal) {
            isEmployee = isEmployeeVal;

            var loadDataPromise = loadData();
            $.when(loadDataPromise).done(function () {
                rebindEvents();
            });          
        }
    };
}();

