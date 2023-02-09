function banPublication(id) {
    $.ajax({
        url: "/Admin/BanPublication",
        method: "POST",
        data: { "id": id },
        success: function () {
            document.getElementById("status").innerHTML = "Banned"
        }

        
    })
}

function unbanPublication(id) {
    $.ajax({
        url: "/Admin/UnBanPublication",
        method: "POST",
        data: { "id": id },
        success: function () {
            document.getElementById("status").innerHTML = "Accepted"
        }


    })
}