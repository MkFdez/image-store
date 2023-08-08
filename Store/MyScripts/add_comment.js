

$("#commentForm").submit(function (e) {
    e.preventDefault()
    let content = document.getElementById("CommentTextArea").value
  
    $.ajax({
        type: "POST",
        url: "/Comment/Create",
        dataType: "json",
        data: { "content": content, "publicationId": document.getElementById("modelId").value.replace("/", "") },
        success: function (result) { complete(result) },
        error: function (xhr, status, error) {
            alert(error);
        }
    })
    function complete(result) {
        result = JSON.parse(result)
        let comment = `<div class="col-md-8" >
                <div class="media c-mb-30 media-comment">
                    <div class="media-body u-shadow-v18 g-bg-secondary g-pa-30">
                            <div class="g-mb-15">
                                    <div style="display:flex; flex-wrap: wrap; gap: 10px; align-items:center">
                                        <img class="d-flex g-width-50 g-height-50 rounded-circle g-mt-3 g-mr-15" src="${document.cookie.replace('vals=picture=', "").split("&")[0]}" alt="Image Description">
                                        <h5 class="h5 g-color-gray-dark-v1 mb-0">   ${result.UserName} </h5>
                                    </div>
                                <span class="g-color-gray-dark-v4 g-font-size-12">${result.DaysSinceCreated} days ago</span>
                            </div>

                        <p>
                            ${result.Content}
                        </p>
                    </div>
                </div>
            </div>`
        let container = document.getElementById("commentContainer")
        container.innerHTML = container.innerHTML + comment
        document.getElementById("CommentTextArea").value = ""
       
    }
})