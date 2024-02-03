function createModal() {

    const create = () => {
        onCreateCollection()
        document.querySelector("#create-button").removeEventListener("click", create)
    }
    document.querySelector("#create-button").addEventListener("click", create)

    $('#c-collection-modal').modal('show')
}

function onCreateCollection() {
    $.ajax({
        url: '/Collection/Create',
        method: "POST",
        data: { 'name': $("#collection-input").val() },
        success: () => {
            loadCollectionsDatatable()
            $('#c-collection-modal').modal('hide')
        },
        error: () => { alert('all bad') },
    })
}