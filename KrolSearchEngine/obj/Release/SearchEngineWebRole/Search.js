window.onload = function () {
    setup();
}

function setup() {
    document.getElementById("submit").onclick = searchPlayer;

    document.getElementById("search_bar").addEventListener('keyup', function () {
        var searchBarText = document.getElementById('search_bar').value;
        if (searchBarText.length == 0) {
            document.getElementById('suggestions').style.visibility = "hidden";
            document.getElementById('suggestions').innerHTML = '';
        }
        makeRequest();
    });
}

function makeRequest() {
    var searchBarText = document.getElementById('search_bar').value;
    if (searchBarText.length > 0) {
        searchBarText = searchBarText.replace(/\s+/g, '_').toLowerCase();

        $.ajax({
            type: "GET",
            url: "Wikiservice.asmx/Suggestions?input=" + searchBarText,
            dataType: "json",
            success: function (json) {
                document.getElementById('suggestions').innerHTML = '';
                document.getElementById('suggestions').style.visibility = "visible";
                var suggestions = $(document.getElementById('suggestions'));
                for (var key in json) {
                    var suggestionElement = document.createElement('div');
                    suggestionElement.innerHTML = beautify(key);
                    suggestionElement.onclick = autoComplete;
                    suggestionElement.classList.add('entry');

                    if (json[key]) {
                        suggestionElement.classList.add('history');
                        suggestions.prepend(suggestionElement);
                    }
                    else {
                        suggestions.append(suggestionElement);
                    }
                }
            }
        });
    }
}


// Sends AJAX request to search.php and resets player container div
function searchPlayer() {
    $("#players_container").html("");
    var query = document.getElementById("search_bar").value;
    $.ajax({
        crossDomain: true,
        contentType: "application/json; charset=utf-8",
        url: "http://ec2-52-39-201-171.us-west-2.compute.amazonaws.com/search.php",
        data: { search_input: query },
        dataType: "jsonp",
        success: getPlayers
    });

    var searchAzure = new XMLHttpRequest();
    searchAzure.open("GET", "Admin.asmx/GetResults?partial=" + query, true);
    searchAzure.onload = parseUrls;
    searchAzure.send();

    if (query > 0) {
        $.ajax({
            type: "GET",
            url: "wikiservice.asmx/Search?input=" + searchBarText.replace(/\s+/g, '_').toLowerCase(),
            success: function () {

            }
        });
    }
}

function parseUrls() {
    var xml = this.responseXML;
    document.getElementById("url_results").innerHTML = "";
    var urls = xml.getElementsByTagName("string");
    for (var i = 0; i < urls.length; i++) {
        var urlSplit = urls[i].textContent.split("||");
        document.getElementById("url_results").innerHTML += "<p><a href=" + urlSplit[1] +">" + urlSplit[0] + "</a></p>" + "</br>"
    }
}

// Parse JSON response and populate players container with player
// stat values
function getPlayers(data) {
    var playersContainer = $("#players_container");

    // Return if there's no input
    if (searchIsEmpty()) {
        document.getElementById("loading").style.visibility = 'hidden';
        return;
    }

    var playersJson = data;
    console.log(JSON.stringify(data));
    if (!playersJson || playersJson.length == 0) {
        // print out "no players"
    }
    for (var i = 0; i < playersJson.length; i++) {
        var player = playersJson[i];

        var name = player['name'];
        var team = player['team'];
        var gp = player['gp'];
        var min = player['min'];
        var off = player['off'];
        var def = player['def'];
        var tot = player['tot'];
        var ast = player['ast'];
        var to = player['to'];
        var stl = player['stl'];
        var blk = player['blk'];
        var pf = player['pf'];
        var ppg = player['ppg'];

        var fg = player['fg'];
        var tpt = player['tpt'];
        var ft = player['ft'];

        var playersDiv = $(document.createElement('div'));
        playersDiv.addClass("player");
        var firstname = name.split(" ")[0];
        var lastname = name.split(" ")[1];

        var playersInfo = document.createElement('div');
        playersInfo.classList.add("player_info");
        playersInfo.innerHTML = "<h2>" + name + "</h2><br /><ul><li>Team: " + team + "</li><li>Games Played: " + gp +
                                "</li><li>Minutes: " + min + "</li><li>Offensive: " + off +
                                "</li><li>Defensive: " + def + "</li><li>Total: " + tot + "</li><li>Assists: " + ast +
                                "</li><li>Turnovers: " + to + "</li><li>Steals: " + stl + "</li><li>Blocks: " + blk +
                                "</li><li>Personal Fouls: " + pf + "</li><li>Points per Game: " + ppg + "</li></ul>";

        playersInfo.innerHTML += "<img src='https://nba-players.herokuapp.com/players/" + lastname + "/" + firstname + "/' alt='" + name + "' />";

        playersDiv.append(playersInfo);

        var playerPoints = $(document.createElement('div'));
        playerPoints.addClass('points');
        playerPoints.append("FG <ul><li><span id='field'>A: </span>" + fg['a'] + "</li><li><span id='field'>M: </span>" + fg['m'] + "</li><li><span id='field'>Pct: </span>" + fg['pct'] + "%</li></ul>");

        playerPoints.append("3PT <ul><li><span id='field'>A: </span>" + tpt['a'] + "</li><li><span id='field'>M: </span>" + tpt['m'] + "</li><li><span id='field'>Pct: </span>" + tpt['pct'] + "%</li></ul>");

        playerPoints.append("FT <ul><li><span id='field'>A: </span>" + ft['a'] + "</li><li><span id='field'>M: </span>" + ft['m'] + "</li><li><span id='field'>Pct: </span>" + ft['pct'] + "%</li></ul>");

        playersDiv.append(playerPoints);

        var youtubeLogo = $(document.createElement('img'));
        youtubeLogo.val(firstname + " " + lastname);
        youtubeLogo.attr('src', 'youtube.png');
        youtubeLogo.attr('alt', 'youtube_link');
        youtubeLogo.attr('id', firstname + "_" + i);
        youtubeLogo.click(loadVideos);

        playerPoints.append(youtubeLogo);

        playersContainer.append(playersDiv);
        $(playersDiv).css('visibility', 'visible').hide().fadeIn('slow');
    }
    document.getElementById("loading").style.visibility = 'hidden';
}

// Calls Youtube API and populates pop-up div with embedded videos
function loadVideos() {
    var name = this.value;
    var ytContainer = $(document.createElement('div'));
    ytContainer.attr('id', 'youtube_container');
    ytContainer.addClass('page_container');

    var close = $(document.createElement('img'));
    close.attr('src', 'close.png');
    close.attr('alt', 'close');
    close.attr('id', 'close');
    ytContainer.append(close);
    $(document.body).append(ytContainer);
    close.click(function () {
        $("#youtube_container").remove();
    });

    ytEmbed.init(
      {
          'block': 'youtube_container',
          'key': 'AIzaSyBcveCGeFljNHG6MFubXL1aoOPrINowr48',
          'q': name,
          'type': 'search',
          'results': 4,
          'meta': true,
          'player': 'embed',
          'layout': 'full'
      }
    );
}

function searchIsEmpty() {
    var name = document.getElementById("search_bar").value;
    return name == null || name.length == 0;
}

function beautify(input) {
    var fragments = input.split('_');
    for (i = 0; i < fragments.length; i++) {
        fragments[i] = fragments[i].charAt(0).toUpperCase() + fragments[i].slice(1);
    }
    return fragments.join(' ');
}

function autoComplete() {
    var completedText = this.innerHTML;
    document.getElementById('search_bar').value = completedText;
    sendHistory();
}