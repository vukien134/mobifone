$(function () {
    var l = abp.localization.getResource('Accounting');
    var createModal = new abp.ModalManager(abp.appPath + 'Windows/CreateModal');
    var editModal = new abp.ModalManager(abp.appPath + 'Windows/EditModal');

    var dataTable = $('#WindowsTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(accounting.windows.window.getList),
            columnDefs: [
                {
                    title: l('Code'),
                    data: "code",
                    rowAction: {
                        items:
                            [
                                {
                                    text: l('Edit'),
                                    action: function (data) {
                                        editModal.open({ id: data.record.id });
                                    }
                                },
                                {
                                    text: l('Delete'),
                                    confirmMessage: function (data) {
                                        return l(
                                            'BookDeletionConfirmationMessage',
                                            data.record.name
                                        );
                                    },
                                    action: function (data) {
                                        accounting.windows.window
                                            .delete(data.record.id)
                                            .then(function () {
                                                abp.notify.info(
                                                    l('SuccessfullyDeleted')
                                                );
                                                dataTable.ajax.reload();
                                            });
                                    }
                                }
                            ]
                    }
                },
                {
                    title: l('Name'),
                    data: "name"
                }                
            ]
        })
    );
   

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $('#NewWindowButton').click(function (e) {
        e.preventDefault();
        createModal.open();
    });
});