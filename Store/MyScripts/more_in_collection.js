$(document).ready(function () {
    let collectionid = $("#collectionid").val()
    let publicationid = $("#publicationid").val()
    console.log(collectionid)
    console.log(publicationid)
    if (collectionid) {
        $.ajax({
            url: "/Collection/More",
            dataType: "json",
            method: "POST",
            data: { "collectionid": collectionid, "publicationid": publicationid },
            success: function (res) {
                fillImages(JSON.parse(res))
            }
        })
    }
})
function fillImages(data) {
    let count = 0;
    for (let item of data) {
        let element = $(`#${count}-image`)
        element.attr("src", item.Image)
        let container = $(`#${count}-container`)
        container.attr("href", `/publication/view/${item.PublicationId}`)
        count++
    }
}