function LoadAnimation() {
    const loader = document.getElementById('loader')
    setTimeout(() => {
        loader.classList.add('fadeOut')
    }, 300)
}

function OpenFolder(path){
    let url = "/sharing/folder?folderPath=" + path.replace("\"", "-")
    location .href = url
}

$("#uploadButton").click(function(){
    $('#fileInput').trigger('click')
})

function Upload(){
    $("#uploadingModal").removeClass("d-none")
    LoadAnimation()
    $('#sendFile').trigger('click')
}
