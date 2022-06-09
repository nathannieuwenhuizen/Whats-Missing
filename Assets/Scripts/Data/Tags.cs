using UnityEngine;

public class Tags 
{
    public static string Picked = "Picked";
    public static string Stairs = "Stairs";
    public static string Death = "Death";
    public static string Environment_GRASS = "environment: grass";
}

public class Scenes {
    public static string FIRST_LEVEL_SCENE_NAME = "FirstLevels";
    public static string SECOND_LEVEL_SCENE_NAME = "SecondLevels";
    public static string THIRD_LEVEL_SCENE_NAME = "ThirdLevels";
    public static string FOURTH_LEVEL_SCENE_NAME = "Fourth_Levels";
    public static string FIFTH_LEVEL_SCENE_NAME = "Acceptance";
    public static string MENU_SCENE_NAME = "Menu";


    ///<summary>
    /// Returns the scene name associated with the area index
    ///</summary>
    public static string GetSceneNameBasedOnAreaIndex(int _areaIndex) {
        string result = Scenes.FIRST_LEVEL_SCENE_NAME;
        switch(_areaIndex) {
            case 0:
                result = Scenes.SECOND_LEVEL_SCENE_NAME;
                break;
            case 1:
                result = Scenes.SECOND_LEVEL_SCENE_NAME;
                break;
            case 2:
                result = Scenes.THIRD_LEVEL_SCENE_NAME;
                break;
            case 3:
                result = Scenes.FOURTH_LEVEL_SCENE_NAME;
                break;
            case 4:
                result = Scenes.FIFTH_LEVEL_SCENE_NAME;
                break;
        }
        return result;
    }
}