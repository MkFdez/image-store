function loadTransactionDatatable() {
    document.getElementById('recent-sales-table-container').innerHTML = `<div class="row" >
            <div class="card mb-4">
                <div class="card-header text-black">
                    <i class="fas fa-table me-1"></i>
                    Sales
                </div>
                <div id="tblUpdatePcInfo">
         </div>
                </div>
                    </div >`
    var table;
    GetAllEmployyesData();

    function GetAllEmployyesData() {
        var tablecontent = '<table id="tblPCInfo" class="table table-bordered table-striped display nowrap" style="width:100%"><thead><tr>\
    <th>Publication</th>\
    <th>Date</th>\
    <th>Amount</th>\
    </tr></thead><tbody></tbody></table>';
        $("#tblUpdatePcInfo").html(tablecontent);
        table = $('#tblPCInfo').dataTable({
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
                url: "/Sales/GetTransactionsData",
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

                { "data": "publication", "name": "Publication", "searchable": false },
                { "data": "date", "name": "Date", "searchable": false },
                { "data": "amount", "name": "Amount", "searchable": false },

            ]
        });
    }
}