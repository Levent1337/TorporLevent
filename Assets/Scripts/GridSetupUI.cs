using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GridSetupUI : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField widthInput;
    public TMP_InputField heightInput;
    public TMP_InputField zoneWidthInput;
    public TMP_InputField zoneHeightInput;
    public TMP_InputField playerCountInput;
    public TMP_InputField tokensPerPlayerInput;

    [Header("Start Button & Panel")]
    public Button startButton;
    public GameObject setupPanel;

    [Header("References")]
    public GridManager gridManager;
    public GameManager gameManager;

    void Start()
    {
        startButton.onClick.AddListener(ApplySettingsAndStart);

        // Default values
        widthInput.text = "6";
        heightInput.text = "6";
        zoneWidthInput.text = "6";
        zoneHeightInput.text = "3";
        playerCountInput.text = "2";
        tokensPerPlayerInput.text = "3";
    }

    void ApplySettingsAndStart()
    {
        int width = int.Parse(widthInput.text);
        int height = int.Parse(heightInput.text);
        int zoneWidth = int.Parse(zoneWidthInput.text);
        int zoneHeight = int.Parse(zoneHeightInput.text);
        int numPlayers = Mathf.Clamp(int.Parse(playerCountInput.text), 2, 4);
        int tokensRequested = int.Parse(tokensPerPlayerInput.text);

        // Limit max tokens to available tiles in placement zone
        int maxTokensPerPlayer = zoneWidth * zoneHeight;
        int tokensToPlace = Mathf.Min(tokensRequested, maxTokensPerPlayer);

        if (tokensRequested > maxTokensPerPlayer)
        {
            Debug.LogWarning($"Requested {tokensRequested} tokens, but only {maxTokensPerPlayer} placement tiles available per player. Clamping.");
        }

        // Default player zone colors (you can customize this)
        Color[] defaultColors = {
        new Color(0.3f, 0.6f, 1f), // Blue
        new Color(1f, 0.4f, 0.4f), // Red
        new Color(0.4f, 1f, 0.4f), // Green
        new Color(1f, 1f, 0.4f)    // Yellow
    };

        // Apply values to GridManager
        gridManager.width = width;
        gridManager.height = height;
        gridManager.playerZoneWidth = zoneWidth;
        gridManager.playerZoneHeight = zoneHeight;
        gridManager.numPlayers = numPlayers;

        // Create player data
        gameManager.players = new List<GameManager.PlayerData>();
        for (int i = 0; i < numPlayers; i++)
        {
            gameManager.players.Add(new GameManager.PlayerData
            {
                playerName = $"Player {i + 1}",
                tokensToPlace = tokensToPlace,
                playerColor = defaultColors[i]
            });
        }

        setupPanel.SetActive(false);
        gridManager.GenerateGrid();
        gridManager.CenterCameraAboveGrid();

        // Start placement phase after generating the grid
        gameManager.StartPlacementPhase();
    }

}
