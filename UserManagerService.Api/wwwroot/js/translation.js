
app.controller("translation", function ($scope, $http, DTOptionsBuilder, httpRequest) {

    $scope.init = function () {
        //const profile = localStorage.getItem('Profile');
        //$scope.user = JSON.parse(profile);
        $scope.translation = [];
        $scope.dateFormat = 'dd/MM/yyyy hh:mm:ss';
        $scope.auth = { accessToken: "" };
        $scope.requestInput = {};
        $scope.dtOptions = DTOptionsBuilder.newOptions()
            .withButtons([
                {
                    extend: "excelHtml5",
                    filename: "File",
                    title: '',
                    exportData: { decodeEntities: false }
                }])
            .withDOM("Bfrtip")
            .withOption("pageLength", 7).withOption('order', [0, 'desc']);  //'<"top"Blf>tipr'

        $scope.domain = "https://translation.rainycorp.net";
        $scope.actionType = 0; //CRUD
        $scope.getTranslations();
    };

    $scope.getTranslations = function () {

        $http.get($scope.domain + '/api/translations').then(function (result, status, headers, config) {
            if (result.status == 200 && result.data.data.length > 0)
                $scope.translation = result.data.data;
        },
            function (error) {
                alert2('error', `Failed to get translations`);
            });
    };

    $scope.getName = function (id, data) {
        var item = data.find(i => i.id == id);
        return item ? item.name : id;
    }

    $scope.getItem = function (id, data) {
        var item = data.find(i => i.id == id);
        return item ? item : null;
    }

    $scope.add = function () {
        if ($scope.isEdit)
            $scope.requestInput = {};
           $scope.isEdit = false;
        $('#translationModal').modal('show');
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
        var url = $scope.domain + '/api/translations/';

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
                $scope.translation.push(result.data.data);
            }
            else if ($scope.actionType == 3) {
                var item = $scope.getItem($scope.actionId, $scope.translation);
                var data = result.data.data
                /*TODO fill data*/
                item.english = data.english;
                item.french = data.french;
                item.spanish = data.spanish;
                item.german = data.german;
                item.russian = data.russian;
                item.italian = data.italian;
            }
            else if ($scope.actionType == 4) {
                var item = $scope.getItem($scope.actionId, $scope.translation);
                $scope.deleteArrayItem($scope.translation, item);
            }
            else {

            }
            alert2("success", result.data.message, 5000, null, 'top-right');
            $('#translationModal').modal('hide');
        }
    };

    $scope.deleteArrayItem = function (array, item) {
        const index = array.indexOf(item);
        array.splice(index, 1);
    };
    $scope.edit = function (id) {
        var item = $scope.getItem(id, $scope.translation);
        if (item) {
            // Assign
            var model = angular.copy(item);
            //model.validity = model.validityId.toString();
            $scope.requestInput = model;
            $scope.isEdit = true;
            $scope.actionId = id;
            $('#translationModal').modal('show');
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