var Settings = (function () {

    var settingid = null;
    var settingsobject = {};

    function onLoad() {
        var req = new XMLHttpRequest();
        req.open("GET", GetGlobalContext().getClientUrl() + "/api/data/v8.0/uwt_settingses?$select=uwt_settingsid,uwt_settingsstring", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\",odata.maxpagesize=1");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var results = JSON.parse(this.response);

                    if (results.value.length !== 0) {
                        settingid = results.value[0].uwt_settingsid;
                        var settingsstring = results.value[0]["uwt_settingsstring"];
                        settingsobject = JSON.parse(settingsstring);
                    }

                    if (typeof settingsobject.BingMapsKey !== "undefined")
                        $("#teBingMapsKey").val(settingsobject.BingMapsKey);

                    if (typeof settingsobject.CloudConvertKey !== "undefined")
                        $("#teCloudConvertKey").val(settingsobject.CloudConvertKey);

                    if (typeof settingsobject.CurrencylayerKey !== "undefined")
                        $("#teCurrencylayerKey").val(settingsobject.CurrencylayerKey);

                    if (typeof settingsobject.BaseUrl === "undefined") {
                        settingsobject.BaseUrl = GetGlobalContext().getClientUrl();
                    }

                    $("#teBaseUrl").val(settingsobject.BaseUrl);
                }
            }
        };
        req.send();
    }

    function onSave() {
        settingsobject = {};

        if ($("#teBingMapsKey").val() != null) {
            settingsobject.BingMapsKey = $("#teBingMapsKey").val();
        }

        if ($("#teCloudConvertKey").val() != null) {
            settingsobject.CloudConvertKey = $("#teCloudConvertKey").val();
        }

        if ($("#teCurrencylayerKey").val() != null) {
            settingsobject.CurrencylayerKey = $("#teCurrencylayerKey").val();
        }

        if ($("#teBaseUrl").val() != null) {
            settingsobject.BaseUrl = $("#teBaseUrl").val();
        }

        var settingsRecord = {
            uwt_settingsstring: JSON.stringify(settingsobject)
        };

        var req = new XMLHttpRequest();

        if (settingid == null)
            req.open("POST", GetGlobalContext().getClientUrl() + "/api/data/v8.0/uwt_settingses", true);
        else 
            req.open("PATCH", GetGlobalContext().getClientUrl() + "/api/data/v8.0/uwt_settingses(" + settingid + ")", true);

        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 204) {
                    if (settingid == null) {
                        var uri = this.getResponseHeader("OData-EntityId");
                        var regExp = /\(([^)]+)\)/;
                        var matches = regExp.exec(uri);
                        settingid = matches[1];
                    }
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send(JSON.stringify(settingsRecord));
    }

    return {
        OnLoad: onLoad,
        OnSave: onSave
    };
})();