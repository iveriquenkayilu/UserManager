$(document).ready(function () {
});

var getAuth = function (sessionId, userId, companyId) {

    var url = "/api/v4/login";

    var data = {
        sessionId, userId, companyId: companyId === '' ? null : companyId
    };

        $.ajax({
            method: "POST",
            url: url,
            headers: { 'Content-Type': 'application/json;charset=utf-8' },
            data: JSON.stringify(data),
            success: function (data, status, request) {
                //var headers = request.getAllResponseHeaders();
                data.data.expiresAt = moment(Date.now()).add(data.data.duration, 'm').toDate();
                localStorage.setItem('Auth', JSON.stringify(data.data));
                setCookie('Authentication', data.data.accessToken, 1);
                alert2('success', `Logged in successfully`);
                setTimeout(window.location.href = "/home", 3000)
                //window.location.href = "/home";
            },
            error: function (error) {
                // sweet alert
                alert2('error', `Failed to authenticate`);
            }
        });
}