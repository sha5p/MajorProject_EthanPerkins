using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class VehicleMenuGenerator : MonoBehaviour
{
    [Header("Setup")]
    public GameObject uiEntryPrefab;
    public RectTransform uiParent;

    [Header("Layout Settings")]
    public float xPosition = 15f;
    public float verticalSpacing = 30f;

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
    private float nextY = 0f;

    void Start()
    {
        if (uiParent == null)
        {
            Debug.LogError("UI Parent is not assigned! Please assign a RectTransform to the uiParent field in the Inspector.");
            return;
        }

        previewRoot = new GameObject("PreviewRoot").transform;

        // Load assets and then process them
        Addressables.LoadAssetsAsync<GameObject>("Vehicles", null).Completed += OnAllPrefabsLoaded;
    }

    void OnAllPrefabsLoaded(AsyncOperationHandle<IList<GameObject>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
        {
            Debug.Log($"Loaded {handle.Result.Count} vehicle prefabs. Creating UI now.");
            foreach (var prefab in handle.Result)
            {
                CreateUIEntry(prefab);
            }
        }
        else
        {
            Debug.LogError("Failed to load vehicle prefabs. Check your Addressables configuration.");
        }
    }
    public float bob;
    void CreateUIEntry(GameObject prefab)
    {
        if (uiEntryPrefab == null)
        {
            Debug.LogError("UI Entry Prefab is not assigned.");
            return;
        }

        GameObject uiEntry = Instantiate(uiEntryPrefab, uiParent);
        uiEntry.SetActive(true);

        RectTransform rt = uiEntry.GetComponent<RectTransform>();

        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);

        rt.anchoredPosition = new Vector2(xPosition, bob);

        bob -= 50;

        RawImage renderImg = uiEntry.transform.Find("RenderImg")?.GetComponent<RawImage>();
        TextMeshProUGUI label = uiEntry.transform.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();

        if (renderImg != null)
        {
            int row = prefabIndex / carsPerRow;
            int col = prefabIndex % carsPerRow;
            Vector3 prefabPosition = previewStartPosition + new Vector3(col * previewOffset.x, 0, row * previewOffset.z);

            GameObject instance = Instantiate(prefab, prefabPosition, Quaternion.identity, previewRoot);
            instance.name = prefab.name;

            Bounds bounds = CalculateBounds(instance);

            RenderTexture rtTexture = new RenderTexture(renderTextureSize, renderTextureSize, 16);
            rtTexture.name = prefab.name + "_RT";
            renderImg.texture = rtTexture;

            GameObject camObj = new GameObject(prefab.name + "_PreviewCam");
            Camera cam = camObj.AddComponent<Camera>();
            cam.targetTexture = rtTexture;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = backgroundColor;
            cam.cullingMask = LayerMask.GetMask("Default");
            cam.transform.position = bounds.center + new Vector3(0, bounds.size.y * zoomFactor, -bounds.size.magnitude * zoomFactor);
            cam.transform.LookAt(bounds.center);
            cam.fieldOfView = 30f;
        }

        if (label != null)
        {
            label.text = prefab.name;
        }

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