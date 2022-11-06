const domain = "https://rainycorp.net";
const domainUsers = "https://users.rainycorp.net";
//"https://localhost:44332";
//angular.element($0).scope().request

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
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
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

var sendDataToParent = function () {
    var data = getTokensFromLocalStorage();
    if (data) {
        debugger;
        const message = JSON.stringify({
            message: 'Data from user management',
            date: Date.now(),
            data: data
        });
        window.parent.postMessage(message, '*');
    }
};

window.addEventListener('message', function (e) {
    debugger;

    if (inIframe()) // Can remove stuff
    {
        //document.getElementById('topbar').style.display = 'none';
        //$('#topbar').removeClass('align-items-stretch flex-shrink-0');
        $('.topbar').hide();
        //$('#topbar').css('display', 'none');

        if (e.data) {
            console.log("User management received data");
            const data = JSON.parse(e.data);
            localStorage.setItem('Auth', JSON.stringify(data));
            setCookie('Authentication', data.accessToken, 1);
            window.location.reload();
        }
        if (e.data == null)
            sendDataToParent();
    }
});

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