using UnityEngine;
using System.Collections.Generic;

public class PlacementController : MonoBehaviour
{
    public static PlacementController Instance { get; private set; }

    [Header("References")]
    public GridManager gridManager;
    public GameObject tokenPrefab;

    [System.Serializable]
    public class PlayerPlacement
    {
        public string playerName;
        public List<TokenData> tokensToPlace;
        public List<Tile> allowedTiles;
    }

    [Header("Players")]
    public List<PlayerPlacement> players = new List<PlayerPlacement>();
    private int currentPlayerIndex = 0;
    private int currentTokenIndex = 0;

    private bool isPlacing = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        if (players.Count == 0)
        {
            Debug.LogError("No players configured for placement.");
            return;
        }

        gridManager.numPlayers = players.Count;
        TurnManager.Instance.totalPlayers = players.Count;
        AssignZones();

        Debug.Log($"Placement phase started. {players[currentPlayerIndex].playerName}'s turn.");
    }

    void AssignZones()
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
        if (!isPlacing) return;
        if (currentPlayerIndex >= players.Count) return;

        var currentPlayer = players[currentPlayerIndex];

        if (currentTokenIndex >= currentPlayer.tokensToPlace.Count) return;
        if (!currentPlayer.allowedTiles.Contains(tile)) return;
        if (tile.IsOccupied) return;

        TokenData data = currentPlayer.tokensToPlace[currentTokenIndex];
        if (data == null) return;

        GameObject tokenGO = Instantiate(tokenPrefab, tile.transform.position, Quaternion.identity);
        Token token = tokenGO.GetComponent<Token>();
        token.Initialize(data, currentPlayerIndex + 1);
        tile.PlaceToken(token);

        Debug.Log($"{currentPlayer.playerName} placed token: {data.tokenName}");

        currentTokenIndex++;

        if (currentTokenIndex >= currentPlayer.tokensToPlace.Count)
        {
            NextPlayer();
        }
    }

    void NextPlayer()
    {
        currentPlayerIndex++;
        currentTokenIndex = 0;

        if (currentPlayerIndex >= players.Count)
        {
            isPlacing = false;
            GameManager.Instance?.StartCombatPhase();
        }
    }
}