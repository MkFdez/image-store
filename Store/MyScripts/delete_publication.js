function deleteModal(data) {
   
    document.querySelector("#exampleModalLongTitle").innerHTML = `Do you want to delete <b>${data.title}</b>`
    document.querySelector('#modal-image').src = data.image
    const del = () => {
        onDeletePublication(data.id)
        document.querySelector("#delete-button").removeEventListener("click", del)
    }
    document.querySelector("#delete-button").addEventListener("click", del)

    $('#delete-modal').modal('show')
}

function onDeletePublication(publicationid) {
    $.ajax({
        url: '/Publication/Delete',
        method: "POST",
        data: {'pubId':publicationid},
        success: () => {       
            loadPublicationDatatable()
            console.log('pescao')
            $('#delete-modal').modal('hide')
        },
        error: () => { alert('all bad') },
    })
}