using UnityEngine;
using UnityEngine.UI;

public class PlacementUIController : MonoBehaviour
{
    [Header("Manual Token Buttons")]
    public Button attackTokenButton;
    public Button defenseTokenButton;

    [Header("Assign Corresponding TokenData")]
    public TokenData attackTokenData;
    public TokenData defenseTokenData;

    void Start()
    {
        attackTokenButton.onClick.AddListener(() => SelectToken(attackTokenData));
        defenseTokenButton.onClick.AddListener(() => SelectToken(defenseTokenData));
    }

    void Update()
    {
        // Only show buttons during placement phase
        gameObject.SetActive(GameManager.Instance.CurrentPhase == GameManager.GamePhase.Placement);
    }

    void SelectToken(TokenData tokenData)
    {
        GameManager.Instance.selectedTokenData = tokenData;
        Debug.Log($"Selected token: {tokenData.tokenName}");
    }
}
