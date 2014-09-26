$(function () {
    var ttGame = $.connection.KnotsCrosses,
        gamesSection = $("#knotsCrossesGame");

    function init() {
        //return ttGame.server.getAllGames()

        gamesSection.empty();
        gamesSection.append("<table><tr><td id='00'></td><td id='01'></td><td id='02'></td></tr><tr><td id='10'></td><td id='11'></td><td id='12'></tr><tr><td id='20'></td><td id='21'></td><td id='22'></td></tr></table>");
        gamesSection.click(function () {
            // Is cell already occupied?
            
            ttGame.server.playerMove();
        });
    }

    $.extend(ttGame.client, {
        sendMessage: function (msg) {

        },
        activePlayer: function (isActive) {

        },
        update: function (ticTacBoard) {

        }
    });
    $.connection.hub.start()
        .then(init)
        .then(function () {
            //ttGame.server.getGameState();
        })
        .done(function (state) { })
});