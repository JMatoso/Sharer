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

function SetClipboardText(input){
    navigator.clipboard.readText()
        .then(text => {
            $(input).val(text)
        })
        .catch(err => {
            console.error(err)
        })
}

$(window).bind('scroll', function(){
    if($(window).scrollTop() > 50){
        $('.custom-nav').addClass("nav-saturated ")
    }else{
        $('.custom-nav').removeClass("nav-saturated ")
    }
})