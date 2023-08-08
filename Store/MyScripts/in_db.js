var ok = true;
function check(not = false, edit = false) {
    ok = true
    let element = document.getElementById("userEntry")
    
    if (element != null) {
        let value = element.value
        $.ajax({
            url: "/Account/InDb",
            dataType: "text",
            data: { "text": value, "not": not, "edit": edit },
            success: function (result) {
                if (result == "1") {
                    document.getElementById("userVMessage").innerHTML = "User already exists"
                    ok = false;
                }
                checkEmail(not, edit)
            }
        })
    } else {
        checkEmail(not, edit)
    }
    function checkEmail(not, edit) {
        let value2 = document.getElementById("emailEntry").value
        $.ajax({
            url: "/Account/InDb",
            dataType: "text",
            data: { "text": value2, email: true, "not": not, "edit": edit },
            success: function (result) {
                if (result == "1") {
                    document.getElementById("emailVMessage").innerHTML = "Email already exists"
                    ok = false;
                }
                everythingFine()
            }
        })
    }
    function everythingFine() {
        if (ok == true) {
            alert("submited")
            $("#form").submit()

        }
    }
}