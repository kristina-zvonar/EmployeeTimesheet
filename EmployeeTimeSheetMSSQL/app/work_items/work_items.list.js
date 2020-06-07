var WorkItemModule = function () {
    var isEmployee;
    var isAdmin;
    var workItemDataSource;
    var dataGrid;
   
    var rebindEvents = function () {
        $(".cmd-delete").each(function (index, value) {
            $(value).on("click", function () {
                bootbox.confirm({
                    message: "If the work item has time logged to it, it will not be deleted. Do you wish to proceed?",
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
                            var workItemID = $(value).data("id");

                            var data = {
                                ID: workItemID
                            };

                            $.ajax({
                                url: "/WorkItem/Delete/",
                                type: "POST",
                                data: JSON.stringify(data),
                                dataType: "json",
                                contentType: "application/json; charset=utf-8"
                            }).done(function (data, status) {
                                if (data.Success) {
                                    toastr.success("The delete operation was successful.");
                                    dataGrid.refresh();
                                } else {
                                    toastr.error(data.ErrorMessage);
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
        workItemDataSource = new DevExpress.data.DataSource({
            load: function (loadOptions) {
                var d = new $.Deferred();

                var params = {
                    Skip: loadOptions.skip,
                    Take: loadOptions.take
                };  

                $.ajax({
                    url: "/WorkItem/Get/",
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
        dataGrid = $("#work_items").dxDataGrid({
            dataSource: workItemDataSource,
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
                fileName: "WorkItemsExport",
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
                        $("<a href='/WorkItem/Edit/" + data.ID + "' data-toggle='tooltip' data-placement='top' title='Edit' class='btn btn-xs btn-primary btn-theme-info'><i class='fa fa-edit'></i></a><span>&nbsp;</span>").appendTo(container);
                        $("<button id='" + data.ID + "' data-id='" + data.ID + "' class='btn btn-xs btn-primary red cmd-delete'  data-toggle='tooltip' data-placement='top' title='Delete'><i class='fa fa-trash'></i></button>").appendTo(container);
                    },
                    allowHeaderFiltering: false,
                    allowFiltering: false
                },
                {
                    dataField: "Name",
                    allowHeaderFiltering: false
                },
                {
                    dataField: "Estimate",
                    caption: "Estimate (Hours)",
                    allowHeaderFiltering: false
                },
                {
                    dataField: "TimeLogged",
                    caption: "Time Logged (Hours)",
                    allowHeaderFiltering: false,
                    visible: !isEmployee
                },
                {
                    dataField: "Creator",
                    caption: "Created By",
                    allowHeaderFiltering: false,
                    allowFiltering: false
                },
                {
                    dataField: "Type"
                },
                {
                    dataField: "Status"
                },
                {
                    dataField: "Supervisor",
                    caption: "Supervisor",
                    visible: isAdmin
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
                                window.location = "/WorkItem/Add/";
                            }
                        }
                    },
                    {
                        location: "before",
                        widget: "dxButton",
                        options: {
                            icon: "check",
                            onClick: function () {
                                finishWorkItems();
                            }
                        }
                    },
                    {
                        location: "before",
                        widget: "dxButton",
                        options: {
                            icon: "repeat",
                            onClick: function () {
                                unfinishWorkItems();
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

    var finishWorkItems = function () {
        var selectedWorkItems = dataGrid.getSelectedRowKeys();
        if (selectedWorkItems.length === 0) {
            toastr.error("You need to select at least one work item.");
            return;
        }

        var workItemIDs = [];
        $.each(selectedWorkItems, function (index, value) {
            if (value.Status === "Active") {
                workItemIDs.push(value.ID);
            }
        });

        if (workItemIDs.length === 0) {
            toastr.error("All of the selected items are already finished.");
            return;
        }

        if (workItemIDs.length !== selectedWorkItems.length) {
            toastr.warning("Some of the selected items are already finished. They will be ignored.");
        }

        var data = {
            WorkItemIDs: workItemIDs,
            Status: 2
        };

        $.ajax({
            url: "/WorkItem/UpdateStatus/",
            type: "POST",
            data: JSON.stringify(data),
            dataType: "json",
            contentType: "application/json; charset=utf-8"
        }).done(function (data, status) {
            if (data.Success) {
                toastr.info("Work item(s) finished successfully.");
                dataGrid.refresh();
            }
        }).fail(function () {
            toastr.error("There was an error processing your request.");
        });
    };

    var unfinishWorkItems = function () {
        var selectedWorkItems = dataGrid.getSelectedRowKeys();
        if (selectedWorkItems.length === 0) {
            toastr.error("You need to select at least one work item.");
            return;
        }

        var workItemIDs = [];
        $.each(selectedWorkItems, function (index, value) {
            if (value.Status === "Finished") {
                workItemIDs.push(value.ID);
            }
        });

        if (workItemIDs.length === 0) {
            toastr.error("All of the selected items are already active.");
            return;
        }

        if (workItemIDs.length !== selectedWorkItems.length) {
            toastr.warning("Some of the selected items are already active. They will be ignored.");
        }

        var data = {
            WorkItemIDs: workItemIDs,
            Status: 1
        };

        $.ajax({
            url: "/WorkItem/UpdateStatus/",
            type: "POST",
            data: JSON.stringify(data),
            dataType: "json",
            contentType: "application/json; charset=utf-8"
        }).done(function (data, status) {
            if (data.Success) {
                toastr.info("Work item(s) unfinished successfully.");
                dataGrid.refresh();
            }
        }).fail(function () {
            toastr.error("There was an error processing your request.");
        });
    };

    return {
        Init: function (isEmployeeVal, isAdminVal) {
            isEmployee = isEmployeeVal;
            isAdmin = isAdminVal;

            var loadDataPromise = loadData();
            $.when(loadDataPromise).done(function () {
                rebindEvents();
            });
        }        
    };
}();

