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
        public void SetColor(Color c)
        {
            color = c;
        }
        public void SetParams(int _column, int _row, Color _color)
        {
            column = _column;
            row = _row;
            color = _color;
        }
    }
    [SerializeField, HideInInspector]
    new protected Renderer renderer;

    [SerializeField]
    protected TileData tileData;

    public bool alphaBlended;

    MaterialPropertyBlock propertyBlock;

    public const string TEXTURE_PROPERTY = "_MainTex";
    public const string COLUMN_PROPERTY = "_Column";
    public const string ROW_PROPERTY = "_Row";
    public const string COLOR_PROPERTY = "_Color";
    public const string OFFSET_PROPERTY = "_Offset";


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
        }
    }

    public void SetTileIndex(int _column, int _row)
    {
        SetTileIndex(_column, _row, tileData.color);
    }
    public void SetTileIndex(int _column, int _row, Color _color)
    {
        tileData.SetParams(_column, _row, _color);
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
        renderer.SetPropertyBlock(propertyBlock);
    }
}
