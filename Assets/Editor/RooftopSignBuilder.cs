using UnityEditor;
using UnityEngine;

public static class RooftopSignBuilder
{
    // 호텔 건물 규격
    const float RoofY = 13.5f;
    const float RoofCenterX = 22f;
    const float RoofCenterZ = 2f;

    [MenuItem("Tools/Hotel Blockout/Build Custom Rooftop Sign (Final Layout)")]
    public static void BuildSign()
    {
        var existing = GameObject.Find("Rooftop_Sign_Custom");
        if (existing) Object.DestroyImmediate(existing);

        GameObject root = new GameObject("Rooftop_Sign_Custom");
        Undo.RegisterCreatedObjectUndo(root, "Build Final Layout Sign");
        
        root.transform.position = new Vector3(RoofCenterX, RoofY, RoofCenterZ);

        Material frameMat = new Material(Shader.Find("Standard"));
        frameMat.color = new Color(0.1f, 0.1f, 0.1f);
        frameMat.SetFloat("_Metallic", 0.8f);
        frameMat.SetFloat("_Glossiness", 0.3f);

        Color neonCyan = new Color(0.3f, 0.8f, 1f);     
        Color dimCyan = new Color(0.1f, 0.2f, 0.3f);    
        
        Color neonGold = new Color(1f, 0.7f, 0.2f);     
        Color dimGold = new Color(0.3f, 0.15f, 0.05f);  
        
        Color deadNeonFront = new Color(0.12f, 0.12f, 0.12f); 
        Color deadNeonBack = new Color(0.05f, 0.05f, 0.05f);

        // 1. 철골 프레임 조립 (글자가 넓어짐에 따라 너비를 20m로 확장)
        BuildBillboardFrame(root.transform, frameMat);

        // 2. 상단: H O T E L (자간을 시원하게 넓힘)
        float topY = 5.8f; 
        float topSpacing = 4.3f; // ⭐️ 기존 3.8 -> 4.3으로 자간 넓힘
        float topSize = 0.55f; 
        
        string[] hotelLetters = { "H", "O", "T", "E", "L" };
        for (int i = 0; i < hotelLetters.Length; i++)
        {
            float xPos = -(topSpacing * 2) + (i * topSpacing);
            Color frontColor = (hotelLetters[i] == "O") ? deadNeonFront : neonCyan;
            Color backColor = (hotelLetters[i] == "O") ? deadNeonBack : dimCyan;
            
            CreateTextGlow(root.transform, "Letter_" + hotelLetters[i], hotelLetters[i], 
                new Vector3(xPos, topY, -0.4f), 120, topSize, frontColor, backColor);
        }

        // 3. 하단: GRAND NOWHERE (호텔 품 안으로 들어오도록 크기와 자간 축소)
        float bottomY = 2.2f; 
        float bottomSize = 0.22f; // ⭐️ 기존 0.32 -> 0.22로 폰트 크기 축소
        float bottomSpacing = 1.6f;
        
        string grandNowhere = "GRAND NOWHERE";
        float startX = -((grandNowhere.Length - 1) * bottomSpacing) / 2f;

        for (int i = 0; i < grandNowhere.Length; i++)
        {
            if (grandNowhere[i] == ' ') continue; 

            float xPos = startX + (i * bottomSpacing);
            CreateTextGlow(root.transform, "Text_GN_" + i, grandNowhere[i].ToString(), 
                new Vector3(xPos, bottomY, -0.4f), 100, bottomSize, neonGold, dimGold);
        }

        // 4. 듀얼 조명 시스템
        CreatePointLight(root.transform, "Light_Cyan_Left", new Vector3(-4.5f, topY, -3f), neonCyan, 6f, 20f);
        CreatePointLight(root.transform, "Light_Cyan_Right", new Vector3(4.5f, topY, -3f), neonCyan, 6f, 20f);
        CreatePointLight(root.transform, "Light_Gold_Center", new Vector3(0f, bottomY, -3f), neonGold, 4f, 15f);

        Selection.activeGameObject = root;
        Debug.Log("호텔 자간 확장 및 그랜드 노웨어 크기 조정 완료!");
    }

    // --- Helper Methods ---
    static void BuildBillboardFrame(Transform parent, Material mat)
    {
        GameObject frameRoot = new GameObject("Iron_Frame");
        frameRoot.transform.SetParent(parent, false);

        // 수직 기둥 5개 (간격을 4.5m씩 벌리고 너비에 맞춤)
        for (int i = 0; i < 5; i++)
        {
            float xPos = -9f + (i * 4.5f); 
            CreateBox(frameRoot.transform, "Pillar_" + i, new Vector3(xPos, 4.0f, 0f), new Vector3(0.25f, 8.0f, 0.25f), mat);
        }

        // 수평 빔 3개 (너비를 20m로 늘림)
        CreateBox(frameRoot.transform, "Beam_Top", new Vector3(0f, 7.5f, 0f), new Vector3(20f, 0.25f, 0.25f), mat);
        CreateBox(frameRoot.transform, "Beam_Mid", new Vector3(0f, 4.0f, 0f), new Vector3(20f, 0.25f, 0.25f), mat);
        CreateBox(frameRoot.transform, "Beam_Bot", new Vector3(0f, 0.5f, 0f), new Vector3(20f, 0.25f, 0.25f), mat);
    }

    static void CreateBox(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(parent, false);
        box.transform.localPosition = pos;
        box.transform.localScale = scale;
        box.GetComponent<MeshRenderer>().sharedMaterial = mat;
    }

    static void CreateTextGlow(Transform parent, string name, string text, Vector3 pos, int fontSize, float scale, Color front, Color back)
    {
        GameObject textRoot = new GameObject(name);
        textRoot.transform.SetParent(parent, false);
        textRoot.transform.localPosition = pos;

        GameObject backText = new GameObject("Back");
        backText.transform.SetParent(textRoot.transform, false);
        backText.transform.localPosition = new Vector3(0f, 0f, 0.3f); 
        TextMesh tmBack = backText.AddComponent<TextMesh>();
        tmBack.text = text;
        tmBack.fontSize = fontSize;
        tmBack.characterSize = scale;
        tmBack.anchor = TextAnchor.MiddleCenter;
        tmBack.alignment = TextAlignment.Center;
        tmBack.color = back;

        GameObject frontText = new GameObject("Front");
        frontText.transform.SetParent(textRoot.transform, false);
        frontText.transform.localPosition = Vector3.zero;
        TextMesh tmFront = frontText.AddComponent<TextMesh>();
        tmFront.text = text;
        tmFront.fontSize = fontSize;
        tmFront.characterSize = scale;
        tmFront.anchor = TextAnchor.MiddleCenter;
        tmFront.alignment = TextAlignment.Center;
        tmFront.color = front;
    }

    static void CreatePointLight(Transform parent, string name, Vector3 pos, Color color, float intensity, float range)
    {
        GameObject lightObj = new GameObject(name);
        lightObj.transform.SetParent(parent, false);
        lightObj.transform.localPosition = pos;
        
        Light l = lightObj.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = color;
        l.intensity = intensity;
        l.range = range;
        l.shadows = LightShadows.Soft;
    }
}