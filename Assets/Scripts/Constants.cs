using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    // Tilesheet
    //public const int TILESHEET_COLUMNS_AMOUNT = 48;
    //public const int TILESHEET_LINES_AMOUNT = 22;

    // Tiles
    public const int TILE_FINAL_SIZE = 14;
    public const int TILE_BASE_SIZE = 16;
    //public const float TILE_SIZE_IN_WORLD_SPACE = 1f;

    public const string TARGET_LAYER_NAME = "Target";

    // Grid
    public const int GRID_SIZE = 9; // Assumed to be uneven a bit everywhere
    public const int RESOLUTION_UPSCALE = 4;


    // Commands
    public const KeyCode FLOOD_KEYCODE = KeyCode.F;
    public const KeyCode STOP_FLOOD_KEYCODE = KeyCode.S;
    public const KeyCode RESTART_KEYCODE = KeyCode.R;
    public const KeyCode CONTINUE_KEYCODE = KeyCode.C;
}
