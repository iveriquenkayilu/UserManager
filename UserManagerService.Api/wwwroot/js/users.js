app.controller("users", function ($scope, $http, DTOptionsBuilder, httpRequest) {

    $scope.init = function () {
        const profile = localStorage.getItem('Profile');
        $scope.user = JSON.parse(profile);
        $scope.users = [];
        $scope.dateFormat = 'dd/MM/yyyy hh:mm:ss';
        $scope.auth = { accessToken: "" };
        $scope.requestInput = { organizationId: 0};
        $scope.dtOptions2 = { /*paging: false,*/ searching: true, pageLength: 7 };
        $scope.dtOptions = DTOptionsBuilder.newOptions()
            .withButtons([
                {
                    extend: "excelHtml5",
                    filename: "File",
                    title: '',
                    exportData: { decodeEntities: false }
                }])
            .withDOM("Bfrtip") //.withOption("ordering", false)
            .withOption("pageLength", 7).withOption('order', [0, 'desc']);  //'<"top"Blf>tipr'

        $scope.getUsers();
    };

    $scope.addUser = function () {
        var model = angular.copy($scope.requestInput);
        debugger;
        if (model.password != model.repeatedPassword)
        {
            alert2("error", "Password does not match repeated password");
            return;
        }           

        var requestModel = {
            method: "POST", url: domain + '/api/auth/register',
            errorMessage: "Failed to send request",
            model: model,
            successCallBack: $scope.addUserCallBack,
            //errorCallBack: $scope.addRequestErrorCallBack
        };
        httpRequest.send(requestModel);
    }

    $scope.addUserCallBack = function (result) {
        debugger;
        if (result.data.error) {
            alert2("error", result.data.message);
        }
        if (result.data.error == false && result.data.data != null) {
            $scope.users.push(result.data.data);
            alert2("success", "User created successfully", 5000, null, 'top-right');
            $('#usersModal').modal('hide');
        }        
    };

    $scope.getUsers = function () {

        var requestModel = {
            method: "GET", url: domain + '/api/auth/users/admin',
            errorMessage: "Failed to get users",
            //model: $scope.newStock,
            successCallBack: $scope.getUsersCallBack,
            //errorCallBack: $scope.addStockCallBack
        };
        httpRequest.send(requestModel);
    };

    $scope.getUsersCallBack = function (result) {
        if (result.status == 200 && result.data.length > 0)
            $scope.users = result.data;
    };

    $scope.getName = function (id, data) {
        var item = data.find(i => i.id == id);
        return item ? item.name : id;
    }
    $scope.getUser = function (id) {
        var item = $scope.users.find(i => i.id == id);
        return item ? item.name + " " + item.surname : id;
    }

    $scope.getItem = function (id, data) {
        var item = data.find(i => i.id == id);
        return item ? item : null;
    }

    $scope.add = function () {
        $scope.isEdit = false;
        //$('#add_test_form').attr('action', "/admin/addTest");
        $('#usersModal').modal('show');
    };
});