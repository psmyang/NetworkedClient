using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClient : MonoBehaviour
{

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;

    GameObject gameSystemManager;
    GameObject replaySystemManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.GetComponent<GameSystemManager>() != null)
                gameSystemManager = go;
            if (go.GetComponent<ReplaySystemManager>() != null)
                replaySystemManager = go;
        }

        Connect();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }
    
    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "fe80::454b:9621:4ad5:4039%12", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }
    
    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }
    
    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');

        int signifier = int.Parse(csv[0]);

        if (signifier == ServerToClientSignifiers.AccountCreationComplete)
        {
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
        }
        else if (signifier == ServerToClientSignifiers.LoginComplete)
        {
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
        }
        else if (signifier == ServerToClientSignifiers.GameStart)
        {
            Debug.Log("Joined a Game!");
            gameSystemManager.GetComponent<GameSystemManager>().OurTeam = int.Parse(csv[1]);

            gameSystemManager.GetComponent<GameSystemManager>().SetTurn(int.Parse(csv[1]));
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
        }
        else if (signifier == ServerToClientSignifiers.OpponentPlayed)
        {
            Debug.Log("Opponent Played an X or O!");

            var location = int.Parse(csv[1]);
            var team = int.Parse(csv[2]);
            var continuePlay = int.Parse(csv[3]);

            gameSystemManager.GetComponent<GameSystemManager>().SetOpponentPlay(location, team);

            if (continuePlay == WinStates.ContinuePlay)
                gameSystemManager.GetComponent<GameSystemManager>().SetTurn(TurnSignifier.MyTurn);
        }
        else if (signifier == ServerToClientSignifiers.GameOver)
        {
            var outcome = int.Parse(csv[1]);

            Debug.Log("Game is over");

            gameSystemManager.GetComponent<GameSystemManager>().SetWinLoss(outcome);
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.GameEnd);

        }
        else if (signifier == ServerToClientSignifiers.TextMessage)
        {
            gameSystemManager.GetComponent<GameSystemManager>().DisplayMessage(csv[1]);
        }
        else if (signifier == ServerToClientSignifiers.ReplayInformation)
        {
            replaySystemManager.GetComponent<ReplaySystemManager>().SaveReplay(csv[1]);
        }
        else if (signifier == ServerToClientSignifiers.ServerList)
        {
            int roomID = int.Parse(csv[1]);
            int observerCount = int.Parse(csv[2]);

            gameSystemManager.GetComponent<GameSystemManager>().CreateRoom(roomID, observerCount);
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }


}

public static class TeamSignifier
{
    public const int None = -1;
    public const int O = 0;
    public const int X = 1;
}

public static class ClientToServerSignifiers
{
    public const int CreateAccount = 1;
    public const int Login = 2;

    public const int JoinQueueForGameRoom = 3;

    public const int TicTacToePlay = 4;

    public const int LeaveRoom = 5;

    public const int TextMessage = 6;

    public const int RequestReplay = 7;
    public const int GetServerList = 8;
    public const int SpectateGame = 9;
}

public static class ServerToClientSignifiers
{
    public const int LoginComplete = 1;
    public const int LoginFailed = 2;
    public const int AccountCreationComplete = 3;
    public const int AccountCreationFailed = 4;

    public const int OpponentPlayed = 5;
    public const int GameStart = 6;

    public const int GameOver = 7;

    public const int TextMessage = 8;

    public const int ReplayInformation = 9;

    public const int ServerList = 10;
}

public static class WinStates
{
    public const int ContinuePlay = 0;
    public const int OsWin = 1;
    public const int XsWin = 2;
    public const int Tie = 3;
}