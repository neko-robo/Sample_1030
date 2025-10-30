using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constant
{

    public const int BLOCK_HP = 3;
    public const float defaultColor = 0f;
    public const float wallRGB = 0.443f;
    public const float wallA = 1.0f;

    public const float NAIL_DIRECTION = 0.4f;

    public const float LOT_L_X_MIN = -7.2f;
    public const float LOT_L_X_MAX = -4.2f;
    public const float LOT_L_Y_MIN = -4.0f;
    public const float LOT_L_Y_MAX = 4.2f;

    public const float LOT_R_X_MIN = 4.2f;
    public const float LOT_R_X_MAX = 7.2f;
    public const float LOT_R_Y_MIN = -4.0f;
    public const float LOT_R_Y_MAX = 4.2f;

    public const float LOT_0_X_MIN = -7.2f;
    public const float LOT_0_X_MAX = -4.2f;
    public const float LOT_0_Y_MIN = 0.1f;
    public const float LOT_0_Y_MAX = 4.0f;

    public const float LOT_PU_X_MIN = 7.64f;
    public const float LOT_PU_X_MAX = 9.52f;


    public const float STG_X_MIN = -3.92f;
    public const float STG_X_MAX = 3.92f;
    public const float STG_Y_MIN = -3.92f;
    public const float STG_Y_MAX = 4.08f;

    public const float STG_SPACE_X_BY_TEAM = 4.00f;
    public const float STG_SPACE_Y_BY_TEAM = 4.00f;

    public const float BALL_DEFAULT_X = -5.7f;
    public const float BALL_DEFAULT_Y = 3.55f;

    public const float LOT_EXPAND_TIME = 650.0f;
    public const float LOT_EXPAND_RANGE = 1.3f;
    public const float LOT_EXPAND_ACCELERATION = 18.0f;

    public const float CANNON_DEFAULT_X = -3.4f;
    public const float CANNON_DEFAULT_Y = 3.4f;
    public const float CANNON_ANGLE_TOP_MIN = -30f;
    public const float CANNON_ANGLE_BOT_MIN = 60f;
    public const float CANNON_ANGLE_RANGE = 150f;

    public const int LAYER_NUM_TEAM_0 = 13;
    public const int LAYER_WHITE = 17;
    public const int LAYER_MOB_NUM_TEAM_0 = 18;
    public const int LAYER_MOBSEARCH = 22;
    public const int LAYER_WHITEMOB = 23;
    public const int LAYER_BULLET_NUM_TEAM_0 = 24;
    public const int LAYERMASK_ALL = -1;
    public const int LAYERMASK_CANNON = 4096;
    public const int LAYERMASK_BLUE = 8192;
    public const int LAYERMASK_RED = 16384;
    public const int LAYERMASK_GREEN = 32768;
    public const int LAYERMASK_YELLOW = 65536;
    public const int LAYERMASK_WHITE = 131072;

    public const int LAYERMASK_BLUEMOB = 262144;
    public const int LAYERMASK_REDMOB = 524288;
    public const int LAYERMASK_GREENMOB = 1048576;
    public const int LAYERMASK_YELLOWMOB = 2097152;

    public const int LAYERMASK_BLUE_BULLET = 16777216;
    public const int LAYERMASK_RED_BULLET = 33554432;
    public const int LAYERMASK_GREEN_BULLET = 67108864;
    public const int LAYERMASK_YELLOW_BULLET = 134217728;
    public const float SCALE_ADJUST_WAIT = 0.35f;

    public const int LAYERMASK_WALL = 256;
    public const int LAYERMASK_BLUEMOB_BLUE = 270336;
    public const int LAYERMASK_REDMOB_RED = 540672;
    public const int LAYERMASK_GREENMOB_GREEN = 1081344;
    public const int LAYERMASK_YELLOWMOB_YELLOW = 2162688;
    public const int LAYERMASK_MOBSEARCH = 4194304;
    public const int LAYERMASK_WHITEMOB = 8388608;
    public const int LAYERMASK_ALLCOLORMOB = 3932160;
    public const int LAYERMASK_ALLCOLOR_COLORMOB = 4055040;
    public const int LAYERMASK_ALLBULLET = 251658240;

    public const int LAYERMASK_MONOBLOCK_ONLY = 268435456;
    public const int LAYERMASK_RAINBLOCK_ONLY = 536870912;

    public const float TANK_DEAD_TIME = 0.5f;
    public const float BOSS_DEAD_TIME = 2.0f;
    public const float BULLET_DEAD_TIME = 0.2f;
    public const float BLINK_ALPHA = 0.4f;
    public const float BLINK_TIME = 0.15f;
    //public const float MULTI_TXT_DEFAULT_X = -0.67f;
    //public const float RELEASE_TXT_DEFAULT_X = 1.25f;
    //public const float TXT_DEFAULT_Y = -4.125f;
    public const bool OLD_LOTTERY_ACTIVE = false;
    public const bool SCALE_SMALLER = false;

    public const int CIRCLE_BLOCK_COUNT_BY_LINE = 16;

    public const int MONO_BULLET_LAYER = 26;
    public const int RAIN_BULLET_LAYER = 27;

    public const int TROUGH_LAYER = 12;

    public const int MONO_BLOCK_LAYER = 28;
    public const int RAIN_BLOCK_LAYER = 29;

    public const int GREEN_LEAF_LAYER = 24;
    public const int PURPLE_LEAF_LAYER = 25;

    public const int GREEN_BULLET_LAYER = 26;
    public const int PURPLE_BULLET_LAYER = 27;

    public const int GREEN_TREE_LAYER = 28;
    public const int PURPLE_TREE_LAYER = 29;

    public const int STICK0_LAYER = 20;
    public const int STICK1_LAYER = 21;

    public const int LAYERMASK_STICK0_ONLY = 1048576;
    public const int LAYERMASK_STICK1_ONLY = 2097152;

    public const int LAYERMASK_ALL_WITHOUT_STICK0 = 2146172791;
    public const int LAYERMASK_ALL_WITHOUT_STICK1 = 2145124215;

    public const int LAYERMASK_GREEN_ONLY = 285212672;
    public const int LAYERMASK_PURPLE_ONLY = 570425344;


    public const bool BRANCH_STARIGHT_ALL = true;
    public const bool BRANCH_LEAF_ALL = true;
    //public const int CIRCLE_BLOCK_LINE_COUNT = 15;
    public enum ColorType{
        wall,
        red,
        green,
        blue,
        yellow,
        none
    }



}
