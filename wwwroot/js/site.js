
function OpenFolder(path){
    let url = "/sharing/folder?folderPath=" + path.replace("\"", "-")
    location .href = url
}

$("#uploadButton").click(function(){
    $('#fileInput').trigger('click')
})

function Upload(){
    console.log("I changed")
    $('#sendFile').trigger('click')
}