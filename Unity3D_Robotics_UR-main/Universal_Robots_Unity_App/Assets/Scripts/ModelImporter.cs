using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SFB; // Standalone File Browser

public class ModelImporter : MonoBehaviour
{
    public Button importModelButton;
    [SerializeField] private Extension extensionManager; // Manually assign in the Inspector
    public Transform referenceModel;
    private string extensionPath = "Resources/Extensions/";

    void Start()
    {
        importModelButton.onClick.AddListener(ImportModel);

        if (extensionManager == null)
        {
            Debug.LogError("Extension Manager is not assigned. Please assign it in the Inspector.");
        }
    }

    // Opens file browser and imports selected model
    void ImportModel()
    {
        string path = OpenFileBrowser();
        if (string.IsNullOrEmpty(path)) return;

        string modelName = Path.GetFileNameWithoutExtension(path);
        string destinationPath = Path.Combine(Application.dataPath, extensionPath, modelName + Path.GetExtension(path));

        // Ensure the destination directory exists
        if (!Directory.Exists(Application.dataPath + "/" + extensionPath))
        {
            Directory.CreateDirectory(Application.dataPath + "/" + extensionPath);
        }

        File.Copy(path, destinationPath, true);
        Debug.Log("Imported model: " + destinationPath);

        CreatePrefab(modelName);
    }

    // Creates a prefab from the imported model
    void CreatePrefab(string modelName)
    {
        GameObject importedModel = Resources.Load<GameObject>("Extensions/" + modelName);
        if (importedModel == null)
        {
            Debug.LogError("Failed to load imported model at runtime: " + modelName);
            return;
        }

        GameObject tempModel = Instantiate(importedModel);
        tempModel.transform.position = Vector3.zero;
        tempModel.transform.rotation = Quaternion.identity;

        // Ensure the model has a Renderer component
        Renderer renderer = tempModel.GetComponentInChildren<Renderer>(true);
        if (renderer == null)
        {
            Debug.LogError($"No Renderer found in {modelName}. Ensure the model has a MeshRenderer.");
            Destroy(tempModel);
            return;
        }

        if (!Directory.Exists(Application.dataPath + "/" + extensionPath))
        {
            Directory.CreateDirectory(Application.dataPath + "/" + extensionPath);
        }

        string prefabPath = extensionPath + modelName;
        GameObject prefab = Instantiate(tempModel);
        prefab.name = modelName;
        Destroy(tempModel);

        Debug.Log($"Saved prefab at: {prefabPath}");

        // Update the extension dropdown in UI
        if (extensionManager != null)
        {
            extensionManager.LoadExtensions();
            extensionManager.PopulateDropdown();
            Debug.Log("Dropdown updated with new extension: " + modelName);
        }
        else
        {
            Debug.LogError("Extension Manager is not assigned. Cannot update dropdown.");
        }
    }

    // Opens a file browser to select a 3D model
    string OpenFileBrowser()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Select a 3D Model File", "",
            new[] { new ExtensionFilter("3D Models", "fbx", "obj", "blend", "glb", "gltf", "dae", "stl") }, false); // only tested obj, don't think other formats work yet
        return paths.Length > 0 ? paths[0] : "";
    }
}