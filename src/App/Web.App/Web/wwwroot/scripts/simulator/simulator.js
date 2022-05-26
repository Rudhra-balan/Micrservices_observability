


var promises = [];
window.simulate = false;




function getMethod(number) {
    if (number === 0) {
        return simulateDeposit();
    }
    if (number === 1) {
        return simulateWithdraw();
    }
    if (number === 2) {
        return simulateBalance();
    }
}

function startSimulate() {
    window.timerID = setInterval(function () {
        var randomRequest = Math.floor(Math.random() * 3);
        getMethod(randomRequest);
    }, 10 * 100);

};


function stopSimulate() {
    clearInterval(window.timerID);
    
};


function simulateDeposit() {
    var transactionModel = {
        transactionType: TransactionType.Deposit,
        amount: randomNumberFromRange(100, 999999)
    };

    return $.ajax({
        url: "Transaction/Deposit",
        type: "POST",
        data: JSON.stringify(transactionModel),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (data) {
            console.log(`[${new Date().toISOString().substr(0, 19).replace('T', ' ')}] [Deposit] : Amount deposited into bank account number ${data.sourceObject.accountNumber}. <br> Your account balance ${data.sourceObject.balance.amount} ${data.sourceObject.balance.currency}`);
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            console.log(err.Message);
        }
    });
  
}

function simulateWithdraw() {
    var transactionModel = {
        transactionType: TransactionType.Withdraw,
        amount: randomNumberFromRange(100, 999999)
    };
    return $.ajax({
        url: "Transaction/Withdraw",
        type: "POST",
        data: JSON.stringify(transactionModel),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (data) {
            console.log(`[${new Date().toISOString().substr(0, 19).replace('T', ' ')}] [Withdraw] : Amount deposited into bank account number ${data.sourceObject.accountNumber}. <br> Your account balance ${data.sourceObject.balance.amount} ${data.sourceObject.balance.currency}`);
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            console.log(err.Message);
        }
    });
    
}

function simulateBalance() {
    return  $.ajax({
        url: "Balance/BalanceSimulation",
        type: "GET",
        async: true,
        success: function (data) {
            console.log(`[${new Date().toISOString().substr(0, 19).replace('T', ' ')}] [Balance] :  ${JSON.stringify(data)}`);
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
            console.log(err.Message);
        }
    });
}

var TransactionType = {
    Unknown: 0,
    Deposit: 1,
    Withdraw: 2
};

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

function onClickTransaction() {
   
    if (TransactionType.Deposit === parseInt($("#transactionType").val())) {

        var transactionModel = {
            transactionType: TransactionType.Deposit,
            amount: $("#amount").val()
        };
       
        postCommon("Transaction/Deposit", transactionModel, null, onDepositCallback, onDepositFailure);
    }
    else if (TransactionType.Withdraw === parseInt($("#transactionType").val())) {
        var transactionModel = {
            transactionType: TransactionType.Withdraw,
            amount: $("#amount").val()
        };
        postCommon("Transaction/Withdraw", transactionModel, null, onWithdrawCallback, onWithdrawFailure);
    }
    else {
        showError("Invalid Transaction.Please try again");
    }
}

function onDepositCallback(data) {

    if (data !== undefined && data.sourceObject !== undefined) {
        showInformation(`Amount deposited into bank account number ${data.sourceObject.accountNumber}. <br> Your account balance ${data.sourceObject.balance.amount} ${data.sourceObject.balance.currency}`);

    }
    $("#amount").val("");
    $("#transactionType").val(0);
}

function onDepositFailure(data) {
    showError("Deposit Failed", "Error");
    $("#amount").val("");
    $("#transactionType").val(0);
}

function onWithdrawCallback(data) {
    if (data !== undefined && data.sourceObject !== undefined) {
        showInformation(`Amount credited into bank account number ${data.sourceObject.accountNumber}. <br> Your account balance ${data.sourceObject.balance.amount} ${data.sourceObject.balance.currency}`);
    }
    $("#amount").val("");
    $("#transactionType").val(0);
}

function onWithdrawFailure(data) {
    showError("Withdraw Failed", "Error");
    $("#amount").val("");
    $("#transactionType").val(0);
}