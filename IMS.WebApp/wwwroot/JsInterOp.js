function PreventFormSubmition(formId) {
    document.querySelector(`#${formId}`).addEventListener("keydown", function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    })
}