using UnityEngine;
using TMPro;

public class Token : MonoBehaviour
{
    public TokenData data;
    [HideInInspector] public int currentHealth;

    public bool IsAlive => currentHealth > 0;
    public bool isDefending = false;
    public int ownerId;

    private Renderer rend;
    private Color originalColor;

    public GameObject hoverUI;
    private TextMeshProUGUI statsText;

    void Start()
    {
        currentHealth = data.maxHealth;

        // Instantiate and configure hover UI
        hoverUI = Instantiate(hoverUI, transform);
        hoverUI.transform.localPosition = new Vector3(0, 2.5f, 0);
        hoverUI.SetActive(false);

        statsText = hoverUI.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Initialize(TokenData newData, int owner = -1)
    {
        data = newData;
        ownerId = owner;

        if (data == null)
        {
            Debug.LogError("Token received null TokenData!");
            return;
        }

        currentHealth = data.maxHealth;

        if (data.shapePrefab != null)
        {
            GameObject shape = Instantiate(data.shapePrefab, transform);
            shape.transform.localPosition = Vector3.zero;

            rend = shape.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                rend.material = new Material(rend.material);
                Color baseColor = GridManager.Instance.GetPlayerColor(ownerId);
                Color playerColor = DarkenColor(baseColor, 0.6f);
                rend.material.color = playerColor;
                originalColor = playerColor;
            }
        }
        else
        {
            Debug.LogWarning("TokenData has no shapePrefab assigned.");
        }
    }

    public void SetSelected(bool selected)
    {
        if (rend != null)
        {
            rend.material.color = selected ? Color.yellow : originalColor;
        }
    }

    public void TryInteractWith(Tile targetTile)
    {
        if (isDefending)
        {
            Debug.Log($"{name} is defending and cannot act.");
            return;
        }

        if (!GameManager.Instance.IsPlayerTurn(ownerId))
        {
            Debug.Log("Not your turn.");
            return;
        }

        if (GameManager.Instance.actionPoints <= 0)
        {
            Debug.Log("No AP left.");
            return;
        }

        if (!targetTile.IsOccupied && IsAdjacentTo(targetTile))
        {
            Debug.Log("Trying to move to empty tile.");
            MoveTo(targetTile);
        }
        else if (targetTile.IsOccupied && IsAdjacentTo(targetTile))
        {
            Token enemy = targetTile.occupyingToken;
            if (enemy != null && enemy.ownerId != ownerId)
            {
                Debug.Log("Attacking enemy!");
                Attack(enemy);
            }
        }
        else
        {
            Debug.Log("Target not valid for interaction.");
        }
    }

    public void MoveTo(Tile targetTile, bool consumeAP = true)
    {
        Tile current = CurrentTile();
        if (current != null)
            current.ClearToken();

        transform.position = targetTile.transform.position;
        targetTile.PlaceToken(this);

        if (consumeAP)
            TokenSelector.Instance.ConsumeAP();

        Debug.Log($"{name} moved.");
    }

    public void Attack(Token defender)
    {
        defender.TakeDamage(data.attack);
        Debug.Log($"{name} attacked {defender.name} with {data.attack} attack.");

        bool defenderDied = !defender.IsAlive;
        Tile defenderTile = defender.CurrentTile();

        if (defenderDied)
        {
            defender.Die();
            if (defenderTile != null)
                MoveTo(defenderTile, consumeAP: false);
        }

        TokenSelector.Instance.ConsumeAP();
    }

    public void TakeDamage(int amount)
    {
        int actualDefense = isDefending ? data.defense * 2 : data.defense;
        int rawDamage = amount - actualDefense;
        int damage = Mathf.Max(rawDamage, 0);
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{name} took {damage} damage (raw {amount}, defense {actualDefense})");
    }

    public void Defend()
    {
        isDefending = true;
        Debug.Log($"{name} is defending.");
        TokenSelector.Instance.ConsumeAP();
    }

    public void ResetDefending()
    {
        isDefending = false;
        Debug.Log($"{name} is no longer defending.");
    }

    public void Die()
    {
        Tile current = CurrentTile();
        if (current != null)
            current.ClearToken();

        GameManager.Instance.OnTokenDeath(this);
        Destroy(gameObject);

        Debug.Log($"{name} died.");
    }

    public Tile CurrentTile()
    {
        Tile tile = GridManager.Instance.GetTileAtPosition(transform.position);
        if (tile == null)
            Debug.LogWarning($"{name} is not on a valid tile!");
        return tile;
    }

    bool IsAdjacentTo(Tile tile)
    {
        Tile current = CurrentTile();
        if (current == null) return false;

        Vector2Int diff = tile.gridPosition - current.gridPosition;
        return Mathf.Abs(diff.x) + Mathf.Abs(diff.y) == 1;
    }

    void OnMouseEnter()
    {
        if (statsText != null)
        {
            string stats = $"HP: {currentHealth}\nATK: {data.attack}\nDEF: {(isDefending ? data.defense * 2 : data.defense)}";
            statsText.text = stats;
            hoverUI.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        if (hoverUI != null)
            hoverUI.SetActive(false);
    }
    private Color DarkenColor(Color original, float factor)
    {
        return new Color(original.r * factor, original.g * factor, original.b * factor, original.a);
    }
}

