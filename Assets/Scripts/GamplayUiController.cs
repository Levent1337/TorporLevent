using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayUIController : MonoBehaviour
{
    public void OnDefendButtonPressed()
    {
        Token selected = TokenSelector.Instance?.GetSelectedToken();
       selected.Defend();
    }

    public void OnSkipTurnButtonPressed()
    {
        GameManager.Instance?.EndTurn();
    }

    public void OnResetGameButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
