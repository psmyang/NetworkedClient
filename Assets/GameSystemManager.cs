using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject inputFieldUserName, inputFieldPassword, buttonSubmit, toggleLogin, toggleCreate;

    GameObject infoText1, infoText2;

    GameObject joinGameRoomButton;
    

    GameObject winText, returnButton, backButton, refreshButton;

    GameObject textHistory, chatPanel;
    GameObject helloButton, niceButton;
    GameObject inputMessageField, sendButton;

    GameObject gameRoom;
    List<GameObject> gameRoomButtonList = new List<GameObject>();

    GameObject replayStepsPanel, saveButton, playAgainButton, watchReplayButton;

    GameObject testBoard;
    List<GameObject> testButtonList = new List<GameObject>();

    GameObject networkedClient, replayManager;

    public int OurTeam;

    public GameObject TextMessage;
    public GameObject gameRoomPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.name == "InputFieldUserName")
                inputFieldUserName = go;
            else if (go.name == "InputFieldPassword")
                inputFieldPassword = go;
            else if (go.name == "ButtonSubmit")
                buttonSubmit = go;
            else if (go.name == "ToggleLogin")
                toggleLogin = go;
            else if (go.name == "ToggleCreate")
                toggleCreate = go;
            else if (go.name == "NetworkedClient")
                networkedClient = go;
            else if (go.name == "JoinGameRoomButton")
                joinGameRoomButton = go;
            else if (go.name == "InfoText1")
                infoText1 = go;
            else if (go.name == "InfoText2")
                infoText2 = go;
            else if (go.name == "TestBoard")
                testBoard = go;
            else if (go.name == "WinText")
                winText = go;
            else if (go.name == "WatchReplayButton")
                watchReplayButton = go;
            else if (go.name == "TextHistory")
                textHistory = go;
            else if (go.name == "ChatPanel")
                chatPanel = go;
            else if (go.name == "HelloButton")
                helloButton = go;
            else if (go.name == "NiceButton")
                niceButton = go;
            else if (go.name == "InputMessage")
                inputMessageField = go;
            else if (go.name == "SendButton")
                sendButton = go;
            else if (go.name == "SaveButton")
                saveButton = go;
            else if (go.name == "ReplayPanel")
                replayStepsPanel = go;
            else if (go.GetComponent<ReplayManager>() != null)
                replayManager = go;
            else if (go.name == "ReturnButton")
                returnButton = go;
            else if (go.name == "BackButton")
                backButton = go;
            else if (go.name == "RefreshButton")
                refreshButton = go;
            else if (go.name == "GameRoom")
                gameRoom = go;
        }

        buttonSubmit.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChanged);
        toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChanged);
        joinGameRoomButton.GetComponent<Button>().onClick.AddListener(JoinGameRoomButtonPressed);       
        //watchReplayButton.GetComponent<Button>().onClick.AddListener(WatchReplay);
        helloButton.GetComponent<Button>().onClick.AddListener(HelloButtonPressed);
        niceButton.GetComponent<Button>().onClick.AddListener(NiceButtonPressed);
        sendButton.GetComponent<Button>().onClick.AddListener(SendButtonPressed);
        saveButton.GetComponent<Button>().onClick.AddListener(SaveButtonPressed);
        returnButton.GetComponent<Button>().onClick.AddListener(MenuButtonPressed);
        backButton.GetComponent<Button>().onClick.AddListener(MenuButtonPressed);
        refreshButton.GetComponent<Button>().onClick.AddListener(AskForRooms);

        for (int i = 0; i < testBoard.transform.childCount; i++)
        {
            int index = i;
            testButtonList.Add(testBoard.transform.GetChild(index).gameObject);
            testButtonList[i].GetComponent<Button>().onClick.AddListener(delegate { TestButtonPressed(index); });
        }

        ChangeState(GameStates.LoginMenu);
    }

    public void SubmitButtonPressed()
    {
        string n = inputFieldUserName.GetComponent<InputField>().text;
        string p = inputFieldPassword.GetComponent<InputField>().text;

        if (toggleLogin.GetComponent<Toggle>().isOn)
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.Login + "," + n + "," + p);
        else
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.CreateAccount + "," + n + "," + p);

    }

    public void ToggleCreateValueChanged(bool newValue)
    {
        toggleLogin.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    public void ToggleLoginValueChanged(bool newValue)
    {
        toggleCreate.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    public void ChangeState(int newState)
    {
        joinGameRoomButton.SetActive(false);
        buttonSubmit.SetActive(false);
        inputFieldUserName.SetActive(false);
        inputFieldPassword.SetActive(false);
        toggleCreate.SetActive(false);
        toggleLogin.SetActive(false);
        infoText1.SetActive(false);
        infoText2.SetActive(false);
        winText.SetActive(false);
        watchReplayButton.SetActive(false);
        textHistory.SetActive(false);
        chatPanel.SetActive(false);
        helloButton.SetActive(false);
        niceButton.SetActive(false);
        saveButton.SetActive(false);
        replayStepsPanel.SetActive(false);
        backButton.SetActive(false);
        returnButton.SetActive(false);
        refreshButton.SetActive(false);
        gameRoom.SetActive(false);

        foreach (var square in testButtonList)
        {
            square.SetActive(false);
        }

        if (newState == GameStates.LoginMenu)
        {
            buttonSubmit.SetActive(true);
            inputFieldUserName.SetActive(true);
            inputFieldPassword.SetActive(true);
            toggleCreate.SetActive(true);
            toggleLogin.SetActive(true);
            infoText1.SetActive(true);
            infoText2.SetActive(true);
        }
        else if (newState == GameStates.MainMenu)
        {
            joinGameRoomButton.SetActive(true);
            refreshButton.SetActive(true);
            gameRoom.SetActive(true);

            AskForRooms();
        }
        else if (newState == GameStates.WaitingInQueueForOtherPlayer)
        {

        }
        else if (newState == GameStates.Test)
        {
            foreach (var square in testButtonList)
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
            helloButton.SetActive(true);
            niceButton.SetActive(true);

            if (OurTeam == TeamSignifier.None)
            {
                returnButton.SetActive(true);
            }
        }
        else if (newState == GameStates.GameEnd)
        {
            foreach (var square in testButtonList)
            {
                square.GetComponent<Button>().interactable = false;
            }

            foreach (var square in testButtonList)
            {
                square.SetActive(true);
            }

            winText.SetActive(true);
            saveButton.SetActive(true);           
            textHistory.SetActive(true);
            chatPanel.SetActive(true);
            helloButton.SetActive(true);
            niceButton.SetActive(true);
            returnButton.SetActive(true);
        }
        else if (newState == GameStates.Replay)
        {
            foreach (var square in testButtonList)
            {
                square.SetActive(true);
            }

            replayStepsPanel.SetActive(true);
            backButton.SetActive(true);
        }
    }

    public void ResetBoard()
    {
        foreach (var square in testButtonList)
        {
            square.transform.GetChild(0).GetComponent<Text>().text = "";
        }
    }

    public void WatchReplay()
    {
        foreach (var square in testButtonList)
        {
            square.GetComponent<Button>().interactable = false;
        }

        ResetBoard();

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.LeaveRoom + "");

        replayManager.GetComponent<ReplayManager>().LoadReplayInformation();

        ChangeState(GameStates.Replay);
    }

    public void SaveButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.RequestReplay + "");

        saveButton.SetActive(false);
        watchReplayButton.SetActive(true);
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


    public void SetWinLoss(int winLoss)
    {
        if (winLoss == WinStates.Win)
        {
            winText.GetComponent<Text>().text = "You Win!";
        }
        else if (winLoss == WinStates.Loss)
        {
            winText.GetComponent<Text>().text = "You Loss";
        }
        else if (winLoss == WinStates.Tie)
        {
            winText.GetComponent<Text>().text = "It's a Tie.";
        }
    }

    public void SetTurn(int turn)
    {
        if (OurTeam == TeamSignifier.None)
        {
            // Disable squares
            foreach (var square in testButtonList)
            {
                square.GetComponent<Button>().interactable = false;
            }

            return;
        }

        if (turn == TurnSignifier.MyTurn)
        {
            // Enable squares
            foreach (var square in testButtonList)
            {
                // Check if there is something in that square
                if (square.transform.GetChild(0).GetComponent<Text>().text == "")
                    square.GetComponent<Button>().interactable = true;
            }
        }
        else if (turn == TurnSignifier.TheirTurn)
        {
            // Disable squares
            foreach (var square in testButtonList)
            {
                square.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void SetOpponentPlay(int index, int team)
    {
        if (team == TeamSignifier.O)
            testButtonList[index].transform.GetChild(0).GetComponent<Text>().text = "O";
        if (team == TeamSignifier.X)
            testButtonList[index].transform.GetChild(0).GetComponent<Text>().text = "X";
    }

    public void TestButtonPressed(int index)
    {
        // Set to not our turn
        SetTurn(TurnSignifier.TheirTurn);

        // Update board
        if (OurTeam == TeamSignifier.O)
            testButtonList[index].transform.GetChild(0).GetComponent<Text>().text = "O";
        if (OurTeam == TeamSignifier.X)
            testButtonList[index].transform.GetChild(0).GetComponent<Text>().text = "X";

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TestPlay + "," + index);
    }

    public void HelloButtonPressed()
    {
        SendTextMessage("Hello!");
    }


    public void NiceButtonPressed()
    {
        SendTextMessage("Nice!");
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


    public void SendButtonPressed()
    {
        var inputField = inputMessageField.GetComponent<InputField>();
        var text = inputField.text;

        SendTextMessage(text);

        inputField.text = "";
    }

    public void DisplayMessage(string msg)
    {
        var content = textHistory.transform.GetChild(0).GetChild(0);
        var scrollbar = textHistory.transform.GetChild(1).GetComponent<Scrollbar>();

        GameObject text = Instantiate(TextMessage);
        text.GetComponent<Text>().text = msg;
        text.transform.SetParent(content);

        scrollbar.value = 0;
    }

    public void MenuButtonPressed()
    {
        ResetBoard();

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.LeaveRoom + "");

        ChangeState(GameStates.MainMenu);
    }
    public void AskForRooms()
    {
        // Remove all rooms from room panel
        var content = gameRoom.transform.GetChild(0).GetChild(0);

        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        gameRoomButtonList.Clear();

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.GetServerList + ",");
    }

    public void CreateRoom(int index, int spectatorCount)
    {
        var content = gameRoom.transform.GetChild(0).GetChild(0);

        GameObject room = Instantiate(gameRoomPrefab);
        room.transform.SetParent(content);
        var text = room.transform.GetChild(0).GetComponent<Text>();
        gameRoomButtonList.Add(room);

        text.text = "Game Room " + (index + 1);

        text.text += " | Watching: " + spectatorCount;

        var spectateButton = room.transform.GetChild(1).GetComponent<Button>();
        spectateButton.onClick.AddListener(delegate { JoinRoomAsObserver(index); });
    }

    private void JoinRoomAsObserver(int index)
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.SpectateGame + "," + index);
    }
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
    public const int Test = 4;
    public const int GameEnd = 5;
    public const int Replay = 6;
}