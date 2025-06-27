using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class CourseScript : MonoBehaviour
{
    public List<GameObject> holeSpawnPoints;
    public List<int> holeParList; // List of par values for each hole, indexed by hole index
    [NonSerialized] public List<GameObject> players;
    private List<GameObject> finishedPlayers = new List<GameObject>();
    private int currentHoleIndex = 0;
    private int finishedPlayersCount;
    [SerializeField] private int playerCount;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int startCardCount = 4; // Number of cards to start with
    [SerializeField] private float baseBallForce = 10f; // Base speed for the ball, can be adjusted as needed
    [SerializeField] private TextMeshProUGUI strokeText; // Reference to the UI text for displaying strokes
    [SerializeField] private int scoreboardDisplayTime = 5; // Time to display the scoreboard after the last player finishes a hole
    [SerializeField] private float timerPerShot = 120f; // Time limit for each hole in seconds
    [SerializeField] private TextMeshProUGUI timerText; // Reference to the UI text for displaying the timer
    [SerializeField] private float courseTimeLimit = 1800f; // Total time limit for the course in seconds
    [SerializeField] private TextMeshProUGUI parTMP;
    //[SerializeField] private int outOfTimeScorePenalty = 10; // Score penalty for running out of time on a hole
    private float shotTimer;
    private float courseTimer;
    private bool isNextHoleTriggered = false; // Flag to prevent multiple triggers of NextHole
    private SwingManager swingManager;
    private PlayerCardManager playerCardManager;
    private GraphManager graphManager;
    private LeaderboardScript leaderboardScript;
    int[,] playerScores;

    public void ResetPlayer(GameObject player)
    {
        // Reset the player's position to the location before the shot
        player.transform.position = swingManager.LastShotPosition;
        player.GetComponent<MathBall>().ResetMove();
        player.GetComponent<MathBall>().SwitchGravity(false);
    }

    public void MadeShot(int playerIndex, bool recycleHand = false)
    {
        int currentStroke = playerScores[playerIndex, currentHoleIndex]++;
        if (strokeText)
        {
            strokeText.text = currentStroke.ToString();
        }

        if (!recycleHand) shotTimer = 0; // Reset the timer for the next shot

        if (graphManager) graphManager.SetPlayerStill(false);
        if (leaderboardScript) leaderboardScript.UpdateLeaderboard(currentStroke);
    }

    public void BallStoppedMoving()
    {
        if (graphManager) graphManager.SetPlayerStill(true);
    }

    public void PlayerFinishedHole(GameObject player)
    {
        int playerIndex = players.IndexOf(player);
        if (playerIndex == -1 && finishedPlayers.Contains(player))
        {
            Debug.LogError("Player already finished the hole.");
            return;
        }
        finishedPlayers.Add(player);

        if (!players[playerIndex].activeSelf)
        {
            Debug.LogError("Player already finished...");
            return;
        }
        
        finishedPlayersCount++;
        players[playerIndex].SetActive(false);
        //playerScores[playerIndex, currentHoleIndex] = // Add the logic to get this score from the player script; 
        
        if (finishedPlayersCount == playerCount && !isNextHoleTriggered) NextHole();
    }

    private void MoveAllPlayersToSpawn(int spawn)
    {
        if (spawn >= holeSpawnPoints.Count)
        {
            return;
        }

        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.position = holeSpawnPoints[spawn].transform.position;
        }
    }

    private void NextHole()
    {
        if (isNextHoleTriggered) return; // Prevent multiple triggers
        isNextHoleTriggered = true; // Set the flag to true to prevent further triggers
        currentHoleIndex++;
        finishedPlayersCount = 0;

        if (currentHoleIndex >= holeSpawnPoints.Count)
        {
            StartCoroutine(ShowScoreboard());
            return;
        }
        
        swingManager.SetLastShotPosition(holeSpawnPoints[currentHoleIndex].transform.position); // Reset the last shot position to the new hole's spawn point
        leaderboardScript.SetCurrentHole(currentHoleIndex);
        leaderboardScript.NextHoleUpdate();

        for (int i = 0; i < playerCount; i++)
        {
            players[i].SetActive(true);
            
            // Reset the player's collider to be enabled and not a trigger
            var collider = players[i].GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.enabled = true;
                collider.isTrigger = false;
            }
            
            players[i].transform.position = holeSpawnPoints[currentHoleIndex].transform.position;
            //playerScores[i, currentHoleIndex] = -1; // Reset the score for the new hole
            players[i].GetComponent<MathBall>().ResetMove();
            MadeShot(i);
        }
        
        // Force Unity to update physics colliders immediately
        Physics.SyncTransforms();
        
        playerCardManager.FlushHand(); 
        playerCardManager.AddCardsToHand(startCardCount);
        shotTimer = 0;
        isNextHoleTriggered = false; // Reset the flag for the next hole
        finishedPlayers.Clear(); // Clear the list of finished players for the next hole
        UpdatePartText();
    }
    
    private IEnumerator ShowScoreboard()
    {
        // Show the scoreboard UI
        // Saving all scores
        for (int p = 0; p < playerCount; p++)
        {
            for (int h = 0; h < holeSpawnPoints.Count; h++)
            {
                PlayerPrefs.SetInt($"Player{p}_Hole{h}", playerScores[p, h]);
            }
        }
        PlayerPrefs.Save();
        
        playerCardManager.SetLeaderboardState(true);

        /*// Loading all scores
        for (int p = 0; p < playerCount; p++)
        {
            for (int h = 0; h < holeCount; h++)
            {
                playerScores[p, h] = PlayerPrefs.GetInt($"Player{p}_Hole{h}", 0);
            }
        }*/
        yield return new WaitForSeconds(scoreboardDisplayTime);
        
        // Change to the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main menu");
    }

    private void UpdatePartText()
    {
        parTMP.text = "Par: " + holeParList[currentHoleIndex].ToString();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScores = new int[playerCount, holeSpawnPoints.Count];
        players = new List<GameObject>();
        swingManager = FindFirstObjectByType<SwingManager>();

        for (int i = 0; i < playerCount; i++)
        {
            players.Add(Instantiate(playerPrefab, holeSpawnPoints[currentHoleIndex].transform.position, Quaternion.identity));
            //players[i].GetComponent<MathBall>().SetBaseShootForce(baseBallForce);
            MadeShot(i); // Initialize stroke count for each player
        }

        Camera.main!.GetComponent<FollowTarget>().SetTarget(players[0].transform);
        playerCardManager = FindFirstObjectByType<PlayerCardManager>();
        playerCardManager.AddCardsToHand(startCardCount);
        graphManager = FindFirstObjectByType<GraphManager>();
        if (graphManager) graphManager.SetPlayer(players[0].transform);
        leaderboardScript = FindFirstObjectByType<LeaderboardScript>(FindObjectsInactive.Include);
        leaderboardScript.LoadLeaderboard(this);

        UpdatePartText();

        //strokeText.text = "Stroke: 0"; // Initialize stroke text
    }

    private void FixedUpdate()
    {
        if (currentHoleIndex >= holeSpawnPoints.Count) return; // If all holes are finished, do nothing
        
        if (courseTimer < courseTimeLimit)
        {
            courseTimer += Time.fixedDeltaTime;
        }
        else
        {
            // If the course time limit is reached, show the scoreboard and reset to main menu
            StartCoroutine(ShowScoreboard());
            return;
        }
        
        if (shotTimer < timerPerShot)
        {
            shotTimer += Time.fixedDeltaTime;
            timerText.text = "Time: " + Mathf.Max(0, Mathf.Ceil(timerPerShot - shotTimer)).ToString(CultureInfo.CurrentCulture);
        }
        else
        {
            shotTimer = 0; // Reset the timer for the next shot
            strokeText.text = playerScores[0, currentHoleIndex]++.ToString(); // Apply penalty to the first player as an example
            /*if (nextHole) return; // If next hole is already triggered, do nothing
            nextHole = true; // Set nextHole to true to prevent multiple triggers
            // If the timer runs out, reset all players to the start of the hole
            for (int i = 0; i < playerCount; i++)
            {
                if (players[i].activeSelf)
                {
                    playerScores[i, currentHoleIndex] += outOfTimeScorePenalty; // Reset the score for this hole
                }
            }
            NextHole();*/
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                MoveAllPlayersToSpawn(currentHoleIndex);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                MoveAllPlayersToSpawn(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                MoveAllPlayersToSpawn(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                MoveAllPlayersToSpawn(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                MoveAllPlayersToSpawn(3);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                MoveAllPlayersToSpawn(4);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                MoveAllPlayersToSpawn(5);
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                MoveAllPlayersToSpawn(6);
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                MoveAllPlayersToSpawn(7);
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                MoveAllPlayersToSpawn(9);
            }
        }
    }
}
