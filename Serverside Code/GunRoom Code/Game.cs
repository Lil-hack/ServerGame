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
        public int hp = 100;
        public int win = 0;
        public int lose = 0;
        public int team = 0;

    }



	[RoomType("GunTypeOne")]
	public class GameCode : Game<Player> {
        private int countMatchs = 0;
        private int teamname = 0;
        private bool gameStatus = true;
		// This method is called when an instance of your the game is created
		public override void GameStarted() {
          
			// anything you write to the Console will show up in the 
			// output window of the development server
			Console.WriteLine("Game is started: " + RoomId);

			// spawn 10 toads at server start
	

			// respawn new toads each 5 seconds
			AddTimer(respawntoads, 5000);
			// reset game every 2 minutes
			AddTimer(resetgame, 120000);


		}

		private void resetgame() {
			// scoring system
			Player winner = new Player();
			int maxscore = -1;
		//	foreach(Player pl in Players) {
		//		if(pl.toadspicked > maxscore) {
		//			winner = pl;
		//			maxscore = pl.toadspicked;
		//		}
		//	}

			// broadcast who won the round
		//	if(winner.toadspicked > 0) {
		//		Broadcast("Chat", "Server", winner.ConnectUserId + " picked " + winner.toadspicked + " Toadstools and won this round.");
		//	} else {
		//		Broadcast("Chat", "Server", "No one won this round.");
		//	}

			// reset everyone's score
		//	foreach(Player pl in Players) {
		//		pl.toadspicked = 0;
		//	}
		//	Broadcast("ToadCount", 0);
		}

		private void respawntoads() {
			
		}

		// This method is called when the last player leaves the room, and it's closed down.
		public override void GameClosed() {
			Console.WriteLine("RoomId: " + RoomId);
		}

		// This method is called whenever a player joins the game
		public override void UserJoined(Player player) {
            teamname++;
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

            player.Send("Team", teamname);
        

        }

        // This method is called when a player leaves the game
        public override void UserLeft(Player player) {
			Broadcast("PlayerLeft", player.ConnectUserId);
		}
        public void RestartGame()
        {
            if (gameStatus == true)
                return;
            if (countMatchs < 5)
            {
                countMatchs++;
                foreach (Player pl in Players)
                {
                    pl.hp = 100;
                    pl.Send("Restart", 100, pl.win, pl.lose);
                    pl.Send("Team", pl.team);
                }
                gameStatus = true;
            }
            else {
                foreach (Player pl in Players)
                {
                    pl.hp = 100;
                    pl.Send("StopGame", 100, pl.win, pl.lose);
             
                }
                }
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
                    Broadcast("Move", player.ConnectUserId, player.posx, player.posy, player.posz, player.roty);
                    break;

                case "Rotation":

                    player.roty = message.GetFloat(0);
                    Broadcast("Rotation", player.ConnectUserId, player.roty);
                    break;
                case "GetMoney":

                 //   player.Send("GetMoney", player.toadspicked);
                    break;

                case "Fire":
                    foreach (Player pl in Players)
                    {
                        if (gameStatus == true)
                        {

                            if (pl.ConnectUserId == message.GetString(0))
                            {
                                if (message.GetInt(1) == 1)
                                    pl.hp -= 10;
                                if (message.GetInt(1) == 2)
                                    pl.hp -= 30;
                                if (pl.hp < 0)
                                {
                                    gameStatus = false;
                                    player.win++;
                                    pl.lose++;
                                    // создаем таймер
                                    AddTimer(RestartGame, 10000);
                                    Broadcast("Die", player.ConnectUserId, pl.ConnectUserId);


                                }
                                Broadcast("Fire", player.ConnectUserId, pl.ConnectUserId, pl.hp);
                            }
                        }
                    }
                    
                    break;
               
              
             
                
				
			
			}

           
		}
	}
}