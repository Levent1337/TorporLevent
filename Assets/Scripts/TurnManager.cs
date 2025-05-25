using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public int totalPlayers = 2;

    public int CurrentPlayer { get; private set; } = 1; // 1 or 2
    public int ActionPoints { get; private set; } = 4;
    public int TurnNumber { get; private set; } = 1;

    public delegate void TurnChanged(int currentPlayer, int turnNumber);
    public event TurnChanged OnTurnChanged;

    private void Awake()
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
        BeginTurn();
    }

    public void BeginTurn()
    {
        ActionPoints = 4;
        Debug.Log($"Player {CurrentPlayer}'s turn. Turn #{TurnNumber}");
        OnTurnChanged?.Invoke(CurrentPlayer, TurnNumber);
    }

    public void UseActionPoint()
    {
        ActionPoints--;

        if (ActionPoints <= 0)
        {
            EndTurn();
        }
    }

    public void EndTurn()
    {
        CurrentPlayer++;
        if (CurrentPlayer > totalPlayers)
            CurrentPlayer = 1;

        TurnNumber++;
        BeginTurn();
    }

    public bool IsPlayerTurn(int playerId)
    {
        return playerId == CurrentPlayer;
    }
}
