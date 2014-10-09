$(function () {
    var ttGame = $.connection.KnotsCrosses,
        gamesSection = $("#knotsCrossesGame"),
        naught = $("<img src='../Images/Naught.png'>"),
        cross = $("<img src='../Images/Cross.png'>"),
        nowTime = function () {
            var now = new Date();
            var padfunc = function (n, pad) {
                return ((n < 10) ? ("0" + n) : (n + ""));
            };
            return now.getHours() + ":" + padfunc(now.getMinutes(), "0") + ":" + padfunc(now.getSeconds(), "0");
        };

    function init() {
        //return ttGame.server.getAllGames()

        gamesSection.empty();
        gamesSection.append("<table><tr><td id='00'></td><td id='01'></td><td id='02'></td></tr><tr><td id='10'></td><td id='11'></td><td id='12'></tr><tr><td id='20'></td><td id='21'></td><td id='22'></td></tr></table>");
    }

    $.extend(ttGame.client, {
        sendMessage: function (msg) {
            $(".messages").prepend("<div>" + nowTime() + "| " + msg + "</div>")
        },
        activePlayer: function (isActive) {
            if (isActive === true) {
                $("table td:not(:has('img'))", gamesSection).click(function () {
                    // Is cell already occupied?
                    var x = this.id.charAt(0), y = this.id.charAt(1);
                    var move = { XPosn: x, YPosn: y };
                    ttGame.server.playerMove(move);
                    $(this).removeClass("unselectedCellPointer");
                    //alert(move.XPosn + ", " + move.YPosn);
                }).addClass("unselectedCellPointer");
            }
            else {
                $("table td", gamesSection)
                        .unbind("click")
                        .removeClass("unselectedCellPointer");
            }
        },
        update: function (ticTacBoard) {
            $("img", gamesSection).remove();
            $.each(ticTacBoard.Positions, function (x, row) {
                $.each(row, function (y, colVal) {
                    if (colVal === 1) { $("#" + x + y, gamesSection).append(cross.clone()); }
                    if (colVal === 2) { $("#" + x + y, gamesSection).append(naught.clone()); }
                });
            });
        },
        updatePlayerList: function (playerList) {
            $(".playerList").empty();
            $.each(playerList, function (index, val) {
                if ($("#hidUserId").val() != val.PlayerName) {
                    var lia = $("<li></li>").append("<h4><a href='#'>" + val.PlayerName + "</a></h4>");
                    if (val.Available) lia.append("<span>Busy</span>");
                    $(".playerList").append(lia);
                }
            });
            $(".playerList a:not(:has('span'))").bind("click", function () {
                var opponent = $(this).text();
                ttGame.server.challengePlayer(opponent);
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