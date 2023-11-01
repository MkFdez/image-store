document.getElementById("PublicationsList").innerHTML = `
<div style="display:flex; justify-content:center">
<div class="loader">
    <div class="circle"></div>
    <div class="circle"></div>
    <div class="circle"></div>
    <div class="circle"></div>
</div>
</div >`
let search = document.getElementById("Hidden") != null ? document.getElementById("Hidden").value : "";
let mG = document.getElementById("Hidden2") != null ? document.getElementById("Hidden2").value : false;
let cat =  document.getElementById("Hidden3") != null ? document.getElementById("Hidden3").value : "";
let oFm = document.getElementById("Hidden4") != null ? document.getElementById("Hidden4").value : false;
mG = mG == "True" ? true : false;
if (search != "") {
    $.ajax({
        dataType: "html",
        url: "/Publication/ChangePage",
        data: { "search": search, "personalPage": mG, onlyForMe = oFm },
        type: "POST",
        success: function (result) { document.getElementById("PublicationsList").innerHTML = result; },

    }
    )
} else {
    $.ajax({
        dataType: "html",
        url: "/Publication/ChangePage",
        data: { "category": cat, "personalPage": mG, onlyForMe = oFm },
        type: "POST",
        success: function (result) {
            document.getElementById("PublicationsList").innerHTML = result;
            window.dispatchEvent(new Event('resize'));
        },

    }
    )
}

function setGif() {
   
    document.getElementById("PublicationsList").innerHTML = `
<div style="display:flex; justify-content:center; margin-top: 20px">
<div class="loader">
    <div class="circle"></div>
    <div class="circle"></div>
    <div class="circle"></div>
    <div class="circle"></div>
</div>
</div >`
    

}