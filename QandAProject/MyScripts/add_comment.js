

$("#commentForm").submit(function (e) {
    e.preventDefault()
    let content = document.getElementById("CommentTextArea").value
    console.log(content)
    console.log(document.getElementById("modelId").value)
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
        let container = document.getElementById("commentContainer")
        let comment = document.createElement("div")
        comment.className = "comment"
        let username = document.createElement("div")
        username.style.display = "block"
        username.innerHTML = '<p>Commented By: ' + result.UserName + '</p>'
        comment.appendChild(username)
        let content = document.createElement("div")
        content.style.display = "block"
        content.innerHTML = '<p>' + result.Content + '</p>'
        comment.appendChild(content)
        container.appendChild(comment)
    }
})