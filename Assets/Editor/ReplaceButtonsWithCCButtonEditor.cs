using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using ChenChen_UI;

public class ReplaceButtonsWithCCButtonEditor : EditorWindow
{
    [MenuItem("Tools/Replace Buttons With CC_Button")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceButtonsWithCCButtonEditor>("Replace Buttons With CC_Button");
    }

    private List<string> prefabPaths = new List<string>();

    void OnGUI()
    {
        if (GUILayout.Button("Replace All Buttons In Scene"))
        {
            ReplaceAllButtonsInScene();
        }

        if (GUILayout.Button("Replace All Buttons In Project Prefabs"))
        {
            FindAllPrefabs();
            ReplaceAllButtonsInPrefabs();
        }
    }

    void ReplaceAllButtonsInScene()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include,FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            ReplaceButton(button.gameObject);
        }

        Debug.Log("Replaced all buttons in the scene with CC_Button.");
    }

    void ReplaceAllButtonsInPrefabs()
    {
        foreach (string path in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Button[] buttons = prefab.GetComponentsInChildren<Button>(true);
            if (buttons.Length > 0)
            {
                bool prefabModified = false;

                foreach (Button button in buttons)
                {
                    ReplaceButton(button.gameObject);
                    prefabModified = true;
                }

                if (prefabModified)
                {
                    PrefabUtility.SavePrefabAsset(prefab);
                    Debug.Log($"Replaced buttons in prefab: {path}");
                }
            }
        }
    }

    void ReplaceButton(GameObject buttonGameObject)
    {
        Button button = buttonGameObject.GetComponent<Button>();
        if (button != null)
        {
            // Get the current button's properties
            var currentOnClick = button.onClick;
            var buttonImage = button.image;
            var transition = button.transition;
            var colors = button.colors;
            var navigation = button.navigation;

            // Remove the original Button component
            DestroyImmediate(button);

            // Add the new CC_Button component
            CC_Button customButton = buttonGameObject.AddComponent<CC_Button>();

            // Restore the original button's properties
            customButton.onClick = currentOnClick;
            customButton.image = buttonImage;
            customButton.transition = transition;
            customButton.colors = colors;
            customButton.navigation = navigation;
        }
    }

    void FindAllPrefabs()
    {
        prefabPaths.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            prefabPaths.Add(path);
        }
    }
}
