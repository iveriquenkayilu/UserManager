﻿$(document).ready(function () {
    //localStorage.removeItem("Auth");
    //localStorage.removeItem("Profile");
});

var register = async function () {
    var bodyData = {
        Email: $("[name='email']").val(),
        Username: $("[name='username']").val(),
        Password: $("[name='password']").val(),
        Name: $("[name='first-name']").val(),
        Surname: $("[name='last-name']").val(),
    };
    debugger;
    var jsonStringData = JSON.stringify(bodyData);
    var url ="/api/register";

    $.ajax({
        method: "POST",
        url,
        headers: { 'Content-Type': 'application/json;charset=utf-8' },
        data: jsonStringData,
        success: function (result, status, request) {
            debugger;
            //var headers = request.getAllResponseHeaders(); 
            if (result.error || result.responseCode==500) {
                alert2('error', result.message);
                submitButton.removeAttribute('data-kt-indicator');
                // Enable button
                submitButton.disabled = false;
                return;
            }
                
            //result.data.expiresAt = moment(Date.now()).add(result.data.duration, 'm').toDate();
            //localStorage.setItem('Auth', JSON.stringify(result.data));
            //setCookie('Authentication', result.data.accessToken, 1);
            alert2('success', `Registered account successfully`);
            setTimeout(window.location.href = "/home/login", 3000)
            //window.location.href = "/home";
        },
        error: function (error) {
            debugger;
            alert2('error', `Failed to register`);
            submitButton.removeAttribute('data-kt-indicator');
            // Enable button
            submitButton.disabled = false;
        }
    });
};

"use strict";
var submitButton;

// Class definition
var KTSignupGeneral = function () {
    // Elements
    var form;
    var validator;
    var passwordMeter;

    // Handle form
    var handleForm = function (e) {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'first-name': {
                        validators: {
                            notEmpty: {
                                message: 'First Name is required'
                            }
                        }
                    },
                    'last-name': {
                        validators: {
                            notEmpty: {
                                message: 'Last Name is required'
                            }
                        }
                    },
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
                            },
                            callback: {
                                message: 'Please enter valid password',
                                callback: function (input) {
                                    if (input.value.length > 0) {
                                        return validatePassword();
                                    }
                                }
                            }
                        }
                    },
                    'confirm-password': {
                        validators: {
                            notEmpty: {
                                message: 'The password confirmation is required'
                            },
                            identical: {
                                compare: function () {
                                    return form.querySelector('[name="password"]').value;
                                },
                                message: 'The password and its confirm are not the same'
                            }
                        }
                    },
                    'toc': {
                        validators: {
                            notEmpty: {
                                message: 'You must accept the terms and conditions'
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger({
                        event: {
                            password: false
                        }
                    }),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row',
                        eleInvalidClass: '',
                        eleValidClass: ''
                    })
                }
            }
        );

        // Handle form submit
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();

            validator.revalidateField('password');

            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    // Show loading indication
                    submitButton.setAttribute('data-kt-indicator', 'on');

                    // Disable button to avoid multiple click 
                    submitButton.disabled = true;

                    register();
                    // Simulate ajax request             

                    //setTimeout(function () {
                    //    // Hide loading indication
                    //    submitButton.removeAttribute('data-kt-indicator');

                    //    // Enable button
                    //    submitButton.disabled = false;

                    //    // Show message popup. For more info check the plugin's official documentation: https://sweetalert2.github.io/
                    //    Swal.fire({
                    //        text: "You have successfully reset your password!",
                    //        icon: "success",
                    //        buttonsStyling: false,
                    //        confirmButtonText: "Ok, got it!",
                    //        customClass: {
                    //            confirmButton: "btn btn-primary"
                    //        }
                    //    }).then(function (result) {
                    //        if (result.isConfirmed) {
                    //            form.reset();  // reset form                    
                    //            passwordMeter.reset();  // reset password meter
                    //            //form.submit();
                    //        }
                    //    });
                    //}, 1500);

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

        // Handle password input
        form.querySelector('input[name="password"]').addEventListener('input', function () {
            if (this.value.length > 0) {
                validator.updateFieldStatus('password', 'NotValidated');
            }
        });
    }

    // Password input validation
    var validatePassword = function () {
        return (passwordMeter.getScore() === 100);
    }

    // Public functions
    return {
        // Initialization
        init: function () {
            // Elements
            form = document.querySelector('#kt_sign_up_form');
            submitButton = document.querySelector('#kt_sign_up_submit');
            passwordMeter = KTPasswordMeter.getInstance(form.querySelector('[data-kt-password-meter="true"]'));

            handleForm();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTSignupGeneral.init();
});