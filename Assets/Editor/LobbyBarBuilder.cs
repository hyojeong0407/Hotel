using UnityEditor;
using UnityEngine;

public static class LobbyBarBuilder
{
    [MenuItem("Tools/Hotel Blockout/Build Luxury Horror Bar (1F) - Prefab Version")]
    public static void BuildBar()
    {
        var existing = GameObject.Find("Horror_Lobby_Bar");
        if (existing != null) Object.DestroyImmediate(existing);
        
        GameObject barRoot = new GameObject("Horror_Lobby_Bar");
        Undo.RegisterCreatedObjectUndo(barRoot, "Build Horror Bar");

        // 1. 바 카운터 (X값을 3.8f로 밀어 중앙 정렬)
        PlacePrefab(barRoot.transform, "Prop_Bar_Counter", 
            new Vector3(3.8f, 0f, -2f), 
            Vector3.zero, 
            new Vector3(13f, 1f, 1f));

        // 2. 바 스툴 6개 (바닥 파묻힘 방지)
        for (int i = 0; i < 6; i++)
        {
            Vector3 pos, rot;
            if (i == 2 || i == 4) 
            {
                pos = new Vector3(-3f + i * 1.2f, 0.25f, -3.2f);
                rot = new Vector3(90f, 45f, 0f);
            }
            else
            {
                pos = new Vector3(-3f + i * 1.2f, 0f, -3f);
                rot = Vector3.zero;
            }
            PlacePrefab(barRoot.transform, "Prop_Bar_Stool", pos, rot, Vector3.one);
        }

        // 3. 샹들리에 조명 4개 (Z축은 앞으로 빼고(-1.8f), Y축은 1층 천장 아래(2.5f)로 설정)
        for (int i = 0; i < 4; i++)
        {
            GameObject lantern = PlacePrefab(barRoot.transform, "Prop_Pendant_Light", 
                new Vector3(-3f + i * 2f, 2.5f, -1.8f), // ★ Z축을 진열장이 아닌 카운터 쪽(-1.8f)으로 다시 뺌
                Vector3.zero, 
                new Vector3(0.5f, 0.5f, 0.5f)); 
            
            if (lantern != null)
            {
                Light light = lantern.GetComponent<Light>();
                if (light == null) light = lantern.AddComponent<Light>(); 
                
                light.type = LightType.Point;
                light.color = new Color(0.6f, 0.05f, 0.05f); // 핏빛 조명
                light.intensity = 2f;
                light.range = 7f;
                light.shadows = LightShadows.Soft;
            }
        }
        
        // 4. 주류 진열장 (180도 회전)
        PlacePrefab(barRoot.transform, "Prop_Liquor_Shelf", 
            new Vector3(0f, 0f, -0.25f), 
            new Vector3(0f, 180f, 0f),
            new Vector3(8f, 1f, 1f));

        // 5. 전체 배치 위치 유지
        barRoot.transform.position = new Vector3(19f, 0f, 12f);

        Selection.activeGameObject = barRoot;
        Debug.Log("조명을 앞으로 빼고(-1.8f) 1층 천장 높이(2.5f)에 배치 완료!");
    }

    // --- 프리팹 생성 및 위치/회전/스케일 설정 헬퍼 함수 ---
    private static GameObject PlacePrefab(Transform parent, string prefabName, Vector3 localPosition, Vector3 localRotation, Vector3 localScale)
    {
        string path = $"Assets/Art/Models/Prefabs/{prefabName}.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (prefab == null)
        {
            GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fallback.name = "Missing_" + prefabName;
            fallback.transform.SetParent(parent);
            fallback.transform.localPosition = localPosition;
            fallback.transform.localRotation = Quaternion.Euler(localRotation);
            fallback.transform.localScale = localScale;
            Debug.LogWarning($"[Builder] '{prefabName}' 프리팹을 찾지 못해 임시 큐브로 대체했습니다.");
            return fallback;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.SetParent(parent);
        instance.transform.localPosition = localPosition;
        instance.transform.localRotation = Quaternion.Euler(localRotation);
        instance.transform.localScale = localScale;
        
        Undo.RegisterCreatedObjectUndo(instance, "Place " + prefabName);
        return instance;
    }
}