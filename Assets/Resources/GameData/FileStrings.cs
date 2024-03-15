using System;
using System.IO;
using UnityEngine.SceneManagement;
static class FileStrings
{
    // SCENE
    public static string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    // GAMEDATA
    public static string GameDataPath = "GameData";
    public static string GraphFileExtension = ".xml";
    public static string JsonExtension = ".json";

    // ENVIRONMENT
    public static string SkyBoxesPath = "Environment/Skyboxes";
    public static string EnvironmentLightsPath = "Environment/Lights";
    public static string SkyBoxNighttime = "Sunset";
    public static string SkyBoxDaytime = "Night";
    public static string Moonlight = "Moonlight";
    public static string Sunlight = "Sunlight";

    // BUTTON CONTROLLER
    public static string ButtonOpaque = "ButtonOpaque";
    public static string ButtonOpaquePushButton = "ButtonOpaque/PushButton";
    public static string ButtonTransparent= "ButtonTransparent";
    public static string ButtonTransparentPushButton = "ButtonTransparent/PushButton";
    public static string SceneryOpaque = "SceneryOpaque";
    public static string SceneryTransparent = "SceneryTransparent";

    // CONSTRAINTS OBJECT
    public static string Lock = "Lock";
    public static string Key = "Key";

    // EFFECTSCONTAINER
    public static string Glitter = "Glitter";
    public static string GlitterKey = "GlitterKey";
    public static string GlitterLock = "GlitterLock";
    public static string GlitterBurst = "GlitterBurst";
    public static string SceneryLight = "SceneryLight";
    public static string PulsatingLight = "PulsatingLight";
    public static string Fog = "Fog";
    public static string DoorAndWalls = "DoorAndWalls";
    public static string Firework = "Firework";
    public static string GodRay = "GodRay";

    // RESOURCES
    private static string Environment = "Environment";
    private static string Materials = "Materials";
    private static string Common = "Common";
    private static string ButtonGreen = "ButtonGreen";
    private static string Transparent = "Transparent";
    private static string ButtonGreenEmission = "ButtonGreenEmission";
    public static string ButtonGreenPath = Materials+"/"+Common+"/"+ButtonGreen;
    public static string ButtonGreenEmissionPath = Materials+"/"+Common+"/"+ButtonGreenEmission;
    public static string TransparentPath = Materials+"/"+Common+"/"+Transparent;

    
    // ACTIVITY - BUTTONS CONTAINER OBJECTS
    public static string PendingTrueGlitter = "PendingTrueEffectsContainer/";
}