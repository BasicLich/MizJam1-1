using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileAnim", menuName = "Flood/TileAnim")]
public class TileAnim : ScriptableObject
{
    [System.Serializable]
    public struct AnimFrame
    {
        public Tile.TileData tileData;
        public float duration;
    }
    public AnimFrame[] frames;
    public bool loop;
    public bool color;
}
