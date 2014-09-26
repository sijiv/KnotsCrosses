﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace My.SignalRTest.Models
{
    public class TicTacModels
    {
    }
    
    public class TicTacBoard
    {
        public Symbol[,] Positions = new Symbol[2, 2];
    }

    public class Game
    {
        public int GameId { get; set; }
        public Player Challenger { get; set; }
        public Player Rival { get; set; }
        public TicTacBoard Board { get; set; }
        public PlayerType ActivePlayer { get; set; }
    }

    public enum PlayerType
    {
        Challenger, 
        Rival
    }

    public enum Symbol
    {
        None,
        X,
        O
    }
    public class Player
    {
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public Symbol GameSymbol { get; set; }
        public Game CurrentGame { get; set; }
    }

    public static class PlayerLookupQueries
    {

        private static List<Player> _PlayersList;
        //private static readonly PlayerLookupQueries _playerInstance;
        static PlayerLookupQueries()
        {
            _PlayersList = new List<Player>();
        }

        public static void AddPlayer(string connectionId, string userId)
        {
            Player player = _PlayersList.SingleOrDefault(p => p.UserId == userId);
            if (player == null)
                _PlayersList.Add(new Player() { ConnectionId = connectionId, UserId = userId });
            else
                player.ConnectionId = connectionId; //new connectionId
        }

        public static Player GetPlayer(string connectionId)
        {
            return _PlayersList.SingleOrDefault(p => p.ConnectionId == connectionId);
        }

        public static Player GetPlayerByUserId(string userId)
        {
            return _PlayersList.SingleOrDefault(p => p.UserId == userId);
        }
    }

    public class Move
    {
        public int XPosn { get; set; }
        public int YPosn { get; set; }
    }
}