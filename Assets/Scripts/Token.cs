using UnityEngine;

public class Token : MonoBehaviour
{
    public TokenData data;
    [HideInInspector] public int currentHealth;

    public bool IsAlive => currentHealth > 0;
    public bool isDefending = false;
    public int ownerId; // 1 = Player 1, 2 = Player 2



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

            Renderer rend = shape.GetComponentInChildren<Renderer>();
            if (rend != null)
                rend.material.color = data.tokenColor;
        }
        else
        {
            Debug.LogWarning("TokenData has no shapePrefab assigned.");
        }
    }


    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
    }
    public void SetSelected(bool selected)
    {
        // Optional: show outline, change color, etc.
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material.color = selected ? Color.yellow : data.tokenColor;
        }
    }

    public void TryInteractWith(Tile targetTile)
    {
        if (!TurnManager.Instance.IsPlayerTurn(ownerId)) return;
        if (TurnManager.Instance.ActionPoints <= 0) return;

        if (!targetTile.IsOccupied && IsAdjacentTo(targetTile))
        {
            MoveTo(targetTile);
        }
        else if (targetTile.IsOccupied && IsAdjacentTo(targetTile))
        {
            Token enemy = targetTile.occupyingToken;
            if (enemy != null && enemy.ownerId != ownerId && !enemy.isDefending)
            {
                Attack(enemy);
            }
        }
    }

    bool IsAdjacentTo(Tile tile)
    {
        Vector2Int diff = tile.gridPosition - CurrentTile().gridPosition;
        return Mathf.Abs(diff.x) + Mathf.Abs(diff.y) == 1;
    }

    public void MoveTo(Tile targetTile)
    {
        CurrentTile().ClearToken();
        transform.position = targetTile.transform.position;
        targetTile.PlaceToken(this);
        TokenSelector.Instance.ConsumeAP();
        Debug.Log($"{name} moved.");
    }

    public void Attack(Token defender)
    {
        int rawDamage = data.attack - defender.data.defense;
        int damage = Mathf.Max(rawDamage, 0);
        defender.TakeDamage(damage);

        Debug.Log($"{name} attacked {defender.name} for {damage} damage.");

        if (!defender.IsAlive)
        {
            defender.Die();
            MoveTo(defender.CurrentTile()); // attacker moves in
        }

        TokenSelector.Instance.ConsumeAP();
    }

    public void Defend()
    {
        isDefending = true;
        Debug.Log($"{name} is defending.");
        TokenSelector.Instance.ConsumeAP();
    }

    public void Die()
    {
        CurrentTile().ClearToken();
        Destroy(gameObject);
        Debug.Log($"{name} died.");
    }

    public Tile CurrentTile()
    {
        return GridManager.Instance.GetTileAtPosition(transform.position);
    }
}
