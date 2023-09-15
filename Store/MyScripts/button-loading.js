function onLoad(id) {
    document.getElementById(id).innerHTML = `<div style='display:flex; justify-content:center'><span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span></div></div>`
}
function onLoadFail(id, text) {
    document.getElementById(id).innerHTML = text

}
function formSuccess(data, buttonid, text) {
    if (data.redirect) {
        window.location.href = data.redirect
    } else if (data.toast) {
        var toastEl = document.getElementById(`${data.toast}-toast`)
        console.log(toastEl)
        var toast = new bootstrap.Toast(toastEl, { animation: true, autohide: true, delay: 4000 })
        toast.show()
        document.getElementById(buttonid).innerHTML = text
    }
    else if(data.errors){
        console.log(data.errors)
        for (let error of data.errors) {
            if (error.type != "") {
                document.getElementById(error.type + "-error").innerHTML = error.message

            }
        }
        document.getElementById(buttonid).innerHTML = text
    }
}