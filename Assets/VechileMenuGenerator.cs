using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class VehicleMenuGenerator : MonoBehaviour
{
    [Header("Setup")]
    public GameObject uiEntryPrefab;      
    public Transform uiParent;           

    [Header("Preview Settings")]
    public Vector3 previewStartPosition = new Vector3(0, 0, 0); 
    public Vector3 previewOffset = new Vector3(5f, 0, 5f);      
    public int carsPerRow = 5;                                  
    public int renderTextureSize = 256;                          
    public float zoomFactor = 2f;                                
    public Color backgroundColor = new Color(0.2f, 0.4f, 1f, 1f); 

    // Internal tracking
    private Transform previewRoot;
    private int prefabIndex = 0; 

    void Start()
    {
        Debug.Log("Loading vehicles from Addressables...");

        previewRoot = new GameObject("PreviewRoot").transform;

        Addressables.LoadAssetsAsync<GameObject>("Vehicles", prefab =>
        {
            CreateUIEntry(prefab);

        }).Completed += OnAllPrefabsLoaded;
    }

    void OnAllPrefabsLoaded(AsyncOperationHandle<IList<GameObject>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            Debug.Log($"Loaded {handle.Result.Count} vehicle prefabs.");
        else
            Debug.LogError("Failed to load vehicle prefabs.");
    }

    void CreateUIEntry(GameObject prefab)
    {
        GameObject uiEntry = Instantiate(uiEntryPrefab, uiParent);
        RawImage renderImg = uiEntry.transform.Find("RenderImg").GetComponent<RawImage>();
        TextMeshProUGUI label = uiEntry.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        label.text = prefab.name;

        int row = prefabIndex / carsPerRow;
        int col = prefabIndex % carsPerRow;
        Vector3 prefabPosition = previewStartPosition + new Vector3(col * previewOffset.x, 0, row * previewOffset.z);

        GameObject instance = Instantiate(prefab, prefabPosition, Quaternion.identity, previewRoot);
        instance.name = prefab.name;

        Bounds bounds = CalculateBounds(instance);

        RenderTexture rt = new RenderTexture(renderTextureSize, renderTextureSize, 16);
        rt.name = prefab.name + "_RT";
        renderImg.texture = rt;

        GameObject camObj = new GameObject(prefab.name + "_PreviewCam");
        Camera cam = camObj.AddComponent<Camera>();
        cam.targetTexture = rt;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = backgroundColor;
        cam.cullingMask = LayerMask.GetMask("Default"); 

        cam.transform.position = bounds.center + new Vector3(0, bounds.size.y * zoomFactor, -bounds.size.magnitude * zoomFactor);
        cam.transform.LookAt(bounds.center);
        cam.fieldOfView = 30f;

        prefabIndex++;
    }

    Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);
        return bounds;
    }
}
