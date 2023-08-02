function loadComments(actual) {
    let val = document.getElementById('count-comments').value
    console.log(val)
    console.log(document.getElementById('count-comments'))
    $.ajax({
        url: "/Comment/MoreComments",
        data: { "actual": val },
        method: "POST",
        dataType: "Json",
        success: function (result) {
           fill(result)
        },
        error: function (xhr, status, error) {
            alert(error);
        },
    })
}
function fill(elements) {
    let json = JSON.parse(elements)
    for (var item of json) {
        let commentElement = ` <div class="col-md-8" >
                <div class="media c-mb-30 media-comment">
                    <div class="media-body u-shadow-v18 g-bg-secondary g-pa-30">
                            <div class="g-mb-15">
                                    <div style="display:flex; flex-wrap: wrap; gap: 10px; align-items:center">
                                        <img class="d-flex g-width-50 g-height-50 rounded-circle g-mt-3 g-mr-15" src="${item.ProfilePicture == null ? "/ProfilePictures/default.png" : item.ProfilePicture}" alt="Image Description">
                                        <h5 class="h5 g-color-gray-dark-v1 mb-0">   ${item.UserName} </h5>
                                    </div>
                                <span class="g-color-gray-dark-v4 g-font-size-12">${item.DaysSinceCreated} days ago</span>
                            </div>

                        <p>
                            ${item.Content}
                        </p>
                    </div>
                </div>
            </div>`
        let container = document.getElementById("commentContainer")
        container.innerHTML += commentElement
    }
    let el = document.getElementById('count-comments')
    el.value = parseInt(el.value) + json.length
}