using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{

    GameObject submitButton, userNameInput, passwordInput, createToggle, loginToggle;
    GameObject textNameInfo, textPassordInfo;

    GameObject joinGameRoomButton, viewReplayButton, refreshRoomsButton;
    GameObject gameRoomPanel;
    List<GameObject> gameRoomButtonList = new List<GameObject>();

    GameObject tictactoeBoard;
    List<GameObject> tictactoeSquareButtonList = new List<GameObject>();

    GameObject gameOverText, returnToMenuButton, backToMenuButton;

    GameObject textHistory, chatPanel;
    GameObject greetButton, ggButton, niceButton, oopsButton;
    GameObject inputMessageField, sendButton;

    GameObject replayStepsPanel, saveReplayButton, gotoReplayButton, replayDropDown;

    GameObject networkedClient, replaySystemManager;

    public int OurTeam;
    public GameObject messagePrefab;
    public GameObject gameRoomPrefab;

    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.name == "UserNameInputField")
                userNameInput = go;
            else if (go.name == "PasswordInputField")
                passwordInput = go;
            else if (go.name == "SubmitButton")
                submitButton = go;
            else if (go.name == "CreateToggle")
                createToggle = go;
            else if (go.name == "LoginToggle")
                loginToggle = go;
            else if (go.name == "Client")
                networkedClient = go;
            else if (go.name == "JoinGameRoomButton")
                joinGameRoomButton = go;
            else if (go.name == "TextNameInfo")
                textNameInfo = go;
            else if (go.name == "TextPassInfo")
                textPassordInfo = go;
            else if (go.name == "TicTacToeBoard")
                tictactoeBoard = go;
            else if (go.name == "GameOverText")
                gameOverText = go;
            else if (go.name == "ReturnToMenuButton")
                returnToMenuButton = go;
            else if (go.name == "BackToMenuButton")
                backToMenuButton = go;
            else if (go.name == "GoToReplayButton")
                gotoReplayButton = go;
            else if (go.name == "TextHistory")
                textHistory = go;
            else if (go.name == "ChatPanel")
                chatPanel = go;
            else if (go.name == "GreetButton")
                greetButton = go;
            else if (go.name == "GGButton")
                ggButton = go;
            else if (go.name == "NiceButton")
                niceButton = go;
            else if (go.name == "OopsButton")
                oopsButton = go;
            else if (go.name == "InputMessage")
                inputMessageField = go;
            else if (go.name == "SendButton")
                sendButton = go;
            else if (go.name == "SaveReplayButton")
                saveReplayButton = go;
            else if (go.name == "ReplayPanel")
                replayStepsPanel = go;
            else if (go.name == "ReplayDropDown")
                replayDropDown = go;
            else if (go.name == "ViewReplayButton")
                viewReplayButton = go;
            else if (go.name == "RefreshGameRoomsButton")
                refreshRoomsButton = go;
            else if (go.name == "GameRoomPanel")
                gameRoomPanel = go;
            else if (go.GetComponent<ReplaySystemManager>() != null)
                replaySystemManager = go;
        }

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        loginToggle.GetComponent<Toggle>().onValueChanged.AddListener(LoginToggleChanged);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(CreateToggleChanged);
        joinGameRoomButton.GetComponent<Button>().onClick.AddListener(JoinGameRoomButtonPressed);
        returnToMenuButton.GetComponent<Button>().onClick.AddListener(GoToMenuButtonPressed);
        backToMenuButton.GetComponent<Button>().onClick.AddListener(GoToMenuButtonPressed);
        gotoReplayButton.GetComponent<Button>().onClick.AddListener(GoToReplayButtonPressed);
        saveReplayButton.GetComponent<Button>().onClick.AddListener(SaveReplayButtonPressed);

        greetButton.GetComponent<Button>().onClick.AddListener(GreetButtonPressed);
        ggButton.GetComponent<Button>().onClick.AddListener(GGButtonPressed);
        niceButton.GetComponent<Button>().onClick.AddListener(NiceButtonPressed);
        oopsButton.GetComponent<Button>().onClick.AddListener(OopsButtonPressed);
        sendButton.GetComponent<Button>().onClick.AddListener(SendButtonPressed);

        viewReplayButton.GetComponent<Button>().onClick.AddListener(ViewReplaysButtonPressed);
        refreshRoomsButton.GetComponent<Button>().onClick.AddListener(AskForRooms);

        for (int i = 0; i < tictactoeBoard.transform.childCount; i++)
        {
            int index = i;
            tictactoeSquareButtonList.Add(tictactoeBoard.transform.GetChild(index).gameObject);
            tictactoeSquareButtonList[i].GetComponent<Button>().onClick.AddListener(delegate { TicTacToeSquareButtonPressed(index); } );
        }

        ChangeState(GameStates.LoginMenu);
    }

    public void SubmitButtonPressed()
    {

        string n = userNameInput.GetComponent<InputField>().text;
        string p = passwordInput.GetComponent<InputField>().text;

        string msg;

        if(createToggle.GetComponent<Toggle>().isOn)
            msg = ClientToServerSignifiers.CreateAccount + "," + n + "," + p;
        else
            msg = ClientToServerSignifiers.Login + "," + n + "," + p;

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);

        Debug.Log(msg);
    }

    public void LoginToggleChanged(bool newValue)
    {
        Debug.Log("Login Changed!");
        createToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    public void CreateToggleChanged(bool newValue)
    {
        Debug.Log("Create Changed!");
        loginToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    public void ChangeState(int newState)
    {
        joinGameRoomButton.SetActive(false);
        viewReplayButton.SetActive(false);
        refreshRoomsButton.SetActive(false);
        gameRoomPanel.SetActive(false);

        submitButton.SetActive(false);
        userNameInput.SetActive(false);
        passwordInput.SetActive(false);
        createToggle.SetActive(false);
        loginToggle.SetActive(false);

        textNameInfo.SetActive(false);
        textPassordInfo.SetActive(false);

        gameOverText.SetActive(false);
        gotoReplayButton.SetActive(false);
        backToMenuButton.SetActive(false);
        returnToMenuButton.SetActive(false);

        textHistory.SetActive(false);
        chatPanel.SetActive(false);
        greetButton.SetActive(false);
        ggButton.SetActive(false);
        niceButton.SetActive(false);
        oopsButton.SetActive(false);

        saveReplayButton.SetActive(false);
        replayStepsPanel.SetActive(false);
        replayDropDown.SetActive(false);

        // tictactoeSquareButton.SetActive(false);
        foreach (var square in tictactoeSquareButtonList)
        {
            square.SetActive(false);
        }

        if (newState == GameStates.LoginMenu)
        {
            submitButton.SetActive(true);
            userNameInput.SetActive(true);
            passwordInput.SetActive(true);
            createToggle.SetActive(true);
            loginToggle.SetActive(true);
            textNameInfo.SetActive(true);
            textPassordInfo.SetActive(true);
        }
        else if (newState == GameStates.MainMenu)
        {
            joinGameRoomButton.SetActive(true);
            viewReplayButton.SetActive(true);
            refreshRoomsButton.SetActive(true);
            gameRoomPanel.SetActive(true);

            AskForRooms();
        }
        else if (newState == GameStates.WaitingInQueueForOtherPlayer)
        {

        }
        else if (newState == GameStates.TicTacToe)
        {

            foreach (var square in tictactoeSquareButtonList)
            {
                square.SetActive(true);
            }

            var content = textHistory.transform.GetChild(0).GetChild(0);

            for (int i = content.childCount - 1; i >= 0; i--)
            {
                Destroy(content.GetChild(i).gameObject);
            }

            textHistory.SetActive(true);
            chatPanel.SetActive(true);
            greetButton.SetActive(true);
            ggButton.SetActive(true);
            niceButton.SetActive(true);
            oopsButton.SetActive(true);

            if (OurTeam == TeamSignifier.None)
            {
                returnToMenuButton.SetActive(true);
            }
        }
        else if (newState == GameStates.GameEnd)
        {

            foreach (var square in tictactoeSquareButtonList)
            {
                square.GetComponent<Button>().interactable = false;
            }

            foreach (var square in tictactoeSquareButtonList)
            {
                square.SetActive(true);
            }

            gameOverText.SetActive(true);
            saveReplayButton.SetActive(true);
            returnToMenuButton.SetActive(true);

            textHistory.SetActive(true);
            chatPanel.SetActive(true);
            greetButton.SetActive(true);
            ggButton.SetActive(true);
            niceButton.SetActive(true);
            oopsButton.SetActive(true);
        }
        else if (newState == GameStates.Replay)
        {
            foreach (var square in tictactoeSquareButtonList)
            {
                square.SetActive(true);
            }

            replayStepsPanel.SetActive(true);
            backToMenuButton.SetActive(true);
            replayDropDown.SetActive(true);

            replaySystemManager.GetComponent<ReplaySystemManager>().LoadReplays();

            Dropdown dropdown = replayDropDown.GetComponent<Dropdown>();
            dropdown.options.Clear();
            foreach (string option in replaySystemManager.GetComponent<ReplaySystemManager>().replayNames)
            {
                dropdown.options.Add(new Dropdown.OptionData(option));
            }

            dropdown.value = dropdown.options.Count - 1;
        }
    }

    public void ResetBoard()
    {
        foreach (var square in tictactoeSquareButtonList)
        {
            square.transform.GetChild(0).GetComponent<Text>().text = "";
        }
    }

    public void GoToMenuButtonPressed()
    {
        ResetBoard();

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.LeaveRoom + "");

        ChangeState(GameStates.MainMenu);
    }

    public void PlayAgainButtonPressed()
    {
        ResetBoard();

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.LeaveRoom + "");

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinQueueForGameRoom + "");

        ChangeState(GameStates.WaitingInQueueForOtherPlayer);

    }

    public void JoinGameRoomButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinQueueForGameRoom + "");
        ChangeState(GameStates.WaitingInQueueForOtherPlayer);
    }

    public void AskForRooms()
    {
        var content = gameRoomPanel.transform.GetChild(0).GetChild(0);

        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        gameRoomButtonList.Clear();

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.GetServerList + ",");
    }

    public void CreateRoom(int index, int spectatorCount)
    {
        var content = gameRoomPanel.transform.GetChild(0).GetChild(0);

        GameObject room = Instantiate(gameRoomPrefab);
        room.transform.SetParent(content);
        var text = room.transform.GetChild(0).GetComponent<Text>();
        gameRoomButtonList.Add(room);

        text.text = "Game Room " + (index + 1);

        text.text += " | Watching: " + spectatorCount;

        var spectateButton = room.transform.GetChild(1).GetComponent<Button>();
        spectateButton.onClick.AddListener(delegate { JoinRoomAsObserver(index); } );
    }

    private void JoinRoomAsObserver(int index)
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.SpectateGame + "," + index);
    }

    public void GoToReplayButtonPressed()
    {
        foreach (var square in tictactoeSquareButtonList)
        {
            square.GetComponent<Button>().interactable = false;
        }

        ResetBoard();

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.LeaveRoom + "");

        replaySystemManager.GetComponent<ReplaySystemManager>().LoadReplayInformation(replaySystemManager.GetComponent<ReplaySystemManager>().lastIndexUsed);

        ChangeState(GameStates.Replay);
    }
    
    public void ViewReplaysButtonPressed()
    {
        foreach (var square in tictactoeSquareButtonList)
        {
            square.GetComponent<Button>().interactable = false;
        }

        ResetBoard();

        replaySystemManager.GetComponent<ReplaySystemManager>().LoadReplayInformation(replaySystemManager.GetComponent<ReplaySystemManager>().lastIndexUsed);

        ChangeState(GameStates.Replay);
    }

    public void SaveReplayButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.RequestReplay + "");

        saveReplayButton.SetActive(false);
        gotoReplayButton.SetActive(true);
    }

    public void SetWinLoss(int winLoss)
    {
        if (winLoss == WinStates.OsWin)
        {
            gameOverText.GetComponent<Text>().text = "Team O Wins";
        }
        else if (winLoss == WinStates.XsWin)
        {
            gameOverText.GetComponent<Text>().text = "Team X Wins";
        }
        else if (winLoss == WinStates.Tie)
        {
            gameOverText.GetComponent<Text>().text = "It's a Tie.";
        }
    }

    public void SetTurn(int turn)
    {
        if (OurTeam == TeamSignifier.None)
        {
            foreach (var square in tictactoeSquareButtonList)
            {
                square.GetComponent<Button>().interactable = false;
            }

            return;
        }

        if (turn == TurnSignifier.MyTurn)
        {
            foreach (var square in tictactoeSquareButtonList)
            {
                if (square.transform.GetChild(0).GetComponent<Text>().text == "")
                    square.GetComponent<Button>().interactable = true;
            }
        }
        else if (turn == TurnSignifier.TheirTurn)
        {
            foreach (var square in tictactoeSquareButtonList)
            {
                square.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void SetOpponentPlay(int index, int team)
    {
        if (team == TeamSignifier.O)
            tictactoeSquareButtonList[index].transform.GetChild(0).GetComponent<Text>().text = "O";
        if (team == TeamSignifier.X)
            tictactoeSquareButtonList[index].transform.GetChild(0).GetComponent<Text>().text = "X";
    }

    public void TicTacToeSquareButtonPressed(int index)
    {
        SetTurn(TurnSignifier.TheirTurn);

        if (OurTeam == TeamSignifier.O)
            tictactoeSquareButtonList[index].transform.GetChild(0).GetComponent<Text>().text = "O";
        if (OurTeam == TeamSignifier.X)
            tictactoeSquareButtonList[index].transform.GetChild(0).GetComponent<Text>().text = "X";

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TicTacToePlay + "," + index);
    }

    public void SendButtonPressed()
    {
        var inputField = inputMessageField.GetComponent<InputField>();
        var text = inputField.text;

        SendTextMessage(text);

        inputField.text = "";
    }

    public void GreetButtonPressed()
    {
        SendTextMessage("Hello!");
    }

    public void GGButtonPressed()
    {
        SendTextMessage("Good Game!");
    }

    public void NiceButtonPressed()
    {
        SendTextMessage("Nice one!");
    }

    public void OopsButtonPressed()
    {
        SendTextMessage("Oops!");
    }

    public void SendTextMessage(string msg)
    {
        if (msg != "")
        {
            var csv = msg.Split(',');
            string newMsg = "";

            foreach (var str in csv)
            {
                newMsg += str + " ";
            }

            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TextMessage + "," + newMsg);
        }
    }

    public void DisplayMessage(string msg)
    {
        var content = textHistory.transform.GetChild(0).GetChild(0);
        var scrollbar = textHistory.transform.GetChild(1).GetComponent<Scrollbar>();

        GameObject text = Instantiate(messagePrefab);
        text.GetComponent<Text>().text = msg;
        text.transform.SetParent(content);

        scrollbar.value = 0;
    }
}

public class NameAndIndex
{
    public string name;
    public int index;

    public NameAndIndex(int Index, string Name)
    {
        index = Index;
        name = Name;
    }
}

static public class ReplayReadSignifier
{
    public const int LastUsedIndexSignifier = 1;
    public const int IndexAndNameSignifier = 2;
}

static public class TurnSignifier
{
    public const int MyTurn = 0;
    public const int TheirTurn = 1;
    public const int Observer = 2;
}
static public class GameStates
{
    public const int LoginMenu = 1;
    public const int MainMenu = 2;
    public const int WaitingInQueueForOtherPlayer = 3;

    public const int TicTacToe = 4;

    public const int GameEnd = 5;

    public const int Replay = 6;
}