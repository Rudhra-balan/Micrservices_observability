function onSumbit() {

    var command ="";
    var commandText = $("#commandType option:selected").attr("command");
    if (isStringNullOrEmpty(commandText) === true)
        command = $('#commandTextBox').val();
    else
        command = commandText;

    var diagnosticsModel = {};
    diagnosticsModel.Command = command;
    var url = "/Putty/Command/";
    postCommon(url, diagnosticsModel, null, onSuccessDiagnosticCommand, onFailDiagnosticCommand, null, null, null);
}

function onSuccessDiagnosticCommand(data) {
    var currentReponse = $('#CommandResponseTextbox').children().first();
    currentReponse.removeClass("diagnostics-response-current");
    currentReponse.addClass("diagnostics-response")
    
    $("#CommandResponseTextbox").prepend($('<div class="diagnostics-response-current"><pre style="font-size:100%"> ' + data.sourceObject.stdout + '</pre> </div> <br/>'));

    if (isStringNullOrEmpty(data.sourceObject.stderr) === false)
        $("#CommandResponseTextbox").prepend($('<div class="diagnostics-response-current"><pre style="font-size:100%"> ' + data.sourceObject.stderr + '</pre> </div> <br/>'));
}

function onFailDiagnosticCommand(data) {

}


function isStringNullOrEmpty (val) {
    switch (val) {
        case "":
        case 0:
        case "0":
        case null:
        case false:
        case undefined:
        case typeof this === 'undefined':
            return true;
        default: return false;
    }
}

function commandType(select) {
    if (select.value == 5) {
        $("#commandTextBox").prop('disabled', false);
    }
    else {
        $("#commandTextBox").prop('disabled', true);
    }
}