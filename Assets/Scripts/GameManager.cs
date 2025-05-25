using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GamePhase
    {
        Placement,
        Combat,
        GameOver
    }

    public GamePhase CurrentPhase { get; private set; } = GamePhase.Placement;

    [Header("Setup")]
    public GridManager gridManager;
    public GameObject tokenPrefab;

    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        public List<TokenData> tokensToPlace;
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

        if (currentPlayerIndex >= players.Count) return;

        var currentPlayer = players[currentPlayerIndex];
        if (currentTokenIndex >= currentPlayer.tokensToPlace.Count)
            return;

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

        TokenData data = currentPlayer.tokensToPlace[currentTokenIndex];
        if (data == null)
        {
            Debug.LogError("TokenData is null.");
            return;
        }

        GameObject tokenGO = Instantiate(tokenPrefab, tile.transform.position, Quaternion.identity);
        Token token = tokenGO.GetComponent<Token>();
        token.Initialize(data, currentPlayerIndex + 1); // ownerId = 1-based
        tile.PlaceToken(token);

        Debug.Log($"{currentPlayer.playerName} placed {data.tokenName}");

        currentTokenIndex++;

        if (currentTokenIndex >= currentPlayer.tokensToPlace.Count)
        {
            currentTokenIndex = 0;
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
        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Count)
            currentPlayerIndex = 0;

        turnNumber++;
        actionPoints = 4;

        Debug.Log($"Turn {turnNumber}: Player {CurrentPlayerId}'s turn.");
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
}
