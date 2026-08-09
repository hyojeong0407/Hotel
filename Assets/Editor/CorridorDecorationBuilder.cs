using UnityEditor;
using UnityEngine;

public static class CorridorDecorationBuilder
{
    // HotelBlockoutBuilder의 기준 수치
    const float StoryHeight = 2.7f; // 층간 높이 (2.5 + 0.2)
    const float NorthGapCenter_X = 19f; 
    const float NorthGapCenter_Z = 8f;
    const float WestVoidCenter_X = 19f;
    const float WestVoidCenter_Z = -4f;
    const float ElLobbyCenter_X = 2.2f;
    const float ElLobbyCenter_Z = 2f;

    [MenuItem("Tools/Hotel Blockout/Decorate Corridor - 2F (Elegance)")]
    public static void DecorateFloor2()
    {
        int floorNum = 2;
        float yOffset = (floorNum - 1) * StoryHeight;
        
        GameObject root = CreateRoot("Floor2_Decorations", yOffset);

        // 1. North Gap (시간의 알코브)
        GameObject northGap = new GameObject("North_Gap_Decor");
        northGap.transform.SetParent(root.transform);
        northGap.transform.localPosition = new Vector3(NorthGapCenter_X, 0f, NorthGapCenter_Z);

        MakeBlockout(northGap.transform, "Grandfather_Clock", PrimitiveType.Cube, new Vector3(0, 1.2f, 3.8f), new Vector3(0.8f, 2.4f, 0.4f), Color.white);
        MakeBlockout(northGap.transform, "Armchair_Left", PrimitiveType.Cube, new Vector3(-1.5f, 0.4f, 2f), new Vector3(1f, 0.8f, 1f), Color.white);
        MakeBlockout(northGap.transform, "Armchair_Right", PrimitiveType.Cube, new Vector3(1.5f, 0.4f, 2f), new Vector3(1f, 0.8f, 1f), Color.white);
        MakeBlockout(northGap.transform, "Tea_Table", PrimitiveType.Cylinder, new Vector3(0, 0.3f, 2f), new Vector3(1.2f, 0.3f, 1.2f), Color.white);

        // 2. West Void (미니 갤러리)
        GameObject westVoid = new GameObject("West_Void_Decor");
        westVoid.transform.SetParent(root.transform);
        westVoid.transform.localPosition = new Vector3(WestVoidCenter_X, 0f, WestVoidCenter_Z);

        MakeBlockout(westVoid.transform, "Statue_Base", PrimitiveType.Cube, new Vector3(0, 0.4f, -2f), new Vector3(1f, 0.8f, 1f), Color.white);
        MakeBlockout(westVoid.transform, "Marble_Statue", PrimitiveType.Cylinder, new Vector3(0, 1.6f, -2f), new Vector3(0.6f, 0.8f, 0.6f), Color.white);
        MakeBlockout(westVoid.transform, "Brass_Pot", PrimitiveType.Cylinder, new Vector3(3f, 0.4f, -3f), new Vector3(0.8f, 0.4f, 0.8f), Color.white);

        // 3. EL 로비 (수하물 대기열)
        GameObject elLobby = new GameObject("EL_Lobby_Decor");
        elLobby.transform.SetParent(root.transform);
        elLobby.transform.localPosition = new Vector3(ElLobbyCenter_X, 0f, ElLobbyCenter_Z);

        MakeBlockout(elLobby.transform, "Luggage_Cart", PrimitiveType.Cube, new Vector3(-1.5f, 0.8f, -1.5f), new Vector3(1.5f, 1.6f, 0.8f), Color.white);
        
        MakeBlockout(northGap.transform, "Console_Table", PrimitiveType.Cube, new Vector3(2.5f, 0.4f, 0f), new Vector3(1.5f, 0.8f, 0.4f), Color.white);
        MakeBlockout(northGap.transform, "Porcelain_Vase", PrimitiveType.Cylinder, new Vector3(2.5f, 0.9f, 0f), new Vector3(0.3f, 0.4f, 0.3f), Color.white);
        
        MakeBlockout(elLobby.transform, "Brass_TrashBin", PrimitiveType.Cylinder, new Vector3(1.5f, 0.6f, -1f), new Vector3(0.5f, 0.6f, 0.5f), Color.white);

        // --- 2층 복도 카펫 (우아함) ---
        GameObject carpet2F = new GameObject("Red_Carpet");
        carpet2F.transform.SetParent(root.transform, false); 
        MakeBlockout(carpet2F.transform, "Main_Carpet", PrimitiveType.Cube, new Vector3(10.5f, 0.01f, 2f), new Vector3(15f, 0.02f, 2f), new Color(0.6f, 0.1f, 0.1f));

        // --- 2층 객실 벽부등 (오렌지빛) ---
        Color light2F = new Color(1f, 0.8f, 0.5f);
        
        // 북쪽 벽 (1, 2, 3호실) - 벽 표면(Z:3.8)에 완벽하게 밀착되는 Z:3.75, 회전 0도
        AddWallSconce(root.transform, "Sconce_Room1", new Vector3(41.9f, 1.8f, 3.75f), 0f, light2F);
        AddWallSconce(root.transform, "Sconce_Room2", new Vector3(31.9f, 1.8f, 3.75f), 0f, light2F);
        AddWallSconce(root.transform, "Sconce_Room3", new Vector3(11.9f, 1.8f, 3.75f), 0f, light2F);
        
        // 남쪽 벽 (4, 5호실) - 벽 표면(Z:0.2)에 완벽하게 밀착되는 Z:0.25, 회전 180도
        AddWallSconce(root.transform, "Sconce_Room4", new Vector3(36.1f, 1.8f, 0.25f), 180f, light2F);
        AddWallSconce(root.transform, "Sconce_Room5", new Vector3(6.1f, 1.8f, 0.25f), 180f, light2F);

        // 💡 2층 메인 조명
        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), light2F, 4f, 8f);

        Debug.Log("2F 복도 장식 및 조명 세팅 완료!");
    }

    [MenuItem("Tools/Hotel Blockout/Decorate Corridor - 3F (Decaying)")]
    public static void DecorateFloor3()
    {
        int floorNum = 3;
        float yOffset = (floorNum - 1) * StoryHeight;
        
        GameObject root = CreateRoot("Floor3_Decorations", yOffset);

        // 1. North Gap (망가진 티타임)
        GameObject northGap = new GameObject("North_Gap_Decor");
        northGap.transform.SetParent(root.transform);
        northGap.transform.localPosition = new Vector3(NorthGapCenter_X, 0f, NorthGapCenter_Z);

        GameObject clock = MakeBlockout(northGap.transform, "Broken_Clock", PrimitiveType.Cube, new Vector3(0, 1.2f, 3.8f), new Vector3(0.8f, 2.4f, 0.4f), Color.white);
        clock.transform.localRotation = Quaternion.Euler(0, 0, 15f);
        GameObject chair = MakeBlockout(northGap.transform, "Overturned_Chair", PrimitiveType.Cube, new Vector3(-1.5f, 0.5f, 2f), new Vector3(1f, 1f, 0.8f), Color.white);
        chair.transform.localRotation = Quaternion.Euler(90f, 30f, 0); 
        MakeBlockout(northGap.transform, "Broken_Table", PrimitiveType.Cylinder, new Vector3(0.5f, 0.1f, 2.5f), new Vector3(1.2f, 0.1f, 0.8f), Color.white);

        // 2. West Void (불길한 린넨 카트)
        GameObject westVoid = new GameObject("West_Void_Decor");
        westVoid.transform.SetParent(root.transform);
        westVoid.transform.localPosition = new Vector3(WestVoidCenter_X, 0f, WestVoidCenter_Z);

        MakeBlockout(westVoid.transform, "Creepy_Linen_Cart", PrimitiveType.Cube, new Vector3(2f, 0.6f, -1f), new Vector3(1.2f, 1.2f, 1.8f), Color.white);
        MakeBlockout(westVoid.transform, "Covered_Figure", PrimitiveType.Sphere, new Vector3(2f, 1.4f, -0.5f), new Vector3(0.8f, 0.8f, 0.8f), Color.white);

        // 3. EL 로비 (오염)
        GameObject elLobby = new GameObject("EL_Lobby_Decor");
        elLobby.transform.SetParent(root.transform);
        elLobby.transform.localPosition = new Vector3(ElLobbyCenter_X, 0f, ElLobbyCenter_Z);

        MakeBlockout(elLobby.transform, "Dirty_Rug", PrimitiveType.Cube, new Vector3(0, 0.01f, 0), new Vector3(3f, 0.02f, 2f), Color.gray);
        GameObject janitorCart = MakeBlockout(elLobby.transform, "Broken_Janitor_Cart", PrimitiveType.Cube, new Vector3(1.5f, 0.4f, 1f), new Vector3(0.8f, 1.2f, 0.6f), Color.white);
        janitorCart.transform.localRotation = Quaternion.Euler(0, 0, 75f); 
        MakeBlockout(elLobby.transform, "Spilled_Liquid", PrimitiveType.Cube, new Vector3(1.2f, 0.01f, 0.5f), new Vector3(2f, 0.02f, 1.5f), Color.black);
        
        GameObject creepyPainting = MakeBlockout(westVoid.transform, "Crooked_Painting", PrimitiveType.Cube, new Vector3(-2f, 1.5f, -3.9f), new Vector3(1f, 1.2f, 0.05f), Color.white);
        creepyPainting.transform.localRotation = Quaternion.Euler(0, 0, -25f);

        // --- 3층 복도 카펫 (부패, 훼손됨) ---
        GameObject carpet3F = new GameObject("Torn_Carpet");
        carpet3F.transform.SetParent(root.transform, false);
        MakeBlockout(carpet3F.transform, "Carpet_Piece_1", PrimitiveType.Cube, new Vector3(5f, 0.01f, 2f), new Vector3(4f, 0.02f, 1.8f), new Color(0.3f, 0.1f, 0.1f));
        GameObject piece2 = MakeBlockout(carpet3F.transform, "Carpet_Piece_2", PrimitiveType.Cube, new Vector3(10f, 0.01f, 2.1f), new Vector3(3.5f, 0.02f, 1.5f), new Color(0.2f, 0.05f, 0.05f));
        piece2.transform.localRotation = Quaternion.Euler(0, 5f, 0);
        MakeBlockout(carpet3F.transform, "Carpet_Piece_3", PrimitiveType.Cube, new Vector3(15f, 0.01f, 1.9f), new Vector3(4.5f, 0.02f, 1.7f), new Color(0.25f, 0.1f, 0.1f));

        // --- 3층 객실 벽부등 (차갑고 탁한 빛) ---
        Color light3F = new Color(0.6f, 0.7f, 0.7f);
        
        AddWallSconce(root.transform, "Sconce_Room1", new Vector3(41.9f, 1.8f, 3.75f), 0f, light3F);
        AddWallSconce(root.transform, "Sconce_Room2", new Vector3(31.9f, 1.8f, 3.75f), 0f, light3F);
        AddWallSconce(root.transform, "Sconce_Room3", new Vector3(11.9f, 1.8f, 3.75f), 0f, light3F);
        
        AddWallSconce(root.transform, "Sconce_Room4", new Vector3(36.1f, 1.8f, 0.25f), 180f, light3F);
        AddWallSconce(root.transform, "Sconce_Room5", new Vector3(6.1f, 1.8f, 0.25f), 180f, light3F);

        // 💡 3층 메인 조명
        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), light3F, 3f, 7f);

        Debug.Log("3F 복도 장식 및 조명 세팅 완료!");
    }

    [MenuItem("Tools/Hotel Blockout/Decorate Corridor - 5F (Occult)")]
    public static void DecorateFloor5()
    {
        int floorNum = 5;
        float yOffset = (floorNum - 1) * StoryHeight;
        
        GameObject root = CreateRoot("Floor5_Decorations", yOffset);

        // 1. North Gap (광신도의 제단)
        GameObject northGap = new GameObject("North_Gap_Decor");
        northGap.transform.SetParent(root.transform);
        northGap.transform.localPosition = new Vector3(NorthGapCenter_X, 0f, NorthGapCenter_Z);

        MakeBlockout(northGap.transform, "Altar_Table", PrimitiveType.Cube, new Vector3(0, 0.5f, 2f), new Vector3(4f, 1f, 1.5f), Color.white);
        MakeBlockout(northGap.transform, "Phonograph_Base", PrimitiveType.Cube, new Vector3(0, 1.2f, 2f), new Vector3(0.6f, 0.4f, 0.6f), Color.white);
        GameObject horn = MakeBlockout(northGap.transform, "Phonograph_Horn", PrimitiveType.Cylinder, new Vector3(0, 1.6f, 1.8f), new Vector3(0.4f, 0.4f, 0.4f), Color.white);
        horn.transform.localRotation = Quaternion.Euler(45f, 0, 0);

        // 2. West Void (기괴한 상징물)
        GameObject westVoid = new GameObject("West_Void_Decor");
        westVoid.transform.SetParent(root.transform);
        westVoid.transform.localPosition = new Vector3(WestVoidCenter_X, 0f, WestVoidCenter_Z);

        MakeBlockout(westVoid.transform, "Tapestry", PrimitiveType.Cube, new Vector3(0, 1.5f, -3.9f), new Vector3(5f, 2f, 0.1f), Color.white);
        MakeBlockout(westVoid.transform, "Brazier", PrimitiveType.Cylinder, new Vector3(0, 0.5f, -2f), new Vector3(1f, 0.6f, 1f), Color.white);

        // 3. EL 로비 (봉인된 수하물)
        GameObject elLobby = new GameObject("EL_Lobby_Decor");
        elLobby.transform.SetParent(root.transform);
        elLobby.transform.localPosition = new Vector3(ElLobbyCenter_X, 0f, ElLobbyCenter_Z);

        MakeBlockout(elLobby.transform, "Chained_Trunk_1", PrimitiveType.Cube, new Vector3(-1.5f, 0.3f, -1.5f), new Vector3(1f, 0.6f, 0.8f), Color.white);
        MakeBlockout(elLobby.transform, "Chained_Trunk_2", PrimitiveType.Cube, new Vector3(-1.4f, 0.8f, -1.6f), new Vector3(0.8f, 0.4f, 0.6f), Color.white);

        MakeBlockout(westVoid.transform, "Candle_1", PrimitiveType.Cylinder, new Vector3(1f, 0.1f, -1.5f), new Vector3(0.1f, 0.2f, 0.1f), Color.white);
        MakeBlockout(westVoid.transform, "Candle_2", PrimitiveType.Cylinder, new Vector3(-0.8f, 0.1f, -2.2f), new Vector3(0.1f, 0.2f, 0.1f), Color.white);
        MakeBlockout(westVoid.transform, "Candle_3", PrimitiveType.Cylinder, new Vector3(0.5f, 0.05f, -2.8f), new Vector3(0.1f, 0.1f, 0.1f), Color.white);
        MakeBlockout(westVoid.transform, "Candle_4", PrimitiveType.Cylinder, new Vector3(-0.2f, 0.15f, -1.8f), new Vector3(0.1f, 0.3f, 0.1f), Color.white);

        MakeBlockout(northGap.transform, "Bone_Pile", PrimitiveType.Sphere, new Vector3(2f, 0.3f, 2.5f), new Vector3(1.2f, 0.6f, 1f), Color.white);

        // --- 5층 복도 카펫 (오컬트, 핏자국) ---
        GameObject carpet5F = new GameObject("Bloody_Carpet");
        carpet5F.transform.SetParent(root.transform, false);
        MakeBlockout(carpet5F.transform, "Main_Drag_Mark", PrimitiveType.Cube, new Vector3(11f, 0.01f, 2f), new Vector3(14f, 0.02f, 1.2f), new Color(0.3f, 0f, 0f));
        MakeBlockout(carpet5F.transform, "Altar_Drag_Mark", PrimitiveType.Cube, new Vector3(18f, 0.01f, 5f), new Vector3(1.2f, 0.02f, 6f), new Color(0.3f, 0f, 0f));

        // --- 5층 객실 벽부등 (핏빛 조명) ---
        Color light5F = new Color(0.8f, 0.1f, 0.1f);
        
        AddWallSconce(root.transform, "Sconce_Room1", new Vector3(41.9f, 1.8f, 3.75f), 0f, light5F);
        AddWallSconce(root.transform, "Sconce_Room2", new Vector3(31.9f, 1.8f, 3.75f), 0f, light5F);
        AddWallSconce(root.transform, "Sconce_Room3", new Vector3(11.9f, 1.8f, 3.75f), 0f, light5F);
        
        AddWallSconce(root.transform, "Sconce_Room4", new Vector3(36.1f, 1.8f, 0.25f), 180f, light5F);
        AddWallSconce(root.transform, "Sconce_Room5", new Vector3(6.1f, 1.8f, 0.25f), 180f, light5F);

        // 💡 5층 메인 조명
        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), light5F, 5f, 8f);

        Debug.Log("5F 복도 장식 및 조명 세팅 완료!");
    }

    // --- Helper Methods ---

    static GameObject CreateRoot(string name, float yOffset)
    {
        var existing = GameObject.Find(name);
        if (existing != null) Object.DestroyImmediate(existing);
        
        GameObject root = new GameObject(name);
        root.transform.position = new Vector3(0f, yOffset, 0f);
        Undo.RegisterCreatedObjectUndo(root, "Build Decorations");
        return root;
    }

    static GameObject MakeBlockout(Transform parent, string name, PrimitiveType type, Vector3 localPos, Vector3 scale, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localScale = scale;

        Collider col = go.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);

        return go;
    }

    static void AddLight(Transform parent, Vector3 localPos, Color color, float intensity, float range)
    {
        GameObject lightObj = new GameObject("Corridor_MoodLight");
        lightObj.transform.SetParent(parent, false);
        lightObj.transform.localPosition = localPos;
        
        lightObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        Light l = lightObj.AddComponent<Light>();
        
        l.type = LightType.Spot;
        l.spotAngle = 140f; 
        l.color = color;
        l.intensity = intensity;
        l.range = range; 
        
        l.shadows = LightShadows.Soft;
    }

    // 벽면에 완벽하게 부착되도록 수정한 벽부등 함수
    static void AddWallSconce(Transform parent, string name, Vector3 pos, float yRotation, Color lightColor)
    {
        GameObject sconce = new GameObject(name);
        sconce.transform.SetParent(parent, false);
        sconce.transform.localPosition = pos;
        
        sconce.transform.localRotation = Quaternion.Euler(0, yRotation, 0);

        MakeBlockout(sconce.transform, "Base", PrimitiveType.Cube, Vector3.zero, new Vector3(0.2f, 0.4f, 0.1f), Color.black);
        MakeBlockout(sconce.transform, "Bulb", PrimitiveType.Sphere, new Vector3(0, 0.1f, -0.1f), new Vector3(0.2f, 0.2f, 0.2f), lightColor);

        GameObject lightObj = new GameObject("Light");
        lightObj.transform.SetParent(sconce.transform, false);
        lightObj.transform.localPosition = new Vector3(0, 0.1f, -0.2f);
        
        Light l = lightObj.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = lightColor;
        l.intensity = 2f;
        l.range = 4f;
        l.shadows = LightShadows.Soft;
    }
}