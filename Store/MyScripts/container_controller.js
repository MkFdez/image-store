function updateView(index) {
    
    let containers = ['daily-chart-container', 'monthly-chart-container', 'recent-sales-table-container', 'publications-table-container']
    console.log(document.getElementById(containers[index]).innerHTML.trim() == "")
   
    for (let container of containers) {
        $(`#${container}`).css("display", "none")
    }
    $(`#${containers[index]}`).css('display', 'inline')
    switch (index) {
        case 0:
            if (document.getElementById(containers[index]).innerHTML.trim() == "") {
                loadAreaChart()
            }
            break;
        case 1:
            if (document.getElementById(containers[index]).innerHTML.trim() == "") {
                loadBarChart()
            }
            break;
        case 2:
            if (document.getElementById(containers[index]).innerHTML.trim() == "") {
                loadTransactionDatatable()
            }
            break;
        case 3:
            if (document.getElementById(containers[index]).innerHTML.trim() == "") {
                loadPublicationDatatable()
            }
            break;
    }
}