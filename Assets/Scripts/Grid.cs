using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
    [Header("References")]
    public GameObject gridEdgeContainer;
    public GameObject tilePrefab;

    [Header("Helpers")]
    public bool regenGrid;
    public Vector2Int topTile, rightTile, bottomTile, leftTile, topRightTile, bottomRightTile, bottomLeftTile, topLeftTile;

    // Flood
    public Material simpleTextureMaterial;
    public Material floodMaterial;
    public float floodSpeed = 0.3f;
    Camera floodCamera;
    public RenderTexture FloodRenderTexture { get; private set; }
    RenderTexture iFloodRenderTexture;
    Renderer floodRenderer;
    bool isFlooding;
    float nextFloodDate;

    Tile[] edgeTiles;
    float floodColorAnimStartDate = -1f;
    float floodColorAnimFrameDuration = 0.05f;
    int floodColorAnimStatesAmount = 20;
    float floodColorAnimStateAmplitude = 0.03f;
    int currentEdgeColorStep;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying)
        {
            GameObject floodCameraGo = new GameObject("FloodCamera");
            floodCameraGo.transform.SetParent(gameObject.transform);
            floodCameraGo.transform.localPosition = Vector3.zero - Vector3.forward * 1f;
            floodCameraGo.transform.localRotation = Quaternion.identity;
            floodCameraGo.transform.localScale = Vector3.one;
            floodCamera = floodCameraGo.AddComponent<Camera>();
            floodCamera.orthographic = true;
            floodCamera.orthographicSize = Constants.GRID_SIZE / 2f;
            floodCamera.clearFlags = CameraClearFlags.SolidColor;
            floodCamera.backgroundColor = Color.clear;
            floodCamera.cullingMask = ~LayerMask.GetMask(Constants.TARGET_LAYER_NAME);

            FloodRenderTexture = new RenderTexture(Constants.GRID_SIZE * Constants.RESOLUTION_UPSCALE * Constants.TILE_FINAL_SIZE, Constants.GRID_SIZE * Constants.RESOLUTION_UPSCALE * Constants.TILE_FINAL_SIZE, 0, RenderTextureFormat.ARGB32, 0);
            FloodRenderTexture.filterMode = FilterMode.Point;
            FloodRenderTexture.Create();

            iFloodRenderTexture = new RenderTexture(Constants.GRID_SIZE * Constants.RESOLUTION_UPSCALE * Constants.TILE_FINAL_SIZE, Constants.GRID_SIZE * Constants.RESOLUTION_UPSCALE * Constants.TILE_FINAL_SIZE, 0, RenderTextureFormat.ARGB32, 0);
            iFloodRenderTexture.filterMode = FilterMode.Point;
            iFloodRenderTexture.Create();

            floodCamera.targetTexture = FloodRenderTexture;

            GameObject floodRendererGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
            floodRendererGo.transform.SetParent(gameObject.transform);
            floodRendererGo.transform.localPosition = Vector3.zero - Vector3.forward * 0.1f;
            floodRendererGo.transform.localRotation = Quaternion.identity;
            floodRendererGo.transform.localScale = new Vector3(Constants.GRID_SIZE, Constants.GRID_SIZE, 1f);
            floodRenderer = floodRendererGo.GetComponent<Renderer>();
            floodRenderer.sharedMaterial = simpleTextureMaterial;
            simpleTextureMaterial.SetTexture("_MainTex", FloodRenderTexture);

            floodCameraGo.SetActive(false);
            floodRendererGo.SetActive(false);

            floodMaterial.SetTexture("_MainTex", FloodRenderTexture);
            floodMaterial.SetFloat("_PixelSize", 1f / FloodRenderTexture.width);

            edgeTiles = gridEdgeContainer.GetComponentsInChildren<Tile>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (regenGrid)
        {
            RegenGrid();
            regenGrid = false;
        }

        if (Application.isPlaying &&
            GameManager.Instance.CurrentLevel != null &&
            GameManager.Instance.CurrentLevel.IsFlooding)
        {
            while (Time.realtimeSinceStartup >= nextFloodDate)
            {
                Graphics.Blit(FloodRenderTexture, iFloodRenderTexture, floodMaterial);
                Graphics.Blit(iFloodRenderTexture, FloodRenderTexture);
                nextFloodDate += 1f / (60f * floodSpeed);
            }

            int edgeColorStep = Mathf.CeilToInt((Time.time - floodColorAnimStartDate) / floodColorAnimFrameDuration) + floodColorAnimStatesAmount / 2;
            edgeColorStep %= (floodColorAnimStatesAmount * 2);
            if (edgeColorStep >= floodColorAnimStatesAmount)
                edgeColorStep = (floodColorAnimStatesAmount * 2) - edgeColorStep;
            if (edgeColorStep != currentEdgeColorStep)
            {
                currentEdgeColorStep = edgeColorStep;
                float amplitude = (currentEdgeColorStep - floodColorAnimStatesAmount / 2) * floodColorAnimStateAmplitude;
                amplitude += GameManager.Instance.CurrentLevel.IsWon ? 0.3f : -0.2f;
                Color c = GameManager.Instance.edgeColor;
                c.r += amplitude;
                c.g += amplitude;
                c.b += amplitude;
                ColorEdge(c);
            }
        }
    }

    public void StartFlood()
    {
        floodCamera.gameObject.SetActive(true);
        floodCamera.Render();
        floodCamera.gameObject.SetActive(false);
        floodRenderer.gameObject.SetActive(true);
        nextFloodDate = Time.realtimeSinceStartup;
        floodColorAnimStartDate = Time.time;
        currentEdgeColorStep = -1;
    }
    public void StopFlood()
    {
        floodRenderer.gameObject.SetActive(false);
        ColorEdge(GameManager.Instance.edgeColor);
    }

    void RegenGrid()
    {
        //gridSizeX = Mathf.Max(1, gridSizeX);
        //gridSizeY = Mathf.Max(1, gridSizeY);

        while (gridEdgeContainer.transform.childCount > 0)
        {
            if (Application.isPlaying)
                Destroy(gridEdgeContainer.transform.GetChild(0).gameObject);
            else
                DestroyImmediate(gridEdgeContainer.transform.GetChild(0).gameObject);
        }

        float horizontalEdgePosition = (Constants.GRID_SIZE - 1) / 2f + 1f;
        float verticalEdgePosition = (Constants.GRID_SIZE - 1) / 2f + 1f;
        for (int flip = -1; flip <= 1; flip += 2)
        {
            for (float x = -verticalEdgePosition; x <= verticalEdgePosition; x++)
            {
                Vector2Int tileIndex = x == -verticalEdgePosition ? (flip > 0 ? topLeftTile : bottomLeftTile)
                    : x == verticalEdgePosition ? (flip > 0 ? topRightTile : bottomRightTile)
                    : (flip > 0 ? topTile : bottomTile);
                AddEdgeTile(new Vector3(x, flip * horizontalEdgePosition, 0), tileIndex);
            }
            for (float y = -horizontalEdgePosition + 1; y <= horizontalEdgePosition - 1; y++)
            {
                Vector2Int tileIndex = flip > 0 ? rightTile : leftTile;
                AddEdgeTile(new Vector3(flip * verticalEdgePosition, y, 0), tileIndex);
            }
        }
    }
    void AddEdgeTile(Vector3 worldPosition, Vector2Int tileIndex)
    {
        GameObject newTile = Instantiate(tilePrefab);
        newTile.transform.SetParent(gridEdgeContainer.transform);
        newTile.transform.SetPositionAndRotation(worldPosition, Quaternion.identity);
        newTile.transform.localScale = Vector3.one;
        Tile tile = newTile.GetComponent<Tile>();
        tile.SetTileIndex(tileIndex.x, tileIndex.y, GameManager.Instance.edgeColor);

    }

    void ColorEdge(Color c)
    {
        for (int i = 0; i < edgeTiles.Length; i++)
        {
            edgeTiles[i].SetColor(c);
        }
    }
}
