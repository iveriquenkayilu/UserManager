
app.controller("products", function ($scope, $http) {
    $scope.init = function (model) {
        $scope.model = model;
        $scope.dateFormat = 'dd/MM/yyyy';
        $scope.newAttribute = {};
        $scope.newProduct = {};     
    };

    $scope.addProduct = function () {

        //var requestModel = $('#product_form').serialize();

        $http({
            method: 'POST',
            url: '/Products/Add',
            headers: { 'Content-Type': 'application/json' },
            data: $scope.newProduct
        }).then(function (result) {
            $scope.model.products.push(result.data);
            alert("success", "The product has been added successfully", 5000, null, 'top-right');
        }, function (error) {
            var message = "Failed to add product : " + error.data;
            alert("error", message, null, null, 'top-right');
        });
    };

    $scope.addAttribute = function () {

        if ($scope.newAttribute.type == undefined || $scope.newAttribute.type == null)
            return alert("error", "The type cannot be null", null, null, 'top-right');

        $scope.newAttribute.type = parseInt($scope.newAttribute.type);

        $http({
            method: 'POST',
            url: '/Products/AddAttribute',
            headers: { 'Content-Type': 'application/json' },
            data: $scope.newAttribute
        }).then(function (result) {
            $scope.model.attributes.push(result.data);
            alert("success", "The attribute has been added successfully", 5000, null, 'top-right');
        }, function (error) {
                var message = "Failed to add attribute : " + error.data;
                alert("error", message , null, null, 'top-right');
        });
    };

    $scope.addBrand = function () {

        var requestModel = {
            name: $('#brand_name').val()
        }

        $http({
            method: 'POST',
            url: '/Products/AddBrand',
            headers: { 'Content-Type': 'application/json' },
            data: requestModel
        }).then(function (result) {
            $scope.model.brands.push(result.data);
            alert("success", "The brand has been added successfully", 5000, null, 'top-right');
        }, function (error) {
                alert("error", "Failed to add brand", null, null, 'top-right');
        });
    };
});