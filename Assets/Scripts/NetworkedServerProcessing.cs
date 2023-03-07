using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkedServerProcessing
{
    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int id)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        string[] temp = msg.Split(',');
        int signifierID = int.Parse(temp[0]);

        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        //---------------------------------------------------------------------------------------------------
        if(signifierID == ClientToServerSignifiers.PlayAsGuestAccount)
        {
            gameLogic.PlayAsGuestLogin(temp, id);
        }

        if (signifierID == ClientToServerSignifiers.CreatePlayerAccount)
        {
            gameLogic.CreateLoginAccount(temp, id);
        }

        if (signifierID == ClientToServerSignifiers.Login)
        {
            gameLogic.LoginAccount(temp, id);
        }

        if (signifierID == ClientToServerSignifiers.CreatePrivateLobby)
        {
            gameLogic.CreateRoomSevrer(temp, id);
        }

        if (signifierID == ClientToServerSignifiers.JoinPrivateRoom)
        {
            gameLogic.JoinRoomSevrer(temp, id);
        }

        if (signifierID == ClientToServerSignifiers.SendPlayerData)
        {
            gameLogic.SncPlayerData(temp, id);
        }

        if (signifierID == ClientToServerSignifiers.CheckIfPlayerIsReady)
        {
            gameLogic.CheckIfPlayerAreReady(temp, id);
        }


        //---------------------------------------------------------------------------------------------------
    }

    static public void SendMessageToClient(string msg, int clientConnectionID)
    {
        networkedServer.SendMessageToClient(msg, clientConnectionID);
    }

    #endregion

    #region Connection Events

    static public void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Connection, ID == " + clientConnectionID);
        gameLogic.AddConnectedClinet(clientConnectionID);
    }
    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Disconnection, ID == " + clientConnectionID);
        gameLogic.RemoveConnectedClinet(clientConnectionID);
        gameLogic.RemovePlayerCreateRoomSevrer(clientConnectionID);
    }

    #endregion

    #region Setup
    static NetworkedServer networkedServer;
    static GameLogic gameLogic;

    static public void SetNetworkedServer(NetworkedServer NetworkedServer)
    {
        networkedServer = NetworkedServer;
    }
    static public NetworkedServer GetNetworkedServer()
    {
        return networkedServer;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion
}

#region Protocol Signifiers
public static class ClientToServerSignifiers
{
    public const int PlayAsGuestAccount = 0;
    public const int CreatePlayerAccount = 1;
    public const int Login = 2;
    public const int LoginFailed = 3;
    public const int LoginSuccessfull = 4;
    public const int CreatePrivateLobby = 5;
    public const int EnterVersusGameMode = 6;
    public const int JoinPrivateRoom = 7;

    public const int SendPlayerData = 8;
    public const int CheckIfPlayerIsReady = 9;
    public const int SendPlayerToMainMenu = 10;
}

public static class ServerToClientSignifiers
{
    public const int PlayAsGuestAccount = 0;
    public const int CreatePlayerAccount = 1;
    public const int Login = 2;
    public const int LoginFailed = 3;
    public const int LoginSuccessfull = 4;
    public const int CreatePrivateLobby = 5;
    public const int EnterVersusGameMode = 6;
    public const int JoinPrivateRoom = 7;

    public const int SendPlayerData = 8;
    public const int CheckIfPlayerIsReady = 9;
    public const int SendPlayerToMainMenu = 10;


}

#endregion

