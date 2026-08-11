using UnityEditor;
using UnityEngine;

public static class LobbyLoungeBuilder
{
    [MenuItem("Tools/Hotel Blockout/Build Lobby Lounge (1F)")]
    public static void BuildLounge()
    {
        var existing = GameObject.Find("Horror_Lobby_Lounge");
        if (existing != null) Object.DestroyImmediate(existing);

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
        ApplyMaterial(mainSofa, "Mat_Lobby_Carpet"); // 무거운 붉은 가죽/패브릭 재질 활용

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

        // 8. 105호 객실을 벗어나, 프론트 데스크 옆 통로(Passage) 중앙으로 이동
        // X: 20.5 (PassageWidth 7m 구간의 중앙)
        loungeRoot.transform.position = new Vector3(20.5f, 0f, -4f);

        Selection.activeGameObject = loungeRoot;
        Debug.Log("1층 로비 라운지(가구가 매테리얼과 함께 생성되었습니다!");
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