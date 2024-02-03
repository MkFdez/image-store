function updateView(index) {
    
    let containers = ['daily-chart-container', 'monthly-chart-container', 'recent-sales-table-container', 'publications-table-container', 'collections-table-container', 'view-collection']
    console.log(document.getElementById(containers[index]).innerHTML.trim() == "")
   
    for (let container of containers) {
        $(`#${container}`).css("display", "none")
        $(`#${container}`).css("color", "white")
    }
    let items = document.querySelector("#menu").children

    for (let i = 0; i < items.length; i++) {
        items[i].className = i == index ? "nav-item sidebar-link-active" : "nav-item sidebar-link"
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
        case 4:
            if (document.getElementById(containers[index]).innerHTML.trim() == "") {
                loadCollectionsDatatable()

            }      
            break;
    }
}