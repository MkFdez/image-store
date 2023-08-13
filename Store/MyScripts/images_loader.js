var count = 0;
var lastView = null
var observer = new IntersectionObserver(function (entries) {
    // isIntersecting is true when element and viewport are overlapping
    // isIntersecting is false when element and viewport don't overlap
    let actual = new Date()
    let diff = 0
    if (lastView == null) {
        diff = 1000
    } else {
        diff = (actual-lastView)
    }
    lastView = actual;
    if (entries[0].isIntersecting === true && diff >= 1000) {
        observer.unobserve(document.querySelector("#final_image"))
        ImageList()
    }
}, { threshold: [0] });


observer.observe(document.querySelector("#final_image"))
function ImageList() {
    $.ajax({
        url: "/Account/CreatorImages",
        dataType: "Json",
        data: { "username" : document.getElementById("username").value, "count" : count },
        method: "POST",
        success: function (response) {
            generateList(response)
        }
    })

    function generateList(response) {
        var data = JSON.parse(response)
        var elements = ""
     
        for (let i = 0; i < data.length; i++) {

            elements += `<div class="row mt-2"><a href="/Publication/View/${data[i].PublicationId}"><img style="width:100%" src='${data[i].Image}' /></a></div>`
            count++
        }
        document.querySelector("#image-container").insertAdjacentHTML("beforeend", elements)
        console.log(document.body.offsetHeight)
        setTimeout(() => { window.dispatchEvent(new Event('resize')); }, 200)        
        if (data.length > 0) {
            observer.observe(document.querySelector("#final_image"))
        }
    }
}

   

