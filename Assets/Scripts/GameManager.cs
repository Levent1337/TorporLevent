using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [HideInInspector] public TokenData selectedTokenData = null;

    public enum GamePhase
    {
        Placement,
        Combat,
        GameOver
    }

    public GamePhase CurrentPhase { get; private set; } = GamePhase.Placement;
    private Dictionary<int, List<Token>> activeTokensByPlayer = new Dictionary<int, List<Token>>();

    [Header("Setup")]
    public GridManager gridManager;
    public GameObject tokenPrefab;

    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        [HideInInspector] public List<TokenData> tokensPlaced = new List<TokenData>();
        public int tokensToPlace = 3; 
        [HideInInspector] public List<Tile> allowedTiles;
    }

    [Header("Players")]
    public List<PlayerData> players = new List<PlayerData>();
    private int currentPlayerIndex = 0;
    private int currentTokenIndex = 0;

    [Header("Turn Info")]
    public int actionPoints = 4;
    private int turnNumber = 1;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        StartPlacementPhase();
    }

    void StartPlacementPhase()
    {
        CurrentPhase = GamePhase.Placement;
        gridManager.numPlayers = players.Count;
        AssignPlacementZones();

        Debug.Log($"Placement phase started. {players[currentPlayerIndex].playerName}'s turn.");
    }

    void AssignPlacementZones()
    {
        for (int i = 0; i < players.Count; i++)
        {
            switch (i)
            {
                case 0:
                    players[i].allowedTiles = gridManager.player1StartTiles;
                    break;
                case 1:
                    players[i].allowedTiles = gridManager.player2StartTiles;
                    break;
                case 2:
                    players[i].allowedTiles = gridManager.player3StartTiles;
                    break;
                case 3:
                    players[i].allowedTiles = gridManager.player4StartTiles;
                    break;
            }
        }
    }

    public void TryPlaceToken(Tile tile)
    {
        if (CurrentPhase != GamePhase.Placement)
            return;

        if (currentPlayerIndex >= players.Count)
            return;

        var currentPlayer = players[currentPlayerIndex];

        if (selectedTokenData == null)
        {
            Debug.Log("No token selected yet.");
            return;
        }

        if (!currentPlayer.allowedTiles.Contains(tile))
        {
            Debug.Log("Tile not in your placement zone.");
            return;
        }

        if (tile.IsOccupied)
        {
            Debug.Log("Tile already occupied.");
            return;
        }

        if (currentPlayer.tokensPlaced.Count >= currentPlayer.tokensToPlace)
        {
            Debug.Log("You have already placed all your tokens.");
            return;
        }

        TokenData data = selectedTokenData;
        if (data == null)
        {
            Debug.LogError("TokenData is null.");
            return;
        }

        // Instantiate and initialize the token
        GameObject tokenGO = Instantiate(tokenPrefab, tile.transform.position, Quaternion.identity);
        Token token = tokenGO.GetComponent<Token>();
        int ownerId = currentPlayerIndex + 1;
        token.Initialize(data, ownerId);
        tile.PlaceToken(token);

        // Track this token for the player
        if (!activeTokensByPlayer.ContainsKey(ownerId))
            activeTokensByPlayer[ownerId] = new List<Token>();

        activeTokensByPlayer[ownerId].Add(token);

        // Record that this token was placed
        currentPlayer.tokensPlaced.Add(data);
        selectedTokenData = null;

        Debug.Log($"{currentPlayer.playerName} placed {data.tokenName}");

        // Check if player finished placing all allowed tokens
        if (currentPlayer.tokensPlaced.Count >= currentPlayer.tokensToPlace)
        {
            currentPlayerIndex++;

            if (currentPlayerIndex >= players.Count)
            {
                Debug.Log("All players have placed their tokens.");
                StartCombatPhase();
            }
            else
            {
                Debug.Log($"{players[currentPlayerIndex].playerName}'s turn to place tokens.");
            }
        }
        else
        {
            Debug.Log($"{currentPlayer.playerName}, place your next token.");
        }
    }



    void StartCombatPhase()
    {
        CurrentPhase = GamePhase.Combat;
        currentPlayerIndex = 0;
        actionPoints = 4;
        turnNumber = 1;

        Debug.Log($"Combat phase started. Player {CurrentPlayerId}'s turn.");
    }

    public void UseActionPoint()
    {
        actionPoints--;

        if (actionPoints <= 0)
        {
            EndTurn();
        }
    }

    public void EndTurn()
    {
        TokenSelector.Instance?.ForceDeselect();

        do
        {
            currentPlayerIndex++;
            if (currentPlayerIndex >= players.Count)
                currentPlayerIndex = 0;
        }
        while (!PlayerHasTokens(CurrentPlayerId));

        turnNumber++;
        actionPoints = 4;

        ResetDefenders(CurrentPlayerId);

        Debug.Log($"Turn {turnNumber}: Player {CurrentPlayerId}'s turn.");
    }

    private void ResetDefenders(int playerId)
    {
        if (activeTokensByPlayer.TryGetValue(playerId, out var tokens))
        {
            foreach (var token in tokens)
            {
                token.ResetDefending();
            }
        }
    }
    public bool IsPlayerTurn(int playerId)
    {
        return playerId == CurrentPlayerId;
    }

    public int CurrentPlayerId => currentPlayerIndex + 1;

    public void EndGame(string winnerName)
    {
        CurrentPhase = GamePhase.GameOver;
        Debug.Log($"Game Over! {winnerName} wins!");
    }
    public void OnTokenDeath(Token token)
    {
        if (activeTokensByPlayer.ContainsKey(token.ownerId))
        {
            activeTokensByPlayer[token.ownerId].Remove(token);

            if (activeTokensByPlayer[token.ownerId].Count == 0)
            {
                Debug.Log($"{players[token.ownerId - 1].playerName} has no more tokens.");
            }

            CheckWinCondition();
        }
    }
    void CheckWinCondition()
    {
        List<int> playersWithTokens = new List<int>();

        foreach (var kvp in activeTokensByPlayer)
        {
            if (kvp.Value.Count > 0)
                playersWithTokens.Add(kvp.Key);
        }

        if (playersWithTokens.Count == 1)
        {
            string winnerName = players[playersWithTokens[0] - 1].playerName;
            EndGame(winnerName);
        }
    }
    private bool PlayerHasTokens(int playerId)
    {
        return activeTokensByPlayer.ContainsKey(playerId) && activeTokensByPlayer[playerId].Count > 0;
    }
}
