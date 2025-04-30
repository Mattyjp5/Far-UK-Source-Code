using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using Dummiesman; // Make sure Runtime OBJ Loader is installed

public class Extension : MonoBehaviour
{
    public Transform link6Transform;
    public TMP_Dropdown extensionDropdown;
    private string extensionFolderPath;

    private List<GameObject> extensions = new List<GameObject>();
    private List<string> extensionNames = new List<string>();
    private int selectedExtensionIndex = 0;
    private GameObject currentExtension;

    void Start()
    {
        // Set the path to the "Extensions" folder inside "StreamingAssets"
        extensionFolderPath = Path.Combine(Application.streamingAssetsPath, "Extensions");

        // Ensure StreamingAssets folder exists, if not, create it
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Debug.Log("StreamingAssets folder not found, creating it.");
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        // Ensure Extensions folder exists inside StreamingAssets, if not, create it
        if (!Directory.Exists(extensionFolderPath))
        {
            Debug.Log("Extensions folder not found, creating it.");
            Directory.CreateDirectory(extensionFolderPath);
        }

        // Load existing extensions and populate dropdown
        LoadExtensions();
        PopulateDropdown();
        extensionDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Loads all OBJ models from the Extensions folder
    public void LoadExtensions()
    {
        extensions.Clear();
        extensionNames.Clear();

        // Check if new OBJ files have been added to the Extensions folder
        string[] files = Directory.GetFiles(extensionFolderPath, "*.obj");
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (!extensionNames.Contains(fileName))  // Prevent duplicates in the list
            {
                extensionNames.Add(fileName);
                StartCoroutine(LoadOBJ(file));  // Load OBJ asynchronously
            }
        }

        // Ensure that the dropdown gets updated only after all extensions are loaded
        StartCoroutine(WaitForExtensionsAndPopulateDropdown());
    }

    IEnumerator WaitForExtensionsAndPopulateDropdown()
    {
        // Wait until all OBJ files are loaded
        yield return new WaitUntil(() => extensions.Count == extensionNames.Count);

        Debug.Log("Loaded " + extensions.Count + " extensions.");
        PopulateDropdown();  // Populate dropdown after extensions are fully loaded
    }

    IEnumerator LoadOBJ(string path)
    {
        Debug.Log("Loading OBJ from path: " + path);

        yield return null;  // Allow Unity to continue rendering

        GameObject loadedObj = new GameObject(Path.GetFileNameWithoutExtension(path));

        // Load OBJ using Runtime OBJ Loader
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            GameObject objModel = new OBJLoader().Load(stream);
            if (objModel != null)
            {
                objModel.transform.SetParent(loadedObj.transform);
                objModel.transform.localPosition = Vector3.zero;
                objModel.transform.localRotation = Quaternion.Euler(90, 180, 0);
                objModel.transform.localScale = Vector3.one;

                extensions.Add(loadedObj);  // Add the loaded model to the extensions list
                Debug.Log("Loaded OBJ: " + path);
            }
            else
            {
                Debug.LogError("Failed to load OBJ: " + path);
            }
        }
    }

    public void PopulateDropdown()
    {
        List<string> options = new List<string> { "None" };
        options.AddRange(extensionNames);  // Add the model names to the dropdown options
        extensionDropdown.ClearOptions();
        extensionDropdown.AddOptions(options);
        extensionDropdown.value = 0;  // Set the default value of the dropdown
    }


    // Triggered when the dropdown value changes
    void OnDropdownValueChanged(int index)
    {
        selectedExtensionIndex = index;
        ApplySelectedExtension();
    }

    // Applies the selected extension to Link 6
    void ApplySelectedExtension()
    {
        if (selectedExtensionIndex == 0)
        {
            // If "None" is selected, detach the extension if one exists
            if (currentExtension != null)
            {
                Debug.Log("Removing current extension as 'None' is selected.");
                DetachExtension();
            }
            return;
        }

        // Detach the previous extension before applying a new one
        if (currentExtension != null)
        {
            DetachExtension();
        }

        // Ensure the selected index is valid
        int prefabIndex = selectedExtensionIndex - 1;  // Dropdown index starts from 1
        if (prefabIndex < 0 || prefabIndex >= extensions.Count)
        {
            Debug.LogError("Invalid selection index. No valid extension found.");
            return;
        }

        // Instantiate the new extension
        GameObject newExtension = Instantiate(extensions[prefabIndex]);
        newExtension.transform.SetParent(link6Transform, false);
        newExtension.transform.localPosition = Vector3.zero;
        newExtension.transform.localRotation = Quaternion.Euler(90, 180, 0);

        // Ensure that the object is visible and not out of bounds
        newExtension.SetActive(true);

        Debug.Log($"Successfully attached {newExtension.name} to {link6Transform.name}.");
        currentExtension = newExtension;
    }


    // Removes the currently attached extension
    void DetachExtension()
    {
        if (currentExtension != null)
        {
            Debug.Log($"Destroying extension: {currentExtension.name}");
            Destroy(currentExtension);
            currentExtension = null;
        }
    }
}
