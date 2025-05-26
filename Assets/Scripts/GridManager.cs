using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public GameObject tilePrefab;
    public float spacing = 1.1f;
    public List<Tile> player1StartTiles = new List<Tile>();
    public List<Tile> player2StartTiles = new List<Tile>();
    public List<Tile> player3StartTiles = new List<Tile>();
    public List<Tile> player4StartTiles = new List<Tile>();
    [HideInInspector] public int numPlayers = 2;

    [Header("Player Zone Colors")]
    public Color player1ZoneColor = new Color(0.3f, 0.6f, 1f); // default blue
    public Color player2ZoneColor = new Color(1f, 0.4f, 0.4f);  // default red
    public Color player3ZoneColor = new Color(0.4f, 1f, 0.4f);   // green
    public Color player4ZoneColor = new Color(1f, 1f, 0.4f);     // yellow


    [Header("Player Start Zone Sizes")]
    public int playerZoneWidth = 6;   // full grid width by default
    public int playerZoneHeight = 3; // 3 rows tall by default


    private Tile[,] grid;


    public static GridManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

  


    public void GenerateGrid()
    {
        grid = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * spacing, 0, y * spacing);
                GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                Tile tile = tileObj.GetComponent<Tile>();
                tile.SetPosition(x, y);
                grid[x, y] = tile;
            }
        }

        Debug.Log("Grid generated: " + width + " x " + height);
        GeneratePlayerZones();
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return null;
        return grid[x, y];
    }
    void GeneratePlayerZones()
    {

        numPlayers = Mathf.Clamp(numPlayers, 2, 4);

        player1StartTiles.Clear();
        player2StartTiles.Clear();
        player3StartTiles.Clear();
        player4StartTiles.Clear();

        int xStart = (width - playerZoneWidth) / 2;
        int xEnd = xStart + Mathf.Min(playerZoneWidth, width);
        int yHeight = Mathf.Min(playerZoneHeight, height);

        // Player 1 (bottom zone)
        for (int x = xStart; x < xEnd; x++)
        {
            for (int y = 0; y < yHeight; y++)
            {
                Tile tile = grid[x, y];
                player1StartTiles.Add(tile);
                tile.GetComponent<Renderer>().material.color = player1ZoneColor;
            }
        }

        // Player 2 (top zone)
        for (int x = xStart; x < xEnd; x++)
        {
            for (int y = height - yHeight; y < height; y++)
            {
                Tile tile = grid[x, y];
                player2StartTiles.Add(tile);
                tile.GetComponent<Renderer>().material.color = player2ZoneColor;
            }
        }

        // Player 3 (left side)
        if (numPlayers >= 3)
        {
            int yStart = (height - playerZoneWidth) / 2;
            int yEnd = yStart + Mathf.Min(playerZoneWidth, height); // <- width used as height
            for (int x = 0; x < Mathf.Min(playerZoneHeight, width); x++) // <- height used as width
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    Tile tile = grid[x, y];
                    player3StartTiles.Add(tile);
                    tile.GetComponent<Renderer>().material.color = player3ZoneColor;
                }
            }
        }

        // Player 4 (right side)
        if (numPlayers >= 4)
        {
            int yStart = (height - playerZoneWidth) / 2;
            int yEnd = yStart + Mathf.Min(playerZoneWidth, height);
            for (int x = width - Mathf.Min(playerZoneHeight, width); x < width; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    Tile tile = grid[x, y];
                    player4StartTiles.Add(tile);
                    tile.GetComponent<Renderer>().material.color = player4ZoneColor;
                }
            }
        }


    }
    public Tile GetTileAtPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / spacing);
        int y = Mathf.RoundToInt(worldPosition.z / spacing);

        return GetTileAt(x, y);
    }



    public void CenterCameraAboveGrid()
    {
        Vector3 center = GetGridCenter();

        Camera cam = Camera.main;
        if (cam == null) return;

        // Assume orthographic camera looking straight down
        if (!cam.orthographic)
        {
            Debug.LogWarning("Camera must be orthographic for top-down view.");
            return;
        }

        // Move camera directly above center
        Vector3 camPos = center;
        camPos.y = 10f; // default height, will adjust below
        cam.transform.position = camPos;
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Fit the grid in view
        float gridAspect = (float)width / height;
        float screenAspect = (float)Screen.width / Screen.height;

        // Determine which size (width or height) is the limiting factor
        float neededHalfSize = spacing * Mathf.Max(width, height) / 2f;

        if (gridAspect > screenAspect)
        {
            // Wider than screen, fit width
            cam.orthographicSize = neededHalfSize / screenAspect;
        }
        else
        {
            // Taller or square, fit height
            cam.orthographicSize = neededHalfSize;
        }
    }
    public Vector3 GetGridCenter()
    {
        float centerX = (width - 1) * spacing / 2f;
        float centerZ = (height - 1) * spacing / 2f;
        return new Vector3(centerX, 0f, centerZ);
    }


}
