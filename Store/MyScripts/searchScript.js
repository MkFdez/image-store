function searchInput() {
    alert("esta");
    $.ajax({
        url: "/Publication/Index",
        data: { "search": document.getElementById("search").value },
        method: "GET",
        t
    })

}