function loadCollectionsDatatable() {
    console.log('here')
    document.getElementById('collections-table-container').innerHTML = `<div class="row">
                        <div class="card mb-4">
                            <div class="card-header text-black">
                                <i class="fas fa-table me-1"></i>
                                Collections
                                <div class='btn primary-btn' onclick="createModal()">New Collection</div>
                            </div>
                            <div id="tbCollections">

                            </div>
                        </div>
                    </div>`
    var table;
    GetCollectionData();

    function GetCollectionData() {
        var tablecontent = '<table id="tblCollectionsInfo" class="table table-bordered table-striped display nowrap" style="width:100%"><thead><tr>\
    <th>Name</th>\
    <th>Count</th>\
    <th>Actions</th>\
    </tr></thead><tbody></tbody></table>';
        $("#tbCollections").html(tablecontent);
        table = $('#tblCollectionsInfo').dataTable({
            clear: true,
            destroy: true,
            searching: false,
            serverSide: true,
            pageLength: 50,          
            autoFill: false,
            "initComplete": function (settings, json) {
                $(this.api().table().container()).find('input').attr('autocomplete', 'off');
            },
            "ajax": {
                url: "/Collection/Index",
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
                    console.log(JSON.stringify(data.data))
                    json.data = JSON.parse(data.data);
                    return json.data;
                }
            },

            "columns": [

                { "data": "name", "name": "Name", "searchable": false},
                { "data": "count", "name": "Count", "searchable": false },
                { "data": "actions", "name": "Actions", "searchable": false, "render": function (data, type, row, meta) { return `<center><p class='table-button' onclick='viewCollection(${data.collectionid}, "${data.name}")'>View</p></center>` } },

            ]
        });
    }
}