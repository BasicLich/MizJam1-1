using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTile : GameTile
{
    //References
    public TileAnim idleAnim, successAnim, victoryIdleAnim;

    // Starts from upper-left corner, goes clockwise
    bool[] coloredEdges;

    public bool IsSuccess { get; private set; }
    Texture2D readTexture;
    const int sampleTextureSize = 1; //25 * 4;
    int currentSide = 0;

    const float epsilon = 0.001f;

    const bool debugRenderering = false;
    const int debugRendererAdditionnalWidth = 1;
    Renderer debugRenderer;
    Texture2D debugTexture;

    protected override void Start()
    {
        base.Start();
        coloredEdges = new bool[Constants.TILE_FINAL_SIZE * 4 - 4];
        Texture2D baseTexture = renderer.sharedMaterial.GetTexture(TEXTURE_PROPERTY) as Texture2D;
        int baseX = tileData.column * Constants.TILE_BASE_SIZE + tileData.offset.x + 1;
        int baseY = tileData.row * Constants.TILE_BASE_SIZE + tileData.offset.y + 1;

        string outputDebug = "";

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
                outputDebug += "Side " + side + ", pixel " + i + " (" + (baseX + localX) + ":" + (baseY + localY) + "): " + colored + "\n";
            }
        }
        Debug.Log("Got colored edges for target " + gameObject.name + ": " + outputDebug);

        if (Application.isPlaying)
        {
            readTexture = new Texture2D(sampleTextureSize, sampleTextureSize, TextureFormat.ARGB32, false);
            ResetTarget();

            if (debugRenderering)
            {
                GameObject newRenderer = GameObject.CreatePrimitive(PrimitiveType.Quad);
                newRenderer.transform.SetParent(transform);
                newRenderer.transform.SetPositionAndRotation(transform.position + Vector3.right * 1.5f - Vector3.forward, transform.rotation);
                newRenderer.transform.localScale = Vector3.one;
                newRenderer.layer = gameObject.layer;
                debugRenderer = newRenderer.GetComponent<Renderer>();
                int debugTextureSize = (Constants.TILE_FINAL_SIZE + 2) * Constants.RESOLUTION_UPSCALE + debugRendererAdditionnalWidth;
                debugTexture = new Texture2D(debugTextureSize, debugTextureSize, TextureFormat.ARGB32, false);
                const int coloredPixelsAmount = 20*30;
                Color[] colors = new Color[coloredPixelsAmount];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = Color.red;
                }
                debugTexture.SetPixels(0, 0, 20, 30, colors);
                debugTexture.Apply();
                debugRenderer.material = GameManager.Instance.SimpleTextureMaterial;
                debugRenderer.material.SetTexture("_MainTex", debugTexture);
            }
        }
    }

    public void ResetTarget()
    {
        IsSuccess = false;
        PlayAnim(idleAnim);
    }

    public bool CheckSuccess()
    {
        if (!IsSuccess)
        {
            RenderTexture resultTexture = GameManager.Instance.Grid.FloodRenderTexture;
            RenderTexture.active = resultTexture;

            int baseX = (int) ((transform.localPosition.x + (Constants.GRID_SIZE - 1) / 2) * Constants.TILE_FINAL_SIZE);
            int baseY = (int) ((transform.localPosition.y + (Constants.GRID_SIZE - 1) / 2) * Constants.TILE_FINAL_SIZE);
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

                    float x = (baseX + localX) * Constants.RESOLUTION_UPSCALE + 1;
                    float y = resultTexture.height - ((baseY + localY) * Constants.RESOLUTION_UPSCALE + 1) - sampleTextureSize;
#if UNITY_WEBGL && !UNITY_EDITOR
                    y = resultTexture.height - y;
#endif
                    if (x >= 0 && y >= 0 && x < resultTexture.width && y < resultTexture.height)
                    {
                        Rect sampleRect = new Rect(x, y, sampleTextureSize, sampleTextureSize);
                        readTexture.ReadPixels(sampleRect, 0, 0);
                        readTexture.Apply();
                        Color c = readTexture.GetPixel(0, 0);
                        if (Mathf.Abs(c.r - tileData.color.r) < epsilon &&
                            Mathf.Abs(c.g - tileData.color.g) < epsilon &&
                            Mathf.Abs(c.b - tileData.color.b) < epsilon)
                        {
                            IsSuccess = true;
                            break;
                        }
                    }
                }
            }

            if (debugRenderering)
            {
                resultTexture = GameManager.Instance.Grid.FloodRenderTexture;
                RenderTexture.active = resultTexture;

                float x = (baseX - 1) * Constants.RESOLUTION_UPSCALE - debugRendererAdditionnalWidth / 2;
                float y = resultTexture.height - (((baseY - 1) * Constants.RESOLUTION_UPSCALE) - debugRendererAdditionnalWidth / 2) - debugTexture.height;
#if UNITY_WEBGL && !UNITY_EDITOR
                y = resultTexture.height - y;
#endif

                if (x >= 0 && y >= 0 && x + debugTexture.width < resultTexture.width && y + debugTexture.height < resultTexture.height)
                {
                    Debug.Log("Refreshing texture");
                    Rect sampleRect = new Rect(x, y, debugTexture.width, debugTexture.height);
                    debugTexture.ReadPixels(sampleRect, 0, 0);
                    debugTexture.Apply();

                    debugRenderer.material = GameManager.Instance.SimpleTextureMaterial;
                    debugRenderer.material.SetTexture("_MainTex", debugTexture);
                }
            }

            RenderTexture.active = null;

            currentSide++;
            currentSide %= 4;

            if (IsSuccess)
            {
                PlayAnim(successAnim, OnSuccessAnimEnd);
            }
        }
        return IsSuccess;
    }

    void OnSuccessAnimEnd()
    {
        PlayAnim(victoryIdleAnim);
    }
}
