using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GamePhase { Placement, Combat, GameOver }
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Placement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        StartPlacementPhase();
    }

    public void StartPlacementPhase()
    {
        CurrentPhase = GamePhase.Placement;
        Debug.Log("Placement phase started.");
    }

    public void StartCombatPhase()
    {
        CurrentPhase = GamePhase.Combat;
        Debug.Log("Combat phase started.");
    }

    public void EndGame(string winnerName)
    {
        CurrentPhase = GamePhase.GameOver;
        Debug.Log($"Game Over! {winnerName} wins!");
    }
}
