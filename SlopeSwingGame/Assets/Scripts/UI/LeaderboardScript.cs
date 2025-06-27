using System;
using TMPro;
using UnityEngine;

public class LeaderboardScript : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardPanel;
    
    [SerializeField] private GameObject HolesParent;
    [SerializeField] private GameObject ParParent;
    [SerializeField] private GameObject PlayerScoreParent;

    [SerializeField] private GameObject PlayerScorePrefab;
    [SerializeField] private GameObject scorePrefab;
    
    private GameObject[,] playerScores; 
    private CourseScript courseScript;
    private TextMeshProUGUI[] currentHoleTexts;
    
    private int currentHole = 0;
    private int numberOfPlayers = 0;
    private int numberOfHoles = 0;
    public void SetCurrentHole(int hole) {currentHole = hole;}

    public void LoadLeaderboard(CourseScript script)
    {
        // Find the CourseScript component in the scene
        courseScript = script;
        // Initialize the player scores array based on the number of players and holes
        numberOfPlayers = courseScript.players.Count; 
        numberOfHoles = courseScript.holeSpawnPoints.Count; 
        playerScores = new GameObject[numberOfPlayers, numberOfHoles];
        
        DynamicLeaderboardUIStart(ref PlayerScorePrefab, scorePrefab);
        DynamicLeaderboardUIStart(ref HolesParent, scorePrefab);
        DynamicLeaderboardUIStart(ref ParParent, scorePrefab);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            GameObject scoreObject = (i > 1) switch
            {
                false => PlayerScorePrefab,
                true => PlayerScorePrefab//Instantiate(PlayerScorePrefab, PlayerScoreParent.transform)
            };
            for (int j = 0; j < numberOfHoles; j++)
            {
                try
                {
                    playerScores[i, j] = scoreObject.transform.GetChild(j + 1).gameObject; // +1 to skip the first child which is the player name
                    playerScores[i, j].GetComponent<TextMeshProUGUI>().text = "0"; // Initialize with 0
                }
                catch (Exception e)
                {
                    Instantiate(scoreObject, scoreObject.transform);
                    playerScores[i, j] = scoreObject.transform.GetChild(j + 1).gameObject; // +1 to skip the first child which is the player name and filler
                }
            }
        }
        
        currentHoleTexts = new TextMeshProUGUI[courseScript.players.Count];
        
        var parHoles = courseScript.holeParList;
        var parTexts = ParParent.GetComponentsInChildren<TextMeshProUGUI>();
        var pars = courseScript.holeParList.Count;
        
        // Fill the par list
        for (int i = 1; i < pars + 1; i++)
        {
            parTexts[i].text = parHoles[i - 1].ToString();
        }
        
        NextHoleUpdate();
    }

    public void NextHoleUpdate()
    {
        currentHoleTexts[0] = playerScores[0, currentHole].GetComponent<TextMeshProUGUI>();
        currentHoleTexts[0].text = 0.ToString();
    }
    
    public void NextHoleUpdateMultiPlayer()
    {
        for (int i = 0; i < courseScript.players.Count; i++)
        {
            currentHoleTexts[i] = playerScores[i, currentHole].GetComponent<TextMeshProUGUI>();
        }
    }
    
    public void UpdateLeaderboard(int stroke)
    {
        currentHoleTexts[0].text = stroke.ToString();
    }

    public void UpdateLeaderboard(int player, int stroke)
    {
        currentHoleTexts[player].text = stroke.ToString();
    }
    
    private void DynamicLeaderboardUIStart(ref GameObject uiObject, GameObject prefab)
    {
        // Dynamically create the leaderboard UI based on the number of players and holes
        switch (numberOfHoles)
        {
            case < 7:
                var holeDifference = 7 - numberOfHoles;
                for (int i = 0; i < holeDifference; i++)
                {
                    Destroy(uiObject.transform.GetChild(uiObject.transform.childCount - 1 - i).gameObject);
                }
                break;
            case 7:
                break;
            case > 7:
                var holeDifference2 = numberOfHoles - 7;
                for (int i = 0; i < holeDifference2; i++)
                {
                    Instantiate(prefab, uiObject.transform);
                }
                break;
        }
    }
}
