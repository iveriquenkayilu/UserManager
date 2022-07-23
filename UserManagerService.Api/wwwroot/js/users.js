app.controller("users", function ($scope, $http, DTOptionsBuilder, httpRequest) {

    $scope.init = function () {
        const profile = localStorage.getItem('Profile');
        $scope.user = JSON.parse(profile);
        $scope.users = [];
        $scope.dateFormat = 'dd/MM/yyyy hh:mm:ss';
        $scope.auth = { accessToken: "" };
        $scope.requestInput = { organizationId: 0 };
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
        if (model.password != model.repeatedPassword) {
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
            method: "GET", url:'/api/users/admin',
            errorMessage: "Failed to get users",
            //model: $scope.newStock,
            successCallBack: $scope.getUsersCallBack,
            //errorCallBack: $scope.addStockCallBack
        };
        httpRequest.send(requestModel);
    };

    $scope.getUsersCallBack = function (result) {
        if (result.status == 200 && !result.data.error &&  result.data.data.length > 0)
            $scope.users = result.data.data;
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

    //action

    $scope.action = function (actionType) { //CRUD
        $scope.actionType = actionType;

        var id = $scope.actionId;
        //TODO input validation    
        var model = (actionType == 1 | actionType == 3) ?
            angular.copy($scope.requestInput) : null;

        if (actionType == 1 | actionType == 3) {
            model.validity = parseInt(model.validity);
        }
        var method = actionType == 1 ? "POST" :
            actionType == 2 ? "GET" :
                actionType == 3 ? "PUT" :
                    actionType == 4 ? "DELETE" : "";
        var url = '/api/users/';
 
        if (actionType != 1)
            url += id.toString();

        var requestModel = {
            method: method,
            url: url,
            errorMessage: "Failed to execute action",
            model: model,
            successCallBack: $scope.actionCallBack
        };
        httpRequest.send(requestModel);
    }

    $scope.handleError = function (result) {
        if (result.data.error) {
            alert2("error", result.data.message);
        }
        if (result.data.message == "Error Occured") {
            alert2("error", result.data.message);
        }
    };

    $scope.actionCallBack = function (result) {

        $scope.handleError(result);

        if (result.status == 200 && result.data != null) {

            if ($scope.actionType == 1) {
                $scope.users.push(result.data.data);
            }
            else if ($scope.actionType == 3) {
                var item = $scope.getItem($scope.actionId, $scope.users);
                var data = result.data.data
                /*TODO fill data*/
                item.name = data.name;
                item.surname = data.surname;
                item.email = data.email;
                item.username = data.username;
            }
            else if ($scope.actionType == 4) {
                var item = $scope.getItem($scope.actionId, $scope.users);
                $scope.deleteArrayItem($scope.users, item);
            }
            else {

            }
            alert2("success", result.data.message, 5000, null, 'top-right');
            $('#usersModal').modal('hide');
        }
    };

    $scope.deleteArrayItem = function (array, item) {
        const index = array.indexOf(item);
        array.splice(index, 1);
    };
    $scope.edit = function (id) {
        var item = $scope.getItem(id, $scope.users);
        if (item) {
            // Assign
            var model = angular.copy(item);
            //model.validity = model.validityId.toString();
            $scope.requestInput = model;
            $scope.isEdit = true;
            $scope.actionId = id;
            $('#usersModal').modal('show');
        }
        else {
            alert2("error", "Item not found");
        }
    };

    $scope.delete = function (id) {
        $scope.actionId = id;

        swal({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            buttons: {
                cancel: {
                    text: "Oops! No",
                    value: null,
                    visible: true,
                    className: "",
                    closeModal: true,
                    color: '#d33'
                },
                confirm: {
                    text: "Delete It Already",
                    value: true,
                    visible: true,
                    className: "",
                    closeModal: true,
                    color: '#3085d6'
                }
            },
        }).then((result) => {
            if (result) {
                $scope.action(4);
            }
        });
    };
});