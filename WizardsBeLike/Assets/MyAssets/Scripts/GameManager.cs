using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // This namespace is required for SceneManager

using Cinemachine;

public class GameManager : MonoBehaviour
{
    public List<Player> players;
    public TMP_Text turnTimerText; // Reference to the timer text UI
    public TMP_Text playerNameText; // Reference to the player's name text UI
    public float turnDuration = 20f;
    public float turnDelayDuration = 5f;
    public TMP_Text skipTurnMessageText; // Reference to the skip turn message text UI
    public TMP_Text timesUpMessageText; // Reference to the time's up message text UI
    public Transform[] respawnPoints; // Assign these in the Inspector
    public Transform[] spawnPoints; // Assign these in the Inspector



    private int currentPlayerIndex = 0;
    private float turnTimer;
    private bool turnInProgress = false;
    private bool isTurnTransitionInProgress = false;
    private int turnCounter = 0;
    private int nextWindChangeTurn = 0;
    public TMP_Text[] turnOrderTexts; // Assign these in the inspector to correspond with the player turn order

    public int royalTeamScore;
    public int goldTeamScore;
    public TMP_Text royalTeamScoreText; // Assign in Inspector
    public TMP_Text goldTeamScoreText; // Assign in Inspector

    private bool isGameOver = false;

    public AudioClip respawnSound; // Array to hold grunt sounds
    public AudioClip victorySound; // Add this for victory sound

    private AudioSource audioSource; // AudioSource to play the grunt sounds

    public GameObject optionsPanel; 

    public CanvasGroup fadePanelCanvasGroup;
    public TextMeshProUGUI countdownText;
    public float fadeDuration = 1.0f; // Duration for fade in/out
    public float countdownDuration = 1.0f; // Duration for each countdown number

    public GameObject playerSpawnEffectPrefab;

    private void InitializeTeamScores()
    {
        royalTeamScore = 0;
        goldTeamScore = 0;

        foreach (var player in players)
        {
            if (player.team == Player.Team.Royal)
            {
                royalTeamScore += player.lives;
            }
            else if (player.team == Player.Team.Gold)
            {
                goldTeamScore += player.lives;
            }
        }

        UpdateScoreUI();
    }


    public static GameManager Instance { get; private set; }

    public List<PlayerData> selectedPlayersData = new List<PlayerData>();

    // This method is called from the character select scene
    public void AddPlayerData(PlayerData data)
    {
        selectedPlayersData.Add(data);
    }

    void Awake()
    {
        players.Clear(); // Clear any existing players before instantiation.

        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            // Remove the DontDestroyOnLoad call
        }

        audioSource = gameObject.AddComponent<AudioSource>(); // Initialize the AudioSource
    }



    void Start()
    {
        // Since we are using TransitionalData to pass player data between scenes, we don't need LoadPlayerData here.
        // LoadPlayerData(); // This line can be commented out or removed.
        // InitializePlayers(); // This might be redundant if InstantiatePlayers is properly setting up players.
        // DecideStartingPlayer(); // Ensure this only runs after players are correctly instantiated.
        // InitializeTeamScores(); // Ensure this runs after players are correctly instantiated.
        // SetNextWindChangeTurn(); // Make sure wind change is still relevant.
        // UpdateTurnOrderUI(); // Ensure this only runs after players are correctly instantiated.

        //InstantiatePlayers();
        StartCoroutine(GameStartSequence());
        //InitializeGameState();


    }

    private IEnumerator GameStartSequence()
    {
        // Start with the screen faded to black
        fadePanelCanvasGroup.alpha = 1f;

        // Fade in
        yield return StartCoroutine(Fade(1f, 0f));

        // Countdown before the game starts
        yield return StartCoroutine(Countdown());

        // Initialize game state here (e.g., InstantiatePlayers, DecideStartingPlayer)
        InitializeGameState();
        InstantiatePlayers();
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadePanelCanvasGroup.alpha = newAlpha;
            yield return null;
        }
        fadePanelCanvasGroup.alpha = endAlpha;
    }

    private IEnumerator Countdown()
    {
        countdownText.gameObject.SetActive(true);
        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(countdownDuration);
            count--;
        }
        countdownText.text = "SPELL CAST";
        yield return new WaitForSeconds(countdownDuration);

        countdownText.gameObject.SetActive(false);
    }

    void SetNextWindChangeTurn()
    {
        // Calculate the random turn interval based on the number of players
        int minTurns = 4 * players.Count / 2; // because it's based on teams of two players
        int maxTurns = 9 * players.Count / 2;
        nextWindChangeTurn = turnCounter + Random.Range(minTurns, maxTurns + 1);
    }

    void Update()
    {
        if (turnInProgress && !isTurnTransitionInProgress)
        {
            if (turnTimer > 0)
            {
                turnTimer -= Time.deltaTime;
                UpdateTimerUI(turnTimer);
                if (turnTimer <= 0)
                {
                    // Time has run out, so show the "Time's Up" message and end the turn.
                    turnTimer = 0; // Stop the timer at zero
                    StartCoroutine(DisplayTimesUpMessage()); // Show the "Time's Up" message

                    // Interrupt the current player's spell casting
                    SpellCasting currentPlayerSpellCasting = players[currentPlayerIndex].GetComponent<SpellCasting>();
                    if (currentPlayerSpellCasting != null)
                    {
                        currentPlayerSpellCasting.InterruptSpellCasting();
                    }

                    EndTurn();
                }
            }
        }

        // Check for the TAB key press to skip turn
        if (turnInProgress && Input.GetKeyDown(KeyCode.Tab))
        {
            SkipTurn();
        }

        // Toggle the options panel when Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptions();
        }
    }

    void UpdateTimerUI(float time)
    {
        int timeInt = Mathf.CeilToInt(time);
        // Update the text color based on the remaining time
        if (timeInt > 0)
        {
            turnTimerText.text = timeInt.ToString(); // Only update the text if time is greater than zero

            if (timeInt > 10)
            {
                turnTimerText.color = Color.black;
            }
            else if (timeInt <= 10 && timeInt > 4)
            {
                turnTimerText.color = Color.yellow;
            }
            else if (timeInt <= 4)
            {
                turnTimerText.color = Color.red;
            }
        }
        else
        {
            // Optional: Hide the timer text or set it to "0" when the time is up
            turnTimerText.text = "0";
            turnTimerText.color = Color.red; // Keep it red when time is up
        }
    }


    private void InitializePlayers()
    {
        // Find all Player objects in the scene and add them to the players list
        players = new List<Player>(FindObjectsOfType<Player>());

        // Initially set all players to inactive
        foreach (Player player in players)
        {
            player.isActivePlayer = false; // Make sure players are initially inactive
            player.playerNameText = playerNameText; // Assuming all players share the same UI text element for names
            player.UpdatePlayerUI();
        }
    }

    private void DecideStartingPlayer()
    {
        // Randomly decide which player starts and initiate their turn
        currentPlayerIndex = Random.Range(0, players.Count);
        StartTurn();
    }

    private void StartTurn()
    {
        if (isGameOver) return;

        turnTimer = turnDuration;
        turnInProgress = true;

        if (turnCounter >= nextWindChangeTurn)
        {
            WindManager.Instance.RandomizeWind();
            SetNextWindChangeTurn(); // Set the next turn for wind change
        }

        // Update UI to reflect the current player's turn
        StartCoroutine(DisplayPlayerName(players[currentPlayerIndex].playerName));

        // Activate the current player for their turn
        players[currentPlayerIndex].BeginTurn();

        // Update the turn order display when a turn starts
        UpdateTurnOrderUI();

    }

    public void EndTurn()
    {
        if (isGameOver || isTurnTransitionInProgress) return;

        if (isTurnTransitionInProgress) return;

        turnCounter++; // Increment the turn counter

        // Make sure the current player's turn is fully ended
        players[currentPlayerIndex].EndTurn();

        // Indicate that a turn transition is in progress
        isTurnTransitionInProgress = true;

        // Note: Do NOT call DisplayTimesUpMessage here since it's called when the timer reaches zero in Update

        // Now wait for a delay before starting the next player's turn
        StartCoroutine(StartTurnAfterDelay());

        // Update the turn order display when a turn ends
        UpdateTurnOrderUI();
    }

    // Call this method to update the turn order display
    private void UpdateTurnOrderUI()
    {
        // First, deactivate all the text elements
        foreach (var text in turnOrderTexts)
        {
            text.gameObject.SetActive(false);
        }

        // Now activate and update the text for the active players
        for (int i = 0; i < players.Count; i++)
        {
            turnOrderTexts[i].gameObject.SetActive(true);
            int turnIndex = (currentPlayerIndex + i) % players.Count;
            turnOrderTexts[i].text = $"{i + 1}: {players[turnIndex].playerName}";
        }
    }


    private IEnumerator StartTurnAfterDelay()
    {
        // Wait for the delay between turns
        yield return new WaitForSeconds(turnDelayDuration);

        // Move to the next player
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

        // Reset the transition flag to allow the next transition
        isTurnTransitionInProgress = false;

        // Start the next turn
        StartTurn();
    }


    IEnumerator DisplayPlayerName(string name)
    {
        playerNameText.text = name;
        playerNameText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2); // Display name for 2 seconds
        playerNameText.gameObject.SetActive(false);
    }

    public void SkipTurn()
    {
        if (turnInProgress && !isTurnTransitionInProgress)
        {
            ShowSkipTurnMessage(); // Display the skip turn message
            EndTurn();
        }
    }
    public void ShowSkipTurnMessage()
    {
        StartCoroutine(DisplaySkipTurnMessage());
    }

    private IEnumerator DisplaySkipTurnMessage()
    {
        skipTurnMessageText.gameObject.SetActive(true); // Show the skip turn message
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        skipTurnMessageText.gameObject.SetActive(false); // Hide the skip turn message
    }

    private IEnumerator DisplayTimesUpMessage()
    {
        timesUpMessageText.gameObject.SetActive(true); // Show the time's up message
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        timesUpMessageText.gameObject.SetActive(false); // Hide the time's up message
    }

    // Call this method when a player loses a life
    public void NotifyPlayerDeath(Player player)
    {
        if (player != null)
        {
            // Decrease the team score by one for the player's team
            UpdateTeamScore(player.team, -1);

            // If the team still has a score, queue the respawn
            if (GetTeamScore(player.team) > 0)
            {
                StartCoroutine(QueueRespawn(player));
            }
            // Check if the game is over after adjusting the score
            if (royalTeamScore <= 0 || goldTeamScore <= 0)
            {
                CheckForGameOver();
            }
        }
    }

    private IEnumerator QueueRespawn(Player player, bool dueToDeath = true)
    {
        // Check if the team still has points
        if (GetTeamScore(player.team) <= 0) yield break;

        // Deactivate the player gameObject here, ensuring the coroutine has started first
        player.gameObject.SetActive(false);

        // If the player died during their own turn, wait until the turn ends
        while (currentPlayerIndex == players.IndexOf(player) && turnInProgress)
        {
            yield return null;
        }

        // Wait until it's this player's turn again
        while (currentPlayerIndex != players.IndexOf(player))
        {
            yield return null;
        }

        // Now it's this player's turn again, call RespawnPlayer
        StartCoroutine(RespawnPlayer(player, dueToDeath));
    }


    private IEnumerator RespawnPlayer(Player player, bool resetHealth)
    {
        if (respawnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(respawnSound);
        }

        // Randomly select a respawn point
        Transform respawnTransform = respawnPoints[Random.Range(0, respawnPoints.Length)];
        Vector3 respawnPosition = respawnTransform.position;

        // Set the player's position to the respawn point
        player.gameObject.SetActive(true);
        player.gameObject.transform.position = new Vector3(respawnPosition.x, respawnPosition.y, respawnPosition.z);

        // Make sure the player falls down by enabling the Rigidbody component
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Make sure the Rigidbody is not kinematic
            rb.velocity = Vector3.zero; // Reset velocity if needed
        }

        if (resetHealth)
        {
            player.ResetHealth();
        }

        player.spellCasting.StopChargingSound(); // Stop charging sound
        player.ClearDamageTexts();


        // If need to wait for the player to land, insert logic here
        // Example: yield return new WaitUntil(() => playerHasLanded());
        yield return null;

        // End the player's turn after the respawn
        EndTurn();
    }


    // Update the scores on the UI
    private void UpdateScoreUI()
    {
        if (royalTeamScoreText != null)
        {
            royalTeamScoreText.text = royalTeamScore.ToString();
        }

        if (goldTeamScoreText != null)
        {
            goldTeamScoreText.text = goldTeamScore.ToString();
        }
    }

    // Add a team score updating method
    private void UpdateTeamScore(Player.Team team, int scoreChange)
    {
        if (team == Player.Team.Royal)
        {
            royalTeamScore += scoreChange;
        }
        else if (team == Player.Team.Gold)
        {
            goldTeamScore += scoreChange;
        }

        UpdateScoreUI();
    }
    private int GetTeamScore(Player.Team team)
    {
        return team == Player.Team.Royal ? royalTeamScore : goldTeamScore;
    }

    private void CheckForGameOver()
    {
        // Determine the winning and losing teams
        bool isRoyalVictory = royalTeamScore > goldTeamScore;
        bool isGoldVictory = goldTeamScore > royalTeamScore;

        if (isRoyalVictory || isGoldVictory)
        {
            isGameOver = true;
            StopAllCoroutines(); // This will stop all coroutines including StartTurnAfterDelay

            // Play the victory sound
            if (victorySound != null && audioSource != null)
            {
                audioSource.PlayOneShot(victorySound);
            }

            // Stop all player inputs and the timer
            foreach (var player in players)
            {
                player.playerMovement.EnableMovement(false);
                // Trigger the appropriate animation based on the player's team and victory/defeat.
                if ((isRoyalVictory && player.team == Player.Team.Royal) ||
                    (isGoldVictory && player.team == Player.Team.Gold))
                {
                    player.playerMovement.PlayVictoryAnimation();
                }
                else
                {
                    player.playerMovement.PlayDefeatAnimation();
                }
            }

            // Stop the turn system and timer
            turnInProgress = false;
            turnTimer = 0; // This stops the timer
            UpdateTimerUI(0); // This updates the timer display immediately

            // Display the end game panels based on the winning team
            if (isRoyalVictory)
            {
                // Display Royal Victory and Gold Defeat
                UIManager.Instance.ShowEndGamePanel(true, Player.Team.Royal);
                UIManager.Instance.ShowEndGamePanel(false, Player.Team.Gold);
            }
            else if (isGoldVictory)
            {
                // Display Gold Victory and Royal Defeat
                UIManager.Instance.ShowEndGamePanel(true, Player.Team.Gold);
                UIManager.Instance.ShowEndGamePanel(false, Player.Team.Royal);
            }
        }
    }

    // Call this method in the Start or Init method of the game scene
    public void InstantiatePlayers()
    {
        players.Clear();

        // Create a list to keep track of used spawn points
        List<int> usedSpawnPoints = new List<int>();

        foreach (PlayerData data in TransitionalData.PlayersData)
        {
            Vector3 spawnPointPosition;
            int spawnPointIndex;

            // Find an unused spawn point
            do
            {
                spawnPointIndex = GetRandomSpawnPointIndex();
            } while (usedSpawnPoints.Contains(spawnPointIndex));

            // Add the spawn point index to the used spawn points list
            usedSpawnPoints.Add(spawnPointIndex);

            // Get the spawn point position based on the index
            spawnPointPosition = spawnPoints[spawnPointIndex].position;

            GameObject playerObject = Instantiate(data.characterPrefab, spawnPointPosition, Quaternion.identity);
            Player playerComponent = playerObject.GetComponent<Player>();

            // Instantiate the particle effect prefab at the player's spawn location
            if (playerSpawnEffectPrefab != null)
            {
                Instantiate(playerSpawnEffectPrefab, spawnPointPosition, Quaternion.identity);
            }

            // Assign the data to the player component.
            playerComponent.playerName = data.playerName;
            playerComponent.chosenGrimoire = data.chosenGrimoire;
            playerComponent.items = new List<Item>(data.items);
            playerComponent.team = data.team;
            playerComponent.UpdatePlayerUI();

            players.Add(playerComponent); // Add to the list of players in the game.
        }

        // Deciding starting player, initializing scores, and setting up UI should be done after all players are instantiated.
        DecideStartingPlayer();
        InitializeTeamScores();
        UpdateTurnOrderUI();
    }

    public void NotifyPlayerEvasion(Player player)
    {
        if (player != null)
        {
            // Do not decrease the team score, only trigger respawn
            StartCoroutine(NotifyPlayerEvasionCoroutine(player));
        }
    }

    private IEnumerator NotifyPlayerEvasionCoroutine(Player player)
    {
        yield return StartCoroutine(QueueRespawn(player, false)); // Wait until respawn logic is complete.
        EndTurn(); // Now, end the turn.
    }

    private Vector3 GetSpawnPoint()
    {
        // Check if spawnPoints is not null and has elements
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            // Logic to choose a random spawn point
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex].position;
        }
        else
        {
            Debug.LogError("Spawn points array is either null or empty.");
            return Vector3.zero; // Return a default position if no spawn points are set
        }
    }

    public void ReplayStage()
    {
        // Clear the player data from the current game before reloading the scene
        players.Clear();
        // Also ensure the GameManager is the only instance present
        if (Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate GameManager
            return; // Exit to avoid further code execution
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // The new scene's Start() method will handle re-instantiating players.
    }

    public void LoadCharacterSelection()
    {
        // Clear the transitional data here as we're going back to character selection
        Time.timeScale = 1;
        TransitionalData.PlayersData.Clear();
        Destroy(gameObject); // Destroy the GameManager instance
        SceneManager.LoadScene("CharacterSelection"); // Replace with your actual scene name
    }


    public void QuitToDesktop()
    {
        Application.Quit();
    }

    public void ToggleOptions()
    {
        bool isActive = optionsPanel.activeSelf;
        optionsPanel.SetActive(!isActive);
        Time.timeScale = isActive ? 1 : 0; // Pausing and unpausing the game
    }

    void InitializeGameState()
    {
        // Initialize scores and any other game state here
        InitializeTeamScores();
        // Rest of your initialization
    }

    private int GetRandomSpawnPointIndex()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            return Random.Range(0, spawnPoints.Length);
        }
        else
        {
            Debug.LogError("Spawn points array is either null or empty.");
            return -1;
        }
    }

}