
var languages = [
    { lang: "English", flag: "assets/media/flags/united-states.svg", symbol: "en" },
    { lang: "French", flag: "assets/media/flags/france.svg", symbol: "fr" },
    { lang: "German", flag: "assets/media/flags/germany.svg", symbol: "de" },
    { lang: "Spanish", flag: "assets/media/flags/spain.svg", symbol: "es" },
];
$(document).ready(function () {

    //var redirectTo = localStorage.getItem("RedirectTo")
    //if (redirectTo) {
    //    if (redirectTo != window.location.href) {
    //        localStorage.removeItem("RedirectTo");
    //        window.location.href = redirectTo;
    //    }
    //}
   
    if (inIframe()) {
        sendDataToParent();
    }
    getProfileFromLocalStorage();
    var symbol = localStorage.getItem('lang');
    if (!symbol)
        symbol = "en";
    var language = languages.find(l => l.symbol == symbol);
    if (language) {
        $('#selected_lang').prepend(language.lang);
        $('#lang_image').attr('src', language.flag);
    }
  
    languages.forEach(l => {
        var active = "";
        if (l.symbol == symbol)
            active = " active";

        var onClick = `onclick=changeLanguage("${l.symbol}")`
        var lan = '<div class="menu-item px-3">' +
            '<a href="javascript:;" '+onClick+' data-lang='+l.symbol+' class="menu-link d-flex px-5 language' + active + '">' +
            '<span class="symbol symbol-20px me-4">' +
            '<img class="rounded-1" src=' + l.flag + ' alt="metronic" />' +
            '</span>' + l.lang + '</a ></div >';
        $('#languages').append(lan);
    });

    getTranslations(language); // = i18next.t('key');
});

var getTranslations = function (lang) {
    var data = {
        query: 'query{translations {english, ' + lang.lang.toLowerCase()+'}}',
        //"variables": {  }
    };

    var url = //"https://localhost:7001/graphql";
        "https://translation.rainycorp.net/graphql";

    fetch(url, {
        method: 'POST', // or 'PUT'
        headers: {
            'Content-Type': 'application/json',
              //'Content-Type': 'application/graphql'
        },
        body: JSON.stringify(data),
    })
        .then(response => response.json())
        .then(result => {

            if (result?.status == 401) {
                
                //alert2('error', `Failed to get translations`);
                return;
            }
            var symbol = localStorage.getItem('lang');
            var language = languages.find(l => l.symbol == symbol);
        
            var translation = result.data.translations.reduce(function (acc, curr) {
                var key = curr['english'];
                acc[key] = curr[language.lang.toLowerCase()];
                return acc;
            }, {});

            //i18next
            jqueryI18next.init({
                lng: symbol,
                debug: true,
                resources: {
                    [symbol]: {
                        translation
                    }
                }
            }, (err, t) => {
                // initialized and ready to go!
                console.log("i18next initialized");
                //document.getElementById('output').innerHTML = i18next.t('key');
            });
        })
        .catch((error) => {
           // alert2('error', `Failed to get translations`);
        });
};

//$(".language").on("click", function () {
//    var symbol = $(this).data('lang');
//    changeLanguage(symbol);
//});

var changeLanguage = function (symbol) {
    localStorage.setItem('lang', symbol);
    window.location.reload();
};

var getProfileFromLocalStorage = function () {

    const auth = localStorage.getItem('Auth');
    // if auth exists and not exipr
    if (auth != null && auth != undefined) {

        const profileString = localStorage.getItem('Profile');
         var profile= JSON.parse(profileString);
        if (profile) {
            // use it, or update it after 5 min for example
            if (profile.expiresAt > Date.now())
                fillUserProfile(profile);
            else
                getUserProfile();
        }
        else
            getUserProfile();
    }
};

var getUserProfile = function () {
    // get profile domainUsers

    var url = "/api/me";

    const auth = getTokensFromLocalStorage();
    if (!auth)
        return;
    var accessToken = auth.accessToken;
    $.ajax({
        method: "GET",
        url,
        headers: {
            'Content-Type': 'application/json;charset=utf-8',
            'Authorization': 'Bearer ' + accessToken
        },
        success: function (result, status,request) {

            if (result.status == 401) {
                //alert2('error', `Failed to get profile`);
                return;
            }

            result.expiresAt = moment(Date.now()).add(5, 'm').toDate();
            localStorage.setItem('Profile', JSON.stringify(result));
            fillUserProfile(result);
        },
        error: function (error) {
            //('error', `Failed to get profile`);
        }
    });
};

var fillUserProfile = function (profile) {
    var fullName = profile?.name + " " + profile?.surname;
    if (fullName)
        $('#profile_name').prepend(fullName);
    $('#user_email').html(profile?.email);
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

$('#go_to_login_button').click(function () {
    redirectToLogin();
});

$('#logout_button').click(function () {
    localStorage.removeItem("Auth");
    localStorage.removeItem("Profile");
    // could send a request to the backend
    document.cookie = 'Authentication=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/';
    window.location.href = "/";
    //redirectToLogin();
});

var httpInterceptor = function ($q) {
    return {
        // optional method
        request: function (config) {
            config.headers["Authorization"] = "Bearer " + JSON.parse(localStorage.getItem("Auth")).accessToken;;
            return config;
        },
        // optional method
        response: function (response) {
            // do something on success
            return response;
        },
        // optional method
        responseError: async function (response) {

            if (response.status === 401 && !response.config._retry) {
                response.config._retry = true;

                try {
                    const accessToken = await retryInterceptor(response);
                    if (accessToken) {
                        response.config.headers.Authorization = `Bearer ${accessToken}`;
                        return $q.resolve(response.config);
                    }
                }
                catch (e) {
                    return $q.reject(e);
                };
            }
            return $q.reject(response);
        },
    };
};

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push(httpInterceptor);
});

async function retryInterceptor(err) {

    const refreshToken = JSON.parse(localStorage.getItem("Auth")).refreshToken;

    return fetch(window.authenticationSettings.domain + "/api/auth/refresh-token", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ refreshToken })
    })
        .then(response => response.json())
        .then(result => {
            if (result.error)
                throw new Error(result.message);
            localStorage.setItem('Auth', JSON.stringify(result.data));
            setCookie(window.authenticationSettings.cookieName, result.data.accessToken, 1);
            return result.data.accessToken;
        });
}

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
                    //contentType: input.contentType == null ? "application/json;charset=UTF-8" : input.contentType,
                    "Content-Type": input.contentType === null ? "application/json;charset=UTF-8" : input.contentType
                    //Authorization: 'Bearer ' + auth.accessToken
                },
                //transformRequest: angular.identity,
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

            alert2('error', `Error : ${error.data["message"]}`); error.status = 400; return error;
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
