using UnityEditor;
using UnityEngine;

public static class LobbyLoungeBuilder
{
    private const string PREFAB_PATH = "Assets/3rdParty/Furniture/Prefabs/";

    [MenuItem("Tools/Hotel Blockout/Build Lobby Lounge (1F)")]
    public static void BuildLounge()
    {
        var existing = GameObject.Find("Horror_Lobby_Lounge");
        if (existing != null) Object.DestroyImmediate(existing);

        var existingEl = GameObject.Find("Horror_Elevator_Details");
        if (existingEl != null) Object.DestroyImmediate(existingEl);

        GameObject loungeRoot = new GameObject("Horror_Lobby_Lounge");
        Undo.RegisterCreatedObjectUndo(loungeRoot, "Build Lobby Lounge");

        // 1. 로비 중앙 대형 카펫 (PBR Plane)
        GameObject carpet = GameObject.CreatePrimitive(PrimitiveType.Plane);
        carpet.name = "Lobby_Carpet_PBR";
        carpet.transform.SetParent(loungeRoot.transform);
        carpet.transform.localPosition = new Vector3(0f, 0.01f, 0f);
        carpet.transform.localScale = new Vector3(0.6f, 1f, 0.5f);
        ApplyMaterial(carpet, "Mat_Lobby_Carpet");

        // 가구 크기 0.7로 통일
        Vector3 furnitureScale = new Vector3(0.7f, 0.7f, 0.7f);

        // 2. 메인 가죽 소파 (Couch)
        SpawnFurniture("Couch", loungeRoot.transform, new Vector3(0f, 0f, 1.8f), Quaternion.Euler(0f, 180f, 0f), furnitureScale);

        // 3. 1인용 가죽 암체어 (Fotel)
        SpawnFurniture("Fotel", loungeRoot.transform, new Vector3(-1.8f, 0f, 0f), Quaternion.Euler(0f, -90f, 0f), furnitureScale);
        SpawnFurniture("Fotel", loungeRoot.transform, new Vector3(1.8f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f), furnitureScale);

        // 4. 중앙 원형 커피 테이블 (RoundTable)
        SpawnFurniture("RoundTable", loungeRoot.transform, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 180f, 0f), furnitureScale);

        // ==========================================
        // 5. 괘종시계 (스케치팹 다운로드 모델 적용)
        // ==========================================
        string clockPath = "Assets/3rdParty/Clock/source/vintage_grandfather_clock.fbx";
        GameObject clockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(clockPath);

        if (clockPrefab != null)
        {
            GameObject clockObj = PrefabUtility.InstantiatePrefab(clockPrefab, loungeRoot.transform) as GameObject;
            clockObj.name = "Grandfather_Clock";
            clockObj.transform.localPosition = new Vector3(-2.5f, 0f, 2.1f);
            clockObj.transform.localRotation = Quaternion.Euler(-90f, 135f, 0f); 
            
            // ★ 수정된 부분: 시계 모델 특성에 맞춰 스케일을 80으로 뻥튀기!
            clockObj.transform.localScale = new Vector3(80f, 80f, 80f); 
            
            Undo.RegisterCreatedObjectUndo(clockObj, "Spawn Grandfather Clock");
        }

        // ==========================================
        // 6. 무드등 (스탠드 조명 + 은은한 포인트 라이트)
        // ==========================================
        // ※ 파일이 들어있는 실제 폴더 경로로 확인해 주세요!
        string lampPath = "Assets/3rdParty/Lamp/source/LampTurn.blend"; 
        GameObject lampPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(lampPath);

        if (lampPrefab != null)
        {
            GameObject lampObj = PrefabUtility.InstantiatePrefab(lampPrefab, loungeRoot.transform) as GameObject;
            lampObj.name = "Floor_Lamp";
            
            // 소파 옆이나 라운지 구석자리에 배치
            lampObj.transform.localPosition = new Vector3(2.2f, 0f, 1.5f);
            // 회전값을 0, 0, 0 대신 X축을 -90(또는 90)으로 꺾어줍니다.
            lampObj.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            // 크기가 너무 크거나 작으면 이 부분을 0.5f, 0.01f 등으로 조절
            lampObj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            // ★ 무드등 갓 내부 전구 위치에 불빛(Point Light) 달아주기
            GameObject lightObj = new GameObject("Lamp_Light");
            lightObj.transform.SetParent(lampObj.transform);
            lightObj.transform.localPosition = new Vector3(0f, 1.8f, 0f); // 전구 높이에 맞게 Y축 조절

            Light lampLight = lightObj.AddComponent<Light>();
            lampLight.type = LightType.Point;
            lampLight.color = new Color(1.0f, 0.7f, 0.4f); // 으스스하고 은은한 주황빛
            lampLight.intensity = 2.0f;                     // 조명 밝기
            lampLight.range = 5.0f;                         // 빛이 도달하는 범위

            Undo.RegisterCreatedObjectUndo(lampObj, "Spawn Floor Lamp");
        }

        // ==========================================
        // 7. 스산한 화분 (라운지 구석 배치)
        // ==========================================
        // ※ 본인이 다운받은 화분 파일 경로와 이름으로 수정해 주세요!
        string plantPath = "Assets/3rdParty/Plant/source/potted_plant.fbx"; 
        GameObject plantPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(plantPath);

        if (plantPrefab != null)
        {
            GameObject plantObj = PrefabUtility.InstantiatePrefab(plantPrefab, loungeRoot.transform) as GameObject;
            plantObj.name = "Potted_Plant";
            
            // 무드등 반대쪽 구석이나 소파 옆에 배치 (원하는 위치로 수정 가능)
            plantObj.transform.localPosition = new Vector3(-2.4f, 0f, -1.5f);
            
            // 잎의 방향이 자연스럽게 랜덤으로 돌아가게 설정
            plantObj.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            
            // 크기가 너무 크거나 작으면 이 부분을 0.5f, 0.01f 등으로 조절
            plantObj.transform.localScale = new Vector3(2f, 2f, 2f); 

            Undo.RegisterCreatedObjectUndo(plantObj, "Spawn Potted Plant");
        }

        // 8. 라운지 전체 위치 이동
        loungeRoot.transform.position = new Vector3(20.5f, 0f, -4f);

        // 9. 엘리베이터 디테일 생성
        BuildElevatorDetails();

        Selection.activeGameObject = loungeRoot;
        Debug.Log("1층 로비 라운지 (스케치팹 괘종시계 추가) 생성 완료!");
    }

    private static GameObject SpawnFurniture(string prefabName, Transform parent, Vector3 localPos, Quaternion localRot, Vector3 scale)
    {
        string path = $"{PREFAB_PATH}{prefabName}.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (prefab != null)
        {
            GameObject obj = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            obj.transform.localPosition = localPos;
            obj.transform.localRotation = localRot;
            obj.transform.localScale = scale;
            
            Undo.RegisterCreatedObjectUndo(obj, $"Spawn {prefabName}");
            return obj;
        }

        Debug.LogWarning($"[{path}] 경로에서 프리팹을 찾을 수 없습니다.");
        return null;
    }

    private static void BuildElevatorDetails()
    {
        GameObject elevatorDetails = new GameObject("Horror_Elevator_Details");
        Undo.RegisterCreatedObjectUndo(elevatorDetails, "Build Elevator Details");

        float elZ = 2f;
        float doorWidth = 1.8f;
        float doorHeight = 2.1f;

        GameObject frameTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frameTop.name = "Frame_Top";
        frameTop.transform.SetParent(elevatorDetails.transform);
        frameTop.transform.position = new Vector3(0.35f, doorHeight + 0.05f, elZ);
        frameTop.transform.localScale = new Vector3(0.1f, 0.1f, doorWidth + 0.2f);
        ApplyMaterial(frameTop, "Mat_Lobby_Brass");

        GameObject frameLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frameLeft.name = "Frame_Left";
        frameLeft.transform.SetParent(elevatorDetails.transform);
        frameLeft.transform.position = new Vector3(0.35f, doorHeight / 2f, elZ - (doorWidth / 2f) - 0.05f);
        frameLeft.transform.localScale = new Vector3(0.1f, doorHeight, 0.1f);
        ApplyMaterial(frameLeft, "Mat_Lobby_Brass");

        GameObject frameRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frameRight.name = "Frame_Right";
        frameRight.transform.SetParent(elevatorDetails.transform);
        frameRight.transform.position = new Vector3(0.35f, doorHeight / 2f, elZ + (doorWidth / 2f) + 0.05f);
        frameRight.transform.localScale = new Vector3(0.1f, doorHeight, 0.1f);
        ApplyMaterial(frameRight, "Mat_Lobby_Brass");

        GameObject callButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
        callButton.name = "Call_Button_Panel";
        callButton.transform.SetParent(elevatorDetails.transform);
        callButton.transform.position = new Vector3(0.32f, 1.1f, elZ - (doorWidth / 2f) - 0.25f);
        callButton.transform.localScale = new Vector3(0.05f, 0.2f, 0.1f);
        ApplyMaterial(callButton, "Mat_Lobby_Wood");

        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        indicator.name = "Floor_Indicator";
        indicator.transform.SetParent(elevatorDetails.transform);
        indicator.transform.position = new Vector3(0.32f, doorHeight + 0.2f, elZ);
        indicator.transform.localScale = new Vector3(0.05f, 0.15f, 0.4f);
        ApplyMaterial(indicator, "Mat_Lobby_Wood");
    }

    private static void ApplyMaterial(GameObject obj, string materialName)
    {
        string path = $"Assets/Art/Models/Materials/{materialName}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (mat != null)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = mat;
            }
        }
    }
}