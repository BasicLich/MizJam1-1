using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTile : GameTile
{
    // Starts from upper-left corner, goes clockwise
    bool[] coloredEdges;

    public bool IsSuccess { get; private set; }
    public Texture2D readTexture;
    const int sampleTextureSize = 1; //25 * 4;
    int currentSide = 0;

    public RenderTexture sampledTexture;

    const float epsilon = 0.001f;

    protected override void Start()
    {
        base.Start();
        coloredEdges = new bool[Constants.TILE_FINAL_SIZE * 4 - 4];
        Texture2D baseTexture = renderer.sharedMaterial.GetTexture(TEXTURE_PROPERTY) as Texture2D;
        int baseX = tileData.column * Constants.TILE_BASE_SIZE + tileData.offset.x + 1;
        int baseY = tileData.row * Constants.TILE_BASE_SIZE + tileData.offset.y + 1;

        //string outputDebug = "";
        //float startTime = Time.realtimeSinceStartup;

        for (int side = 0; side < 4; side++)
        {
            for (int i = 0; i < Constants.TILE_FINAL_SIZE - 1; i++)
            {
                int localX = side == 0 ? i
                    : side == 1 ? Constants.TILE_FINAL_SIZE - 1
                    : side == 2 ? Constants.TILE_FINAL_SIZE - 1 - i
                    : 0;
                int localY = side == 0 ? Constants.TILE_FINAL_SIZE - 1
                    : side == 1 ? Constants.TILE_FINAL_SIZE - 1 - i
                    : side == 2 ? 0
                    : i;

                bool colored = baseTexture.GetPixel(baseX + localX, baseY + localY).a > 0;
                coloredEdges[side * (Constants.TILE_FINAL_SIZE - 1) + i] = colored;
                //outputDebug += "Side " + side + ", pixel " + i + " (" + (baseX + localX) + ":" + (baseY + localY) + "): " + colored + "\n";
            }
        }
        //Debug.Log("Got colored edges: " + outputDebug);
        //Debug.Log("Took " + (Time.realtimeSinceStartup - startTime)*1000 + "ms");

        if (Application.isPlaying)
        {
            readTexture = new Texture2D(sampleTextureSize, sampleTextureSize, TextureFormat.ARGB32, false);
            
        }
    }

    public void ResetTarget()
    {
        IsSuccess = false;
    }

    public bool CheckSuccess()
    {
        if (!IsSuccess)
        {
            RenderTexture resultTexture = GameManager.Instance.Grid.FloodRenderTexture;
            RenderTexture.active = resultTexture;

            int baseX = (int) ((transform.localPosition.x + (Constants.GRID_SIZE - 1) / 2) * Constants.TILE_FINAL_SIZE);
            int baseY = (int)((transform.localPosition.y + (Constants.GRID_SIZE - 1) / 2) * Constants.TILE_FINAL_SIZE);
            for (int i = 0; i <= Constants.TILE_FINAL_SIZE; i++)
            {
                int edgeIndex = currentSide * (Constants.TILE_FINAL_SIZE - 1) + i;
                // TOOD: Precalculate all?
                if ((i < Constants.TILE_FINAL_SIZE && coloredEdges[edgeIndex % coloredEdges.Length]) ||
                    (i > 0 && coloredEdges[(edgeIndex - 1) % coloredEdges.Length]) ||
                    (i > 1 && coloredEdges[(edgeIndex - 2) % coloredEdges.Length]))
                {
                    int localX = currentSide == 0 ? i - 1
                        : currentSide == 1 ? Constants.TILE_FINAL_SIZE
                        : currentSide == 2 ? Constants.TILE_FINAL_SIZE - i
                        : -1;
                    int localY = currentSide == 0 ? Constants.TILE_FINAL_SIZE
                        : currentSide == 1 ? Constants.TILE_FINAL_SIZE - i
                        : currentSide == 2 ? -1
                        : i - 1;

                    Rect sampleRect = new Rect((baseX + localX) * Constants.RESOLUTION_UPSCALE + 1, resultTexture.height - ((baseY + localY) * Constants.RESOLUTION_UPSCALE + 1), sampleTextureSize, sampleTextureSize);
                    readTexture.ReadPixels(sampleRect, 0, 0);
                    readTexture.Apply();
                    Color c = readTexture.GetPixel(0, 0);
                    //if (c.r > 0 || c.g > 0 || c.b > 0)
                    //{
                    //    Debug.Log("Target " + gameObject.name + " has color " + c + " on side " + currentSide + ", i " + i + "; compared to " + tileData.color + " (is same: " + (c == tileData.color) + ") - " + Time.frameCount, gameObject);
                    //}
                    if (Mathf.Abs(c.r - tileData.color.r) < epsilon &&
                        Mathf.Abs(c.g - tileData.color.g) < epsilon &&
                        Mathf.Abs(c.b - tileData.color.b) < epsilon)
                    {
                        IsSuccess = true;
                        break;
                    }
                }
            }

            sampledTexture = RenderTexture.active;
            RenderTexture.active = null;

            currentSide++;
            currentSide %= 4;
        }
        return IsSuccess;
    }
}
