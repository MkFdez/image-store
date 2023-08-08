function loadPublicationDatatable() {
    document.getElementById('publications-table-container').innerHTML = `<div class="row">
                        <div class="card mb-4">
                            <div class="card-header">
                                <i class="fas fa-table me-1"></i>
                                DataTable Example
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
                    json.data = JSON.parse(data.data);
                    return json.data;
                }
            },

            "columns": [

                { "data": "image", "name": "Image", "searchable": false, "render": function (data, type, row, meta) { return `<img style='height:30px' src='${data}' />` } },
                { "data": "publication", "name": "Publication", "searchable": false },
                { "data": "date", "name": "Date", "searchable": false },
                { "data": "downloads", "name": "Downloads", "searchable": false },
                { "data": "actions", "name": "Actions", "searchable": false },

            ]
        });
    }
}