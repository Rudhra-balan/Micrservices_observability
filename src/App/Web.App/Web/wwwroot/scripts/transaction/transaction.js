

function ontransactionPageLoad() {
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