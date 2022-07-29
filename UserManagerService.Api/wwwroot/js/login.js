$(document).ready(function () {
    //localStorage.removeItem("Auth");
    //localStorage.removeItem("Profile");
});

var login = async function () {
    var bodyData = {
        Email: $('#Email').val(),
        Username: $('#Username').val(),
        Password: $('#Password').val()
    };
    var jsonStringData = JSON.stringify(bodyData);
    var url ="/api/login";

    $.ajax({
        method: "POST",
        url,
        headers: { 'Content-Type': 'application/json;charset=utf-8' },
        data: jsonStringData,
        success: function (result, status, request) {
            //var headers = request.getAllResponseHeaders(); 
            if (result.error || result.responseCode==500) {
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
                
            setTimeout(window.location.href = "/home", 3000)
            //window.location.href = "/home";
        },
        error: function (error) {
            // sweet alert
            alert2('error', `Failed to login`);
            submitButton.removeAttribute('data-kt-indicator');
            // Enable button
            submitButton.disabled = false;
        }
    });
};

"use strict";
var submitButton;

// Class definition
var KTSigninGeneral = function () {
    // Elements
    var form;
  
    var validator;

    // Handle form
    var handleForm = function (e) {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'email': {
                        validators: {
                            notEmpty: {
                                message: 'Email address is required'
                            },
                            emailAddress: {
                                message: 'The value is not a valid email address'
                            }
                        }
                    },
                    'password': {
                        validators: {
                            notEmpty: {
                                message: 'The password is required'
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row'
                    })
                }
            }
        );

        // Handle form submit
        submitButton.addEventListener('click', function (e) {
            // Prevent button default action
            e.preventDefault();

            // Validate form
            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    // Show loading indication
                    submitButton.setAttribute('data-kt-indicator', 'on');

                    // Disable button to avoid multiple click 
                    submitButton.disabled = true;

                    login();
                    // Simulate ajax request


                    //setTimeout(function () {
                    //    // Hide loading indication
                    //    submitButton.removeAttribute('data-kt-indicator');

                    //    // Enable button
                    //    submitButton.disabled = false;

                    //    // Show message popup. For more info check the plugin's official documentation: https://sweetalert2.github.io/
                    //    Swal.fire({
                    //        text: "You have successfully logged in!",
                    //        icon: "success",
                    //        buttonsStyling: false,
                    //        confirmButtonText: "Ok, got it!",
                    //        customClass: {
                    //            confirmButton: "btn btn-primary"
                    //        }
                    //    }).then(function (result) {
                    //        if (result.isConfirmed) {
                    //            form.querySelector('[name="email"]').value = "";
                    //            form.querySelector('[name="password"]').value = "";
                    //            //form.submit(); // submit form
                    //        }
                    //    });
                    //}, 2000);


                } else {
                    // Show error popup. For more info check the plugin's official documentation: https://sweetalert2.github.io/
                    Swal.fire({
                        text: "Sorry, looks like there are some errors detected, please try again.",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Ok, got it!",
                        customClass: {
                            confirmButton: "btn btn-primary"
                        }
                    });
                }
            });
        });
    }

    // Public functions
    return {
        // Initialization
        init: function () {
            form = document.querySelector('#kt_sign_in_form');
            submitButton = document.querySelector('#kt_sign_in_submit');

            handleForm();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTSigninGeneral.init();
});
