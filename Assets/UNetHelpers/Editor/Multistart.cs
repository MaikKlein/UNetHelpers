using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

[System.Serializable]
class Multistart : EditorWindow
{
    [SerializeField]
    string path = "Build";
    [SerializeField]
    string fileName = "Game";
    [SerializeField]
    string windowTitle = "YourWindowTitle";
    [SerializeField]
    int timeOut = 10;
    [SerializeField]
    int desktopWidth = 1920;
    [SerializeField]
    int desktopHeight = 1080;
    [SerializeField]
    int horizontalOffset = 0;
    [SerializeField]
    UnityEngine.Object scene;
    [SerializeField]
    bool shouldPlayInEditor = true;

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr FindWindow(string windowClass, string title);
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr hWnd);
    public static string GetWindowText(IntPtr hWnd)
    {
        int size = GetWindowTextLength(hWnd);
        if (size++ > 0)
        {
            var builder = new StringBuilder(size);
            GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }
        return String.Empty;
    }
    string GetPathToScene()
    {
        return (scene != null) ? AssetDatabase.GetAssetPath(scene) : String.Empty;
    }
    public static List<IntPtr> FindWindowsWithText(string titleText)
    {
        List<IntPtr> windows = new List<IntPtr>();
        EnumWindows((IntPtr wnd, IntPtr param) =>
        {
            if (GetWindowText(wnd).Equals(titleText))
            {
                windows.Add(wnd);
            }
            return true;
        },
                    IntPtr.Zero);

        return windows;
    }
    string GetPathToExecutable()
    {
        return path + "/" + fileName + ".exe";
    }
    public void StartGame()
    {
        int windowWidth = desktopWidth / 2;
        int windowHeight = desktopHeight / 2;
        Process proc = new Process();
        proc.StartInfo.FileName = GetPathToExecutable();
        proc.EnableRaisingEvents = false;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        foreach (var _ in System.Linq.Enumerable.Range(1, 4))
        {
            proc.Start();
        }
        var watch = new Stopwatch();
        watch.Start();
        while (watch.Elapsed.Seconds < timeOut)
        {
            List<IntPtr> windows = FindWindowsWithText(windowTitle);
            if (windows.Count.Equals(4))
            {
                MoveWindow(windows[0], 0 + horizontalOffset, 0, windowWidth, windowHeight, true);
                MoveWindow(windows[1], windowWidth + horizontalOffset, 0, windowWidth, windowHeight, true);
                MoveWindow(windows[2], 0 + horizontalOffset, windowHeight, windowWidth, windowHeight, true);
                MoveWindow(windows[3], windowWidth + horizontalOffset, windowHeight, windowWidth, windowHeight, true);
                break;
            }
        }
    }
    public void BuildGame()
    {
        string[] levels = new string[] { GetPathToScene() };
        BuildPipeline.BuildPlayer(levels, GetPathToExecutable(), BuildTarget.StandaloneWindows, BuildOptions.UncompressedAssetBundle |
                                                                                                BuildOptions.AllowDebugging |
                                                                                                BuildOptions.Development);
    }
    [MenuItem("Window/Multistart")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Multistart));
    }
    void PlaySceneInEditor(bool shouldPlay)
    {
        if (shouldPlay )
        {
            EditorApplication.isPlaying = true;
        }
    }
    void OnGUI()
    {
        // The actual window code goes here
        scene = EditorGUILayout.ObjectField("Scene", scene, typeof(UnityEngine.Object), true);
        fileName = EditorGUILayout.TextField("Filename", fileName);
        path = EditorGUILayout.TextField("Relative Path", path);
        windowTitle = EditorGUILayout.TextField("Window Title", windowTitle);
        desktopWidth = EditorGUILayout.IntField("Width", desktopWidth);
        desktopHeight = EditorGUILayout.IntField("Height", desktopHeight);
        timeOut = EditorGUILayout.IntField("TimeOut", timeOut);
        horizontalOffset = EditorGUILayout.IntField("Horizontal Offset", horizontalOffset);
        shouldPlayInEditor = EditorGUILayout.Toggle("Play in Editor", shouldPlayInEditor);
        if (GUILayout.Button("Build & Run"))
        {
            BuildGame();
            StartGame();
            PlaySceneInEditor(shouldPlayInEditor);
        }
        if (GUILayout.Button("Run"))
        {
            StartGame();
            PlaySceneInEditor(shouldPlayInEditor);
        }
    }
}
