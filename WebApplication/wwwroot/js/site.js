function ClearAllInput(selector) {
    $(selector).each(function () {
        this.value = "";
    });
}

function ShowConfimDelete(table, url, id) {
    swal({
        title: 'Delete',
        text: "Are you sure want to delete the data ?",
        icon: 'warning',
        buttons: [
            'Cancel',
            'Yes'
        ],
    }).then((value) => {
        if (value) {
            $.ajax({
                url: url,
                data: { id: id },
                method: "POST",
                success: function (response) {
                    if (response.isSuccess) {
                        table.ajax.reload();
                        swal(
                            'Deleted!',
                            'Your data has been deleted.',
                            'success'
                        )
                    } else {
                        swal('Delete', "Delete failed. Message: " + response.error, "error");

                    }
                },
                error: function () {
                    swal('Delete', "Delete failed. Message: " + response.error, "error");
                }
            });
        }
    });
}

function UpdateNestedSelect(apiUrl, parameter, selectorSelect) {
    selectorSelect.clearChoices();
    selectorSelect.removeActiveItems();
    if (parameter) {
        $.ajax({
            type: "GET",
            url: apiUrl, // The base API URL
            data: { param: parameter }, // The single parameter being sent
            contentType: "application/json",
            success: function (data) {
                var items = data.map(function (items) {
                    return { value: items.id, label: items.name };
                });
                selectorSelect.setValue(items);
            },
            error: function (error) {
                console.error("Error:", error);
                // Handle the error
            }
        });
    }
}