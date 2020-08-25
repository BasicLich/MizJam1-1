using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    [Header("Levels")]
    public GameLevel[] levels;

    [Header("References")]
    [SerializeField]
    Grid grid;
    public Grid Grid { get { return grid; } }
    public GameObject floodLabel, stopLabel, restartLabel, continueLabel, creditsLabel;

    [Space()]
    [SerializeField]
    Material tileMaterial;
    public Material TileMaterial { get { return tileMaterial; } }
    [SerializeField]
    Material tileAlphaMaterial;
    public Material TileAlphaMaterial { get { return tileAlphaMaterial; } }
    [SerializeField]
    Material simpleTextureMaterial;
    public Material SimpleTextureMaterial { get { return simpleTextureMaterial; } }

    [Header("Settings")]
    public Color edgeColor = Color.gray;

    // Game variables
    public GameLevel CurrentLevel { get; private set; }
    int currentLevelIndex = -1;

    private void Start()
    {
        floodLabel.SetActive(false);
        stopLabel.SetActive(false);
        restartLabel.SetActive(false);
        continueLabel.SetActive(false);

        if (levels.Length > 0)
        {
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i].gameObject.SetActive(false);
            }

            currentLevelIndex = 0;
            StartCurrentLevel();
        }
    }

    private void Update()
    {
        if (CurrentLevel != null)
        {
            if (CurrentLevel.IsWon && Input.GetKeyDown(Constants.CONTINUE_KEYCODE) && !IsLastLevel() ||
                (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.RightArrow)))
            {
                CurrentLevel.StopLevel();
                currentLevelIndex++;
                StartCurrentLevel();
            }
#if UNITY_EDITOR
            else if (Input.GetKeyDown(Constants.RESTART_KEYCODE))
#else 
            else if (!CurrentLevel.IsWon && Input.GetKeyDown(Constants.RESTART_KEYCODE))
#endif
            {
                CurrentLevel = levels[currentLevelIndex].StartLevel();
            }

            floodLabel.SetActive((!CurrentLevel.IsWon && !CurrentLevel.IsFlooding) || IsLastLevel());
            stopLabel.SetActive(!CurrentLevel.IsWon && CurrentLevel.IsFlooding && (currentLevelIndex > 0 || CurrentLevel.HasMoved) && !IsLastLevel());
            restartLabel.SetActive(!CurrentLevel.IsWon && (currentLevelIndex > 0 || CurrentLevel.HasMoved) && !IsLastLevel());
            continueLabel.SetActive(CurrentLevel.IsWon && !IsLastLevel());
            creditsLabel.SetActive((currentLevelIndex == 0 && !CurrentLevel.HasMoved && !CurrentLevel.IsWon) || IsLastLevel());
        }


        // Just in case it can be dynamically resized during gameplay; the goal is to have a pixel-perfect look, where each asset pixel
        // takes an integer amount of screen pixel to render. Doesn't seem to work so well in editor though :/
        int playgroundPixelsHeight = Constants.GRID_SIZE * Constants.TILE_FINAL_SIZE;
        int ppp = Screen.height / playgroundPixelsHeight;
        //Debug.Log("PlaygroundPixelsHeight " + playgroundPixelsHeight + " and Screen " + Screen.height + " gives ppp " + ppp);
        if (ppp >= 1)
        {
            float tilesAmount = Constants.GRID_SIZE + (Screen.height - playgroundPixelsHeight * ppp) / ((float)Constants.TILE_FINAL_SIZE * ppp);
            //Debug.Log("That gives a tilesAmount " + tilesAmount);
            Camera.main.orthographicSize = tilesAmount / 2f;
        }
    }

    void StartCurrentLevel()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levels.Length)
        {
            CurrentLevel = levels[currentLevelIndex].StartLevel();
        }
    }

    bool IsLastLevel()
    {
        return currentLevelIndex == levels.Length - 1;
    }
}
