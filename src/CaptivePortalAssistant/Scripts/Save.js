(function () {
    'use strict';

    var forms = [];
    for (var i = 0; i < document.forms.length; i++) {
        var fields = getFormFields(document.forms[i]);
        if (fields !== null)
            forms.push(getFormFields(document.forms[i]));
    }
    var allElements = document.querySelectorAll('input, textarea, select');
    var extraFormFields = [];
    for (var j = 0; j < allElements.length; j++) {
        var element = allElements[j];
        if (!element.form) {
            var extraField = getField(element);
            if (extraField)
                extraFormFields.push(extraField);
        }
    }
    if (extraFormFields.length > 0) {
        forms.push(extraFormFields);
    }
    return JSON.stringify(forms);


    function FormField(name, value) {
        this.name = name;
        this.value = value;
    }

    function getFormFields(form) {
        if (!form || form.nodeName !== 'FORM')
            return null;
        var formFields = [];
        for (var k = 0; k < form.elements.length; k++) {
            var field = getField(form.elements[k]);
            if (field) {
                formFields.push(field);
            }
        }
        return formFields.length > 0 ? formFields : null;
    }

    function getField(fieldElement) {
        if (fieldElement.name === '' && fieldElement.id === '')
            return null;
        switch (fieldElement.nodeName) {
            case 'INPUT':
                switch (fieldElement.type) {
                    case 'text':
                    case 'password':
                    case 'email':
                    case 'number':
                    case 'tel':
                    case 'url':
                    case 'range':
                    case 'search':
                        return new FormField(fieldElement.name || fieldElement.id, fieldElement.value);
                    case 'checkbox':
                    case 'radio':
                        if (fieldElement.checked)
                            return new FormField(fieldElement.name || fieldElement.id, fieldElement.value);
                        break;
                }
                break;
            case 'TEXTAREA':
            case 'SELECT':
                return new FormField(fieldElement.name || fieldElement.id, fieldElement.value);
        }
        return null;
    }
})();