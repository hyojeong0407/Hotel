using UnityEditor;
using UnityEngine;

public static class LobbyLoungeBuilder
{
    [MenuItem("Tools/Hotel Blockout/Build Lobby Lounge (1F)")]
    public static void BuildLounge()
    {
        // 1. 기존 라운지 오브젝트 삭제
        var existing = GameObject.Find("Horror_Lobby_Lounge");
        if (existing != null) Object.DestroyImmediate(existing);

        // 기존 엘리베이터 디테일 오브젝트 삭제 (중복 생성 방지)
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

        // 2. 메인 가죽 소파 (3인용)
        GameObject mainSofa = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mainSofa.name = "Sofa_Main";
        mainSofa.transform.SetParent(loungeRoot.transform);
        mainSofa.transform.localPosition = new Vector3(0f, 0.45f, 1.8f);
        mainSofa.transform.localScale = new Vector3(2.6f, 0.8f, 0.9f);
        ApplyMaterial(mainSofa, "Mat_Lobby_Carpet"); 

        // 소파 등받이
        GameObject mainSofaBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mainSofaBack.name = "Sofa_Main_Back";
        mainSofaBack.transform.SetParent(mainSofa.transform);
        mainSofaBack.transform.localPosition = new Vector3(0f, 0.4f, 0.35f);
        mainSofaBack.transform.localScale = new Vector3(1f, 0.8f, 0.25f);
        ApplyMaterial(mainSofaBack, "Mat_Lobby_Carpet");

        // 3. 1인용 소파 (좌측)
        GameObject chairLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chairLeft.name = "Sofa_Single_Left";
        chairLeft.transform.SetParent(loungeRoot.transform);
        chairLeft.transform.localPosition = new Vector3(-2f, 0.45f, 0f);
        chairLeft.transform.localScale = new Vector3(1f, 0.8f, 0.9f);
        chairLeft.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        ApplyMaterial(chairLeft, "Mat_Lobby_Carpet");

        // 4. 1인용 소파 (우측)
        GameObject chairRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chairRight.name = "Sofa_Single_Right";
        chairRight.transform.SetParent(loungeRoot.transform);
        chairRight.transform.localPosition = new Vector3(2f, 0.45f, 0f);
        chairRight.transform.localScale = new Vector3(1f, 0.8f, 0.9f);
        chairRight.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
        ApplyMaterial(chairRight, "Mat_Lobby_Carpet");

        // 5. 중앙 커피 테이블
        GameObject table = GameObject.CreatePrimitive(PrimitiveType.Cube);
        table.name = "Coffee_Table";
        table.transform.SetParent(loungeRoot.transform);
        table.transform.localPosition = new Vector3(0f, 0.3f, 0f);
        table.transform.localScale = new Vector3(1.8f, 0.4f, 1f);
        ApplyMaterial(table, "Mat_Lobby_Wood");

        // 6. 괘종시계 (Grandfather Clock) - 벽면 구석배치
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

        // 7. 스탠드 조명 (Floor Lamp) - 소파 옆배치
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

        // 은은한 스탠드 조명 추가
        GameObject lightObj = new GameObject("Lamp_Light");
        lightObj.transform.SetParent(lampShade.transform);
        lightObj.transform.localPosition = new Vector3(0f, -0.2f, 0f);

        Light pointLight = lightObj.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.color = new Color(1f, 0.8f, 0.5f);
        pointLight.intensity = 2.5f;
        pointLight.range = 6f;
        pointLight.shadows = LightShadows.Soft;

        // 8. [NEW] 대형 실내 화분 (Plant) - 라운지 구석 장식
        GameObject plantGroup = new GameObject("Potted_Plant");
        plantGroup.transform.SetParent(loungeRoot.transform);
        plantGroup.transform.localPosition = new Vector3(-2f, 0f, -1.5f);

        GameObject pot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pot.name = "Plant_Pot";
        pot.transform.SetParent(plantGroup.transform);
        pot.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        pot.transform.localScale = new Vector3(0.5f, 0.25f, 0.5f);
        ApplyMaterial(pot, "Mat_Lobby_Wood"); // 화분 기둥

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

        // 9. 라운지 전체를 프론트 데스크 옆 통로 중앙으로 이동
        // X: 20.5 (PassageWidth 7m 구간의 중앙)
        loungeRoot.transform.position = new Vector3(20.5f, 0f, -4f);

        // =========================================================
        // 10. [NEW] 엘리베이터 디테일 (문틀, 버튼, 층수 표시기)
        // 엘리베이터는 라운지와 위치가 다르므로 별도의 Root로 생성합니다.
        // =========================================================
        GameObject elevatorDetails = new GameObject("Horror_Elevator_Details");
        Undo.RegisterCreatedObjectUndo(elevatorDetails, "Build Elevator Details");

        float elZ = 2f; 
        float doorWidth = 1.8f;
        float doorHeight = 2.1f;

        // 금속 문틀 (프레임)
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

        // 호출 버튼 (Call Button)
        GameObject callButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
        callButton.name = "Call_Button_Panel";
        callButton.transform.SetParent(elevatorDetails.transform);
        callButton.transform.position = new Vector3(0.32f, 1.1f, elZ - (doorWidth / 2f) - 0.25f);
        callButton.transform.localScale = new Vector3(0.05f, 0.2f, 0.1f);
        ApplyMaterial(callButton, "Mat_Lobby_Wood"); 

        // 층수 표시기 (Indicator)
        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        indicator.name = "Floor_Indicator";
        indicator.transform.SetParent(elevatorDetails.transform);
        indicator.transform.position = new Vector3(0.32f, doorHeight + 0.2f, elZ);
        indicator.transform.localScale = new Vector3(0.05f, 0.15f, 0.4f);
        ApplyMaterial(indicator, "Mat_Lobby_Wood");

        Selection.activeGameObject = loungeRoot;
        Debug.Log("1층 로비 라운지(가구/화분 추가됨) 및 엘리베이터 디테일 생성 완료!");
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