(function () {
    'use strict';

    var fields = JSON.parse('{ fieldsJson }');
    var isLoginEnabled = { isLoginEnabled };
    var submitButton = '{ submitButton }';

    var parentForm = null;
    fields.forEach(function (field) {
        var element = document.querySelector('#' + field.name + ', [name=' + field.name + ']');
        if (!element)
            return;
        if (parentForm === null && element.form)
            parentForm = element.form;
        switch (element.nodeName) {
            case 'INPUT':
                switch (element.type) {
                    case 'text':
                    case 'password':
                    case 'email':
                    case 'number':
                    case 'tel':
                    case 'url':
                    case 'range':
                    case 'search':
                        element.value = field.value;
                        break;
                    case 'checkbox':
                    case 'radio':
                        element = document.querySelector(
                            'input[type=radio][name=' + field.name + '][value=' + field.value + ']');
                        element.checked = true;
                        break;
                }
                break;
            case 'TEXTAREA':
            case 'SELECT':
                element.value = field.value;
                break;
        }
    });

    if (!isLoginEnabled)
        return;
    var submitButtonElement;
    if (submitButton) {
        submitButtonElement = document.querySelector('#' + submitButton + ', [name=' + submitButton + ']');
    } else if (parentForm) {
        submitButtonElement = parentForm.querySelector('input[type="submit"], button[type="submit"], button');
    }
    if (submitButtonElement)
        submitButtonElement.click();
})();