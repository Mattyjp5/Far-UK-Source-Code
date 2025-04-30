using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SFB; // StandaloneFileBrowser namespace
using UnityEditor;

// Create a class that will handle script file uploading
public class URScriptUploader : MonoBehaviour
{
    private ur_data_processing urController;
    private Dictionary<string, string> loadedScripts = new Dictionary<string, string>();

    // References for UI display
    [SerializeField] private string currentLoadedScriptName = "None";
    [SerializeField] private bool showScriptContent = false;
    [SerializeField] private string scriptPreview = "";
    
    // Toggle for opening/closing the panel
    [SerializeField] private bool isPanelOpen = false;
    [SerializeField] private KeyCode toggleKey = KeyCode.F1; // Default key for toggling
    
    // UI positioning and style properties
    [SerializeField] private int windowWidth = 400;
    [SerializeField] private int windowHeight = 500;
    [SerializeField] private int windowX = 20;
    [SerializeField] private int windowY = 20;
    
    // Toggle button properties
    private Rect toggleButtonRect;
    private int toggleButtonWidth = 150;
    private int toggleButtonHeight = 30;

    private void Start()
    {
        // Get reference to the UR controller
        urController = GetComponent<ur_data_processing>();
        if (urController == null)
        {
            urController = FindObjectOfType<ur_data_processing>();
            if (urController == null)
            {
                Debug.LogError("No ur_data_processing component found in the scene.");
            }
        }
        
        // Set up toggle button position (slightly left of top right corner)
        toggleButtonRect = new Rect(
            Screen.width - toggleButtonWidth - 225, // increased offset from the right
            20, // y position from top
            toggleButtonWidth,
            toggleButtonHeight
        );
    }
    
    private void Update()
    {
        // Toggle panel with keyboard input
        if (Input.GetKeyDown(toggleKey))
        {
            isPanelOpen = !isPanelOpen;
        }
    }

    // Method to browse for and load a script file
    public void BrowseAndLoadScript()
    {
        // Configure file browser
        var extensions = new[] {
            new ExtensionFilter("UR Script Files", "script", "urscript", "txt"),
            new ExtensionFilter("All Files", "*"),
        };

        // Open file panel
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Load UR Script", "", extensions, false);

        // Check if a file was selected
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            LoadScriptFromPath(paths[0]);
        }
    }

    // Load script from a file path
    private void LoadScriptFromPath(string filePath)
    {
        try
        {
            // Read the script content
            string scriptContent = File.ReadAllText(filePath);
            string fileName = Path.GetFileName(filePath);

            // Store in dictionary
            loadedScripts[fileName] = scriptContent;
            
            // Update currently loaded script info
            currentLoadedScriptName = fileName;
            scriptPreview = scriptContent;

            Debug.Log($"Script loaded successfully: {fileName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading script: {e.Message}");
        }
    }

    // Execute the currently loaded script
    public void ExecuteCurrentScript()
    {
        if (string.IsNullOrEmpty(currentLoadedScriptName) || currentLoadedScriptName == "None")
        {
            Debug.LogWarning("No script is currently loaded.");
            return;
        }

        if (loadedScripts.TryGetValue(currentLoadedScriptName, out string scriptContent))
        {
            if (urController != null)
            {
                bool success = urController.SendCustomURScript(scriptContent);
                if (success)
                {
                    Debug.Log($"Executing script: {currentLoadedScriptName}");
                }
                else
                {
                    Debug.LogError("Failed to send script. Is the robot connected?");
                }
            }
            else
            {
                Debug.LogError("UR controller not found.");
            }
        }
    }

    // Method to browse and execute a script in one step
    public void BrowseAndExecuteScript()
    {
        // Configure file browser
        var extensions = new[] {
            new ExtensionFilter("UR Script Files", "script", "urscript", "txt"),
            new ExtensionFilter("All Files", "*"),
        };

        // Open file panel
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Execute UR Script", "", extensions, false);

        // Check if a file was selected
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            try
            {
                // Read the script content
                string scriptContent = File.ReadAllText(paths[0]);
                string fileName = Path.GetFileName(paths[0]);

                // Store in dictionary
                loadedScripts[fileName] = scriptContent;
                
                // Update currently loaded script info
                currentLoadedScriptName = fileName;
                scriptPreview = scriptContent;

                // Execute immediately
                if (urController != null)
                {
                    bool success = urController.SendCustomURScript(scriptContent);
                    if (success)
                    {
                        Debug.Log($"Executing script: {fileName}");
                    }
                    else
                    {
                        Debug.LogError("Failed to send script. Is the robot connected?");
                    }
                }
                else
                {
                    Debug.LogError("UR controller not found.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading/executing script: {e.Message}");
            }
        }
    }

    // Get list of all loaded scripts
    public string[] GetLoadedScriptNames()
    {
        string[] scriptNames = new string[loadedScripts.Count];
        loadedScripts.Keys.CopyTo(scriptNames, 0);
        return scriptNames;
    }

    // Get the content of a specific script by name
    public string GetScriptContent(string scriptName)
    {
        if (loadedScripts.TryGetValue(scriptName, out string content))
        {
            return content;
        }
        return null;
    }

    // Clear all loaded scripts
    public void ClearLoadedScripts()
    {
        loadedScripts.Clear();
        currentLoadedScriptName = "None";
        scriptPreview = "";
        Debug.Log("All loaded scripts cleared.");
    }
    
    // Toggle panel visibility
    public void TogglePanel()
    {
        isPanelOpen = !isPanelOpen;
    }

    // UI for the inspector and runtime
    void OnGUI()
    {
        // Only show if in play mode
        if (!Application.isPlaying) return;
        
        // Draw toggle button
        if (GUI.Button(toggleButtonRect, isPanelOpen ? "Close UR Uploader" : "Open UR Uploader"))
        {
            isPanelOpen = !isPanelOpen;
        }
        
        // If panel is not open, don't draw the rest
        if (!isPanelOpen) return;

        // Draw the main window if panel is open
        GUILayout.Window(0, new Rect(windowX, windowY, windowWidth, windowHeight), DrawWindow, "UR Script Uploader");
    }
    
    // Separate method to draw window contents
    private void DrawWindow(int windowID)
    {
        GUILayout.Label("UR Script Uploader", GUI.skin.box);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Browse & Load Script", GUILayout.Height(30)))
        {
            BrowseAndLoadScript();
        }
        
        if (GUILayout.Button("Browse & Execute Script", GUILayout.Height(30)))
        {
            BrowseAndExecuteScript();
        }
        
        GUILayout.Space(10);
        GUILayout.Label($"Current Script: {currentLoadedScriptName}", GUI.skin.box);
        
        if (GUILayout.Button("Execute Current Script", GUILayout.Height(30)))
        {
            ExecuteCurrentScript();
        }
        
        GUILayout.Space(10);
        showScriptContent = GUILayout.Toggle(showScriptContent, "Show Script Content");
        
        if (showScriptContent && !string.IsNullOrEmpty(scriptPreview))
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(200));
            GUILayout.Label("Script Content:");
            GUILayout.TextArea(scriptPreview, GUILayout.ExpandHeight(true));
            GUILayout.EndVertical();
        }
        
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Queue: {urController?.GetScriptQueueCount() ?? 0} scripts", GUILayout.Width(150));
        GUILayout.Label($"Running: {urController?.IsScriptRunning() ?? false}", GUILayout.Width(150));
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("Clear Script Queue"))
        {
            urController?.ClearScriptQueue();
        }
        
        if (GUILayout.Button("Clear Loaded Scripts"))
        {
            ClearLoadedScripts();
        }
        
        // Add close button at the bottom
        GUILayout.Space(10);
        if (GUILayout.Button("Close Panel", GUILayout.Height(30)))
        {
            isPanelOpen = false;
        }
        
        // Make the window draggable
        GUI.DragWindow();
    }
}

// Optional: Editor script for the component
#if UNITY_EDITOR

[CustomEditor(typeof(URScriptUploader))]
public class URScriptUploaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        URScriptUploader uploader = (URScriptUploader)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This component allows loading and executing UR Scripts via file browser.\n\nIn Play Mode, a toggle button will appear in the top-right corner of the game window.", MessageType.Info);
        
        EditorGUILayout.Space();
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Toggle Panel"))
            {
                uploader.TogglePanel();
            }
            
            if (GUILayout.Button("Browse & Load Script"))
            {
                uploader.BrowseAndLoadScript();
            }
            
            if (GUILayout.Button("Browse & Execute Script"))
            {
                uploader.BrowseAndExecuteScript();
            }
            
            if (GUILayout.Button("Execute Current Script"))
            {
                uploader.ExecuteCurrentScript();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Enter Play Mode to test script loading and execution.", MessageType.Warning);
        }
    }
}
#endif