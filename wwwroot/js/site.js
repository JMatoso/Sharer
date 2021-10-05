function LoadAnimation() {
    const loader = document.getElementById('loader')
    setTimeout(() => {
        loader.classList.add('fadeOut')
    }, 300)
}

function ShowToast(message, status = "success", dismissible = true){
    SnackBar({
        position: "bl",
        timeout: 5000,
        status: status,
        dismissible: dismissible,
        speed: 500,
        message: message,
        fixed: true,
        width: "auto"
    });
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
            ShowToast("Pasted!", "info")
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

/* SignalR
-------------------------------------------------- */

function StartHub(addr){
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(addr + "/appHub")
        .configureLogging(signalR.LogLevel.Information)
        .build()
        
    async function start(){
        try{
            await connection.start()
            ShowToast("Connected to Hub!", "success")
        }catch(err){
            console.log(err)
            setTimeout(start, 5000)
        }
    }

    connection.onclose(async () => { await start() })
    start()
}