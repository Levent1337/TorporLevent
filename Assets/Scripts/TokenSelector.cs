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
                Token token = hit.collider.GetComponentInParent<Token>();
                if (token != null)
                {
                    TrySelectToken(token);
                    return;
                }

                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null && selectedToken != null)
                {
                    selectedToken.TryInteractWith(tile);
                }
            }
        }
    }

    void TrySelectToken(Token token)
    {
        if (!TurnManager.Instance.IsPlayerTurn(token.ownerId)) return;

        DeselectToken();
        selectedToken = token;
        selectedToken.SetSelected(true);
    }

    void DeselectToken()
    {
        if (selectedToken != null)
        {
            selectedToken.SetSelected(false);
            selectedToken = null;
        }
    }

    public void ConsumeAP()
    {
        TurnManager.Instance.UseActionPoint();
        if (TurnManager.Instance.ActionPoints <= 0)
        {
            DeselectToken();
        }
    }
}