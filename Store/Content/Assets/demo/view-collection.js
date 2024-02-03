function viewCollection(collectionid, collectionName) {
    console.log("here")
    let html = `<div class='row'>
                <h3 class="text-black">${collectionName}</h3>
                  <hr class="text-black"/>
                </div>
                <div id="pub-container" class='row'>
                </div>`
    $("#view-collection").html(html)
    $.ajax({
        url: '/Collection/Publications',
        dataType: 'json',
        method: "POST",
        data: { "collectionid": collectionid },
        success: function (res) {
            generateView(res)
            updateView(5)
        },

        
    })
   
    function generateView(res) {

        let parsedData = JSON.parse(res)
        let toad = document.createElement('div')
        toad.className = "row"
        let count = 0
        for (let item of parsedData) {            
            if (count % 3 == 0) {
                $("#pub-container").append(toad)
                toad = document.createElement('div')
                toad.className = "row"
                toad.innerHTML = ""
            }
            let element = `<div class='col-md-4'>
                            <div style="width:100%;position: relative;display: inline-block;" >
                            <button id='${item.PublicationId}-delete' type="button" class="delete-button  btn btn-danger" style="display:none" onmouseover="showButton('${item.PublicationId}-delete')" onmouseout="hideButton('${item.PublicationId}-delete')" onclick="removePublication(${item.PublicationId}, ${collectionid}, '${collectionName}')" on>Remove</button>
                            <img  src='${item.Image}' style="width:100%;z-index:-1"onmouseover="showButton('${item.PublicationId}-delete')" onclick="showButton('${item.PublicationId}-delete')" onmouseout="hideButton('${item.PublicationId}-delete')"/>
                            
                            </div>
                    </div>`
            toad.innerHTML += element
            count++
        }
        console.log(toad.innerHTML)
        $("#pub-container").append(toad)
    }

   
}
function showButton(id) {
    $(`#${id}`).css("display", "inline-block")
}
function hideButton(id) {
    console.log($(`#${id}`).find(":hover").length)
    if ($(`#${id}`).find(":hover").length < 1) {
        $(`#${id}`).css("display", "none")
    }
}
function removePublication(pid, cid, name) {
    console.log("remove")
    $.ajax({
        url: '/Collection/DropPublication',
        method: "POST",
        data: { "publicationid": pid },
        success: function (res) {
            console.log(cid)
            console.log(name)
            viewCollection(cid, name)
            loadCollectionsDatatable()
        },
        error: function (error) {
            alert(JSON.stringify(error))
        }
    })
}