﻿using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace MushroomsUnity3DExample {
	public class Player : BasePlayer {
		public float posx = 0;
		 public float posy = 0;
        public float posz = 0;
        public float roty = 0;
        public float skin = 0;


        public int toadspicked = 0;
    }

    public class Toad
    {
        public int id = 0;
        public float posx = 0;
        public float posz = 0;
    }

    [RoomType("WalkTypeOne")]
	public class GameCode : Game<Player> {
      
        private int last_toad_id = 0;
        private List<Toad> Toads = new List<Toad>();
        // This method is called when an instance of your the game is created
        public override void GameStarted() {
          
			// anything you write to the Console will show up in the 
			// output window of the development server
			Console.WriteLine("Game is started: " + RoomId);

            // spawn 10 toads at server start
            System.Random random = new System.Random();
            for (int x = 0; x < 10; x++)
            {

                int px = random.Next(-22, 88);
                int pz = random.Next(-70, 70);
                Toad temp = new Toad();
                temp.id = last_toad_id;
                temp.posx = px;
                temp.posz = pz;
                Toads.Add(temp);
                last_toad_id++;

            }

            // respawn new toads each 5 seconds
            AddTimer(respawntoads, 5000);
            // reset game every 2 minutes
           // AddTimer(resetgame, 10000);


        }

        private void resetgame()
        {
            // scoring system
            Player winner = new Player();
            int maxscore = -1;
            foreach (Player pl in Players)
            {
                if (pl.toadspicked > maxscore)
                {
                    winner = pl;
                    maxscore = pl.toadspicked;
                }
            }

       
            // reset everyone's score
            foreach (Player pl in Players)
            {
                pl.toadspicked = 0;
            }
            Broadcast("ToadCount", 0);
        }

        private void respawntoads()
        {
            if (Toads.Count == 10)
                return;

            System.Random random = new System.Random();
            // create new toads if there are less than 10
            for (int x = 0; x < 10 - Toads.Count; x++)
            {
                int px = random.Next(-22, 88);
                int pz = random.Next(-70, 70);
                Toad temp = new Toad();
                temp.id = last_toad_id;
                temp.posx = px;
                temp.posz = pz;
                Toads.Add(temp);
                last_toad_id++;

                // broadcast new toad information to all players
                Broadcast("Toad", temp.id, temp.posx, temp.posz);
            }
        }



        // This method is called when the last player leaves the room, and it's closed down.
        public override void GameClosed() {
			Console.WriteLine("RoomId: " + RoomId);
		}

		// This method is called whenever a player joins the game
		public override void UserJoined(Player player) {
            player.posx = (float)Convert.ToDouble(player.JoinData["posx"]);
            player.posy = (float)Convert.ToDouble(player.JoinData["posy"]);
            player.posz = (float)Convert.ToDouble(player.JoinData["posz"]);
            player.roty = (float)Convert.ToDouble(player.JoinData["roty"]);
            player.skin = (float)Convert.ToDouble(player.JoinData["skin"]);

            foreach (Player pl in Players)
            {

                if (pl.ConnectUserId != player.ConnectUserId)
                {
                    pl.Send("PlayerJoined", player.ConnectUserId, player.posx, player.posy, player.posz, player.roty, player.skin);
                    player.Send("PlayerJoined", pl.ConnectUserId, pl.posx, pl.posy, pl.posz, pl.roty, Convert.ToDouble(pl.JoinData["skin"]));
                }
            }

            // send current toadstool info to the player
            foreach (Toad t in Toads) {
				player.Send("Toad", t.id, t.posx, t.posz);
			}
		}

		// This method is called when a player leaves the game
		public override void UserLeft(Player player) {
			Broadcast("PlayerLeft", player.ConnectUserId);
		}

      //  public override bool AllowUserJoin(Player player)
       // {
         //   if (PlayerCount >= 20)
           // {
            //    player.Send("Error", 0);
            //    return false;
           // }
           // else
             //   return true;
      //  }
        // This method is called when a player sends a message into the server code
        public override void GotMessage(Player player, Message message) {
			switch(message.Type) {
                // called when a player clicks on the ground
                case "Move":
                    player.posx = message.GetFloat(0);
                    player.posy = message.GetFloat(1);
                    player.posz = message.GetFloat(2);
                    player.roty = message.GetFloat(3);
                    Broadcast("Move", player.ConnectUserId, player.posx, player.posy, player.posz, player.roty);
                    break;

                case "Rotation":

                    player.roty = message.GetFloat(0);
                    Broadcast("Rotation", player.ConnectUserId, player.roty);
                    break;
                case "GetMoney":
                  
                    player.Send("GetMoney", player.toadspicked);
                    break;

                case "Pickup":
                    // called when the player is actually close to the harvesting node
                    int pickupid = int.Parse(message.GetString(0).Replace("Toad", ""));

                    // Find a toad by its id
                    Toad result = Toads.Find(delegate (Toad td) { return td.id == pickupid; });

                    if (result != null)
                    {
                        // sends everyone information that a toad as been picked up
                        // increases player toad count
                        Broadcast("Picked", result.id);
                        Toads.Remove(result);
                        player.toadspicked++;
                        player.Send("ToadCount", player.toadspicked);
                    }
                    else
                    {
                        // id of the toad doesn't exist, either the player
                        // is trying to cheat, or someone else already picked 
                        // that toadstool
                        Console.WriteLine("Not found: {0}", pickupid);
                    }
                    break;

            }
		}
	}
}