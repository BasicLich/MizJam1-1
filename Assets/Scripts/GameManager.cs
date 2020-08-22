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
    public GameObject floodLabel, stopLabel, restartLabel, continueLabel;

    [Space()]
    [SerializeField]
    Material tileMaterial;
    public Material TileMaterial { get { return tileMaterial; } }
    [SerializeField]
    Material tileAlphaMaterial;
    public Material TileAlphaMaterial { get { return tileAlphaMaterial; } }

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
            if (CurrentLevel.IsWon && Input.GetKeyDown(Constants.CONTINUE_KEYCODE)
#if UNITY_EDITOR
                || (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.RightArrow))
#endif
                )
            {
                CurrentLevel.StopLevel();
                currentLevelIndex++;
                //TODO: Check if game has ended
                StartCurrentLevel();
            }
            else if (!CurrentLevel.IsWon && Input.GetKeyDown(Constants.RESTART_KEYCODE))
            {
                CurrentLevel = levels[currentLevelIndex].StartLevel();
            }

            floodLabel.SetActive(!CurrentLevel.IsWon && !CurrentLevel.IsFlooding && (currentLevelIndex > 0 || CurrentLevel.HasMoved));
            stopLabel.SetActive(!CurrentLevel.IsWon && CurrentLevel.IsFlooding && (currentLevelIndex > 0 || CurrentLevel.HasMoved));
            restartLabel.SetActive(!CurrentLevel.IsWon && (currentLevelIndex > 0 || CurrentLevel.HasMoved));
            continueLabel.SetActive(CurrentLevel.IsWon);
        }
    }

    void StartCurrentLevel()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levels.Length)
        {
            CurrentLevel = levels[currentLevelIndex].StartLevel();
        }
    }
}
