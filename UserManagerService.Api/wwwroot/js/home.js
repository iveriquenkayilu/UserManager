app.controller("home", function ($scope, $http, DTOptionsBuilder, httpRequest) {

    $scope.init = function () {
        $('#home-link').addClass("active");

        $scope.dateFormat = 'dd/MM/yyyy hh:mm:ss';
        //$scope.requestInput = angular.copy($scope.user);

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
            .withOption("pageLength", 5).withOption('order', [0, 'desc']);  //'<"top"Blf>tipr'
        $scope.sessions = [];
        $scope.getLoginSesssions();
    };

    //Edit
    $scope.getLoginSesssions = function () {
          

        var requestModel = {
            method: "GET", url:'/api/sessions',
            errorMessage: "Failed to get login history",
            //model: ,
            successCallBack: $scope.getLoginSesssionsCallBack,
            //errorCallBack: $scope.addRequestErrorCallBack
        };
        httpRequest.send(requestModel);
    }

    $scope.getLoginSesssionsCallBack = function (result) {

        if (result.data.error) {
            alert2("error", result.data.message);
        }
        if (result.data.error == false && result.data.data != null) {
            $scope.sessions = result.data.data;
        }        
    };
});