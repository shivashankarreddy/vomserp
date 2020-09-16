var EmptyGUID = '00000000-0000-0000-0000-000000000000';
function CheckOnline() {
    try {
        if (navigator.onLine)
            return true;
        else
            alert('not connected');
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}
window.history.forward();
function noBack() { window.history.forward(); }

var xmlHttp;
function getXMLHttpRequest() {
    try {
        if (window.ActiveXObject) {
            try {
                return new ActiveXObject("Msxml2.XMLHTTP");
            } catch (e) {
                try {
                    return new ActiveXObject("Microsoft.XMLHTTP");
                } catch (e1) {
                    return null;
                }
            }
        } else if (window.XMLHttpRequest) {
            return new XMLHttpRequest();
        } else {
            return null;
        }
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}
function BindPriceBasis(ddlpbval, ddlInco) {
    try {
        var ddlpbval = document.getElementById(ddlpbval).value;
        var ddlIncoval = $('[id$=' + ddlInco + ']')[0];
        $('[id$=' + ddlInco + '] Option').each(function () { $(this).remove(); });
        $.ajax(
                {
                    type: "POST",
                    url: "../Masters/AutoComplete.asmx/GetPriceBasis",
                    data: "{ 'Parent': '" + ddlpbval + "'}",
                    contentType: "application/json",
                    dataType: "json",
                    success: function (data) {
                        for (var i = 0; i < data.d.length; i++) {
                            AddItems(ddlIncoval, data.d[i].split('-')[0], data.d[i].split('-')[1]);
                        }
                    }
                    , error: function (response) {
                        alert('error :' + response.responseText);
                    },
                    failure: function (response) {
                        alert('Failure :' + response.responseText);
                    }
                });
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}
function AddItems(ddlName, ValueField, textField) {
    try {
        var opt = document.createElement("option");
        opt.text = textField;
        opt.value = ValueField;
        ddlName.options.add(opt);
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}
var DstnddlVar;
function BindDropDownList(Dstnddl, pageName) {
    try {
        xmlHttp = null; DstnddlVar = Dstnddl;

        xmlHttp = getXMLHttpRequest();

        if (xmlHttp != null) {

            xmlHttp.onreadystatechange = state_ChangeBind;
            xmlHttp.open("GET", pageName + "?CtgryID=0", true);
            xmlHttp.send(null);
        }
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}
function state_ChangeBind() {
    try {
        if (xmlHttp.readyState == 4) {
            if (xmlHttp.status == 200) {
                var countries = xmlHttp.responseText.split(';');
                var length = countries.length;
                document.getElementById("ctl00_ContentPlaceHolder1_" + DstnddlVar).options.length = 0;
                var dropDown = document.getElementById("ctl00_ContentPlaceHolder1_" + DstnddlVar);
                for (var i = 0; i < length - 1; ++i) {
                    var optn = countries[i].split(',');
                    var option = document.createElement("option");
                    option.text = optn[0];
                    option.value = optn[1];
                    dropDown.options.add(option);
                }
            }
        }
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}
function UpdateDropDownList(Scrddl, pageName, Dstnddl) {
    try {
        xmlHttp = null; DstnddlVar = Dstnddl;

        xmlHttp = getXMLHttpRequest();

        if (xmlHttp != null) {

            var contName = document.getElementById("ctl00_ContentPlaceHolder1_" + Scrddl).value;

            if (contName == 0) {
                document.getElementById("ctl00_ContentPlaceHolder1_" + DstnddlVar).disabled = true;
                return false;
            }
            else {
                document.getElementById("ctl00_ContentPlaceHolder1_" + DstnddlVar).value = 0;
                document.getElementById("ctl00_ContentPlaceHolder1_" + DstnddlVar).disabled = false;
            }

            xmlHttp.onreadystatechange = state_Change;

            xmlHttp.open("GET", pageName + "?ID=" + contName, true);
            xmlHttp.send(null);
        }
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}

//Handle the response of this async request
function state_Change() {
    try {
        if (xmlHttp.readyState == 4) {
            // 4 = “loaded” 
            if (xmlHttp.status == 200) {
                //request was successful. so Retrieve the values in the response.
                var countries = xmlHttp.responseText.split(';');
                var length = countries.length;

                //Change the second dropdownlists items as per the new values foudn in response.
                //let us remove existing items
                document.getElementById("ctl00_ContentPlaceHolder1_" + DstnddlVar).options.length = 0;

                //Now add the new items to the dropdown.
                var dropDown = document.getElementById("ctl00_ContentPlaceHolder1_" + DstnddlVar);
                dropDown.disabled = false;
                for (var i = 0; i < length - 1; ++i) {
                    var optn = countries[i].split(',');
                    var option = document.createElement("option");
                    option.text = optn[0];  //countries[i];
                    option.value = optn[1];   //countries[i];

                    dropDown.options.add(option);
                }
                dropDown.focus();
            }
        }
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}
function RemoveDropDownList(Scrddl, pageName, Dstnddl) {
    try {
        xmlHttp = null; DstnddlVar = Dstnddl;
        xmlHttp = getXMLHttpRequest();
        if (xmlHttp != null) {
            var contName = document.getElementById(Scrddl).value;
            xmlHttp.onreadystatechange = state_ChangeRemove;
            xmlHttp.open("GET", pageName + "?ID=" + contName, true);
            xmlHttp.send(null);
        }
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}
function state_ChangeRemove() {
    try {
        if (xmlHttp.readyState == 4) {
            if (xmlHttp.status == 200) {
                var MainText = xmlHttp.responseText.split('<')[0];
                var countries = MainText.split(';');
                var length = countries.length;

                document.getElementById(DstnddlVar).options.length = 0;
                var dropDown = document.getElementById(DstnddlVar);

                for (var i = 0; i < length - 1; ++i) {
                    var optn = countries[i].split(',');
                    var option = document.createElement("option");
                    option.text = optn[0];  //countries[i];
                    option.value = optn[1];   //countries[i];
                    dropDown.options.add(option);
                }
            }
        }
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}


//var message="Sorry, Right Click is disabled.";
////this will register click funtion for all the mousedown operations on the page
//document.onmousedown=click;
//function click(e) {
//    try {
//        if (navigator.appName == 'Microsoft Internet Explorer') {
//            //for Internet Explore..’2′ is for right click of mouse
//            if (event.button == 2) {
//                alert(message);
//                return false;
//            }
//        }
//        else {
//            //for other browsers like Netscape 4 etc..
//            if (e.which == 3) {
//                alert(message);
//                return false;
//            }
//        }
//    }
//    catch (Error) {
//        ErrorMessage(Error.message);
//    }
//}

//function DisableCopyPaste() {
//    try {
//        alert('This functionality has been disabled !');
//        window.clipboardData.clearData("Text"); //for cleaning up the clipboard
//        // Cancel default behavior
//        event.returnValue = false;
//    }
//    catch (Error) {
//        ErrorMessage(Error.message);
//    }
//}


function GetClientID(id, context) {
    try {
        var el = $("#" + id, context);
        if (el.length < 1)
            el = $("[id$=_" + id + "]", context);
        return el;
    }
    catch (Error) {
        ErrorMessage(Error.message);
        return Error.message;
    }
}

function validations() {

}
function ExitAll() {
    try {
        window.location = "https://www.google.co.in/";
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}
function clearAll() {
    try {
        var inputs = document.getElementsByTagName('input');
        for (var k = 0; k < inputs.length; k++) {
            var input = inputs[k]
            if (input.type != 'text') continue;
            input.value = "";
        }
        var slcts = document.getElementsByTagName('select');
        for (var k = 0; k < slcts.length; k++) {
            slcts[k].value = "0";
        }
        var txtarea = document.getElementsByTagName('textarea');
        for (var k = 0; k < txtarea.length; k++) {
            txtarea[k].innerHTML = "";
        }
        var inputs = document.getElementsByTagName('input');
        for (var k = 0; k < inputs.length; k++) {
            var input = inputs[k]
            if (input.type != 'checkbox') continue;
            input.checked = false;
        }
        var btn = document.getElementById('ctl00_ContentPlaceHolder1_btnSave');
        btn.innerText = "Save";
        //$('[id$=btnSave]').html('Update');
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}

function SelectAll(CheckBoxControl) {
    try {
        var inputs = document.getElementsByTagName('input');
        for (var k = 0; k < inputs.length; k++) {
            var input = inputs[k]
            if (input.type != 'checkbox') continue;
            if (CheckBoxControl.checked == true) {
                input.checked = true;
            }
            else {
                input.checked = false;
            }
        }
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}

function CHeck(ckid, dvid) {
    try {
        //alert(ckid + ' ' + dvid);
        var ChkBox = document.getElementById("ctl00_ContentPlaceHolder1_" + ckid);
        if (ChkBox.checked == true) {
            //var divid = document.getElementById("ctl00_ContentPlaceHolder1_" + dvid);
            $('div[id*=' + dvid + ']').css("display", "block"); ;
            //document.getElementById(divid).style.display = 'block';
            $('div[id*=' + dvid + '] input[type=text]').focus();
        }
        else {
            //document.getElementById(dvid).style.display = 'none';
            $('div[id*=' + dvid + ']').css("display", "none"); ;
        }
    }
    catch (Error) {
        //ErrorMessage(Error.message);
    }
}

function RbtnShow(rbtn, Shdvid, Hedvid, ShGvDvID, HeGvDvID) {
    try {
        var Rbtn = document.getElementById("ctl00_ContentPlaceHolder1_" + rbtn);
        if (Rbtn.checked == true) {
            document.getElementById(Shdvid).style.display = 'block';
            document.getElementById(Hedvid).style.display = 'none';
            document.getElementById(ShGvDvID).style.display = 'block';
            document.getElementById(HeGvDvID).style.display = 'none';
        }
        else {
            document.getElementById(Shdvid).style.display = 'none';
            document.getElementById(Hedvid).style.display = 'block';
            document.getElementById(ShGvDvID).style.display = 'none';
            document.getElementById(HeGvDvID).style.display = 'block';
        }
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}

function calenderpopup(cntid) {
    try {
        new JsDatePick({ useMode: 2, target: cntid, dateFormat: "%d-%M-%Y" });
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}

function oneweek(cntrlid) {
    try {
        var currentTime = new Date()
        currentTime.setDate(currentTime.getDate() + 5);
        var month = currentTime.getMonth() + 1
        var day = currentTime.getDate()
        if (day <= 9)
            day = "0" + day;
        var year = currentTime.getFullYear()
        if (month <= 9)
            month = "0" + month;
        cntrlid.valueOf(day + "/" + month + "/" + year);
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}

function getval(hdfdid, ddlval) {
    try {
        document.getElementById("ctl00_ContentPlaceHolder1_" + hdfdid).value =
        document.getElementById("ctl00_ContentPlaceHolder1_" + ddlval).value;
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}


function FreezGridViewHeader(GridViewID) {
    try {
        var GridId = "";
        GridId = GetClientID(GridViewID).attr("id");
        //alert(GridId);
        //GetClientID(GridViewID);  //"<%=GridViewID.ClientID %>";
        var ScrollHeight = 200;
        var grid = document.getElementById(GridId);
        var gridWidth = grid.offsetWidth;
        var gridHeight = grid.offsetHeight;
        var headerCellWidths = new Array();
        for (var i = 0; i < grid.getElementsByTagName("TH").length; i++) {
            headerCellWidths[i] = grid.getElementsByTagName("TH")[i].offsetWidth;
        }
        grid.parentNode.appendChild(document.createElement("div"));
        var parentDiv = grid.parentNode;
        parentDiv.id = "parentDiv";
        var table = document.createElement("table");
        for (i = 0; i < grid.attributes.length; i++) {
            if (grid.attributes[i].specified && grid.attributes[i].name != "id") {
                table.setAttribute(grid.attributes[i].name, grid.attributes[i].value);
            }
        }
        table.style.cssText = grid.style.cssText;
        //   table.style.width = "500px";
        table.style.width = gridWidth + "px";
        table.appendChild(document.createElement("tbody"));
        table.getElementsByTagName("tbody")[0].appendChild(grid.getElementsByTagName("TR")[0]);
        var cells = table.getElementsByTagName("TH");

        var gridRow = grid.getElementsByTagName("TR")[0];
        for (var i = 0; i < cells.length; i++) {
            var width;
            if (headerCellWidths[i] > gridRow.getElementsByTagName("TD")[i].offsetWidth) {
                width = headerCellWidths[i];
            }
            else {
                width = gridRow.getElementsByTagName("TD")[i].offsetWidth;
            }
            cells[i].style.width = parseInt(width - 3) + "px";
            gridRow.getElementsByTagName("TD")[i].style.width = parseInt(width - 3) + "px";
        }
        parentDiv.removeChild(grid);

        var dummyHeader = document.createElement("div");
        dummyHeader.appendChild(table);
        parentDiv.appendChild(dummyHeader);
        var scrollableDiv = document.createElement("div");
        scrollableDiv.id = "divgridview";
        if (parseInt(gridHeight) > ScrollHeight) {
            gridWidth = parseInt(gridWidth) + 17;
        }
        scrollableDiv.style.cssText = "overflow:auto;height:" + ScrollHeight + "px;width:" + gridWidth + "px";
        //scrollableDiv.style.cssText = "overflow:auto;height:" + ScrollHeight + "px;width: 1000px";
        scrollableDiv.appendChild(grid);
        parentDiv.appendChild(scrollableDiv);
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}


//g_l = []; g_l.MONTHS = ["Janaury", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]; g_l.DAYS_3 = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"]; g_l.MONTH_FWD = "Move a month forward"; g_l.MONTH_BCK = "Move a month backward"; g_l.YEAR_FWD = "Move a year forward"; g_l.YEAR_BCK = "Move a year backward"; g_l.CLOSE = "Close the calendar"; g_l.ERROR_2 = g_l.ERROR_1 = "Date object invalid!"; g_l.ERROR_4 = g_l.ERROR_3 = "Target invalid"; g_jsDatePickImagePath = "img/"; g_jsDatePickDirectionality = "ltr"; g_arrayOfUsedJsDatePickCalsGlobalNumbers = []; g_arrayOfUsedJsDatePickCals = []; g_currentDateObject = {}; g_currentDateObject.dateObject = new Date(); g_currentDateObject.day = g_currentDateObject.dateObject.getDate(); g_currentDateObject.month = g_currentDateObject.dateObject.getMonth() + 1; g_currentDateObject.year = g_currentDateObject.dateObject.getFullYear(); JsgetElem = function(a) { return document.getElementById(a) }; String.prototype.trim = function() { return this.replace(/^\s+|\s+$/g, "") }; String.prototype.ltrim = function() { return this.replace(/^\s+/, "") }; String.prototype.rtrim = function() { return this.replace(/\s+$/, "") }; String.prototype.strpad = function() { return (!isNaN(this) && this.toString().length == 1) ? "0" + this : this }; JsDatePick = function(a) { if (document.all) { this.isie = true; this.iever = JsDatePick.getInternetExplorerVersion() } else { this.isie = false } this.oConfiguration = {}; this.oCurrentDay = g_currentDateObject; this.monthsTextualRepresentation = g_l.MONTHS; this.lastPostedDay = null; this.initialZIndex = 2; this.globalNumber = this.getUnUsedGlobalNumber(); g_arrayOfUsedJsDatePickCals[this.globalNumber] = this; this.setConfiguration(a); this.makeCalendar() }; JsDatePick.getCalInstanceById = function(a) { return g_arrayOfUsedJsDatePickCals[parseInt(a, 10)] }; JsDatePick.getInternetExplorerVersion = function() { var c = -1, a, b; if (navigator.appName == "Microsoft Internet Explorer") { a = navigator.userAgent; b = new RegExp("MSIE ([0-9]{1,}[.0-9]{0,})"); if (b.exec(a) != null) { c = parseFloat(RegExp.$1) } return c } }; JsDatePick.prototype.setC = function(a, b) { if (this.isie && this.iever > 7) { a.setAttribute("class", b) } else { a.className = b } }; JsDatePick.prototype.getUnUsedGlobalNumber = function() { var a = Math.floor(Math.random() * 1000); while (!this.isUnique_GlobalNumber(a)) { a = Math.floor(Math.random() * 1000) } return a }; JsDatePick.prototype.isUnique_GlobalNumber = function(b) { var a; for (a = 0; a < g_arrayOfUsedJsDatePickCalsGlobalNumbers.length; a++) { if (g_arrayOfUsedJsDatePickCalsGlobalNumbers[a] == b) { return false } } return true }; JsDatePick.prototype.addOnSelectedDelegate = function(a) { if (typeof (a) == "function") { this.addonSelectedDelegate = a } return false }; JsDatePick.prototype.setOnSelectedDelegate = function(a) { if (typeof (a) == "function") { this.onSelectedDelegate = a; return true } return false }; JsDatePick.prototype.executeOnSelectedDelegateIfExists = function() { if (typeof (this.onSelectedDelegate) == "function") { this.onSelectedDelegate() } if (typeof (this.addonSelectedDelegate) == "function") { this.addonSelectedDelegate() } }; JsDatePick.prototype.setRepopulationDelegate = function(a) { if (typeof (a) == "function") { this.repopulationDelegate = a; return true } return false }; JsDatePick.prototype.setConfiguration = function(a) { this.oConfiguration.isStripped = (a.isStripped != null) ? a.isStripped : false; this.oConfiguration.useMode = (a.useMode != null) ? a.useMode : 1; this.oConfiguration.selectedDate = (a.selectedDate != null) ? a.selectedDate : null; this.oConfiguration.target = (a.target != null) ? a.target : null; this.oConfiguration.yearsRange = (a.yearsRange != null) ? a.yearsRange : [1971, 2100]; this.oConfiguration.limitToToday = (a.limitToToday != null) ? a.limitToToday : false; this.oConfiguration.field = (a.field != null) ? a.field : false; this.oConfiguration.cellColorScheme = (a.cellColorScheme != null) ? a.cellColorScheme : "ocean_blue"; this.oConfiguration.dateFormat = (a.dateFormat != null) ? a.dateFormat : "%m-%d-%Y"; this.oConfiguration.imgPath = (g_jsDatePickImagePath.length != null) ? g_jsDatePickImagePath : "img/"; this.oConfiguration.weekStartDay = (a.weekStartDay != null) ? a.weekStartDay : 1; this.selectedDayObject = {}; this.flag_DayMarkedBeforeRepopulation = false; this.flag_aDayWasSelected = false; this.lastMarkedDayObject = null; if (!this.oConfiguration.selectedDate) { this.currentYear = this.oCurrentDay.year; this.currentMonth = this.oCurrentDay.month; this.currentDay = this.oCurrentDay.day } }; JsDatePick.prototype.resizeCalendar = function() { this.leftWallStrechedElement.style.height = "0px"; this.rightWallStrechedElement.style.height = "0px"; var a = this.JsDatePickBox.offsetHeight, b = a - 16; if (b < 0) { return } this.leftWallStrechedElement.style.height = b + "px"; this.rightWallStrechedElement.style.height = b + "px"; return true }; JsDatePick.prototype.closeCalendar = function() { this.JsDatePickBox.style.display = "none"; document.onclick = function() { } }; JsDatePick.prototype.populateFieldWithSelectedDate = function() { JsgetElem(this.oConfiguration.target).value = this.getSelectedDayFormatted(); if (this.lastPickedDateObject) { delete (this.lastPickedDateObject) } this.lastPickedDateObject = {}; this.lastPickedDateObject.day = this.selectedDayObject.day; this.lastPickedDateObject.month = this.selectedDayObject.month; this.lastPickedDateObject.year = this.selectedDayObject.year; this.closeCalendar() }; JsDatePick.prototype.makeCalendar = function() { var j = document, e, a, b, k, g, h, f, o, i, m, n, l, c; e = j.createElement("div"); a = j.createElement("div"); b = j.createElement("div"); this.setC(e, "JsDatePickBox"); this.setC(a, "clearfix"); this.setC(b, "jsDatePickCloseButton"); b.setAttribute("globalNumber", this.globalNumber); b.onmouseover = function() { var d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(g_l.CLOSE); d.setC(this, "jsDatePickCloseButtonOver") }; b.onmouseout = function() { var d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(""); d.setC(this, "jsDatePickCloseButton") }; b.onmousedown = function() { var d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(g_l.CLOSE); d.setC(this, "jsDatePickCloseButtonDown") }; b.onmouseup = function() { var d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(""); d.setC(this, "jsDatePickCloseButton"); d.closeCalendar() }; this.JsDatePickBox = e; k = j.createElement("div"); g = j.createElement("div"); h = j.createElement("div"); f = j.createElement("div"); this.setC(h, "topWall"); this.setC(f, "bottomWall"); if (this.isie && this.iever == 6) { f.style.bottom = "-2px" } o = j.createElement("div"); i = j.createElement("div"); m = j.createElement("div"); this.setC(o, "leftTopCorner"); this.setC(i, "leftBottomCorner"); this.setC(m, "leftWall"); this.leftWallStrechedElement = m; this.leftWall = k; this.rightWall = g; k.appendChild(o); k.appendChild(m); k.appendChild(i); o = j.createElement("div"); i = j.createElement("div"); m = j.createElement("div"); this.setC(o, "rightTopCorner"); this.setC(i, "rightBottomCorner"); this.setC(m, "rightWall"); this.rightWallStrechedElement = m; g.appendChild(o); g.appendChild(m); g.appendChild(i); if (this.oConfiguration.isStripped) { this.setC(k, "hiddenBoxLeftWall"); this.setC(g, "hiddenBoxRightWall") } else { this.setC(k, "boxLeftWall"); this.setC(g, "boxRightWall") } e.appendChild(k); e.appendChild(this.getDOMCalendarStripped()); e.appendChild(g); e.appendChild(a); if (!this.oConfiguration.isStripped) { e.appendChild(b); e.appendChild(h); e.appendChild(f) } if (this.oConfiguration.useMode == 2) { if (this.oConfiguration.target != false) { if (typeof (JsgetElem(this.oConfiguration.target)) != null) { n = JsgetElem(this.oConfiguration.target); l = document.createElement("span"); n.parentNode.replaceChild(l, n); l.appendChild(n); n.setAttribute("globalNumber", this.globalNumber); n.onclick = function() { JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")).showCalendar() }; n.onfocus = function() { JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")).showCalendar() }; l.style.position = "relative"; this.initialZIndex++; e.style.zIndex = this.initialZIndex.toString(); e.style.position = "absolute"; e.style.top = "18px"; e.style.left = "0px"; e.style.display = "none"; l.appendChild(e); c = new Function("g_arrayOfUsedJsDatePickCals[" + this.globalNumber + "].populateFieldWithSelectedDate();"); this.setOnSelectedDelegate(c) } else { alert(g_l.ERROR_3) } } } else { if (this.oConfiguration.target != null) { JsgetElem(this.oConfiguration.target).appendChild(e); JsgetElem(this.oConfiguration.target).style.position = "relative"; e.style.position = "absolute"; e.style.top = "0px"; e.style.left = "0px"; this.resizeCalendar(); this.executePopulationDelegateIfExists() } else { alert(g_l.ERROR_4) } } }; JsDatePick.prototype.determineFieldDate = function() { var b, c, e, g, l, d, a, h, k, f = false, j = false; if (this.lastPickedDateObject) { this.setSelectedDay({ year: parseInt(this.lastPickedDateObject.year), month: parseInt(this.lastPickedDateObject.month, 10), day: parseInt(this.lastPickedDateObject.day, 10) }) } else { b = JsgetElem(this.oConfiguration.target); if (b.value.trim().length == 0) { this.unsetSelection(); if (typeof (this.oConfiguration.selectedDate) == "object" && this.oConfiguration.selectedDate) { this.setSelectedDay({ year: parseInt(this.oConfiguration.selectedDate.year), month: parseInt(this.oConfiguration.selectedDate.month, 10), day: parseInt(this.oConfiguration.selectedDate.day, 10) }) } } else { if (b.value.trim().length > 5) { c = this.senseDivider(this.oConfiguration.dateFormat); e = this.oConfiguration.dateFormat; g = b.value.trim().split(c); l = e.trim().split(c); d = a = h = k = 0; for (d = 0; d < l.length; d++) { switch (l[d]) { case "%d": case "%j": a = d; break; case "%m": case "%n": k = d; break; case "%M": k = d; f = true; break; case "%F": k = d; j = true; break; case "%Y": case "%y": h = d } } if (f) { for (d = 0; d < 12; d++) { if (g_l.MONTHS[d].substr(0, 3).toUpperCase() == g[k].toUpperCase()) { k = d + 1; break } } } else { if (j) { for (d = 0; d < 12; d++) { if (g_l.MONTHS[d].toLowerCase() == g[k].toLowerCase()) { k = d + 1; break } } } else { k = parseInt(g[k], 10) } } this.setSelectedDay({ year: parseInt(g[h], 10), month: k, day: parseInt(g[a], 10) }) } else { this.unsetSelection(); return } } } }; JsDatePick.prototype.senseDivider = function(a) { return a.replace("%d", "").replace("%j", "").replace("%m", "").replace("%M", "").replace("%n", "").replace("%F", "").replace("%Y", "").replace("%y", "").substr(0, 1) }; JsDatePick.prototype.showCalendar = function() { if (this.JsDatePickBox.style.display == "none") { this.determineFieldDate(); this.JsDatePickBox.style.display = "block"; this.resizeCalendar(); this.executePopulationDelegateIfExists(); this.JsDatePickBox.onmouseover = function() { document.onclick = function() { } }; this.JsDatePickBox.setAttribute("globalCalNumber", this.globalNumber); this.JsDatePickBox.onmouseout = function() { document.onclick = new Function("g_arrayOfUsedJsDatePickCals[" + this.getAttribute("globalCalNumber") + "].closeCalendar();") } } else { return } }; JsDatePick.prototype.isAvailable = function(c, a, b) { if (c > this.oCurrentDay.year) { return false } if (a > this.oCurrentDay.month && c == this.oCurrentDay.year) { return false } if (b > this.oCurrentDay.day && a == this.oCurrentDay.month && c == this.oCurrentDay.year) { return false } return true }; JsDatePick.prototype.getDOMCalendarStripped = function() { var h = document, e, i, b, a, f, c, g; e = h.createElement("div"); if (this.oConfiguration.isStripped) { this.setC(e, "boxMainStripped") } else { this.setC(e, "boxMain") } this.boxMain = e; i = h.createElement("div"); b = h.createElement("div"); a = h.createElement("div"); f = h.createElement("div"); c = h.createElement("div"); g = h.createElement("div"); this.setC(b, "clearfix"); this.setC(g, "clearfix"); this.setC(i, "boxMainInner"); this.setC(a, "boxMainCellsContainer"); this.setC(f, "tooltip"); this.setC(c, "weekDaysRow"); this.tooltip = f; e.appendChild(i); this.controlsBar = this.getDOMControlBar(); this.makeDOMWeekDays(c); i.appendChild(this.controlsBar); i.appendChild(b); i.appendChild(f); i.appendChild(c); i.appendChild(a); i.appendChild(g); this.boxMainCellsContainer = a; this.populateMainBox(a); return e }; JsDatePick.prototype.makeDOMWeekDays = function(a) { var c = 0, g = document, f = g_l.DAYS_3, e, b; for (c = this.oConfiguration.weekStartDay; c < 7; c++) { b = g.createElement("div"); e = g.createTextNode(f[c]); this.setC(b, "weekDay"); b.appendChild(e); a.appendChild(b) } if (this.oConfiguration.weekStartDay > 0) { for (c = 0; c < this.oConfiguration.weekStartDay; c++) { b = g.createElement("div"); e = g.createTextNode(f[c]); this.setC(b, "weekDay"); b.appendChild(e); a.appendChild(b) } } b.style.marginRight = "0px" }; JsDatePick.prototype.repopulateMainBox = function() { while (this.boxMainCellsContainer.firstChild) { this.boxMainCellsContainer.removeChild(this.boxMainCellsContainer.firstChild) } this.populateMainBox(this.boxMainCellsContainer); this.resizeCalendar(); this.executePopulationDelegateIfExists() }; JsDatePick.prototype.executePopulationDelegateIfExists = function() { if (typeof (this.repopulationDelegate) == "function") { this.repopulationDelegate() } }; JsDatePick.prototype.populateMainBox = function(h) { var f = document, g, l, c = 1, k = false, n = this.currentMonth - 1, j, a, m, e, b; j = new Date(this.currentYear, n, 1, 1, 0, 0); a = j.getTime(); this.flag_DayMarkedBeforeRepopulation = false; this.setControlBarText(this.monthsTextualRepresentation[n] + ", " + this.currentYear); m = parseInt(j.getDay()) - this.oConfiguration.weekStartDay; if (m < 0) { m = m + 7 } e = 0; for (e = 0; e < m; e++) { g = f.createElement("div"); this.setC(g, "skipDay"); h.appendChild(g); if (c == 7) { c = 1 } else { c++ } } while (j.getMonth() == n) { k = false; g = f.createElement("div"); if (this.lastPostedDay) { if (this.lastPostedDay == j.getDate()) { l = parseInt(this.lastPostedDay, 10) + 1 } else { l = f.createTextNode(j.getDate()) } } else { l = f.createTextNode(j.getDate()) } g.appendChild(l); h.appendChild(g); g.setAttribute("globalNumber", this.globalNumber); if (c == 7) { if (g_jsDatePickDirectionality == "ltr") { g.style.marginRight = "0px" } else { g.style.marginLeft = "0px" } } if (this.isToday(j)) { g.setAttribute("isToday", 1) } if (this.oConfiguration.limitToToday) { if (!this.isAvailable(this.currentYear, this.currentMonth, parseInt(j.getDate()))) { k = true; g.setAttribute("isJsDatePickDisabled", 1) } } g.onmouseover = function() { var d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")), i; i = d.getCurrentColorScheme(); if (parseInt(this.getAttribute("isSelected")) == 1) { return } if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } if (parseInt(this.getAttribute("isToday")) == 1) { d.setC(this, "dayOverToday"); this.style.background = "url(" + d.oConfiguration.imgPath + i + "_dayOver.gif) left top no-repeat" } else { d.setC(this, "dayOver"); this.style.background = "url(" + d.oConfiguration.imgPath + i + "_dayOver.gif) left top no-repeat" } }; g.onmouseout = function() { var d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")), i; i = d.getCurrentColorScheme(); if (parseInt(this.getAttribute("isSelected")) == 1) { return } if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } if (parseInt(this.getAttribute("isToday")) == 1) { d.setC(this, "dayNormalToday"); this.style.background = "url(" + d.oConfiguration.imgPath + i + "_dayNormal.gif) left top no-repeat" } else { d.setC(this, "dayNormal"); this.style.background = "url(" + d.oConfiguration.imgPath + i + "_dayNormal.gif) left top no-repeat" } }; g.onmousedown = function() { var d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")), i; i = d.getCurrentColorScheme(); if (parseInt(this.getAttribute("isSelected")) == 1) { return } if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } if (parseInt(this.getAttribute("isToday")) == 1) { d.setC(this, "dayDownToday"); this.style.background = "url(" + d.oConfiguration.imgPath + i + "_dayDown.gif) left top no-repeat" } else { d.setC(this, "dayDown"); this.style.background = "url(" + d.oConfiguration.imgPath + i + "_dayDown.gif) left top no-repeat" } }; g.onmouseup = function() { var d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")), i; i = d.getCurrentColorScheme(); if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } if (parseInt(this.getAttribute("isToday")) == 1) { d.setC(this, "dayNormalToday"); this.style.background = "url(" + d.oConfiguration.imgPath + i + "_dayNormal.gif) left top no-repeat" } else { d.setC(this, "dayNormal"); this.style.background = "url(" + d.oConfiguration.imgPath + i + "_dayNormal.gif) left top no-repeat" } d.setDaySelection(this); d.executeOnSelectedDelegateIfExists() }; if (this.isSelectedDay(j.getDate())) { g.setAttribute("isSelected", 1); this.flag_DayMarkedBeforeRepopulation = true; this.lastMarkedDayObject = g; if (parseInt(g.getAttribute("isToday")) == 1) { this.setC(g, "dayDownToday"); g.style.background = "url(" + this.oConfiguration.imgPath + this.oConfiguration.cellColorScheme + "_dayDown.gif) left top no-repeat" } else { this.setC(g, "dayDown"); g.style.background = "url(" + this.oConfiguration.imgPath + this.oConfiguration.cellColorScheme + "_dayDown.gif) left top no-repeat" } } else { b = this.getCurrentColorScheme(); if (parseInt(g.getAttribute("isToday")) == 1) { if (k) { this.setC(g, "dayDisabled"); g.style.background = "url(" + this.oConfiguration.imgPath + this.oConfiguration.cellColorScheme + "_dayNormal.gif) left top no-repeat" } else { this.setC(g, "dayNormalToday"); g.style.background = "url(" + this.oConfiguration.imgPath + this.oConfiguration.cellColorScheme + "_dayNormal.gif) left top no-repeat" } } else { if (k) { this.setC(g, "dayDisabled"); g.style.background = "url(" + this.oConfiguration.imgPath + this.oConfiguration.cellColorScheme + "_dayNormal.gif) left top no-repeat" } else { this.setC(g, "dayNormal"); g.style.background = "url(" + this.oConfiguration.imgPath + this.oConfiguration.cellColorScheme + "_dayNormal.gif) left top no-repeat" } } } if (c == 7) { c = 1 } else { c++ } a += 86400000; j.setTime(a) } this.lastPostedDay = null; return h }; JsDatePick.prototype.unsetSelection = function() { this.flag_aDayWasSelected = false; this.selectedDayObject = {}; this.repopulateMainBox() }; JsDatePick.prototype.setSelectedDay = function(a) { this.flag_aDayWasSelected = true; this.selectedDayObject.day = parseInt(a.day, 10); this.selectedDayObject.month = parseInt(a.month, 10); this.selectedDayObject.year = parseInt(a.year); this.currentMonth = a.month; this.currentYear = a.year; this.repopulateMainBox() }; JsDatePick.prototype.isSelectedDay = function(a) { if (this.flag_aDayWasSelected) { if (parseInt(a) == this.selectedDayObject.day && this.currentMonth == this.selectedDayObject.month && this.currentYear == this.selectedDayObject.year) { return true } else { return false } } return false }; JsDatePick.prototype.getSelectedDay = function() { if (this.flag_aDayWasSelected) { return this.selectedDayObject } else { return false } }; JsDatePick.prototype.getSelectedDayFormatted = function() { if (this.flag_aDayWasSelected) { var a = this.oConfiguration.dateFormat; a = a.replace("%d", this.selectedDayObject.day.toString().strpad()); a = a.replace("%j", this.selectedDayObject.day); a = a.replace("%m", this.selectedDayObject.month.toString().strpad()); a = a.replace("%M", g_l.MONTHS[this.selectedDayObject.month - 1].substr(0, 3).toUpperCase()); a = a.replace("%n", this.selectedDayObject.month); a = a.replace("%F", g_l.MONTHS[this.selectedDayObject.month - 1]); a = a.replace("%Y", this.selectedDayObject.year); a = a.replace("%y", this.selectedDayObject.year.toString().substr(2, 2)); return a } else { return false } }; JsDatePick.prototype.setDaySelection = function(a) { var b = this.getCurrentColorScheme(); if (this.flag_DayMarkedBeforeRepopulation) { this.lastMarkedDayObject.setAttribute("isSelected", 0); if (parseInt(this.lastMarkedDayObject.getAttribute("isToday")) == 1) { this.setC(this.lastMarkedDayObject, "dayNormalToday"); this.lastMarkedDayObject.style.background = "url(" + this.oConfiguration.imgPath + b + "_dayNormal.gif) left top no-repeat" } else { this.setC(this.lastMarkedDayObject, "dayNormal"); this.lastMarkedDayObject.style.background = "url(" + this.oConfiguration.imgPath + b + "_dayNormal.gif) left top no-repeat" } } this.flag_aDayWasSelected = true; this.selectedDayObject.year = this.currentYear; this.selectedDayObject.month = this.currentMonth; this.selectedDayObject.day = parseInt(a.innerHTML); this.flag_DayMarkedBeforeRepopulation = true; this.lastMarkedDayObject = a; a.setAttribute("isSelected", 1); if (parseInt(a.getAttribute("isToday")) == 1) { this.setC(a, "dayDownToday"); a.style.background = "url(" + this.oConfiguration.imgPath + b + "_dayDown.gif) left top no-repeat" } else { this.setC(a, "dayDown"); a.style.background = "url(" + this.oConfiguration.imgPath + b + "_dayDown.gif) left top no-repeat" } }; JsDatePick.prototype.isToday = function(a) { var b = this.oCurrentDay.month - 1; if (a.getDate() == this.oCurrentDay.day && a.getMonth() == b && a.getFullYear() == this.oCurrentDay.year) { return true } return false }; JsDatePick.prototype.setControlBarText = function(a) { var b = document.createTextNode(a); while (this.controlsBarTextCell.firstChild) { this.controlsBarTextCell.removeChild(this.controlsBarTextCell.firstChild) } this.controlsBarTextCell.appendChild(b) }; JsDatePick.prototype.setTooltipText = function(a) { while (this.tooltip.firstChild) { this.tooltip.removeChild(this.tooltip.firstChild) } var b = document.createTextNode(a); this.tooltip.appendChild(b) }; JsDatePick.prototype.moveForwardOneYear = function() { var a = this.currentYear + 1; if (a < parseInt(this.oConfiguration.yearsRange[1])) { this.currentYear++; this.repopulateMainBox(); return true } else { return false } }; JsDatePick.prototype.moveBackOneYear = function() { var a = this.currentYear - 1; if (a > parseInt(this.oConfiguration.yearsRange[0])) { this.currentYear--; this.repopulateMainBox(); return true } else { return false } }; JsDatePick.prototype.moveForwardOneMonth = function() { if (this.currentMonth < 12) { this.currentMonth++ } else { if (this.moveForwardOneYear()) { this.currentMonth = 1 } else { this.currentMonth = 12 } } this.repopulateMainBox() }; JsDatePick.prototype.moveBackOneMonth = function() { if (this.currentMonth > 1) { this.currentMonth-- } else { if (this.moveBackOneYear()) { this.currentMonth = 12 } else { this.currentMonth = 1 } } this.repopulateMainBox() }; JsDatePick.prototype.getCurrentColorScheme = function() { return this.oConfiguration.cellColorScheme }; JsDatePick.prototype.getDOMControlBar = function() { var h = document, c, f, g, b, a, e; c = h.createElement("div"); f = h.createElement("div"); g = h.createElement("div"); b = h.createElement("div"); a = h.createElement("div"); e = h.createElement("div"); this.setC(c, "controlsBar"); this.setC(f, "monthForwardButton"); this.setC(g, "monthBackwardButton"); this.setC(b, "yearForwardButton"); this.setC(a, "yearBackwardButton"); this.setC(e, "controlsBarText"); c.setAttribute("globalNumber", this.globalNumber); f.setAttribute("globalNumber", this.globalNumber); g.setAttribute("globalNumber", this.globalNumber); a.setAttribute("globalNumber", this.globalNumber); b.setAttribute("globalNumber", this.globalNumber); this.controlsBarTextCell = e; c.appendChild(f); c.appendChild(g); c.appendChild(b); c.appendChild(a); c.appendChild(e); f.onmouseover = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } d = this.parentNode; while (d.className != "controlsBar") { d = d.parentNode } i = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); i.setTooltipText(g_l.MONTH_FWD); i.setC(this, "monthForwardButtonOver") }; f.onmouseout = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(""); d.setC(this, "monthForwardButton") }; f.onmousedown = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } d = this.parentNode; while (d.className != "controlsBar") { d = d.parentNode } i = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); i.setTooltipText(g_l.MONTH_FWD); i.setC(this, "monthForwardButtonDown") }; f.onmouseup = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(g_l.MONTH_FWD); d.setC(this, "monthForwardButton"); d.moveForwardOneMonth() }; g.onmouseover = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(g_l.MONTH_BCK); d.setC(this, "monthBackwardButtonOver") }; g.onmouseout = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(""); d.setC(this, "monthBackwardButton") }; g.onmousedown = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(g_l.MONTH_BCK); d.setC(this, "monthBackwardButtonDown") }; g.onmouseup = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(g_l.MONTH_BCK); d.setC(this, "monthBackwardButton"); d.moveBackOneMonth() }; b.onmouseover = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(g_l.YEAR_FWD); d.setC(this, "yearForwardButtonOver") }; b.onmouseout = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(""); d.setC(this, "yearForwardButton") }; b.onmousedown = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(g_l.YEAR_FWD); d.setC(this, "yearForwardButtonDown") }; b.onmouseup = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(g_l.YEAR_FWD); d.setC(this, "yearForwardButton"); d.moveForwardOneYear() }; a.onmouseover = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(g_l.YEAR_BCK); d.setC(this, "yearBackwardButtonOver") }; a.onmouseout = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(""); d.setC(this, "yearBackwardButton") }; a.onmousedown = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(g_l.YEAR_BCK); d.setC(this, "yearBackwardButtonDown") }; a.onmouseup = function() { var i, d; if (parseInt(this.getAttribute("isJsDatePickDisabled")) == 1) { return } i = this.parentNode; while (i.className != "controlsBar") { i = i.parentNode } d = JsDatePick.getCalInstanceById(this.getAttribute("globalNumber")); d.setTooltipText(g_l.YEAR_BCK); d.setC(this, "yearBackwardButton"); d.moveBackOneYear() }; return c };

function isSpace(evt) {
    try {
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode == 32)
            return false;
        return true;
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}


function NoPermissionMessage() {
    try {
        alert('You are not having the permission to Add/Update. Please contact Administrator!');
        var message = '';
        message = 'You are not having the permission to Add/Update. Please contact Administrator!';
        $('[id$=divMyMessage] span').remove();
        $('[id$=divMyMessage]').append('<span class="Error">' + message + '</span>');
        $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
        return false;
    }
    catch (Error) {
        ErrorMessage(Error.message);
        return false;
    }
}

function changedate(FrmDt) {
    try {
        var strdate = $('[id$=txtFrmDt]').val();
        var strdate1 = strdate.split('-');
        strdate = (strdate1[1] + '-' + strdate1[0] + '-' + strdate1[2]);
        strdate = new Date(strdate.replace(/-/g, "/"));
        $('[id$=txtToDt]').datepicker('option', {
            minDate: new Date(strdate),
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
        });
    }
    catch (Error) {
        ErrorMessage(Error.message);
    }
}

function ClearUploadControle(ULCntrlID) {
    try {
        var AsyncFileUpload = ULCntrlID[0]; //  $("#<%=AsyncFileUpload1.ClientID%>")[0];
        var txts = AsyncFileUpload.getElementsByTagName("input");
        for (var i = 0; i < txts.length; i++) {
            txts[i].value = "";
            txts[i].style.backgroundColor = "transparent";
        }
    }
    catch (Error) {
        ErrorMessage(Error.Message);
    }
}

function SuccessMessage(msg) {
    $('[id$=divMyMessage] span').remove();
    $('[id$=divMyMessage]').append('<span class="Success">' + msg + '</span>');
    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
}
function ErrorMessage(msg) {
    $('[id$=divMyMessage] span').remove();
    $('[id$=divMyMessage]').append('<span class="Error">' + msg + '</span>');
    $('[id$=divMyMessage]').fadeTo(2000, 1).fadeOut(3000);
}

// This function allows AlphaNumerics,Space and Underscore(_) only
function alphaNumericSpaceAnd_(cntrl) {
    cntrl.value = cntrl.value.replace(/[^a-zA-Z0-9_]/g, '')
}

// This function allows AlphaNumerics only
function alphaNumericsOnly(cntrl) {
    cntrl.value = cntrl.value.replace(/[^a-zA-Z0-9 ]/g, '')
}

// This function allows AlphaNumerics and Underscore(_) only
function alphaNumericAnd_(cntrl) {
    cntrl.value = cntrl.value.replace(/[^a-zA-Z0-9_]/g, '')
}

// This function allows AlphaNumerics and Underscore(_) & Space only
function alphaNumericAnd_AndSpace(cntrl) {
    cntrl.value = cntrl.value.replace(/[^a-zA-Z0-9_ ]/g, '')
}

// This function allows AlphaNumerics and Underscore(_) & Space & - / only
function alphaNumericAnd_AndSpace_Ifen_Slash(cntrl) {
    cntrl.value = cntrl.value.replace(/[^a-zA-Z0-9_ -\/]/g, '')
}

// This function allows Alphabets only
function alphabetsOnly(cntrl) {
    cntrl.value = cntrl.value.replace(/[^a-zA-Z]/g, '')
}

// This function allows Alphabets and SPACE only
function alphabetsAndSpaceOnly(cntrl) {
    cntrl.value = cntrl.value.replace(/[^a-zA-Z ]/g, '')
}

// This function allows Numerics and Underscore(_) only
function NumericAnd_(cntrl) {
    cntrl.value = cntrl.value.replace(/[^0-9_]/g, '')
}

// This function allows Numerics and Hyphen(-) only
function NumericAndHyphen(cntrl) {
    cntrl.value = cntrl.value.replace(/[^0-9-]/g, '')
} NumericOnly

// This function allows Numerics  only
function NumericOnly(cntrl) {
    cntrl.value = cntrl.value.replace(/[^0-9]/g, '')
}

// this function allows alphanumerics and underscore(_) only
function ValidateOnly(cntrl) {
    cntrl.value = cntrl.value.replace(/[^a-zA-Z 0-9\_\n\r]+/g, '');
}

function alphaNumericSpaceAnd_AndBracket(cntrl) {
    cntrl.value = cntrl.value.replace(/[^a-zA-Z0-9_() ]/g, '')
}