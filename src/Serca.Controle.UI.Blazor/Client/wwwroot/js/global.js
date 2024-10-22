window.Cookies = {
    CreateCookie: function (name, value, minutes, domain) {

        var expires;
        if (minutes) {

            if (minutes < 0) {
                expires = "; expires = Thu, 01 Jan 1970 00: 00: 00 UTC";
            }
            else {
                var date = new Date();
                date.setTime(date.getTime() + (minutes * 60 * 1000));
                expires = "; expires=" + date.toGMTString();
            }
        }
        else {
            expires = "";
        }

        document.cookie = name + "=" + value + expires + "; path=/; domain=" + domain;
    },
    GetCookies: function () {
        return document.cookie;
    }
}

window.UpdatePageTitle = (value) => {
    var el = document.getElementById("page-title");
    if (el != null) {
        el.innerHTML = value;
    }
}

var sequenceListenerInitialized = false;
var dotnetScanListener = null;

window.initSequenceListener = (objref) => {
    if (sequenceListenerInitialized)
        return;

    dotnetScanListener = objref;

    new SequenceListener({
        debug: true,
        maxKeyboardDelay: 300,
        allowedChars: ".*",
        ignoreInputs: false,
        minLength: 1,
        preventEnter: true,
        overrideTargetButtons: true
    });

    document.body.addEventListener('keyboardSequence', function (e) {

        if (dotnetScanListener == null) {
            console.log("Dotnet reference should be assign.");
            return;
        }

        dotnetScanListener.invokeMethodAsync("OnScan", e.detail.sequence);

    })

    sequenceListenerInitialized = true;
}

window.PlayAudio = (elementName) => {
    document.getElementById(elementName).play();
}

window.scrollIntoView = (id) => {
    if (id !== null) {
        var elem = document.getElementById(id);
        elem.scrollIntoView({ block: "center" });
    }
    else {
        console.log("scrollIntoView - cannot scroll on null");
    }
}