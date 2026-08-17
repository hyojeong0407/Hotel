using UnityEditor;
using UnityEngine;

public static class FrontDeskBuilder
{
    [MenuItem("Tools/Hotel Blockout/Build Vertical Horror Front Desk (1F)")]
    public static void BuildFrontDesk()
    {
        var existing = GameObject.Find("Horror_Front_Desk");
        if (existing != null) Object.DestroyImmediate(existing);
        
        GameObject deskRoot = new GameObject("Horror_Front_Desk");
        Undo.RegisterCreatedObjectUndo(deskRoot, "Build Front Desk");

        // 1. 프론트 데스크 카운터 (Reception Counter)
        SpawnModel(deskRoot.transform, "Reception_Counter_Model", 
            "Assets/3rdParty/reception-desk/source/Mesa007TWINE-N64(Remake).fbx", 
            new Vector3(1.5f, 0f, -1.5f), 
            new Vector3(10f, 10f, 10f), 
            Quaternion.Euler(-90f, 90f, 0f));

        // 2-1. 방명록 (Guestbook)
        SpawnModel(deskRoot.transform, "Guestbook_Model", 
            "Assets/3rdParty/old-book/source/book1.fbx", 
            new Vector3(0.95f, 0.75f, 1f), 
            new Vector3(1.2f, 1.2f, 1.2f), 
            Quaternion.Euler(-90f, -115f, 0f));

        // 2-2. 대형 호텔 종 (Bell)
        SpawnModel(deskRoot.transform, "Reception_Bell_Model", 
            "Assets/3rdParty/reception-bell/source/Reception_Bell.blend", 
            new Vector3(1.235f, 0.72f, 0f), 
            new Vector3(10f, 10f, 10f), 
            Quaternion.Euler(0f, 0f, 0f));

        // 3. 탁상조명 (Desk Lamp)
        GameObject lampObj = SpawnModel(deskRoot.transform, "Desk_Lamp_Model", 
            "Assets/3rdParty/Vintage_table_lamp/prefab/Desk_lamp.prefab", 
            new Vector3(0.2f, 0.7f, -1.2f), 
            new Vector3(1f, 1f, 1f), 
            Quaternion.Euler(0f, 0f, 0f));
            
        if (lampObj != null)
        {
            GameObject lightObj = new GameObject("Light_Source");
            lightObj.transform.SetParent(lampObj.transform);
            lightObj.transform.localPosition = new Vector3(0f, 0.3f, 0.05f); 
            lightObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            Light spotLight = lightObj.AddComponent<Light>();
            spotLight.type = LightType.Spot;
            spotLight.color = new Color(0.9f, 0.7f, 0.3f); 
            spotLight.intensity = 4f; 
            spotLight.range = 5f;
            spotLight.spotAngle = 75f; 
            spotLight.shadows = LightShadows.Soft; 
        }

        // 4. 열쇠 보관함 (Key Cabinet)
        SpawnModel(deskRoot.transform, "Key_Cabinet_Model", 
            "Assets/3rdParty/bookcase/source/SM_Bookcase_embedded.fbx", 
            new Vector3(-1.2f, 0f, -0.65f), 
            new Vector3(70f, 70f, 80f), 
            Quaternion.Euler(-90f, 90f, 0f));

        // 5. 키 캐비넷 내부에 눕혀서 배치 (4단 선반 x 5개)
        for (int i = 0; i < 20; i++)
        {
            int shelf = i / 5; // 0 ~ 3층
            int col = i % 5;   // 0 ~ 4열

            // ★ X축 (깊이): 실측값 기준 고정 (-0.95f)
            float posX = -0.95f; 

            // ★ Y축 (높이): 실측 기준 0.9부터 0.32씩 위로
            float startY = 0.9f;       
            float shelfSpacingY = 0.32f; 
            float posY = startY + (shelf * shelfSpacingY);

            // ★ Z축 (가로 너비): -0.3에서 시작해 오른쪽으로 0.18씩 간격 이동!
            float startZ = -0.3f;     
            float keySpacingZ = 0.18f; 
            float posZ = startZ + (col * keySpacingZ);

            // 회전값 고정
            Quaternion keyRot = Quaternion.Euler(-90f, 90f, 0f);

            SpawnModel(deskRoot.transform, "Room_Key_" + (i + 1), 
                "Assets/3rdParty/Gabies_Assets/Keys/Prefabs/Simple_02.prefab", 
                new Vector3(posX, posY, posZ), 
                Vector3.one, 
                keyRot);
        }

        // 6. 프론트 데스크 그룹 전체 위치
        deskRoot.transform.position = new Vector3(15.5f, 0f, -4f);

        Selection.activeGameObject = deskRoot;
        Debug.Log("Z축 기준 진짜 가로 정렬 완료!");
    }

    // ====================================================================
    // --- 3D 모델 소환 헬퍼 함수 ---
    // ====================================================================
    private static GameObject SpawnModel(Transform parent, string name, string path, Vector3 localPos, Vector3 localScale, Quaternion localRot)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        
        if (prefab != null)
        {
            GameObject obj = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            obj.name = name;
            obj.transform.localPosition = localPos;
            obj.transform.localRotation = localRot;
            obj.transform.localScale = localScale; 
            return obj;
        }
        else
        {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            temp.name = name + "_MISSING_MODEL";
            temp.transform.SetParent(parent);
            temp.transform.localPosition = localPos;
            temp.transform.localRotation = localRot;
            temp.transform.localScale = localScale; 
            
            Renderer renderer = temp.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                renderer.sharedMaterial.color = new Color(0.35f, 0.25f, 0.2f);
            }
            return temp;
        }
    }
}