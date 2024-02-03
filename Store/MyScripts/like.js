var clicks = 0

function addLike(e) {
    if (clicks == 7) {
        document.getElementById("star").disabled = true
    }
    if (clicks <= 6) {
        console.log(clicks)
        let value = !document.getElementById("star").checked
        let publicationid = document.getElementById("modelId").value.replace("/", "")
        $.ajax({
            url: "/Publication/Like",
            method: "POST",
            data: { publicationid: publicationid, like: value },
            success: function (data) {
                let prev = parseInt(document.getElementById("star-count").innerHTML)
                if (value) {
                    prev += 1
                } else {
                    prev -= 1
                }
                document.getElementById('star-count').innerHTML = prev

            }
        })
        
        
        clicks++
    }
}