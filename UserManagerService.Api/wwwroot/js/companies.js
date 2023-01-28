app.controller("companies", function ($scope, $http, DTOptionsBuilder, httpRequest) {

    $scope.init = function () {

        $('#companies-link').addClass("active");

        $scope.companies = [];
        $scope.dateFormat = 'dd/MM/yyyy hh:mm:ss';
        $scope.requestInput = { type: 1 };
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

        $scope.getCompanies();
    };

    //$('#login_submit').click(async function () {
    //    var bodyData = {
    //        Username: $('#Username').val(),
    //        Password: $('#Password').val()
    //    };
    //    var jsonStringData = JSON.stringify(bodyData);
    //    var url = "/api/auth/login";

    //    $.ajax({
    //        method: "POST",
    //        url: domain + url,
    //        headers: { 'Content-Type': 'application/json;charset=utf-8' },
    //        data: jsonStringData,
    //        success: function (data, status, request) {
    //            //var headers = request.getAllResponseHeaders(); 
    //            data.data.expiresAt = moment(Date.now()).add(data.data.duration, 'm').toDate();
    //            localStorage.setItem('Auth', JSON.stringify(data.data));
    //            setCookie('Authentication', data.data.accessToken, 1);
    //            alert2('success', `Logged in successfully`);
    //            setTimeout(window.location.href = "/home", 3000)
    //            //window.location.href = "/home";
    //        },
    //        error: function (error) {
    //            // sweet alert
    //            alert2('error', `Failed to login`);
    //        }
    //    });
    //});

    $scope.addCompany = function () {
        var model = angular.copy($scope.requestInput);

        //var myFile = $('#logo').prop('files');
        var fileToUpload = $('#logo').prop('files')[0];
        debugger;
        //if (model.password != model.repeatedPassword) {
        //    alert2("error", "Password does not match repeated password");
        //    return;
        //}
        var location = {
            name: $('#location_name').val(),
            description: $('#location_description').val(),
            latitude: $('#latitude').val(),
            longitude: $('longitude').val()
        };

        var formData = new FormData();
        formData.append('name', model.name);
        formData.append('description', model.description);
        formData.append('type', model.type);
        formData.append('logo', fileToUpload);
        formData.append('location.name', location.name);
        formData.append('location.description', location.description);
        formData.append('location.latitude', location.latitude);
        formData.append('location.longitude', location.longitude);

        var requestModel = {
            method: "POST", url: '/api/companies',
            contentType: undefined,
            //contentType:'multipart/form-data',//multipart/form-data
            //contentType: "application/x-www-form-urlencoded",
            errorMessage: "Failed to send request",
            model:formData,
            successCallBack: $scope.addCompanyCallBack,
            //errorCallBack: $scope.addRequestErrorCallBack
        };
        httpRequest.send(requestModel);
    }

    $scope.addCompanyCallBack = function (result) {
        
        if (result.data.error) {
            alert2("error", result.data.message);
        }
        if (result.data.error == false && result.data.data != null) {
            $scope.companies.push(result.data.data);
            alert2("success", "Company created successfully", 5000, null, 'top-right');
            $('#companiesModal').modal('hide');
        }
    };

    $scope.getCompanies = function () {

        var requestModel = {
            method: "GET", url:'/api/companies',
            errorMessage: "Failed to get Companies",
            //model: $scope.newStock,
            successCallBack: $scope.getCompaniesCallBack,
            //errorCallBack: $scope.addStockCallBack
        };
        httpRequest.send(requestModel);
    };

    $scope.getCompaniesCallBack = function (result) {
        if (result.status == 200 && !result.data.error &&  result.data.data.length > 0)
            $scope.companies = result.data.data;
    };

    $scope.getName = function (id, data) {
        var item = data.find(i => i.id == id);
        return item ? item.name : id;
    }
    $scope.getCompany = function (id) {
        var item = $scope.companies.find(i => i.id == id);
        return item;
    }

    $scope.getItem = function (id, data) {
        var item = data.find(i => i.id == id);
        return item ? item : null;
    }

    $scope.add = function () {
        $scope.isEdit = false;
        $scope.requestInput = {
            name:""
        };
        //$('#add_test_form').attr('action', "/admin/addTest");
        $('#companiesModal').modal('show');
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
        var url = '/api/companies/';
 
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
                $scope.Companies.push(result.data.data);
            }
            else if ($scope.actionType == 3) {
                var item = $scope.getItem($scope.actionId, $scope.companies);
                var data = result.data.data
                /*TODO fill data*/
                item.name = data.name;
                item.description = data.description;
                item.type = data.type;
                item.createdAt = data.createdAt;
                item.id = data.id;
                item.updatedAt = data.updatedAt;
            }
            else if ($scope.actionType == 4) {
                var item = $scope.getItem($scope.actionId, $scope.companies);
                $scope.deleteArrayItem($scope.companies, item);
            }
            else {

            }
            alert2("success", result.data.message, 5000, null, 'top-right');
            $('#companiesModal').modal('hide');
        }
    };

    $scope.deleteArrayItem = function (array, item) {
        const index = array.indexOf(item);
        array.splice(index, 1);
    };
    $scope.edit = function (id) {
        var item = $scope.getItem(id, $scope.companies);
        if (item) {
            // Assign
            var model = angular.copy(item);
            //model.validity = model.validityId.toString();
            $scope.requestInput = model;
            $scope.isEdit = true;
            $scope.actionId = id;
            $('#companiesModal').modal('show');
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