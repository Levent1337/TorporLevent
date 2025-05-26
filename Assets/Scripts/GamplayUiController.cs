using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayUIController : MonoBehaviour
{
    public TextMeshProUGUI turnIndicatorText;
    public TextMeshProUGUI apIndicatorText;
    public TextMeshProUGUI turnCounterText;
    public TextMeshProUGUI winnerText;
    public GameObject winnerPanel;

    public void OnDefendButtonPressed()
    {
        Token selected = TokenSelector.Instance?.GetSelectedToken();
        selected?.Defend();
    }

    public void OnSkipTurnButtonPressed()
    {
        GameManager.Instance?.EndTurn();
    }

    public void OnResetGameButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void UpdateTurnIndicator(string playerName, Color playerColor)
    {
        if (turnIndicatorText != null)
        {
            turnIndicatorText.text = $"Turn: {playerName}";
            turnIndicatorText.color = playerColor;
        }
    }
    public void UpdateAPIndicator(int currentAP)
    {
        if (apIndicatorText != null)
            apIndicatorText.text = $"AP: {currentAP}";
    }
    public void UpdateTurnCounter(int turnNumber)
    {
        if (turnCounterText != null)
            turnCounterText.text = $"Round: {turnNumber}";
    }
    public void ShowWinner(string playerName)
    {
        if (winnerPanel != null)
            winnerPanel.SetActive(true); // Show the panel

        if (winnerText != null)
            winnerText.text = $"Winner: {playerName}!";
    }





}
