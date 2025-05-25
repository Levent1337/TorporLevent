using UnityEngine;

public class Token : MonoBehaviour
{
    public TokenData data;
    [HideInInspector] public int currentHealth;

    public bool IsAlive => currentHealth > 0;
    public bool isDefending = false;
    public int ownerId; // 1 = Player 1, etc.

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
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material.color = selected ? Color.yellow : data.tokenColor;
        }
    }

    public void TryInteractWith(Tile targetTile)
    {
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
            Debug.Log($"Attempting attack on tile with token: {enemy?.name}, ownerId = {enemy?.ownerId}");

            if (enemy != null && enemy.ownerId != ownerId)
            {
                if (!enemy.isDefending)
                {
                    Debug.Log("Attacking enemy!");
                    Attack(enemy);
                }
                else
                {
                    Debug.Log("Enemy is defending. No attack.");
                }
            }
            else
            {
                Debug.Log("Target is not an enemy.");
            }
        }
        else
        {
            Debug.Log("Target not valid for interaction.");
        }
    }


    bool IsAdjacentTo(Tile tile)
    {
        Tile current = CurrentTile();
        if (current == null) return false;

        Vector2Int diff = tile.gridPosition - current.gridPosition;
        return Mathf.Abs(diff.x) + Mathf.Abs(diff.y) == 1;
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
        int rawDamage = data.attack - defender.data.defense;
        int damage = Mathf.Max(rawDamage, 0);
        defender.TakeDamage(damage);

        Debug.Log($"{name} attacked {defender.name} for {damage} damage.");

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


    public void Defend()
    {
        isDefending = true;
        Debug.Log($"{name} is defending.");
        TokenSelector.Instance.ConsumeAP();
    }

    public void Die()
    {
        Tile current = CurrentTile();
        if (current != null)
            current.ClearToken();

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
}
