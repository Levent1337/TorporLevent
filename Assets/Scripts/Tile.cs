using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public Token occupyingToken;

    public void SetPosition(int x, int y)
    {
        gridPosition = new Vector2Int(x, y);
        name = $"Tile_{x}_{y}";
    }

    public bool IsOccupied => occupyingToken != null;

    public void PlaceToken(Token token)
    {
        occupyingToken = token;
    }

    public void ClearToken()
    {
        occupyingToken = null;
    }

    void OnMouseDown()
    {
        if (!IsOccupied && GameManager.Instance.CurrentPhase == GameManager.GamePhase.Placement)
        {
            GameManager.Instance.TryPlaceToken(this);
        }
    }
}
