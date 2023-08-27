var collections = null;

function loadPublicationDatatable() {
    document.getElementById('publications-table-container').innerHTML = `<div class="row">
                        <div class="card mb-4">
                            <div class="card-header text-black">
                                <i class="fas fa-table me-1"></i>
                                Publications
                            </div>
                            <div id="tbPublications">

                            </div>
                        </div>
                    </div>`
    var table;
    GetPublicationData();

    function GetPublicationData() {
        var tablecontent = '<table id="tblPublicationsInfo" class="table table-bordered table-striped display nowrap" style="width:100%"><thead><tr>\
    <th>Image</th>\
    <th>Publication</th>\
    <th>Date</th>\
    <th>Collection</th>\
    <th>Downloads</th>\
    <th>Actions</th>\
    </tr></thead><tbody></tbody></table>';
        $("#tbPublications").html(tablecontent);
        table = $('#tblPublicationsInfo').dataTable({
            clear: true,
            destroy: true,
            searching: false,
            serverSide: true,
            pageLength: 10,
            lengthMenu: [[10, 15, 20, 25], [10, 15, 20, 25]],
            autoFill: false,
            "initComplete": function (settings, json) {
                $(this.api().table().container()).find('input').attr('autocomplete', 'off');
            },
            "ajax": {
                url: "/Sales/Publications",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: function (d) {
                    var data = { data: d };
                    return JSON.stringify(data);
                },
                AutoWidth: false,
                "dataSrc": function (json) {
                    var data = json;
                    json.draw = data.draw;
                    json.recordsTotal = data.recordsTotal;
                    json.recordsFiltered = data.recordsFiltered;
                    collections = JSON.parse(json.collections)
                    console.log(collections)
                    json.data = JSON.parse(data.data);
                    return json.data;
                }
            },

            "columns": [

                { "data": "image", "name": "Image", "searchable": false, "render": function (data, type, row, meta) { return `<img style='height:30px' src='${data}' />` } },
                { "data": "publication", "name": "Publication", "searchable": false },
                { "data": "date", "name": "Date", "searchable": false, "render": function (data, type, row, meta) { return data.slice(0, 10) } },
                {
                    "data": "collection", "name": "Collection", "searchable": false, "render": function (data, type, row, meta) {
                        console.log(data)
                        let toReturn = `<div class="dropdown">
                                      <a class="btn btn-secondary dropdown-toggle" href="#" role="button"  id="${row.actions.id}-dropdownMenuLink" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                        ${data.collection}
                                      </a>

                                      <div class="dropdown-menu" id="${row.actions.id}-dropdown" aria-labelledby="${row.actions.id}-dropdownMenuLink">
                                        `
                        for (let x of collections) {
              
                            toReturn += `<a class="dropdown-item" onclick="changeCollection(${x.collectionid}, ${data.publicationid}, '${x.collection}', ${row.actions.id})" style="cursor:pointer">${x.collection}</a>`
                        }
                        toReturn += "</div> </div>"

                        return toReturn
                    }
                },
                { "data": "downloads", "name": "Downloads", "searchable": false },                     
                { "data": "actions", "name": "Actions", "searchable": false, "render": function (data, type, row, meta) { return `<center><p style="block" class='table-button' onclick='deleteModal(${JSON.stringify(data)})'>Delete</p></center>` } } ,

            ]
        });
    }
}

function changeCollection(collectionid, publicationid, collectionName, row) {
    $.ajax({
        url: "/Collection/Add",
        method: "POST",
        data: { 'publicationid': publicationid, 'collectionid': collectionid },
        success: function () {
            $(`#${row}-dropdownMenuLink`).text(collectionName)
            loadCollectionsDatatable()
        }
    })
}
function toggleDropdown(id) {
    $(`#${id}-dropdown`).dropdown('toggle')
}