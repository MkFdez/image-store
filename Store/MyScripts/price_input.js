$("#price").on("input", function () {
    let value = document.getElementById("price").value
    value = value.replace(/[^\d^\.]+/g, "");
    if (value.substr(value.length - 1, 1) == ".")
    {
        value+="00"
    }
    value = value ? parseFloat(value, 10) : 0;
    value = value.toFixed(2)
    document.getElementById("price").value = value + ""
    console.log("llegue")
})