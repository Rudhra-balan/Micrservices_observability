function onHomePageLoad() {

    $("#balanceLink").click(function () {
        renderingNavPageMenu(NavPageEnum.Balance);
    });
    $("#transactionLink").click(function () {
        renderingNavPageMenu(NavPageEnum.Transaction);
    });
    $("#logLink").click(function () {
        renderingNavPageMenu(NavPageEnum.ConsoleLog);
    });
    $("#puttyLink").click(function () {
        renderingNavPageMenu(NavPageEnum.Putty);
    });

    renderingNavPageMenu(NavPageEnum.Balance);

    HubConnection();
}

function saveAccessToken(token){
    window.accessToken = token;
}

function renderingNavPageMenu(PageName) {
    switch (PageName) {
        case NavPageEnum.Balance:
            renderBalanceEnterPage();
            break;
        case NavPageEnum.Transaction:
            renderTransactionPage();
            break;
        case NavPageEnum.ConsoleLog:
            renderConsoleLogPage();
            break;
        case NavPageEnum.Putty:
            renderPuttyPage();
            break;
        default:
            renderBalanceEnterPage();
            break;
    }
}


function renderBalanceEnterPage() {
    $(".active").removeClass("active");
    $("#balanceLink").addClass("active");
    const url = "Balance/Balance";
    getCommon(url, "PartialArea");
}


function renderConsoleLogPage() {
    $(".active").removeClass("active");
    $("#logLink").addClass("active");
    const url = "Simulator/Index";
    getCommon(url, "PartialArea");
}


function renderTransactionPage() {
    $(".active").removeClass("active");
    $("#transactionLink").addClass("active");
    const url = "Transaction/Index";
    getCommon(url, "PartialArea");
}

function renderPuttyPage() {
    $(".active").removeClass("active");
    $("#puttyLink").addClass("active");
    const url = "Putty/Index";
    getCommon(url, "PartialArea");
}


function isBase64(encodedString) {
    if (encodedString === undefined || encodedString === null || encodedString === '' || encodedString.trim() === '') { return false; }
    return true;
}

function HubConnection() {

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("eHub/notification")
        .build();
    //This method receive the message and Append to our list

    connection.on("resilience",balanceNotification());


    try {
        connection.start().then(function () {
           
            
        }).catch(function (err) {
           
            return console.error(err.toString());
        });

    } catch (err) {
        console.log(err);
       

    }
}

function balanceNotification() {
  
    return (message) => {
        console.log(message);
        launch_toast('resilience',message);
    };
}

function launch_toast(titile, message) {
    $.unblockUI();
    $('#img').html(titile);
    $('#desc').html(message);
    var x = document.getElementById("toast");
    x.className = "show";
    setTimeout(function () { x.className = x.className.replace("show", ""); }, 5000);
}