using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Tile : MonoBehaviour
{
    //public enum ColorType
    //{
    //    Player,
    //    MovableTile, 
    //    ImmovableTile,
    //    KeepOriginal,
    //}
    [System.Serializable]
    public struct TileData
    {
        [Range(0, 47)]
        public int column;
        [Range(0, 21)]
        public int row;
        public Color color;
        public Vector2Int offset;
        public float angle;
        public bool horizontalFlip;
        public void SetColor(Color c)
        {
            color = c;
        }
        public void SetParams(int _column, int _row, float _angle, Color _color)
        {
            column = _column;
            row = _row;
            angle = _angle;
            color = _color;
        }
    }
    [SerializeField, HideInInspector]
    new protected Renderer renderer;

    [SerializeField]
    protected TileData tileData;

    public bool alphaBlended;

    MaterialPropertyBlock propertyBlock;

    TileAnim currentPlayedAnim;
    int currentAnimIndex;
    float currentAnimIndexStartDate;
    System.Action onCurrentAnimEnd;

    public const string TEXTURE_PROPERTY = "_MainTex";
    public const string COLUMN_PROPERTY = "_Column";
    public const string ROW_PROPERTY = "_Row";
    public const string COLOR_PROPERTY = "_Color";
    public const string OFFSET_PROPERTY = "_Offset";
    public const string ANGLE_PROPERTY = "_Angle";
    public const string HORIZONTAL_FLIP_PROPERTY = "_HorizontalFlip";


    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (renderer == null)
        {
            GameObject newQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            newQuad.transform.SetParent(transform);
            newQuad.transform.localPosition = Vector3.zero;
            newQuad.transform.localRotation = Quaternion.identity;
            newQuad.transform.localScale = Vector3.one;
            renderer = newQuad.GetComponent<Renderer>();
            renderer.sharedMaterial = alphaBlended ? GameManager.Instance.TileAlphaMaterial : GameManager.Instance.TileMaterial;
            tileData.SetColor(Color.white);
        }

        UpdatePropertyBlock();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!Application.isPlaying)
        {
            renderer.sharedMaterial = alphaBlended ? GameManager.Instance.TileAlphaMaterial : GameManager.Instance.TileMaterial;
            UpdatePropertyBlock();
            renderer.transform.localPosition = Vector3.zero;
        }
        else
        {
            UpdateAnim();
        }
    }

    public void SetTileIndex(int _column, int _row)
    {
        SetTileIndex(_column, _row, tileData.color);
    }
    public void SetTileIndex(int _column, int _row, Color _color)
    {
        tileData.SetParams(_column, _row, tileData.angle, _color);
        UpdatePropertyBlock();
    }
    public void SetColor(Color _color)
    {
        tileData.SetColor(_color);
        UpdatePropertyBlock();
    }

    void UpdatePropertyBlock()
    {
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }
        propertyBlock.SetInt(COLUMN_PROPERTY, tileData.column);
        propertyBlock.SetInt(ROW_PROPERTY, tileData.row);
        propertyBlock.SetColor(COLOR_PROPERTY, tileData.color);
        propertyBlock.SetVector(OFFSET_PROPERTY, new Vector4(tileData.offset.x, tileData.offset.y));
        propertyBlock.SetFloat(ANGLE_PROPERTY, tileData.angle);
        propertyBlock.SetFloat(HORIZONTAL_FLIP_PROPERTY, tileData.horizontalFlip ? 1 : 0);
        renderer.SetPropertyBlock(propertyBlock);
    }

    public void PlayAnim(TileAnim anim, System.Action onEndCallback = null)
    {
        if (anim.frames.Length > 0)
        {
            currentPlayedAnim = anim;
            currentAnimIndex = 0;
            ApplyAnimFrame();
        }
        else
        {
            Debug.LogError("Trying to play empty anim " + anim, anim);
            currentPlayedAnim = null;
        }
        onCurrentAnimEnd = onEndCallback;
    }
    void UpdateAnim()
    {
        if (currentPlayedAnim != null)
        {
            TileAnim.AnimFrame animFrame = currentPlayedAnim.frames[currentAnimIndex];
            if (animFrame.duration > 0)
            {
                if ((Time.time - currentAnimIndexStartDate) >= animFrame.duration)
                {
                    currentAnimIndex++;
                    if (currentPlayedAnim.loop)
                        currentAnimIndex %= currentPlayedAnim.frames.Length;

                    if (currentPlayedAnim.frames.Length > currentAnimIndex)
                    {
                        ApplyAnimFrame();
                    }
                    else
                    {
                        currentPlayedAnim = null;
                        onCurrentAnimEnd?.Invoke();
                    }
                }
            }
        }
    }
    void ApplyAnimFrame()
    {
        TileAnim.AnimFrame animFrame = currentPlayedAnim.frames[currentAnimIndex];
        tileData.SetParams(animFrame.tileData.column, animFrame.tileData.row, animFrame.tileData.angle, currentPlayedAnim.color ? animFrame.tileData.color : tileData.color);
        currentAnimIndexStartDate = Time.time;
        UpdatePropertyBlock();
    }
}
