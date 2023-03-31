var gif = document.createElement("img");
gif.style.maxWidth = "50px";
gif.style.maxHeight = "50px";

gif.src = "/uploads/loading.gif"
gif.style.marginLeft = "auto";
gif.style.marginRight = "auto"
document.getElementById("PublicationsList").appendChild(gif)
let search = document.getElementById("Hidden") != null ? document.getElementById("Hidden").value : "";
let mG = document.getElementById("Hidden2") != null ? document.getElementById("Hidden2").value : false;
let cat =  document.getElementById("Hidden3") != null ? document.getElementById("Hidden3").value : "";
mG = mG == "True" ? true : false;
if (search != "") {
    $.ajax({
        dataType: "html",
        url: "/Publication/ChangePage",
        data: { "search": search, "personalPage": mG },
        type: "POST",
        success: function (result) { document.getElementById("PublicationsList").innerHTML = result; },

    }
    )
} else {
    $.ajax({
        dataType: "html",
        url: "/Publication/ChangePage",
        data: { "category": cat, "personalPage": mG },
        type: "POST",
        success: function (result) { document.getElementById("PublicationsList").innerHTML = result; },

    }
    )
}

function setGif() {
    var gif = document.createElement("img");
    gif.style.maxWidth = "50px";
    gif.style.maxHeight = "50px";

    gif.src = "/uploads/loading.gif"
    document.getElementById("PublicationsList").innerHTML = gif
}