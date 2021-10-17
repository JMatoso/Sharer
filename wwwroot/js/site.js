"use strict";

function LoadAnimation() {
    const loader = document.getElementById('loader')
    setTimeout(() => {
        loader.classList.add('fadeOut')
    }, 300)
}

function ShowToast(message, status = "#FFFFFF", dismissible = true, duration = 5000){
    Snackbar.show({ 
        actionTextColor: status, 
        pos: 'bottom-left',
        text: message,
        actionText: 'Dismiss',
        width: 'auto',
        duration: duration,
        textColor: status,
        showAction: dismissible,
        backgroundColor: '#323232'
    });
}

function OpenFolder(path){
    let url = "/sharing/folder?folderPath=" + path
    location .href = url
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

$(window).bind('scroll', () => {
    if($(window).scrollTop() > 20)
        $('.custom-nav').addClass("nav-saturated ")
    else
        $('.custom-nav').removeClass("nav-saturated ")
    
})

/* SignalR
-------------------------------------------------- */
var connection = null

function StartHub(addr){
    connection = new signalR.HubConnectionBuilder()
        .withUrl(addr + "/appHub")
        .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
        .configureLogging(signalR.LogLevel.Warning)
        .build()
        
    async function start(){
        try{
            await connection.start()
        }catch(err){
            console.log(err)
            setTimeout(start, 5000)
        }
    }

    connection.onclose(async () => { await start() })
    start()

    connection.on('BadRequest', (message) => { ShowToast(message, "danger") });
    connection.on('Successful', (message, addData) => {
        ShowToast(message, "success")
    });
}

$("#saveFolder").click(() => {
    let folderName = $("#folderName").val()
    let folderPath = $("#folderPath").val()

    connection.invoke('SaveFolderAsync', folderPath, folderName).catch(err => {
        console.error(err.toString())
        ShowToast("Something went wrong, try again!", "danger")
    })

    $("#closeModal").trigger('click')
})

$("#uploadButton").click(() => { $('#fileInput').trigger('click') })
function Upload(){
    $('#sendFile').trigger('click')

    var data = new FormData()

    data.append("data", $("#fileInput")[0].files[0])
    connection.invoke('UploadAsync', data).catch(err => {
        console.error(err.toString())
        ShowToast("Something went wrong, try again!", "danger")
    })
}

