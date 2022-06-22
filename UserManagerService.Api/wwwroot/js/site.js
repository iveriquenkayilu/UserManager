
$(document).ready(function () {

    var redirectTo = localStorage.getItem("RedirectTo")
    if (redirectTo) {
        if (redirectTo != window.location.href) {
            localStorage.removeItem("RedirectTo");
            window.location.href = redirectTo;
        }
    }

    getProfileFromLocalStorage();
});

var getTokensFromLocalStorage = function () {
    const auth = localStorage.getItem('Auth');
    return JSON.parse(auth);
}

var getProfileFromLocalStorage = function () {

    const auth = localStorage.getItem('Auth');
    // if auth exists and not exipr
    if (auth != null && auth != undefined) {
        /*tokens = JSON.parse(auth);*/
        const profile = localStorage.getItem('Profile');
        if (profile) {
            // use it, or update it after 5 min for example
            if (profile.expiresAt > Date.now())
                fillUserProfile(JSON.parse(profile));
            else
                getUserProfile();
        }
        else
            getUserProfile();
    }
    else
        redirectToLogin();
};

var getUserProfile = function () {
    // get profile
    ajaxRequest("/api/auth/me", "GET").then((result) => {
        result.expiresAt = moment(Date.now()).add(5, 'm').toDate();
        localStorage.setItem('Profile', JSON.stringify(result));
        fillUserProfile(result);
    });
};

var fillUserProfile = function (profile) {
    if (profile?.name)
        $('#profile_name').html(profile.name);
}
var redirectToLogin = function () {

    if (!window.location.href.includes("/home/login")) {
        alert2('warning', `Redirecting to Login Page`);
        setTimeout(window.location.href = "/home/login", 3000)
    }
};

//setTimeout(function () {
//    something();
//}, 1000);

$('#logout_button').click(function () {
    localStorage.removeItem("Auth");
    localStorage.removeItem("Profile");
    // could send a request to the backend
    document.cookie = 'Authentication=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/';
    redirectToLogin();
});

async function ajaxRequest(url, method = "POST", jsonStringData = null) {
    // if the token is expired, request new one
    //if (tokens.expiresAt < Date.now()) ajaxRequest().then()
    //refreshFirst then use that token 
    //else 
    var auth = getTokensFromLocalStorage();
    return $.ajax({
        method: method,
        url: domain + url,
        headers: {
            'Content-Type': 'application/json;charset=utf-8',
            'Authorization': 'Bearer ' + auth.accessToken
            /*'Access-Control-Allow-Origin': '*'*/
        },
        data: jsonStringData, // stringfy for body data, serialiaz for forms, or use formData
    }).then((data, status, result) => {

        if (result.status == 401) {
            localStorage.setItem('RedirectTo', window.location.href);
            redirectToLogin();
        }
        return data;
    }).catch(error => {
        if (error.status == 401) {
            localStorage.setItem('RedirectTo', window.location.href);
            redirectToLogin();
        }
        return error;
    });
};

app.factory('httpRequest', function ($http) {
    return {
        request: function (input) {
            if (input.url == "" || input.url == null) alert("error", "The request url is not valid");

            var auth = getTokensFromLocalStorage();
            return $http({
                method: input.method == null ? "POST" : input.method,
                url: input.url,
                data: input.model == null ? {} : input.model,
                headers: {
                    contentType: input.contentType == null ? "application/json;charset=UTF-8" : input.contentType,
                    Authorization: 'Bearer ' + auth.accessToken
                }
            });
        },
        refreshToken: function (input) {

            debugger;
            var requestModel = {
                method: "POST",
                url: domain + '/api/auth/refresh-token',
                errorMessage: "Failed to refresh token",
                model: {
                    "refreshToken": auth.refreshToken
                },
                //successCallBack: $scope.addDocStoreCallBack,
                //errorCallBack: $scope.addStockCallBack
            };
            return this.request(requestModel).then(function (result) {
                debugger;
                if (!result.data.error) {
                    var auth = getTokensFromLocalStorage();
                    //assigns tokens to auth
                    auth = result.data.data;
                    auth.expiresAt = moment(Date.now()).add(result.data.data.duration, 'm').toDate();
                    localStorage.setItem('Auth', JSON.stringify(auth));
                    setCookie('Authentication', auth.accessToken, 1);

                    return this.request;
                } else return this.error(result);

            });
        },
        send: function (input) {

            var auth = getTokensFromLocalStorage();
            // check if you can get refresh token
            var test = moment(auth.expiresAt) < moment();
            if (test) {

                return this.refreshToken;
            }
            else

                return this.request(input).then(function (result) {
                    var isString = typeof (result.data);
                    if (isString == "string" && result.data.includes("<html")) {
                        alert('error', `A system error happened! Please contact the system management`); return;
                    }
                    if (input.showSuccessMessage) alert('success', input.successMessage);

                    if (input.successCallBack != null) input.successCallBack(result); //todo : possible refactor, other methods may also be updated.
                    else
                        return result;

                }, this.error)
        },
        error: function (error) {
            debugger;
            if (error.status == 401) {
                localStorage.setItem('RedirectTo', window.location.href);
                redirectToLogin();
            }

            alert2('error', `Error : ${error.data["Message"]}`); error.status = 400; return error;
        }
    }
});

var createTable = function (id, pageLegnth = 7, paging = true) {

    $('#' + id + '').dataTable({
        order: [[0, "asc"]],
        pageLength: pageLegnth,
        //lengthMenu: [10, 25, 50, 75, 100],
        paging: paging,
        //dom: "<<'d-flex justify-content-between' <'py-0 mt-3' B>f><rtip>>",
        //buttons: [
        //    {
        //        extend: 'csv', text: 'CVS', filename: '' + id + '_report'
        //    },
        //    { extend: 'pdf', text: 'PDF', filename: '' + id + '_report' }
        //]
    });
};


//// sets the cookie cookie1
//document.cookie = 'cookie1=test; expires=Sun, 1 Jan 2023 00:00:00 UTC; path=/'

//// sets the cookie cookie2 (cookie1 is *not* overwritten)
//document.cookie = 'cookie2=test; expires=Sun, 1 Jan 2023 00:00:00 UTC; path=/'
