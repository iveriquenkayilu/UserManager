app.controller("profile", function ($scope, $http, DTOptionsBuilder, httpRequest) {

    $scope.init = function (id = null) {
        $('#profile-link').addClass("active");

        const profile = localStorage.getItem('Profile');
        $scope.user = JSON.parse(profile);
        $scope.dateFormat = 'dd/MM/yyyy hh:mm:ss';
        $scope.auth = { accessToken: "" };
        $scope.requestInput = angular.copy($scope.user);
        //{
        //    name: user.name,
        //    surname: user.surname,
        //    username: user.username,
        //    picture: user.picture,

        //    organizationId: 0
        //};

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
    };

    $scope.edit = function () {
        //$scope.requestInput = {
        //    name: "",
        //    surname: "",
        //    email: "",
        //    username:""
        //};
        $('#profileModal').modal('show');
    };

    //Edit
    $scope.updateProfile = function () {
        var model = angular.copy($scope.requestInput);
        debugger;
        // validation
        //if (true)
        //{
        //    alert2("error", "");
        //    return;
        //}           

        var requestModel = {
            method: "PUT", url:'/api/users/'+$scope.user.id+'',
            errorMessage: "Failed to send request",
            model: model,
            successCallBack: $scope.updateProfileCallBack,
            //errorCallBack: $scope.addRequestErrorCallBack
        };
        httpRequest.send(requestModel);
    }

    $scope.updateProfileCallBack = function (result) {
        debugger;
        if (result.data.error) {
            alert2("error", result.data.message);
        }
        if (result.data.error == false && result.data.data != null) {
            $scope.user=(result.data.data);
            alert2("success", "User updated successfully", 5000, null, 'top-right');
            $('#profileModal').modal('hide');
        }        
    };
});