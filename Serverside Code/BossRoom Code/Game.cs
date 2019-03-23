using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;
using System.Linq;

namespace MushroomsUnity3DExample
{
    public class Player : BasePlayer
    {
        public float posx = 0;
        public float posy = 0;
        public float posz = 0;
        public float roty = 0;
        public float skin = 0;
        public int hp = 100;
      
   

    }

    public class Boss
    {

        public float posx = 0;
        public float posy = 0;
        public float posz = 0;
        public float roty = 0;
        public int bossHP = 500;

    }

    [RoomType("BossTypeRoom")]
    public class GameCode : Game<Player>
    {
       
       
        private bool gameStatus = true;
        private bool metkaEnd = true;
        Boss boss = new Boss();
        // This method is called when an instance of your the game is created
        public override void GameStarted()
        {
           
            // anything you write to the Console will show up in the 
            // output window of the development server
            Console.WriteLine("Game is started: " + RoomId);

            // spawn 10 toads at server start


            // respawn new toads each 5 seconds
           AddTimer(moveBoss, 15000);
            // reset game every 2 minutes
            //AddTimer(resetgame, 120000);


        }

        //private void resetgame() {
        // scoring system
        //	Player winner = new Player();
        //	int maxscore = -1;
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
   
        //	Broadcast("ToadCount", 0);
        //}

        private void moveBoss() {
            Random random = new Random();
            int randomNumber = random.Next(0, PlayerCount);
           var heroPos= Players.ElementAt(randomNumber);
            Broadcast("MoveBoss", heroPos.posx, heroPos.posz);



        }

        // This method is called when the last player leaves the room, and it's closed down.
        public override void GameClosed()
        {
            Console.WriteLine("RoomId: " + RoomId);
        }

        // This method is called whenever a player joins the game
        public override void UserJoined(Player player)
        {
           
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
            



        }

        // This method is called when a player leaves the game
        public override void UserLeft(Player player)
        {
            Broadcast("PlayerLeft", player.ConnectUserId);
        }
       
        // This method is called when a player sends a message into the server code
        public override void GotMessage(Player player, Message message)
        {
            switch (message.Type)
            {
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
                case "FireBall":
                    Broadcast("FireBall", message.GetFloat(0),
                        message.GetFloat(1),
                        message.GetFloat(2),
                        message.GetFloat(3),
                        message.GetFloat(4),
                        message.GetFloat(5),
                        player.ConnectUserId);


                    break;
                case "Start":
                    gameStatus = true;
                    Broadcast("Start", true);
                    break;
                case "Fire":


                    if (gameStatus == true)
                    {
                        if (message.GetInt(1) == 1)
                        {
                            boss.bossHP -= 20;
                        }

                        if (message.GetInt(1) == 2)
                        {
                            boss.bossHP -= 30;
                        }

                        if (boss.bossHP <= 0)
                        {
                            gameStatus = false;

                            // создаем таймер
                            // AddTimer(RestartGame, 2000);
                            Broadcast("EndGame", true);



                        }
                        else
                        {
                            Broadcast("Fire", player.ConnectUserId, boss.bossHP);
                        }

                    }
                        
                    

                    break;

                case "GetDamage":

                    player.hp -= 25;
                    Broadcast("GetDamage", player.ConnectUserId, player.hp);

                    if (player.hp <= 0)
                    {
                        player.Disconnect();
                    }

                        break;




            }


        }
    }
}