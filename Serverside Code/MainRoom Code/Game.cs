using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace MushroomsUnity3DExample {

	public class Player : BasePlayer {
		public float posx = 0;
		 public float posy = 0;
        public float posz = 0;
        public float roty = 0;
        public float skin = 0;
    }

	
	[RoomType("OceanRoom2")]
	public class GameCode : Game<Player> {

		// This method is called when an instance of your the game is created
		public override void GameStarted() {
          
			// anything you write to the Console will show up in the 
			// output window of the development server
			Console.WriteLine("Game is started: " + RoomId);

			// spawn 10 toads at server start
			
			


		}

		

		
		// This method is called when the last player leaves the room, and it's closed down.
		public override void GameClosed() {
			Console.WriteLine("RoomId: " + RoomId);
		}

		// This method is called whenever a player joins the game
		public override void UserJoined(Player player) {

            player.posx = (float) Convert.ToDouble(player.JoinData["posx"]);
            player.posy = (float) Convert.ToDouble(player.JoinData["posy"]);
            player.posz = (float) Convert.ToDouble(player.JoinData["posz"]);
            player.roty = (float) Convert.ToDouble(player.JoinData["roty"]);
            player.skin = (float) Convert.ToDouble(player.JoinData["skin"]);

            foreach (Player pl in Players) {
     
                if (pl.ConnectUserId != player.ConnectUserId) {
					pl.Send("PlayerJoined", player.ConnectUserId, player.posx, player.posy, player.posz, player.roty, player.skin);
					player.Send("PlayerJoined", pl.ConnectUserId, pl.posx,pl.posy, pl.posz,pl.roty, Convert.ToDouble(pl.JoinData["skin"]));
				}
			}

			// send current toadstool info to the player
			//foreach(Toad t in Toads) {
			//	player.Send("Toad", t.id, t.posx, t.posz);
			//}
		}

		// This method is called when a player leaves the game
		public override void UserLeft(Player player) {
			Broadcast("PlayerLeft", player.ConnectUserId);
		}

		// This method is called when a player sends a message into the server code
		public override void GotMessage(Player player, Message message) {
			switch(message.Type) {
				// called when a player clicks on the ground
				case "Move":
					player.posx = message.GetFloat(0);
                    player.posy = message.GetFloat(1);
                    player.posz = message.GetFloat(2);
                    player.roty = message.GetFloat(3);
                    Broadcast("Move", player.ConnectUserId, player.posx, player.posy, player.posz,player.roty);
					break;

                case "Rotation":

                    player.roty = message.GetFloat(0);
                    Broadcast("Rotation", player.ConnectUserId, player.roty);
                    break;



            }
		}
	}
}