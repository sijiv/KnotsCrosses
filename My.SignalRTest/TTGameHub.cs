using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using My.SignalRTest.Models;
using Microsoft.AspNet.SignalR.Hubs;

namespace My.SignalRTest
{
    [HubName("KnotsCrosses")]
    public class TTGameHub : Hub
    {
        public void Hello()
        {
            var userId = Context.User.Identity;
            Clients.All.SendMessage("Hello {0}", userId);
            Clients.All.hello();
        }

        public override System.Threading.Tasks.Task OnConnected()
        {
            PlayerLookupQueries.AddPlayer(Context.ConnectionId, Context.User.Identity.Name);

            Clients.All.UpdatePlayerList(PlayerLookupQueries.GetAvailablePlayers());
            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var currentPlayer = PlayerLookupQueries.GetPlayer(Context.ConnectionId);
            currentPlayer.ConnectionId = null;
            if (!stopCalled)
            {
                Clients.OthersInGroup(currentPlayer.CurrentGame.GameId.ToString()).SendMessage(string.Format("Player {0} got temporarily disconnected.", currentPlayer.UserId));
            }
            else
            {
                var game = currentPlayer.CurrentGame;
                if (game != null)
                {
                    Groups.Remove(Context.ConnectionId, game.GameId.ToString());
                    Clients.Group(game.GameId.ToString()).SendMessage(string.Format("Player {0} has stopped playing.", currentPlayer.UserId));
                    game.Rival.GameSymbol = Symbol.None;
                    game.Rival.CurrentGame = null;
                    game.Challenger.GameSymbol = Symbol.None;
                    game.Challenger.CurrentGame = null;
                }
            }
            Clients.All.UpdatePlayerList(PlayerLookupQueries.GetAvailablePlayers());
            return base.OnDisconnected(stopCalled);
        }
        public override System.Threading.Tasks.Task OnReconnected()
        {
            var currentPlayer = PlayerLookupQueries.GetPlayerByUserId(Context.User.Identity.Name);
            if (currentPlayer != null)
            {
                var game = currentPlayer.CurrentGame;
                currentPlayer.ConnectionId = Context.ConnectionId;
                Groups.Add(Context.ConnectionId, game.GameId.ToString());
                Clients.OthersInGroup(game.GameId.ToString()).SendMessage(string.Format("Player {0} is reconnected to the Game.", currentPlayer.UserId));
                if (game.ActivePlayer == PlayerType.Challenger)
                {
                    if (currentPlayer == game.Challenger)
                    {
                        Clients.Client(currentPlayer.ConnectionId).ActivePlayer(true);
                    }
                    else
                    {
                        Clients.Client(currentPlayer.ConnectionId).ActivePlayer(false);
                    }
                }
                else if (game.ActivePlayer == PlayerType.Rival)
                {
                    if (currentPlayer == game.Rival)
                    {
                        Clients.Client(currentPlayer.ConnectionId).ActivePlayer(true);
                    }
                    else
                    {
                        Clients.Client(currentPlayer.ConnectionId).ActivePlayer(false);
                    }
                }
                Clients.All.UpdatePlayerList(PlayerLookupQueries.GetAvailablePlayers());
            }
            return base.OnReconnected();
        }

        public void ChallengePlayer(string playerUserId)
        {
            Player currentPlayer = PlayerLookupQueries.GetPlayer(Context.ConnectionId);
            Player rival = PlayerLookupQueries.GetPlayerByUserId(playerUserId);
            Game newGame = new Game() { Challenger = currentPlayer, Rival = rival, Board = new TicTacBoard(), ActivePlayer = PlayerType.Challenger, GameId = 1 };
            currentPlayer.CurrentGame = newGame;
            currentPlayer.GameSymbol = Symbol.X;
            rival.CurrentGame = newGame;
            rival.GameSymbol = Symbol.O;
            Groups.Add(currentPlayer.ConnectionId, newGame.GameId.ToString());
            Groups.Add(rival.ConnectionId, newGame.GameId.ToString());
            Clients.Group(newGame.GameId.ToString()).SendMessage(string.Format("Players {0} challenged {1} for a Tic Tac Game.", currentPlayer.UserId, rival.UserId));
            newGame.ActivePlayer= PlayerType.Challenger;
            Clients.Client(currentPlayer.ConnectionId).ActivePlayer(true);
            Clients.Client(rival.ConnectionId).ActivePlayer(false);
            Clients.Group(newGame.GameId.ToString()).Update(newGame.Board);
        }

        public void PlayerMove(Move move)
        {
            //Find Game Assigned to User!!
            var currentPlayer = PlayerLookupQueries.GetPlayer(Context.ConnectionId);
            var game = currentPlayer.CurrentGame;
            if (game != null)
            {
                Player nextPlayer = null;
                if (game.ActivePlayer == PlayerType.Challenger && game.Challenger == currentPlayer)
                {
                    game.Board.Positions[move.XPosn, move.YPosn] = currentPlayer.GameSymbol;
                    game.ActivePlayer = PlayerType.Rival;
                    nextPlayer = game.Rival;
                }
                else if (game.ActivePlayer == PlayerType.Rival && game.Rival == currentPlayer)
                {
                    game.Board.Positions[move.XPosn, move.YPosn] = currentPlayer.GameSymbol;
                    game.ActivePlayer = PlayerType.Challenger;
                    nextPlayer = game.Challenger;
                }

                var winner = CheckWinLogic(game);
                if (winner == null)
                {
                    Clients.Client(nextPlayer.ConnectionId).ActivePlayer(true);
                    Clients.Client(currentPlayer.ConnectionId).ActivePlayer(false);
                    Clients.Group(game.GameId.ToString()).SendMessage(string.Format("Player '{0}' to Move next.", nextPlayer.UserId));
                }
                else
                {
                    Clients.Client(nextPlayer.ConnectionId).ActivePlayer(false);
                    Clients.Client(currentPlayer.ConnectionId).ActivePlayer(false);
                    Clients.Group(game.GameId.ToString()).SendMessage(string.Format("Game Ended. Player '{0}' wins the Challenge.", winner.UserId));
                    game.Rival.GameSymbol = Symbol.None;
                    game.Rival.CurrentGame = null;
                    game.Challenger.GameSymbol = Symbol.None;
                    game.Challenger.CurrentGame = null;
                }
                Clients.Group(game.GameId.ToString()).Update(game.Board);
            }
        }

        private Player CheckWinLogic(Game currentGame)
        {
            TicTacBoard board = currentGame.Board;

            Player winningPlayer = null;
            Symbol winningSymbol = Symbol.None;

            //Horizontal Rows check
            for (int x = 0; x <= 2; x++)
            {
                if ((board.Positions[x, 0] == board.Positions[x, 1]) && (board.Positions[x, 0] == board.Positions[x, 2]))
                {
                    winningSymbol = board.Positions[x, 0];
                    if (winningSymbol != Symbol.None)
                    {
                        if (currentGame.Challenger.GameSymbol == winningSymbol)
                            winningPlayer = currentGame.Challenger;
                        else if (currentGame.Rival.GameSymbol == winningSymbol)
                            winningPlayer = currentGame.Rival;
                    }
                    if (winningPlayer != null) return winningPlayer;
                }
            }

            //Vertical Cols check
            for (int y = 0; y <= 2; y++)
            {
                if ((board.Positions[0, y] == board.Positions[1, y]) && (board.Positions[0, y] == board.Positions[2, y]))
                {
                    winningSymbol = board.Positions[0, y];
                    if (winningSymbol != Symbol.None)
                    {
                        if (currentGame.Challenger.GameSymbol == winningSymbol)
                            winningPlayer = currentGame.Challenger;
                        else if (currentGame.Rival.GameSymbol == winningSymbol)
                            winningPlayer = currentGame.Rival;
                    }
                    if (winningPlayer != null) return winningPlayer;
                }
            }

            //Diagonal Check
            if ((board.Positions[0, 0] == board.Positions[1, 1]) && (board.Positions[1, 1] == board.Positions[2, 2]))
            {
                winningSymbol = board.Positions[1, 1];
                if (winningSymbol != Symbol.None)
                {
                    if (currentGame.Challenger.GameSymbol == winningSymbol)
                        winningPlayer = currentGame.Challenger;
                    else if (currentGame.Rival.GameSymbol == winningSymbol)
                        winningPlayer = currentGame.Rival;
                }
                if (winningPlayer != null) return winningPlayer;
            }
            if ((board.Positions[0, 2] == board.Positions[1, 1]) && (board.Positions[1, 1] == board.Positions[2, 0]))
            {
                winningSymbol = board.Positions[1, 1];
                if (winningSymbol != Symbol.None)
                {
                    if (currentGame.Challenger.GameSymbol == winningSymbol)
                        winningPlayer = currentGame.Challenger;
                    else if (currentGame.Rival.GameSymbol == winningSymbol)
                        winningPlayer = currentGame.Rival;
                }
                if (winningPlayer != null) return winningPlayer;
            }

            return null;
        }
    }
}