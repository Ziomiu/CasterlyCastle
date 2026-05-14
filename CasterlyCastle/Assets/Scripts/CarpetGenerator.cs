using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CarpetGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> carpetPrefabs = new List<GameObject>();

    [Header("Castle interior area")]
    public Vector3 center = Vector3.zero;
    public float areaWidth = 30f;
    public float areaLength = 30f;

    [Header("Patches")]
    public int patchCountX = 3;
    public int patchCountZ = 3;

    [Header("Scale optimization")]
    public float minPatchScale = 0.5f;
    public float maxPatchScale = 1f;
    public int scaleSearchSteps = 40;

    [Header("Inside patch spacing")]
    public float gapX = -0.10f;
    public float gapZ = -0.10f;

    [Header("Placement")]
    public float yOffset = 0.03f;
    public int seed = 12345;

    [ContextMenu("Generate Carpets")]
    public void GenerateCarpets()
    {
        ClearCarpets();

        if (carpetPrefabs == null || carpetPrefabs.Count == 0)
        {
            Debug.LogWarning("No carpet prefabs assigned.");
            return;
        }

        patchCountX = Mathf.Max(1, patchCountX);
        patchCountZ = Mathf.Max(1, patchCountZ);

        Random.InitState(seed);

        GameObject[,] patchPrefabMap = BuildNeighborSafePatchMap();

        float patchWidth = areaWidth / patchCountX;
        float patchLength = areaLength / patchCountZ;

        float areaMinX = center.x - areaWidth / 2f;
        float areaMinZ = center.z - areaLength / 2f;

        for (int px = 0; px < patchCountX; px++)
        {
            for (int pz = 0; pz < patchCountZ; pz++)
            {
                GameObject prefab = patchPrefabMap[px, pz];

                Vector2 originalSize = MeasureRealPrefabSize(prefab);
                float bestScale = FindBestScale(originalSize, patchWidth, patchLength);

                GeneratePatch(
                    prefab,
                    originalSize,
                    bestScale,
                    areaMinX + px * patchWidth,
                    areaMinZ + pz * patchLength,
                    patchWidth,
                    patchLength,
                    px,
                    pz
                );
            }
        }

        Debug.Log("Patch carpets generated with balanced neighbor-safe prefab distribution.");
    }

    private GameObject[,] BuildNeighborSafePatchMap()
    {
        GameObject[,] map = new GameObject[patchCountX, patchCountZ];

        if (carpetPrefabs.Count == 1)
        {
            for (int x = 0; x < patchCountX; x++)
                for (int z = 0; z < patchCountZ; z++)
                    map[x, z] = carpetPrefabs[0];

            return map;
        }

        int totalPatches = patchCountX * patchCountZ;
        int[] remaining = BuildBalancedCounts(totalPatches);

        bool success = FillPatchMapBacktracking(map, remaining, 0);

        if (!success)
            Debug.LogError("Could not generate neighbor-safe carpet patch map.");

        return map;
    }

    private int[] BuildBalancedCounts(int totalPatches)
    {
        int prefabCount = carpetPrefabs.Count;
        int[] counts = new int[prefabCount];

        int baseAmount = totalPatches / prefabCount;
        int remainder = totalPatches % prefabCount;

        for (int i = 0; i < prefabCount; i++)
            counts[i] = baseAmount;

        List<int> indexes = new List<int>();

        for (int i = 0; i < prefabCount; i++)
            indexes.Add(i);

        ShuffleIndexes(indexes);

        for (int i = 0; i < remainder; i++)
            counts[indexes[i]]++;

        return counts;
    }

    private bool FillPatchMapBacktracking(GameObject[,] map, int[] remaining, int index)
    {
        int total = patchCountX * patchCountZ;

        if (index >= total)
            return true;

        int x = index % patchCountX;
        int z = index / patchCountX;

        List<int> candidates = new List<int>();

        for (int i = 0; i < carpetPrefabs.Count; i++)
        {
            if (remaining[i] > 0 && CanPlacePrefabIndex(map, x, z, i))
                candidates.Add(i);
        }

        ShuffleIndexes(candidates);

        candidates.Sort((a, b) => remaining[b].CompareTo(remaining[a]));

        foreach (int prefabIndex in candidates)
        {
            map[x, z] = carpetPrefabs[prefabIndex];
            remaining[prefabIndex]--;

            if (FillPatchMapBacktracking(map, remaining, index + 1))
                return true;

            remaining[prefabIndex]++;
            map[x, z] = null;
        }

        return false;
    }

    private bool CanPlacePrefabIndex(GameObject[,] map, int x, int z, int prefabIndex)
    {
        GameObject prefab = carpetPrefabs[prefabIndex];

        if (x > 0 && map[x - 1, z] == prefab)
            return false;

        if (z > 0 && map[x, z - 1] == prefab)
            return false;

        if (x < patchCountX - 1 && map[x + 1, z] == prefab)
            return false;

        if (z < patchCountZ - 1 && map[x, z + 1] == prefab)
            return false;

        return true;
    }

    private void ShuffleIndexes(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private float FindBestScale(Vector2 originalSize, float patchWidth, float patchLength)
    {
        float bestScale = maxPatchScale;
        float bestScore = float.MaxValue;

        int steps = Mathf.Max(1, scaleSearchSteps);

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            float scale = Mathf.Lerp(minPatchScale, maxPatchScale, t);

            Vector2 size = originalSize * scale;

            float stepX = Mathf.Max(0.01f, size.x + gapX);
            float stepZ = Mathf.Max(0.01f, size.y + gapZ);

            int countX = Mathf.Max(1, Mathf.FloorToInt(patchWidth / stepX));
            int countZ = Mathf.Max(1, Mathf.FloorToInt(patchLength / stepZ));

            float usedWidth = countX * stepX - gapX;
            float usedLength = countZ * stepZ - gapZ;

            float leftoverX = Mathf.Abs(patchWidth - usedWidth);
            float leftoverZ = Mathf.Abs(patchLength - usedLength);

            float score = leftoverX + leftoverZ;

            if (score < bestScore)
            {
                bestScore = score;
                bestScale = scale;
            }
        }

        return bestScale;
    }

    private void GeneratePatch(
        GameObject prefab,
        Vector2 originalSize,
        float scale,
        float patchMinX,
        float patchMinZ,
        float patchWidth,
        float patchLength,
        int px,
        int pz)
    {
        Vector2 size = originalSize * scale;

        float stepX = Mathf.Max(0.01f, size.x + gapX);
        float stepZ = Mathf.Max(0.01f, size.y + gapZ);

        int countX = Mathf.Max(1, Mathf.FloorToInt(patchWidth / stepX));
        int countZ = Mathf.Max(1, Mathf.FloorToInt(patchLength / stepZ));

        float usedWidth = countX * stepX - gapX;
        float usedLength = countZ * stepZ - gapZ;

        float startX = patchMinX + (patchWidth - usedWidth) / 2f + size.x / 2f;
        float startZ = patchMinZ + (patchLength - usedLength) / 2f + size.y / 2f;

        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                Vector3 position = new Vector3(
                    startX + x * stepX,
                    center.y + yOffset,
                    startZ + z * stepZ
                );

                GameObject carpet = SpawnPrefab(prefab, position, Quaternion.identity);
                carpet.name = $"Generated_Carpet_Patch_{px}_{pz}";
                carpet.transform.localScale *= scale;
                carpet.transform.SetParent(transform, true);
            }
        }
    }

    private Vector2 MeasureRealPrefabSize(GameObject prefab)
    {
        GameObject temp = SpawnPrefab(prefab, Vector3.zero, Quaternion.identity);
        temp.name = "__TEMP_MEASURE_CARPET__";

        Renderer[] renderers = temp.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            DestroyTemp(temp);
            Debug.LogWarning("Prefab has no Renderer: " + prefab.name);
            return Vector2.one;
        }

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer renderer in renderers)
            bounds.Encapsulate(renderer.bounds);

        Vector2 size = new Vector2(bounds.size.x, bounds.size.z);

        DestroyTemp(temp);

        return size;
    }

    private GameObject SpawnPrefab(GameObject prefab, Vector3 position, Quaternion rotation)
    {
#if UNITY_EDITOR
        GameObject obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
#else
        return Instantiate(prefab, position, rotation);
#endif
    }

    private void DestroyTemp(GameObject obj)
    {
#if UNITY_EDITOR
        DestroyImmediate(obj);
#else
        Destroy(obj);
#endif
    }

    [ContextMenu("Clear Carpets")]
    public void ClearCarpets()
    {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Generated_Carpet") ||
                child.name.StartsWith("__TEMP_MEASURE_CARPET__"))
            {
                children.Add(child.gameObject);
            }
        }

        foreach (GameObject child in children)
            DestroyTemp(child);
    }
}
