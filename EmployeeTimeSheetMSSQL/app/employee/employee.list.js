var EmployeeModule = function () {
    var employeeDataSource;
    var dataGrid;
    ajaxBlocking = false;

    var rebindEvents = function () {
        $(".cmd-delete").each(function (index, value) {
            $(value).on("click", function () {
                bootbox.confirm({
                    message: "You are attempting to delete a user and his associated employee. If the user has created work items or logged time, he will be marked inactive and his account locked instead of actual deletion. Do you want to proceed?",
                    buttons: {
                        confirm: {
                            label: 'Yes',
                            className: 'btn-success'
                        },
                        cancel: {
                            label: 'No',
                            className: 'btn-danger'
                        }
                    },
                    callback: function (result) {
                        if (result) {
                            var employeeID = $(value).data("id");

                            var data = {
                                ID: employeeID
                            };

                            $.ajax({
                                url: "/Employee/Delete/",
                                type: "POST",
                                data: JSON.stringify(data),
                                dataType: "json",
                                contentType: "application/json; charset=utf-8"
                            }).done(function (data, status) {
                                if (data.Success) {
                                    toastr.success("The delete operation was successful.");
                                    dataGrid.refresh();
                                }
                            }).fail(function (data, status) {
                                toastr.error("There was an error processing your request.");
                            });
                        }
                    }
                });
            });
        });
    };

    var loadData = function () {
        employeeDataSource = new DevExpress.data.DataSource({
            load: function (loadOptions) {
                var d = new $.Deferred();

                var params = {
                    Skip: loadOptions.skip,
                    Take: loadOptions.take
                };    

                $.ajax({
                    url: "/Employee/Get/",
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
        dataGrid = $("#employees").dxDataGrid({
            dataSource: employeeDataSource,
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
                fileName: "EmployeesExport",
                allowExportSelectedData: true
            },
            paging: {
                pageSize: 10
            },
            pager: {
                enabled: true,
                showPageSizeSelector: true,
                allowedPageSizes: [5, 20, 50],
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
                    width: 100,
                    cellTemplate: function (container, options) {
                        var data = options.row.data;
                        if (data.Active) {
                            $("<a href='/Employee/Edit/" + data.ID + "' data-toggle='tooltip' data-placement='top' title='Edit' class='btn btn-xs btn-primary btn-theme-info'><i class='fa fa-edit'></i></a><span>&nbsp;</span>").appendTo(container);
                            $("<button id='" + data.ID + "' data-id='" + data.ID + "' class='btn btn-xs btn-primary red cmd-delete'  data-toggle='tooltip' data-placement='top' title='Delete'><i class='fa fa-trash'></i></button>").appendTo(container);
                        }
                    },
                    allowHeaderFiltering: false,
                    allowFiltering: false
                },
                {
                    dataField: "FirstName",
                    Caption: "First Name",
                    allowHeaderFiltering: false
                },
                {
                    dataField: "LastName",
                    Caption: "Last Name",
                    allowHeaderFiltering: false
                },
                {
                    dataField: "RankID",
                    caption: "Rank",
                    allowHeaderFiltering: false,
                    allowFiltering: false
                },
                {
                    dataField: "Supervisor",
                    caption: "Supervisor",
                    allowHeaderFiltering: false,
                    allowFiltering: false
                },
                {
                    dataField: "Active",
                    caption: "Active?",
                    allowHeaderFiltering: false
                }

            ],
            onToolbarPreparing: function (e) {
                var dataGrid = e.component;
                e.toolbarOptions.items.unshift(
                    {
                        location: "before",
                        widget: "dxButton",
                        options: {
                            icon: "add",
                            onClick: function () {
                                window.location = "/Employee/Add/";
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
        Init: function () {
            var loadDataPromise = loadData();
            $.when(loadDataPromise).done(function () {
                rebindEvents();
            });
        }        
    };
}();

