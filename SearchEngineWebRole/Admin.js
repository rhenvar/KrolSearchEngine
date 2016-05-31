window.onload = function () {
    setup();
}

function setup() {
    document.getElementById("resume").onclick = resume;
    document.getElementById("stop").onclick = stop;
    document.getElementById("search").onclick = searchTitle;
    document.getElementById("clearqueue").onclick = clearqueue;
    document.getElementById("clearindex").onclick = clearindex;

    dataCall();
    var timeout = setInterval(function () {
        dataCall();
    }, 10000);
}

function clearQueue() {
    var ajax = new XMLHttpRequest();
    ajax.open('GET', '/admin.asmx/ClearQueue', true);
    ajax.onload = function() {}
    ajax.send();
}

function clearIndex() {
    var ajax = new XMLHttpRequest();
    ajax.open('GET', '/admin.asmx/ClearIndex', true);
    ajax.onload = function () { }
    ajax.send();
}

function dataCall() {
    var dataAjax = new XMLHttpRequest();
    dataAjax.open('GET', '/admin.asmx/GetData', true);
    dataAjax.onload = fetchData;
    dataAjax.send();
}

function fetchData() {
    var xml = this.responseXML;
    document.getElementById("lastten").innerHTML = "";
    document.getElementById("lastten").innerHTML += "<legend>Last Ten Urls Crawled</legend>";
    var crawledTag = xml.getElementsByTagName("LastCrawled")[0]; 
    if (crawledTag != undefined && crawledTag != null && crawledTag.textContent != undefined && crawledTag.textContent != null) {
        var lastCrawled = crawledTag.textContent.split("|||");
        for (var i = 0; i < lastCrawled.length; i++) {
            document.getElementById("lastten").innerHTML += lastCrawled[i] + "<br />";
        }
    }

    document.getElementById("lasttenerror").innerHTML = "";
    document.getElementById("lasttenerror").innerHTML += "<legend>Broken links</legend>";
    var errorTag = xml.getElementsByTagName("LastError")[0];
    if (errorTag != undefined && errorTag != null && errorTag.textContent != undefined && errorTag.textContent != null) {
        var lastError = errorTag.textContent.split("|||");
        for (var i = 0; i < lastError.length; i++) {
            document.getElementById("lasttenerror").innerHTML += lastError[i] + "<br />"
        }
    }

    document.getElementById("queuesize").innerHTML = "";
    document.getElementById("queuesize").innerHTML += "Queue Size: " + xml.getElementsByTagName("QueueSize")[0].textContent;

    document.getElementById("indexsize").innerHTML = "";
    document.getElementById("indexsize").innerHTML += "Index Size: " + xml.getElementsByTagName("IndexSize")[0].textContent;

    document.getElementById("cpu").innerHTML = "";
    document.getElementById("cpu").innerHTML += "CPU Usage: " + xml.getElementsByTagName("CpuUsage")[0].textContent;

    document.getElementById("mem").innerHTML = "";
    document.getElementById("mem").innerHTML += "Memory Left: " + xml.getElementsByTagName("MemLeft")[0].textContent;

    document.getElementById("workers").innerHTML = "";
    var workerStatuses = xml.getElementsByTagName("RoleStats")[0].textContent.split("||");
    for (var i = 0; i < workerStatuses.length - 1; i++) {
        if (workerStatuses.length > 0) {
            worker = workerStatuses[i].split("|");
            workers.innerHTML += worker[0] + " status: " + worker[1] + "<br />";
        }
    }

    document.getElementById("triesize").innerHTML = "";
    document.getElementById("trielasttitle").innerHTML = "";
    document.getElementById("triesize").innerHTML = "Trie Size: " + xml.getElementsByTagName("TrieSize")[0].textContent;
    document.getElementById("trielasttitle").innerHTML = "Last Trie Title: " + xml.getElementsByTagName("TrieLastTitle")[0].textContent;

}

function resume() {
    var ajax = new XMLHttpRequest();
    ajax.open('GET', '/admin.asmx/StartCrawling', true);
    ajax.send();
    ajax.onload = function () { };
}

function stop() {
    var ajax = new XMLHttpRequest();
    ajax.open('GET', '/admin.asmx/StopCrawling', true);
    ajax.send();
}

function searchTitle() {
    var url = document.getElementById("url").value;
    if (url != null && url.length > 0) {
        var urlAjax = new XMLHttpRequest();
        urlAjax.open('GET', '/admin.asmx/GetPageTitle?url=' + url, true);
        urlAjax.onload = parseTitle;
        urlAjax.send();
    }
}

function parseTitle() {
    var xml = this.responseXML;
    document.getElementById("titleoutput").innerHTML = "";
    document.getElementById("titleoutput").innerHTML = xml.getElementsByTagName("string")[0].textContent;
}