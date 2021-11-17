﻿using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor functionalities from internal SceneHierarchyWindow and SceneHierarchy classes. 
/// For that we are using reflection.
/// </summary>
public static class SceneHierarchyUtility
{
    /// <summary>
    /// Check if the target GameObject is expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static bool IsExpanded(GameObject go)
    {
        return GetExpandedGameObjects().Contains(go);
    }

    /// <summary>
    /// Get a list of all GameObjects which are expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static List<GameObject> GetExpandedGameObjects()
    {
        object sceneHierarchy = GetSceneHierarchy();

        MethodInfo methodInfo = sceneHierarchy
            .GetType()
            .GetMethod("GetExpandedGameObjects");

        object result = methodInfo.Invoke(sceneHierarchy, new object[0]);

        return (List<GameObject>)result;
    }

    /// <summary>
    /// Set the target GameObject as expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static void SetExpanded(GameObject go, bool expand)
    {
        object sceneHierarchy = GetSceneHierarchy();

        MethodInfo methodInfo = sceneHierarchy
            .GetType()
            .GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(sceneHierarchy, new object[] { go.GetInstanceID(), expand });
    }

    /// <summary>
    /// Set the target GameObject and all children as expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static void SetExpandedRecursive(GameObject go, bool expand)
    {
        object sceneHierarchy = GetSceneHierarchy();
        MethodInfo methodInfo = sceneHierarchy
            .GetType()
            .GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);

        methodInfo.Invoke(sceneHierarchy, new object[] { go.GetInstanceID(), expand });
    }
    private static object GetSceneHierarchy()
    {
        EditorWindow window = GetHierarchyWindow();
        var assembly = typeof(UnityEditor.EditorWindow).Assembly;
        var type = assembly.GetType("UnityEditor.SceneHierarchyWindow").GetProperty("sceneHierarchy").GetValue(window, null);

        return type;
    }
    private static EditorWindow GetHierarchyWindow()
    {
        // // For it to open, so that it the current focused window.
        // //EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        // EditorApplication.ExecuteMenuItem("Window/Hierarchy");
        // return EditorWindow.focusedWindow;

        var assembly = typeof(UnityEditor.EditorWindow).Assembly;
        var type = assembly.GetType("UnityEditor.SceneHierarchyWindow");
        return EditorWindow.GetWindow(type);
    }
}
