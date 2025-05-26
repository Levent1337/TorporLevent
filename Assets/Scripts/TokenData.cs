using UnityEngine;

[CreateAssetMenu(fileName = "NewTokenData", menuName = "StrategyGame/TokenData")]
public class TokenData : ScriptableObject
{
    public string tokenName;

    [Header("Stats")]
    public int maxHealth;
    public int attack;
    public int defense;
    

    [Header("Visuals")]
    public GameObject shapePrefab; // e.g. circle, triangle, etc.
    public Color tokenColor = Color.white;
}
