using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;


[InitializeOnLoad]
public class AutoSaveOnPlay : ScriptableObject
{
    static AutoSaveOnPlay()
    {
        EditorApplication.playModeStateChanged += (PlayModeStateChange state) => 
            {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                Debug.Log("Auto-saving all open scenes...");
                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
            }
        };
    }
}
