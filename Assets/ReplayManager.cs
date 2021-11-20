using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ReplayManager : MonoBehaviour
{
    public GameObject replayStepPrefab;

    public GameObject replayStepsPanel;
    GameObject testBoard;
    List<GameObject> testButtonList = new List<GameObject>();
    List<GameObject> replayStepsButtonList = new List<GameObject>();
    List<string> replayStepBoardStates = new List<string>();

    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.name == "ReplayPanel")
                replayStepsPanel = go;
            else if (go.name == "TestBoard")
                testBoard = go;
        }

        for (int i = 0; i < testBoard.transform.childCount; i++)
        {
            int index = i;
            testButtonList.Add(testBoard.transform.GetChild(index).gameObject);
        }

    }

    public void SaveReplay(string replayInfo)
    {
        string[] steps = replayInfo.Split(';');

        int[] boardState = new int[9];

        for (int i = 0; i < boardState.Length; i++)
        {
            boardState[i] = TeamSignifier.None;
        }

        string name = "TEST";

        StreamWriter sw = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + name + ".txt");

        for (int i = 0; i < steps.Length; i++)
        {
            string[] info = steps[i].Split('.');

            var boardIndex = int.Parse(info[0]);
            var team = int.Parse(info[1]);
            var time = "TIME";

            sw.WriteLine(ReplaySignifiers.MoveInformation + "," + team + "," + time + "," + boardIndex + ",");

            boardState[boardIndex] = team;

            for (int j = 0; j < boardState.Length; j++)
            {
                sw.WriteLine(ReplaySignifiers.BoardState + "," + j + "," + boardState[j]);
            }
        }

        sw.Close();

    }

    public void LoadReplayInformation()
    {
        var content = replayStepsPanel.transform.GetChild(0).GetChild(0);

        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        // Reset the step instructions list
        replayStepBoardStates.Clear();
        replayStepsButtonList.Clear();


        string name = "TEST";

        StreamReader sr = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + name + ".txt");

        string line;
        while ((line = sr.ReadLine()) != null)
        {
            string[] csv = line.Split(',');

            var signifier = int.Parse(csv[0]);

            if (signifier == ReplaySignifiers.MoveInformation)
            {
                var team = int.Parse(csv[1]);
                var time = csv[2];
                var move = csv[3];

                GameObject step = Instantiate(replayStepPrefab);
                step.transform.SetParent(content);
                var text = step.transform.GetChild(0).GetComponent<Text>();
                replayStepsButtonList.Add(step);

                if (team == TeamSignifier.O)
                    text.text = "Team O: ";
                else if (team == TeamSignifier.X)
                    text.text = "Team X: ";

                text.text += move;

                text.text += " - " + time.ToString() + "s";

                replayStepBoardStates.Add("");
            }
            else
            if (signifier == ReplaySignifiers.BoardState)
            {
                var childIndex = replayStepBoardStates.Count - 1;
                var boardIndex = int.Parse(csv[1]);
                var team = int.Parse(csv[2]);

                replayStepBoardStates[childIndex] += boardIndex + "," + team + ",";
            }
        }

        sr.Close();

        for (int i = 0; i < replayStepBoardStates.Count; i++)
        {
            int index = i;
            var replayStep = replayStepsButtonList[index];
            replayStep.GetComponent<Button>().onClick.AddListener(() => LoadReplayStep(index));// delegate { LoadReplayStep(index); });
        }
    }

    public void LoadReplayStep(int index)
    {
        var boardInfo = replayStepBoardStates[index];
        string[] csv = boardInfo.Split(',');

        for (int i = 0; i < csv.Length - 1; i += 2)
        {
            var boardIndex = int.Parse(csv[i]);
            var team = int.Parse(csv[i + 1]);

            if (team == TeamSignifier.O)
                testButtonList[boardIndex].transform.GetChild(0).GetComponent<Text>().text = "O";
            else if (team == TeamSignifier.X)
                testButtonList[boardIndex].transform.GetChild(0).GetComponent<Text>().text = "X";
            else if (team == TeamSignifier.None)
                testButtonList[boardIndex].transform.GetChild(0).GetComponent<Text>().text = "";

        }
    }

}



public static class ReplaySignifiers
{
    public const int MoveInformation = 1;    // Team that played, Time taken, current move
    public const int BoardState = 2;     // Position, Team
}
