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

        // 1. 프론트 데스크 카운터
        GameObject counter = GameObject.CreatePrimitive(PrimitiveType.Cube);
        counter.name = "Reception_Counter";
        counter.transform.SetParent(deskRoot.transform);
        counter.transform.localPosition = new Vector3(0f, 0.55f, 0f);
        counter.transform.localScale = new Vector3(1f, 1.1f, 4f);
        ApplyMaterial(counter, "Mat_Lobby_Wood");

        // 2-1. 방명록
        GameObject book = GameObject.CreatePrimitive(PrimitiveType.Cube);
        book.name = "Guestbook";
        book.transform.SetParent(deskRoot.transform);
        book.transform.localPosition = new Vector3(0f, 1.12f, 1f);
        book.transform.localScale = new Vector3(0.4f, 0.05f, 0.3f);
        book.transform.localRotation = Quaternion.Euler(0f, 75f, 0f);
        ApplyMaterial(book, "Mat_Lobby_Wall"); // 아이보리 계열 매테리얼 적용

        // 2-2. 대형 호텔 종 (Bell)
        GameObject bellRoot = new GameObject("Reception_Bell");
        bellRoot.transform.SetParent(deskRoot.transform);
        bellRoot.transform.localPosition = new Vector3(0.1f, 1.1f, 0f);

        GameObject bellBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bellBase.name = "Bell_Base";
        bellBase.transform.SetParent(bellRoot.transform);
        bellBase.transform.localPosition = new Vector3(0f, 0.02f, 0f);
        bellBase.transform.localScale = new Vector3(0.3f, 0.02f, 0.3f);
        ApplyMaterial(bellBase, "Mat_Lobby_Brass");

        GameObject bellDome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bellDome.name = "Bell_Dome";
        bellDome.transform.SetParent(bellRoot.transform);
        bellDome.transform.localPosition = new Vector3(0f, 0.08f, 0f);
        bellDome.transform.localScale = new Vector3(0.25f, 0.15f, 0.25f);
        ApplyMaterial(bellDome, "Mat_Lobby_Brass");

        GameObject bellButton = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bellButton.name = "Bell_Button";
        bellButton.transform.SetParent(bellRoot.transform);
        bellButton.transform.localPosition = new Vector3(0f, 0.18f, 0f);
        bellButton.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        ApplyMaterial(bellButton, "Mat_Lobby_Wood"); // 단추는 어두운 색 적용

        BoxCollider bellCollider = bellRoot.AddComponent<BoxCollider>();
        bellCollider.center = new Vector3(0f, 0.09f, 0f);
        bellCollider.size = new Vector3(0.3f, 0.2f, 0.3f);

        // 3. 디테일을 살린 실제 탁상조명 (Desk Lamp)
        GameObject lampRoot = new GameObject("Desk_Lamp");
        lampRoot.transform.SetParent(deskRoot.transform);
        lampRoot.transform.localPosition = new Vector3(0.2f, 1.1f, -1.2f); 

        GameObject lampBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        lampBase.name = "Lamp_Base";
        lampBase.transform.SetParent(lampRoot.transform);
        lampBase.transform.localPosition = new Vector3(0f, 0.02f, 0f);
        lampBase.transform.localScale = new Vector3(0.2f, 0.02f, 0.2f);
        ApplyMaterial(lampBase, "Mat_Lobby_Brass");

        GameObject lampStem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        lampStem.name = "Lamp_Stem";
        lampStem.transform.SetParent(lampRoot.transform);
        lampStem.transform.localPosition = new Vector3(0f, 0.15f, 0f);
        lampStem.transform.localScale = new Vector3(0.02f, 0.15f, 0.02f);
        ApplyMaterial(lampStem, "Mat_Lobby_Brass");

        GameObject lampShade = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lampShade.name = "Lamp_Shade";
        lampShade.transform.SetParent(lampRoot.transform);
        lampShade.transform.localPosition = new Vector3(0f, 0.3f, 0.05f);
        lampShade.transform.localScale = new Vector3(0.15f, 0.1f, 0.15f);
        lampShade.transform.localRotation = Quaternion.Euler(30f, 0f, 0f); 
        ApplyMaterial(lampShade, "Mat_Lobby_Brass");

        GameObject lightObj = new GameObject("Light_Source");
        lightObj.transform.SetParent(lampShade.transform);
        lightObj.transform.localPosition = Vector3.zero;
        lightObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        Light spotLight = lightObj.AddComponent<Light>();
        spotLight.type = LightType.Spot;
        spotLight.color = new Color(0.9f, 0.7f, 0.3f); 
        spotLight.intensity = 4f; 
        spotLight.range = 5f;
        spotLight.spotAngle = 75f; 
        spotLight.shadows = LightShadows.Soft; 

        // 4. 뒤쪽 벽면의 열쇠 보관함
        GameObject keyRack = GameObject.CreatePrimitive(PrimitiveType.Cube);
        keyRack.name = "Key_Cabinet";
        keyRack.transform.SetParent(deskRoot.transform);
        keyRack.transform.localPosition = new Vector3(-1.3f, 1.6f, 0f);
        keyRack.transform.localScale = new Vector3(0.1f, 1.2f, 3f);
        ApplyMaterial(keyRack, "Mat_Lobby_Wood");

        // 5. 열쇠 보관함에 낡은 방 열쇠(Keys) 3개 매달기
        for (int i = 0; i < 3; i++)
        {
            GameObject key = GameObject.CreatePrimitive(PrimitiveType.Cube);
            key.name = "Room_Key_" + i;
            key.transform.SetParent(keyRack.transform);
            key.transform.localPosition = new Vector3(0.6f, 0.2f - (i * 0.2f), -0.3f + (i * 0.25f));
            key.transform.localScale = new Vector3(0.5f, 0.1f, 0.05f);
            key.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-10f, 10f));
            ApplyMaterial(key, "Mat_Lobby_Brass");
        }

        // 6. 정확한 위치 지정
        deskRoot.transform.position = new Vector3(15.5f, 0f, -4f);

        Selection.activeGameObject = deskRoot;
        Debug.Log("매테리얼이 입혀진 1층 프론트 데스크 세팅 완료!");
    }

    // --- Assets/Art/Models/Materials 폴더에서 매테리얼을 불러와 적용하는 헬퍼 함수 ---
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
        else
        {
            Debug.LogWarning($"[FrontDeskBuilder] 매테리얼을 찾을 수 없습니다: {path}");
        }
    }
}