$(function () {
    var ttGame = $.connection.KnotsCrosses,
        gamesSection = $("#knotsCrossesGame");

    function init() {
        //return ttGame.server.getAllGames()

        gamesSection.empty();
        gamesSection.append("<table><tr><td id='00'></td><td id='01'></td><td id='02'></td></tr><tr><td id='10'></td><td id='11'></td><td id='12'></tr><tr><td id='20'></td><td id='21'></td><td id='22'></td></tr></table>");
        $("table td", gamesSection).click(function () {
            // Is cell already occupied?
            var x = this.id.charAt(0), y = this.id.charAt(1);
            var move = { XPosn: x, YPosn: y };
            ttGame.server.playerMove(move);
        })
    }

    $.extend(ttGame.client, {
        sendMessage: function (msg) {

        },
        activePlayer: function (isActive) {

        },
        update: function (ticTacBoard) {

        },
        updatePlayerList: function (playerList) {
            $(".playerList").empty();
            $.each(playerList, function(index, val){
                $(".playerList").append("<li><h3>" + val.PlayerName + "</h3><p>" + val.Available + "</p></li>")
            });
            
        }
    });
    $.connection.hub.start()
        .then(init)
        .then(function () {
            //ttGame.server.joinLeague();
            //ttGame.server.getGameState();
        })
        .done(function (state) { })
});