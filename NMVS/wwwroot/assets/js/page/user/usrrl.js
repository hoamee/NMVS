var routeURL = location.protocol + "//" + location.host;
$(document).ready(function () {
    $('.nav-user').addClass('active');
    $('.nav-user-role').addClass('active');
    $('.user-div').addClass('show');

});

function editRole(usrId) {

    $.ajax({
        url: '/api/Users/GetUserRole/' + usrId,
        type: 'GET',
        dataType: 'JSON',
        success: function (res) {
            if (res.status === -1) {
                $.notify("Error!");
            }
            else if (res.status === 1) {
                var obj = res.dataenum;
                $('.u-acc').text(obj.userName);
                $('#u-um').prop('checked', obj.userManagement);
                $('#u-ri').prop('checked', obj.requestInv);
                $('#u-hr').prop('checked', obj.handleRequest);
                $('#u-rcv').prop('checked', obj.receiveInv);
                $('#u-qc').prop('checked', obj.qc);
                $('#u-crso').prop('checked', obj.createSO);
                $('#u-apso').prop('checked', obj.appSO);
                $('#u-reg').prop('checked', obj.regVehicle);
                $('#u-guard').prop('checked', obj.guard);
                $('#u-wo').prop('checked', obj.woCreation);
                $('#u-worp').prop('checked', obj.woReporter);
                $('#u-active').prop('checked', obj.active);
                $('#u-arrange').prop('checked', obj.arrangeInventory);
                $('#u-moveinv').prop('checked', obj.moveInv);
                $('#is-upln').prop('checked', obj.issueUnplanned);
                $('#seeding-modal').modal('toggle');
            }
            else if (res.status === 0) {
                $.notify(res.message);
            }

        }
    });
}


function seedRole() {
    var uName = $('.u-acc').text().trim();
    var usrRole = {
        UserName: uName,
        UserManagement: $('#u-um').is(":checked"),
        requestInv: $('#u-ri').is(":checked"),
        handleRequest: $('#u-hr').is(":checked"),
        ReceiveInv: $('#u-rcv').is(":checked"),
        QC: $('#u-qc').is(":checked"),
        CreateSO: $('#u-crso').is(":checked"),
        AppSO: $('#u-apso').is(":checked"),
        RegVehicle: $('#u-reg').is(":checked"),
        Guard: $('#u-guard').is(":checked"),
        Active: $('#u-active').is(":checked"),
        WoReporter: $('#u-wo').is(":checked"),
        WoCreation: $('#u-worp').is(":checked"),
        MoveInv: $('#u-moveinv').is(":checked"),
        IssueUnplanned: $('#is-upln').is(":checked"),
        ArrangeInventory: $('#u-arrange').is(":checked")
    };


    $.ajax({
        url: routeURL + '/api/Users/SeedUserRole',
        type: 'POST',
        data: JSON.stringify(usrRole),
        contentType: 'application/json',
        success: function (res) {
            if (res.status === 1) {

                $.notify("Done!", 'success');
                setTimeout(
                    function () {

                        window.location.href = window.location.href;
                    }, 1000);
            }
            if (res.status === -1) {
                $.notify("Error!");
            }
            if (res.status === 0) {
                $.notify(res.message);
            }
        }
    });
}
