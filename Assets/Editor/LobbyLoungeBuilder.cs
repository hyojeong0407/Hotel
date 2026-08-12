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

        // 1. 로비 중앙 대형 카펫
        GameObject carpet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        carpet.name = "Lobby_Carpet";
        carpet.transform.SetParent(loungeRoot.transform);
        carpet.transform.localPosition = new Vector3(0f, 0.01f, 0f);
        carpet.transform.localScale = new Vector3(6f, 0.02f, 5f);
        ApplyMaterial(carpet, "Mat_Lobby_Carpet");

        // 가구 크기 0.7로 통일
        Vector3 furnitureScale = new Vector3(0.7f, 0.7f, 0.7f);

        // 2. 메인 가죽 소파 (Couch) - Couch 모델 기준에 맞춰 180도 회전!
        SpawnFurniture("Couch", loungeRoot.transform, new Vector3(0f, 0f, 1.8f), Quaternion.Euler(0f, 180f, 0f), furnitureScale);

        // 3. 1인용 가죽 암체어 (Fotel) - 중앙 테이블을 바라보도록 설정
        SpawnFurniture("Fotel", loungeRoot.transform, new Vector3(-1.8f, 0f, 0f), Quaternion.Euler(0f, -90f, 0f), furnitureScale);
        SpawnFurniture("Fotel", loungeRoot.transform, new Vector3(1.8f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f), furnitureScale);

        // 4. 중앙 원형 커피 테이블 (RoundTable)
        SpawnFurniture("RoundTable", loungeRoot.transform, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 180f, 0f), furnitureScale);

        // 5. 괘종시계 (Grandfather Clock)
        GameObject clockGroup = new GameObject("Grandfather_Clock");
        clockGroup.transform.SetParent(loungeRoot.transform);
        clockGroup.transform.localPosition = new Vector3(-2.5f, 0f, 2.1f);

        GameObject clockBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
        clockBody.name = "Clock_Body";
        clockBody.transform.SetParent(clockGroup.transform);
        clockBody.transform.localPosition = new Vector3(0f, 1.2f, 0f);
        clockBody.transform.localScale = new Vector3(0.7f, 2.4f, 0.5f);
        ApplyMaterial(clockBody, "Mat_Lobby_Wood");

        GameObject clockFace = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        clockFace.name = "Clock_Face";
        clockFace.transform.SetParent(clockGroup.transform);
        clockFace.transform.localPosition = new Vector3(0f, 1.9f, -0.26f);
        clockFace.transform.localScale = new Vector3(0.4f, 0.02f, 0.4f);
        clockFace.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        ApplyMaterial(clockFace, "Mat_Lobby_Brass");

        // 6. 스탠드 조명 (Floor Lamp)
        GameObject lampGroup = new GameObject("Floor_Lamp");
        lampGroup.transform.SetParent(loungeRoot.transform);
        lampGroup.transform.localPosition = new Vector3(2.3f, 0f, 1.8f);

        GameObject lampPole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        lampPole.name = "Pole";
        lampPole.transform.SetParent(lampGroup.transform);
        lampPole.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        lampPole.transform.localScale = new Vector3(0.06f, 0.9f, 0.06f);
        ApplyMaterial(lampPole, "Mat_Lobby_Brass");

        GameObject lampShade = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        lampShade.name = "Shade";
        lampShade.transform.SetParent(lampGroup.transform);
        lampShade.transform.localPosition = new Vector3(0f, 1.7f, 0f);
        lampShade.transform.localScale = new Vector3(0.6f, 0.25f, 0.6f);
        ApplyMaterial(lampShade, "Mat_Lobby_Wall");

        GameObject lightObj = new GameObject("Lamp_Light");
        lightObj.transform.SetParent(lampShade.transform);
        lightObj.transform.localPosition = new Vector3(0f, -0.2f, 0f);

        Light pointLight = lightObj.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.color = new Color(1f, 0.8f, 0.5f);
        pointLight.intensity = 2.5f;
        pointLight.range = 6f;
        pointLight.shadows = LightShadows.Soft;

        // 7. 대형 실내 화분 (Plant)
        GameObject plantGroup = new GameObject("Potted_Plant");
        plantGroup.transform.SetParent(loungeRoot.transform);
        plantGroup.transform.localPosition = new Vector3(-2f, 0f, -1.5f);

        GameObject pot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pot.name = "Plant_Pot";
        pot.transform.SetParent(plantGroup.transform);
        pot.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        pot.transform.localScale = new Vector3(0.5f, 0.25f, 0.5f);
        ApplyMaterial(pot, "Mat_Lobby_Wood");

        GameObject leaves1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves1.name = "Plant_Leaves_1";
        leaves1.transform.SetParent(plantGroup.transform);
        leaves1.transform.localPosition = new Vector3(0f, 0.8f, 0f);
        leaves1.transform.localScale = new Vector3(0.8f, 0.9f, 0.8f);

        GameObject leaves2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves2.name = "Plant_Leaves_2";
        leaves2.transform.SetParent(plantGroup.transform);
        leaves2.transform.localPosition = new Vector3(0.2f, 0.6f, 0.2f);
        leaves2.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        // 8. 라운지 전체 위치 이동
        loungeRoot.transform.position = new Vector3(20.5f, 0f, -4f);

        // 9. 엘리베이터 디테일 생성
        BuildElevatorDetails();

        Selection.activeGameObject = loungeRoot;
        Debug.Log("1층 로비 라운지 (Couch 회전 방향 수정) 생성 완료!");
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