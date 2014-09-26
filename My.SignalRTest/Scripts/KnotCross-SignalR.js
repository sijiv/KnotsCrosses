$(function () {
    var ttGame = $.connection.KnotsCrosses,
        gamesSection = $("#KnotsCrossesGame");

    function init() {
        return ttGame.server.getAllGames()
    }
});