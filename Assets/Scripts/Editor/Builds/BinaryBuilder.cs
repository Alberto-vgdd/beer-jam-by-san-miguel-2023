using UnityEditor;
using UnityEngine;

class BinaryBuilder
{
    // Output directories
    private const string BUILD_DIR = "./builds/";
    private const string WINDOWS_BUILD_DIR = BUILD_DIR + "windows/";
    private const string LINUX_BUILD_DIR = BUILD_DIR + "linux/";
    private const string MAC_BUILD_DIR = BUILD_DIR + "mac/";
    private const string HTML5_BUILD_DIR = BUILD_DIR + "html5/";

    // Game Information
    private const string GAME_NAME = "excelente-servicio";

    // Build variables
    private static string[] buildScenes = new string[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/OnePlayer.unity", "Assets/Scenes/TwoPlayers.unity" };

    [MenuItem("Binary Builder/Build Standalone Windows")]
    static void PerformWindowsBuild()
    {
        BuildPipeline.BuildPlayer(
                     buildScenes,
                     WINDOWS_BUILD_DIR + GAME_NAME + ".exe",
                     BuildTarget.StandaloneWindows64,
                     BuildOptions.None);

    }

    [MenuItem("Binary Builder/Build Standalone Linux")]
    static void PerformLinuxBuild()
    {
        BuildPipeline.BuildPlayer(
                     buildScenes,
                     LINUX_BUILD_DIR + GAME_NAME + ".x86_64",
                     BuildTarget.StandaloneLinux64,
                     BuildOptions.None);
    }

    [MenuItem("Binary Builder/Build Standalone Mac")]
    static void PerformMacBuild()
    {
        BuildPipeline.BuildPlayer(
                     buildScenes,
                     MAC_BUILD_DIR + GAME_NAME + ".app",
                     BuildTarget.StandaloneOSX,
                     BuildOptions.None);
    }

    [MenuItem("Binary Builder/Build HTML5")]
    static void PerformHTML5Build()
    {
        BuildPipeline.BuildPlayer(
                     buildScenes,
                     HTML5_BUILD_DIR + GAME_NAME,
                     BuildTarget.WebGL,
                     BuildOptions.None);
    }


    [MenuItem("Binary Builder/Build All")]
    static void PerformAllBuilds()
    {
        PerformLinuxBuild();
        PerformMacBuild();
        PerformWindowsBuild();
        PerformHTML5Build();
    }
}