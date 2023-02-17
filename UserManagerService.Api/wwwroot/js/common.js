const domain = "https://rainycorp.net";
const domainUsers = "https://users.rainycorp.net";
//"https://localhost:44332";
//angular.element($0).scope().request

$(document).ready(function () {
    //if (inIframe()) {
    //    sendDataToParent();
    //}
});

var alert2 = function (type, message, timer = null, link = null, position = null) {

    var messages = "";

    if (typeof (message) !== "string") {
        message.forEach(function (value) {
            messages = messages + value + "<br/>";
        })
        message = messages;
    }

    Swal.fire({
        position: position,// 'top-right',
        // type: type,
        icon: type,
        title: type == 'success' ? "Success" : type == 'error' ? "Error" : null,
        //html: message,
        text: message,
        button: true,
        //showConfirmButton: true,
        confirmButtonText: "Ok",
        showCancelButton: false,
        //cancelButtonText: "@Translate["OK"]",
        //buttons: ["@Translate["OK"]"],
        /*buttons: {
            cancel: {
                text: "Oops! No",
                value: null,
                visible: true,
                className: "",
                closeModal: true
            },
            confirm: {
                text: "Delete It Already",
                value: true,
                visible: true,
                className: "",
                closeModal: true
            }
        },*/
        timer: timer,
        footer: link == null ? null : `<a class="btn btn-primary" href="${link}">Go to page</a>`
    });
};

var alertConfirm = function (callBack, message = null) {
    swal({
        title: 'Are you sure?',
        text: message ? message : "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes!'
    }).then((result) => {
        if (result.isConfirmed) {
            callBack();
        }
    })
}
function setCookie(cname, cvalue, exdays) {
    const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/;SameSite=None; Secure";
}

function inIframe() {
    try {
        return window.self !== window.top;
    } catch (e) {
        return true;
    }
}

var getTokensFromLocalStorage = function () {
    const auth = localStorage.getItem('Auth');
    return JSON.parse(auth);
}
function isUserLoggedIn() {
    const auth = getTokensFromLocalStorage();
    // if auth exists and not exipr
    debugger;
    if (auth == null || auth == undefined)
        return false;
    if (auth?.accessToken == null || auth?.accessToken == undefined)
        return false;
    //if (auth.expires)
    // check expiration here
    return true;
}

var sendDataToParent = function () { // Tell parent app if you are logged in
    var data = getTokensFromLocalStorage();

    var isLoggedIn = isUserLoggedIn();
    if (!isLoggedIn) {
        const message = JSON.stringify({
            message: 'Data from user management',
            date: Date.now(),
            data: { isUserLoggedIn: isLoggedIn }
        });
        window.parent.postMessage(message, '*');
    }
    else
        location.href = "/home";
};

window.addEventListener('message', function (e) {
    //debugger;

    if (inIframe()) // Can remove stuff
    {
        //document.getElementById('topbar').style.display = 'none';
        //$('#topbar').removeClass('align-items-stretch flex-shrink-0');
        $('.topbar').hide();
        //$('#topbar').css('display', 'none');

        if (e.data) {
            debugger;
     
            const data = JSON.parse(e.data);
            console.log("User management received data", data.message);
            //or you login 
            var auth = data.data;
            if (auth.accessToken == null || auth.accessToken == undefined) {
                loginWithSession(auth.sessionId, auth.userId, auth.companyId);
            }
            else {//either you use token 
                localStorage.setItem('Auth', JSON.stringify(auth));
                setCookie('Authentication', auth.accessToken, 1);
                window.location.reload();
            }
            
        }
    }
});

var loginWithSession = function (sessionId, userId,companyId) {
    var bodyData = {
        sessionId, userId, companyId
    };
    var jsonStringData = JSON.stringify(bodyData);
    var url ="/api/v4/login";

    $.ajax({
        method: "POST",
        url,
        headers: { 'Content-Type': 'application/json;charset=utf-8' },
        data: jsonStringData,
        success: function (result, status, request) {
            //var headers = request.getAllResponseHeaders(); 
            if (result.error || result.responseCode == 500) {
                alert2('error', result.message);
                submitButton.removeAttribute('data-kt-indicator');
                // Enable button
                submitButton.disabled = false;
                return;
            }

            result.data.expiresAt = moment(Date.now()).add(result.data.duration, 'm').toDate();
            localStorage.setItem('Auth', JSON.stringify(result.data));
            setCookie('Authentication', result.data.accessToken, 1);
            alert2('success', `Logged in successfully`);

            setTimeout(window.location.href = "/home", 3000);
        },
        error: function (error) {
            // sweet alert
            alert2('error', `Failed to login`);
        }
    });
};


//function setCookie(cookieName, cookieValue, daysToExpire) {
//    let expirationDate = new Date();
//    expirationDate.setDate(expirationDate.getDate() + daysToExpire);
//    let expires = "expires=" + expirationDate.toUTCString();

//    document.cookie = cookieName + "=" + cookieValue + ";" + expires + ";path=/";
//}

//Checking this
var now, timeToExpire;

function updateTime() {

    now = moment();

    timeToExpire = expire_timestamp.diff(now, 'seconds');
}

function timer() {
    updateTime();
    $('.output').html(`Expires in: ${displayClock(timeToExpire)}`)
}