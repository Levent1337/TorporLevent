using UnityEngine;

public class TokenSelector : MonoBehaviour
{
    public static TokenSelector Instance { get; private set; }

    private Token selectedToken;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentPhase != GameManager.GamePhase.Combat)
            return;

        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DeselectToken();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Token clickedToken = hit.collider.GetComponentInParent<Token>();
                Tile clickedTile = hit.collider.GetComponentInParent<Tile>();

                if (clickedToken != null)
                {
                    if (selectedToken != null && clickedToken != selectedToken)
                    {
                        Tile enemyTile = clickedToken.CurrentTile();
                        if (enemyTile != null)
                        {
                            selectedToken.TryInteractWith(enemyTile);
                            return;
                        }
                    }
                    else
                    {
                        TrySelectToken(clickedToken);
                        return;
                    }
                }

                if (clickedTile != null && selectedToken != null)
                {
                    selectedToken.TryInteractWith(clickedTile);
                }
            }
        }
    }

    void TrySelectToken(Token token)
    {
        if (!GameManager.Instance.IsPlayerTurn(token.ownerId))
        {
            Debug.Log("Can't select another player's token.");
            return;
        }

        DeselectToken();

        selectedToken = token;
        selectedToken.SetSelected(true);
        Debug.Log($"Selected token: {token.name}");
    }

    void DeselectToken()
    {
        if (selectedToken != null)
        {
            selectedToken.SetSelected(false);
            selectedToken = null;
            Debug.Log("Deselected token.");
        }
    }

    public void ConsumeAP()
    {
        GameManager.Instance.UseActionPoint();
        if (GameManager.Instance.CurrentPhase == GameManager.GamePhase.Combat &&
            GameManager.Instance.actionPoints <= 0)
        {
            DeselectToken();
        }
    }

    public Token GetSelectedToken()
    {
        return selectedToken;
    }
    public void ForceDeselect()
    {
        DeselectToken();
    }
}
