function loadComments(actual) {
    $.ajax({
        url: "/Comment/MoreComments",
        data: { "actual": actual },
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
        let container = document.getElementById("commentContainer")
        let comment = document.createElement("div")
        comment.className = "comment"
        let username = document.createElement("div")
        username.style.display = "block"
        username.innerHTML = '<p>Commented By: ' + item.UserName + '</p>'
        comment.appendChild(username)
        let content = document.createElement("div")
        content.style.display = "block"
        content.innerHTML = '<p>' + item.Content + '</p>'
        comment.appendChild(content)
        container.appendChild(comment)
    }
}