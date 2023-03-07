using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System.IO;


public class GameLogic : MonoBehaviour
{
    LinkedList<int> connectedClientIDs;

    public LinkedList<GameRoom> gameRoomList;
    public LinkedList<PlayerAccount> playerAccountList;
    public int waitingPlayerId = -1;
    public int playerTurn = 1;

    public int bothPlayerReady = 0;

    public class GameRoom
    {
        public string roomName;
        public int player1Id = 1, player2Id, spectator = 0;
        public int bothPlayersReady = 0;
        public bool gameRunning = false;

        public GameRoom(int Player1Id, int Player2Id, int Spectator, string RoomName, bool GameRunning, int BothPlayersReady)
        {
            player1Id = Player1Id;
            player2Id = Player2Id;
            spectator = Spectator;
            roomName = RoomName;
            gameRunning = GameRunning;
            bothPlayersReady = BothPlayersReady;
        }
    }

    public class PlayerAccount
    {
        public string playerName, playerPassword, playerId;

        public PlayerAccount(string PlayerName, string PlayerPassword, string PlayerId)
        {
            playerName = PlayerName;
            playerPassword = PlayerPassword;
            playerId = PlayerId;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkedServerProcessing.SetGameLogic(this);

        connectedClientIDs = new LinkedList<int>();

        gameRoomList = new LinkedList<GameRoom>();

        playerAccountList = new LinkedList<PlayerAccount>();
    }

    public void PlayAsGuestLogin(string[] player, int id)
    {
        PlayerAccount playerAcc = new PlayerAccount("Test Username", "Test Password", id.ToString());
        playerAccountList.AddLast(playerAcc);

        if (true)
        {
            NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.PlayAsGuestAccount + ",", id);
        }
       

    }

    public void CreateLoginAccount(string[] usernamePassword, int id)
    {
        Debug.Log("Creating account... username: " + usernamePassword[1] + " Email: " + usernamePassword[2] + " Password: " + usernamePassword[4]);

        DirectoryInfo[] cDirs = new DirectoryInfo(@"c:\").GetDirectories();

        string line;
        List<string> AccountArray = new List<string>();

        bool flag = false;
        using (StreamReader sr = new StreamReader("PlayerAccounts.txt"))
        {

            while ((line = sr.ReadLine()) != null)
            {
                AccountArray.Add(line);
            }

        }
        using (StreamWriter sw = new StreamWriter("PlayerAccounts.txt"))
        {
            for(int x = 0; x < AccountArray.Count; x++)
            {
                sw.WriteLine(AccountArray[x]);
                string[] temp = AccountArray[x].Split(',');

                if (temp[1] == usernamePassword[2])
                {
                    Debug.Log(temp[1] + " " + usernamePassword[2]);
                    flag = true;
                }
            }
            if (!flag)
            {
                sw.WriteLine(usernamePassword[1] + "," + usernamePassword[2] + "," + usernamePassword[4]);

            }
            else
            {
                Debug.Log("try again");
                flag = false;
            }
        }
           
         PlayerAccount player = new PlayerAccount(usernamePassword[1], usernamePassword[2], id.ToString());
         playerAccountList.AddLast(player);
 

    }

    public void LoginAccount(string[] usernamePassword, int id)
    {
        string line;
        string[] AccountArray;

        using (StreamReader sr = new StreamReader("PlayerAccounts.txt"))
        {

            while ((line = sr.ReadLine()) != null)
            {

                AccountArray = line.Split(',');
                
                if (AccountArray[0] == usernamePassword[1] && AccountArray[2] == usernamePassword[2])
                {
                    NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.LoginSuccessfull + ",", id);
                }
                else
                {
                    Debug.Log("Failed");

                    NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.LoginFailed + ",", id);

                }
            }
        }
    }

    public void CreateRoomSevrer(string[] player, int id)
    {
        bool flag = false;
        foreach (GameRoom x in gameRoomList)
        { 
            if(x.roomName == player[1])
            {
                flag = true;
            }
        }

        if (!flag)
        {
            //must check if name is already taken or it will break 
            if (waitingPlayerId == -1)
            {
                waitingPlayerId = id;
                Debug.Log("waitingPlayerId: " + waitingPlayerId);

                GameRoom gameRoom = new GameRoom(waitingPlayerId, -1, -1, player[1], false, 0);
                gameRoomList.AddLast(gameRoom);

                waitingPlayerId = -1;
            }
            else
            {
                if (gameRoomList.Count <= 0)
                {

                    Debug.Log("gameRoomList.Count <= 0 " + gameRoomList.Count);
                    GameRoom gameRoom = new GameRoom(waitingPlayerId, id, -1, player[1], false, 0);
                    gameRoomList.AddLast(gameRoom);
                }
                else
                {
                    foreach (GameRoom x in gameRoomList)
                    {
                        if (x.roomName != player[1])
                        {
                            Debug.Log("Making another gameroom");

                            GameRoom gameRoom = new GameRoom(waitingPlayerId, id, -1, player[1], false, 0);
                            gameRoomList.AddLast(gameRoom);
                        }
                        else
                        {
                            Debug.Log("Making gameroom");
                        }
                    }
                }
            }
        }
    }

    public void JoinRoomSevrer(string[] player, int id)
    {
        Debug.Log("Check room");

        foreach (GameRoom x in gameRoomList)
        {
            Debug.Log("game players: " + x.player1Id + "game players: " + x.player2Id + "room name: " + player[1]);

            if(x.roomName == player[1])
            {
                Debug.Log("Room has already been made. connceting you to room");
                x.gameRunning = true;
                //x.player1Id = waitingPlayerId;
                x.player2Id = id;

                NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.EnterVersusGameMode + ",", x.player1Id);
                NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.EnterVersusGameMode + ",", x.player2Id);
                Debug.Log("EnterVersusGameMode");
            }
        }
    }

    public void SncPlayerData(string[] player, int id)
    {
        Debug.Log("Called");
        foreach (GameRoom x in gameRoomList)
        {
            if (x.player1Id == id)
            {
                NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.SendPlayerData + "," + player[1] + "," + player[2] + "," + x.player1Id + "," + x.player2Id , x.player2Id);//2 id is player 2
                Debug.Log("Player: " + x.player1Id + " Player: " + x.player2Id);
            }
            else if (x.player2Id == id)
            {
                NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.SendPlayerData + "," + player[1] + "," + player[2] + "," + x.player2Id + "," + x.player1Id, x.player1Id);//2 id is player 2
                Debug.Log("Player: " + x.player1Id + " Player: " + x.player2Id);
            }
        } 
    }

    public void CheckIfPlayerAreReady(string[] player, int id)
    {
        foreach(GameRoom x in gameRoomList)
        {
            if(x.player1Id == id || x.player2Id == id)
            {
                Debug.Log("Player " + id + " is ready");
                x.bothPlayersReady += 1;

                if(x.bothPlayersReady >= 2)
                {
                    NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.CheckIfPlayerIsReady + ",", x.player1Id);//2 id is player 2
                    NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.CheckIfPlayerIsReady + ",", x.player2Id);//2 id is player 
                    x.bothPlayersReady = 0;
                }
            }
        }
    }

    public void RemovePlayerCreateRoomSevrer(int id)
    {
        foreach (GameRoom x in gameRoomList)
        {
            if (x.player1Id == id)
            {
                Debug.Log("player1Id was removed: " + x.player1Id);
                x.player1Id = 0;
                x.player2Id = 0;

            }
            if (x.player2Id == id)
            {
                Debug.Log("player2Id was removed: " + x.player2Id);
                x.player2Id = 0;
                x.player1Id = 0;
            }
            Debug.Log(x.player1Id + " : " + x.player2Id);
        }
    }

    public void AddConnectedClinet(int clientID)
    {
        foreach (GameRoom br in gameRoomList)
        {
            //string msg = br.Deserialize();
            //NetworkedServerProcessing.SendMessageToClient(msg, clientID);
        }
        connectedClientIDs.AddLast(clientID);
    }

    public void RemoveConnectedClinet(int clientID)
    {
        foreach (GameRoom x in gameRoomList)
        {
            if (x.player1Id == clientID)
            {
                NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.SendPlayerToMainMenu + ",", x.player2Id);//2 id is player 2
                x.roomName = "";
            }
            else if (x.player2Id == clientID)
            {
                NetworkedServerProcessing.SendMessageToClient(ServerToClientSignifiers.SendPlayerToMainMenu + ",", x.player1Id);
                x.roomName = "";
            }
        }

            connectedClientIDs.Remove(clientID);
    }

    public string Deserialize()
    {
        //return ServerToClientSignifiers.BalloonSpawned + "," + xPosPercent + "," + yPosPercent + "," + id;
        return null;
    }


}
