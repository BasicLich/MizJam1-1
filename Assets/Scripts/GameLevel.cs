using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : MonoBehaviour
{
    GameTile[][] gridTiles;

    PlayerTile player;
    List<TargetTile> targets;

    public bool IsFlooding { get; private set; }
    public bool IsWon { get; private set; }
    public bool HasMoved { get; private set; }

    GameLevel lastInstantiatedLevel;

    public GameLevel StartLevel()
    {
        if (lastInstantiatedLevel != null)
        {
            if (lastInstantiatedLevel.IsFlooding)
            {
                GameManager.Instance.Grid.StopFlood();
            }
            Destroy(lastInstantiatedLevel.gameObject);
        }

        GameObject newLevelGo = Instantiate(gameObject);
        newLevelGo.SetActive(true);
        newLevelGo.transform.SetParent(transform.parent);
        newLevelGo.transform.SetPositionAndRotation(transform.position, transform.rotation);
        newLevelGo.transform.localScale = transform.localScale;
        lastInstantiatedLevel = newLevelGo.GetComponent<GameLevel>();
        lastInstantiatedLevel.Init();
        return lastInstantiatedLevel;
    }

    public void StopLevel()
    {
        if (IsFlooding)
        {
            GameManager.Instance.Grid.StopFlood();
        }
        Destroy(gameObject);
    }

    void Init()
    {
        targets = new List<TargetTile>();

        gridTiles = new GameTile[Constants.GRID_SIZE][];
        for (int i = 0; i < gridTiles.Length; i++)
        {
            gridTiles[i] = new GameTile[Constants.GRID_SIZE];
        }

        GameTile[] tiles = GetComponentsInChildren<GameTile>();
        for (int i = 0; i < tiles.Length; i++)
        {
            int x = PositionToIndex(tiles[i].transform.localPosition.x);
            int y = PositionToIndex(tiles[i].transform.localPosition.y);

            if (x >= 0 && y >= 0 && x < Constants.GRID_SIZE && y < Constants.GRID_SIZE)
            {
                if (gridTiles[x][y] == null)
                {
                    tiles[i].transform.localPosition = new Vector3(IndexToPosition(x), IndexToPosition(y), tiles[i].transform.localPosition.z);
                    gridTiles[x][y] = tiles[i];
                }
                else
                {
                    Debug.LogError("Several tiles at the same spot.", tiles[i].gameObject);
                    tiles[i].gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("Tile out of bounds.", tiles[i].gameObject);
                tiles[i].gameObject.SetActive(false);
            }

            if (tiles[i].gameObject.activeSelf)
            {
                if (tiles[i] is PlayerTile)
                {
                    if (player == null)
                    {
                        player = tiles[i] as PlayerTile;
                    }
                    else
                    {
                        Debug.LogError("Two player tiles.", gameObject);
                    }
                }
                if (tiles[i] is TargetTile)
                {
                    targets.Add(tiles[i] as TargetTile);
                }
            }
        }

        if (player == null)
        {
            Debug.LogError("Error: no player in level.", gameObject);
        }
        if (targets.Count == 0)
        {
            Debug.LogError("Error: no target in level.", gameObject);
        }

        //Debug.Log("Level intiated with " + targets.Count + " targets.");
    }

    void Update()
    {
        if (GameManager.Instance.CurrentLevel == this && player != null && !IsWon)
        {
            if (!IsFlooding)
            {
                if (Input.GetKeyDown(Constants.FLOOD_KEYCODE))
                {
                    IsFlooding = true;
                    GameManager.Instance.Grid.StartFlood();
                }
                else
                {
                    // Player movement
                    Vector2Int movementDirection = Vector2Int.zero;
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        movementDirection.y = -1;
                    }
                    else if(Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        movementDirection.y = 1;
                    }
                    else if(Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        movementDirection.x = -1;
                    }
                    else if(Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        movementDirection.x = 1;
                    }
                    if (movementDirection.x != 0 ||
                        movementDirection.y != 0)
                    {
                        Vector2Int currentPlayerCoordinates = GetTileCoordinate(player);
                        Vector2Int destination = currentPlayerCoordinates + movementDirection;
                        if (IsDestinationOnGrid(destination))
                        {
                            bool movePlayer = false;
                            if (gridTiles[destination.x][destination.y] != null)
                            {
                                Vector2Int existingTileDestination = destination + movementDirection;
                                if (gridTiles[destination.x][destination.y].CanBeMoved &&
                                    IsDestinationOnGrid(existingTileDestination) &&
                                    gridTiles[existingTileDestination.x][existingTileDestination.y] == null)
                                {
                                    movePlayer = true;
                                    GameTile tileToMove = gridTiles[destination.x][destination.y];
                                    tileToMove.transform.localPosition = new Vector3(IndexToPosition(existingTileDestination.x), IndexToPosition(existingTileDestination.y), tileToMove.transform.localPosition.z);
                                    gridTiles[destination.x][destination.y] = null;
                                    gridTiles[existingTileDestination.x][existingTileDestination.y] = tileToMove;
                                }
                            }
                            else
                            {
                                movePlayer = true;
                            }
                            if (movePlayer)
                            {
                                player.transform.localPosition = new Vector3(IndexToPosition(destination.x), IndexToPosition(destination.y), player.transform.localPosition.z);
                                gridTiles[destination.x][destination.y] = player;
                                gridTiles[currentPlayerCoordinates.x][currentPlayerCoordinates.y] = null;
                                HasMoved = true;
                                //Debug.Log("New player position " + destination);
                            }
                        }

                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(Constants.STOP_FLOOD_KEYCODE))
                {
                    IsFlooding = false;
                    GameManager.Instance.Grid.StopFlood();
                    IsWon = false;
                    for (int i = 0; i < targets.Count; i++)
                    {
                        targets[i].ResetTarget();
                    }
                }
                else
                {
                    bool isSuccess = true;
                    for (int i = 0; i < targets.Count; i++)
                    {
                        isSuccess &= targets[i].CheckSuccess();
                    }
                    if (isSuccess)
                    {
                        // Victory!
                        IsWon = true;
                    }
                }
            }
        }
    }

    int PositionToIndex(float localPosition)
    {
        return Mathf.RoundToInt(localPosition) + (Constants.GRID_SIZE - 1) / 2;
    }
    float IndexToPosition(int index)
    {
        return index - (Constants.GRID_SIZE - 1) / 2f;
    }

    Vector2Int GetTileCoordinate(GameTile tile)
    {
        for (int i = 0; i < gridTiles.Length; i++)
        {
            for (int j = 0; j < gridTiles[i].Length; j++)
            {
                if (gridTiles[i][j] == tile)
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        Debug.LogError("Error: couldn't find coordinates for tile " + tile, tile.gameObject);
        return Vector2Int.zero;
    }
    bool IsDestinationOnGrid(Vector2Int destination)
    {
        return destination.x >= 0 && destination.y >= 0 && destination.x < Constants.GRID_SIZE && destination.y < Constants.GRID_SIZE;
    }

    [ContextMenu("OrderTiles")]
    public void OrderTiles()
    {
        GameTile[] tiles = GetComponentsInChildren<GameTile>();
        for (int i = 0; i < tiles.Length; i++)
        {
            Vector3 lp = tiles[i].transform.localPosition;
            tiles[i].transform.localPosition = new Vector3(Mathf.RoundToInt(lp.x), Mathf.RoundToInt(lp.y), lp.z);
        }
    }
}
