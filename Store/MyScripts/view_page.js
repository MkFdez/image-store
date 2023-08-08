function downloadImage(path, id) {
    window.location = '/Publication/Download?path=' + path + "&scale=" + document.getElementById("rangeValue").innerHTML + "&pubid=" + id

    $.ajax({
        url: '@Url.Action("DeleteTempData", "Publication")',
        data: { "path": path, "scale": document.getElementById("rangeValue").innerHTML },
    })

}

function setValue(width, height) {
    document.getElementById("rangeValue").innerHTML = document.getElementById("myRange").value
    document.getElementById("width").value = parseInt(parseInt(width) * document.getElementById("myRange").value / 100)
    document.getElementById("height").value = parseInt(parseInt(height) * document.getElementById("myRange").value / 100)

}

function widthOnChange(width, height) {
    let d = width / height
    let h = parseInt(document.getElementById("width").value) / d
    document.getElementById("height").value = parseInt(h)
    document.getElementById("myRange").value = parseInt((h / height) * 100)
    document.getElementById("rangeValue").innerHTML = document.getElementById("myRange").value
}

function heightOnChange(width, height) {
    let d = width / height
    let w = parseInt(document.getElementById("height").value) * d
    document.getElementById("width").value = parseInt(w)
    document.getElementById("myRange").value = parseInt((w / width) * 100)
    document.getElementById("rangeValue").innerHTML = document.getElementById("myRange").value
}


function downloadImageTry(id) {
        window.location = '/Publication/DownloadFreeTry?id=' + id

        $.ajax({
            url: '@Url.Action("DeleteTempData", "Publication")',
            data: { "path": path, "scale": "" },
        })

    }
