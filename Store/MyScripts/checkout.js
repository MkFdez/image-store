function onGo(){
    waitingDialog.show('Loading Checkout...', {

        // if the option is set to boolean false, it will hide the header and "message" will be set in a paragraph above the progress bar.
        // When headerText is a not-empty string, "message" becomes a content above the progress bar and headerText string will be set as a text inside the H3;
        headerText: 'Info',

        // this will generate a heading corresponding to the size number
        headerSize: 3,

        // extra class(es) for the header tag
        headerClass: '',

        // bootstrap postfix for dialog size, e.g. "sm", "m"
        dialogSize: 'm',

        // bootstrap postfix for progress bar type, e.g. "success", "warning";
        progressType: '',

        // determines the tag of the content element
        contentElement: 'p',

        // extra class(es) for the content tag
        contentClass: 'content'

    });
}
function onLoadSuccess(data) {
    window.location.href = data.url

}