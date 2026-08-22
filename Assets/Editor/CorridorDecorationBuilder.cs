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

    [MenuItem("Tools/Hotel Blockout/Decorate Corridor - 1F (Lobby and Staff)")]
    public static void DecorateFloor1()
    {
        int floorNum = 1;
        float yOffset = (floorNum - 1) * StoryHeight;
        
        GameObject root = CreateRoot("Floor1_Decorations", yOffset);

        Color light1F = new Color(1f, 0.9f, 0.7f);
        
        AddWallSconce(root.transform, "Sconce_Room1", new Vector3(42.1f, 1.8f, 3.75f), 0f, light1F);
        AddWallSconce(root.transform, "Sconce_Room2", new Vector3(31.9f, 1.8f, 3.75f), 0f, light1F);
        AddWallSconce(root.transform, "Sconce_Room3", new Vector3(11.9f, 1.8f, 3.75f), 0f, light1F);
        AddWallSconce(root.transform, "Sconce_Room4", new Vector3(36.3f, 1.8f, 0.25f), 180f, light1F);
        AddWallSconce(root.transform, "Sconce_Room5", new Vector3(6.1f, 1.8f, 0.25f), 180f, light1F);
        
        AddWallSconce(root.transform, "Sconce_StaffRoom", new Vector3(29.8f, 1.8f, 0.25f), 180f, light1F);

        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), light1F, 4f, 8f);
        AddLight(root.transform, new Vector3(29f, 2.4f, 2f), light1F, 4f, 8f);

        // 💡 1층 스태프룸 통합 함수 호출
        BuildStandardStaffRoom(root.transform);

        Debug.Log("1F 복도 및 스태프룸 장식 세팅 완료!");
    }

    [MenuItem("Tools/Hotel Blockout/Decorate Corridor - 2F (Elegance)")]
    public static void DecorateFloor2()
    {
        int floorNum = 2;
        float yOffset = (floorNum - 1) * StoryHeight;
        
        GameObject root = CreateRoot("Floor2_Decorations", yOffset);

        GameObject northGap = new GameObject("North_Gap_Decor");
        northGap.transform.SetParent(root.transform, false);
        northGap.transform.localPosition = new Vector3(NorthGapCenter_X, 0f, NorthGapCenter_Z);

        MakeBlockout(northGap.transform, "Grandfather_Clock", PrimitiveType.Cube, new Vector3(0, 1.2f, 3.7f), new Vector3(0.8f, 2.4f, 0.4f), Color.white);
        MakeBlockout(northGap.transform, "Armchair_Left", PrimitiveType.Cube, new Vector3(-1.5f, 0.4f, 2f), new Vector3(1f, 0.8f, 1f), Color.white);
        MakeBlockout(northGap.transform, "Armchair_Right", PrimitiveType.Cube, new Vector3(1.5f, 0.4f, 2f), new Vector3(1f, 0.8f, 1f), Color.white);
        MakeBlockout(northGap.transform, "Tea_Table", PrimitiveType.Cylinder, new Vector3(0, 0.3f, 2f), new Vector3(1.2f, 0.3f, 1.2f), Color.white);
        MakeBlockout(northGap.transform, "Console_Table", PrimitiveType.Cube, new Vector3(2.5f, 0.4f, 0f), new Vector3(1.5f, 0.8f, 0.4f), Color.white);
        MakeBlockout(northGap.transform, "Porcelain_Vase", PrimitiveType.Cylinder, new Vector3(2.5f, 1.0f, 0f), new Vector3(0.3f, 0.2f, 0.3f), Color.white);

        GameObject westVoid = new GameObject("West_Void_Decor");
        westVoid.transform.SetParent(root.transform, false);
        westVoid.transform.localPosition = new Vector3(WestVoidCenter_X, 0f, WestVoidCenter_Z);

        MakeBlockout(westVoid.transform, "Statue_Base", PrimitiveType.Cube, new Vector3(0, 0.4f, -2f), new Vector3(1f, 0.8f, 1f), Color.white);
        MakeBlockout(westVoid.transform, "Marble_Statue", PrimitiveType.Cylinder, new Vector3(0, 1.6f, -2f), new Vector3(0.6f, 0.8f, 0.6f), Color.white);
        MakeBlockout(westVoid.transform, "Brass_Pot", PrimitiveType.Cylinder, new Vector3(3f, 0.4f, -3f), new Vector3(0.8f, 0.4f, 0.8f), Color.white);

        GameObject elLobby = new GameObject("EL_Lobby_Decor");
        elLobby.transform.SetParent(root.transform, false);
        elLobby.transform.localPosition = new Vector3(ElLobbyCenter_X, 0f, ElLobbyCenter_Z);

        MakeBlockout(elLobby.transform, "Luggage_Cart", PrimitiveType.Cube, new Vector3(-1.3f, 0.8f, -1.5f), new Vector3(1.5f, 1.6f, 0.8f), Color.white);
        MakeBlockout(elLobby.transform, "Brass_TrashBin", PrimitiveType.Cylinder, new Vector3(1.5f, 0.6f, -1f), new Vector3(0.5f, 0.6f, 0.5f), Color.white);

        GameObject carpet2F = new GameObject("Red_Carpet");
        carpet2F.transform.SetParent(root.transform, false); 
        MakeBlockout(carpet2F.transform, "Main_Carpet", PrimitiveType.Cube, new Vector3(10.5f, 0.01f, 2f), new Vector3(15f, 0.02f, 2f), new Color(0.6f, 0.1f, 0.1f));

        Color light2F = new Color(1f, 0.8f, 0.5f);
        
        AddWallSconce(root.transform, "Sconce_Room1", new Vector3(42.1f, 1.8f, 3.75f), 0f, light2F);
        AddWallSconce(root.transform, "Sconce_Room2", new Vector3(31.9f, 1.8f, 3.75f), 0f, light2F);
        AddWallSconce(root.transform, "Sconce_Room3", new Vector3(11.9f, 1.8f, 3.75f), 0f, light2F);
        AddWallSconce(root.transform, "Sconce_Room4", new Vector3(36.3f, 1.8f, 0.25f), 180f, light2F);
        AddWallSconce(root.transform, "Sconce_Room5", new Vector3(6.1f, 1.8f, 0.25f), 180f, light2F);
        
        // 💡 2층 스태프룸 입구 벽부등 추가
        AddWallSconce(root.transform, "Sconce_StaffRoom", new Vector3(29.8f, 1.8f, 0.25f), 180f, light2F);

        AddWelcomeMats(root.transform, "Elegant_Mat", new Color(0.5f, 0.1f, 0.1f));
        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), light2F, 4f, 8f);

        // 💡 2층 스태프룸 통합 함수 호출
        BuildStandardStaffRoom(root.transform);

        Debug.Log("2F 복도 장식 및 조명 세팅 완료!");
    }

    [MenuItem("Tools/Hotel Blockout/Decorate Corridor - 3F (Decaying)")]
    public static void DecorateFloor3()
    {
        int floorNum = 3;
        float yOffset = (floorNum - 1) * StoryHeight;
        
        GameObject root = CreateRoot("Floor3_Decorations", yOffset);

        GameObject northGap = new GameObject("North_Gap_Decor");
        northGap.transform.SetParent(root.transform, false);
        northGap.transform.localPosition = new Vector3(NorthGapCenter_X, 0f, NorthGapCenter_Z);

        GameObject clock = MakeBlockout(northGap.transform, "Broken_Clock", PrimitiveType.Cube, new Vector3(0, 1.2f, 3.7f), new Vector3(0.8f, 2.4f, 0.4f), Color.white);
        clock.transform.localRotation = Quaternion.Euler(0, 0, 15f);
        GameObject chair = MakeBlockout(northGap.transform, "Overturned_Chair", PrimitiveType.Cube, new Vector3(-1.5f, 0.5f, 2f), new Vector3(1f, 1f, 0.8f), Color.white);
        chair.transform.localRotation = Quaternion.Euler(90f, 30f, 0); 
        MakeBlockout(northGap.transform, "Broken_Table", PrimitiveType.Cylinder, new Vector3(0.5f, 0.1f, 2.5f), new Vector3(1.2f, 0.1f, 0.8f), Color.white);

        GameObject westVoid = new GameObject("West_Void_Decor");
        westVoid.transform.SetParent(root.transform, false);
        westVoid.transform.localPosition = new Vector3(WestVoidCenter_X, 0f, WestVoidCenter_Z);

        MakeBlockout(westVoid.transform, "Creepy_Linen_Cart", PrimitiveType.Cube, new Vector3(2f, 0.6f, -1f), new Vector3(1.2f, 1.2f, 1.8f), Color.white);
        MakeBlockout(westVoid.transform, "Covered_Figure", PrimitiveType.Sphere, new Vector3(2f, 1.4f, -0.5f), new Vector3(0.8f, 0.8f, 0.8f), Color.white);
        GameObject creepyPainting = MakeBlockout(westVoid.transform, "Crooked_Painting", PrimitiveType.Cube, new Vector3(-2f, 1.5f, -3.9f), new Vector3(1f, 1.2f, 0.05f), Color.white);
        creepyPainting.transform.localRotation = Quaternion.Euler(0, 0, -25f);

        GameObject elLobby = new GameObject("EL_Lobby_Decor");
        elLobby.transform.SetParent(root.transform, false);
        elLobby.transform.localPosition = new Vector3(ElLobbyCenter_X, 0f, ElLobbyCenter_Z);

        MakeBlockout(elLobby.transform, "Dirty_Rug", PrimitiveType.Cube, new Vector3(0, 0.01f, 0), new Vector3(3f, 0.02f, 2f), Color.gray);
        GameObject janitorCart = MakeBlockout(elLobby.transform, "Broken_Janitor_Cart", PrimitiveType.Cube, new Vector3(1.5f, 0.4f, 1f), new Vector3(0.8f, 1.2f, 0.6f), Color.white);
        janitorCart.transform.localRotation = Quaternion.Euler(0, 0, 75f); 
        MakeBlockout(elLobby.transform, "Spilled_Liquid", PrimitiveType.Cube, new Vector3(1.2f, 0.01f, 0.5f), new Vector3(2f, 0.02f, 1.5f), Color.black);
        
        GameObject carpet3F = new GameObject("Torn_Carpet");
        carpet3F.transform.SetParent(root.transform, false);
        MakeBlockout(carpet3F.transform, "Carpet_Piece_1", PrimitiveType.Cube, new Vector3(5f, 0.01f, 2f), new Vector3(4f, 0.02f, 1.8f), new Color(0.3f, 0.1f, 0.1f));
        GameObject piece2 = MakeBlockout(carpet3F.transform, "Carpet_Piece_2", PrimitiveType.Cube, new Vector3(10f, 0.01f, 2.1f), new Vector3(3.5f, 0.02f, 1.5f), new Color(0.2f, 0.05f, 0.05f));
        piece2.transform.localRotation = Quaternion.Euler(0, 5f, 0);
        MakeBlockout(carpet3F.transform, "Carpet_Piece_3", PrimitiveType.Cube, new Vector3(15f, 0.01f, 1.9f), new Vector3(4.5f, 0.02f, 1.7f), new Color(0.25f, 0.1f, 0.1f));

        Color light3F = new Color(0.6f, 0.7f, 0.7f);
        AddWallSconce(root.transform, "Sconce_Room1", new Vector3(42.1f, 1.8f, 3.75f), 0f, light3F);
        AddWallSconce(root.transform, "Sconce_Room2", new Vector3(31.9f, 1.8f, 3.75f), 0f, light3F);
        AddWallSconce(root.transform, "Sconce_Room3", new Vector3(11.9f, 1.8f, 3.75f), 0f, light3F);
        AddWallSconce(root.transform, "Sconce_Room4", new Vector3(36.3f, 1.8f, 0.25f), 180f, light3F);
        AddWallSconce(root.transform, "Sconce_Room5", new Vector3(6.1f, 1.8f, 0.25f), 180f, light3F);

        // 💡 3층 스태프룸 입구 벽부등 추가
        AddWallSconce(root.transform, "Sconce_StaffRoom", new Vector3(29.8f, 1.8f, 0.25f), 180f, light3F);

        AddWelcomeMats(root.transform, "Dirty_Mat", new Color(0.3f, 0.3f, 0.3f));
        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), light3F, 3f, 7f);

        // 💡 3층 스태프룸 통합 함수 호출
        BuildStandardStaffRoom(root.transform);

        Debug.Log("3F 복도 장식 및 조명 세팅 완료!");
    }

    [MenuItem("Tools/Hotel Blockout/Decorate Corridor - 4F (Ruins / Anomaly)")]
    public static void DecorateFloor4()
    {
        int floorNum = 4;
        float yOffset = (floorNum - 1) * StoryHeight;
        
        // 4층은 공식 객실이 없으므로 전체 복도 및 빈 공간을 아우르는 루트 생성
        GameObject root = CreateRoot("Floor4_Decorations", yOffset);

        // --- 4층 복도 및 공백 구역: 완전히 부서진 폐허 테마 ---
        GameObject ruins = new GameObject("Ruins_Decor");
        ruins.transform.SetParent(root.transform, false);

        // 1. 무너져 내린 천장 잔해 (바닥에 널브러진 콘크리트 및 벽돌 더미)
        MakeBlockout(ruins.transform, "Rubble_Pile_1", PrimitiveType.Cube, new Vector3(15f, 0.2f, 2f), new Vector3(3f, 0.4f, 2f), Color.gray);
        MakeBlockout(ruins.transform, "Rubble_Pile_2", PrimitiveType.Cube, new Vector3(28f, 0.3f, 2.5f), new Vector3(2.5f, 0.6f, 1.8f), new Color(0.25f, 0.25f, 0.25f));
        MakeBlockout(ruins.transform, "Broken_Beam", PrimitiveType.Cube, new Vector3(21f, 0.1f, 1.5f), new Vector3(4f, 0.3f, 0.8f), new Color(0.15f, 0.15f, 0.15f));

        // 2. 찢겨 나가고 흔적만 남은 낡은 카펫 조각들
        GameObject ruinedCarpet = new GameObject("Torn_Ruined_Carpet");
        ruinedCarpet.transform.SetParent(ruins.transform, false);
        MakeBlockout(ruinedCarpet.transform, "Mat_1", PrimitiveType.Cube, new Vector3(10f, 0.01f, 2f), new Vector3(3f, 0.02f, 1.5f), new Color(0.1f, 0.05f, 0.05f));
        GameObject mat2 = MakeBlockout(ruinedCarpet.transform, "Mat_2", PrimitiveType.Cube, new Vector3(25f, 0.01f, 2.2f), new Vector3(2f, 0.02f, 1.2f), new Color(0.08f, 0.04f, 0.04f));
        mat2.transform.localRotation = Quaternion.Euler(0, 25f, 0);

        // 3. 4층 메인 조명 (벽부등 없이 어둡고 스산한 청회색 빛만 은은하게 배치)
        Color light4F = new Color(0.3f, 0.35f, 0.4f);
        AddLight(root.transform, new Vector3(20f, 2.4f, 2f), light4F, 1.5f, 6f);

        // 4. 바닥에 깔리는 스산한 안개 파티클 이펙트
        AddFogEffect(root.transform);

        Debug.Log("4F 폐허 복도 장식 및 안개 파티클 세팅 완료! (벽부등 제거됨)");
    }

    [MenuItem("Tools/Hotel Blockout/Decorate Corridor - 5F (Occult)")]
    public static void DecorateFloor5()
    {
        int floorNum = 5;
        float yOffset = (floorNum - 1) * StoryHeight;
        
        GameObject root = CreateRoot("Floor5_Decorations", yOffset);

        GameObject northGap = new GameObject("North_Gap_Decor");
        northGap.transform.SetParent(root.transform, false);
        northGap.transform.localPosition = new Vector3(NorthGapCenter_X, 0f, NorthGapCenter_Z);

        MakeBlockout(northGap.transform, "Altar_Table", PrimitiveType.Cube, new Vector3(0, 0.5f, 2f), new Vector3(4f, 1f, 1.5f), Color.white);
        MakeBlockout(northGap.transform, "Phonograph_Base", PrimitiveType.Cube, new Vector3(0, 1.2f, 2f), new Vector3(0.6f, 0.4f, 0.6f), Color.white);
        GameObject horn = MakeBlockout(northGap.transform, "Phonograph_Horn", PrimitiveType.Cylinder, new Vector3(0, 1.6f, 1.8f), new Vector3(0.4f, 0.4f, 0.4f), Color.white);
        horn.transform.localRotation = Quaternion.Euler(45f, 0, 0);
        MakeBlockout(northGap.transform, "Bone_Pile", PrimitiveType.Sphere, new Vector3(2f, 0.3f, 2.5f), new Vector3(1.2f, 0.6f, 1f), Color.white);

        GameObject westVoid = new GameObject("West_Void_Decor");
        westVoid.transform.SetParent(root.transform, false);
        westVoid.transform.localPosition = new Vector3(WestVoidCenter_X, 0f, WestVoidCenter_Z);

        MakeBlockout(westVoid.transform, "Tapestry", PrimitiveType.Cube, new Vector3(0, 1.5f, -3.9f), new Vector3(5f, 2f, 0.1f), Color.white);
        
        MakeBlockout(westVoid.transform, "Brazier", PrimitiveType.Cylinder, new Vector3(0, 0.5f, -2f), new Vector3(1f, 0.5f, 1f), Color.white);
        MakeBlockout(westVoid.transform, "Candle_1", PrimitiveType.Cylinder, new Vector3(1f, 0.1f, -1.5f), new Vector3(0.1f, 0.1f, 0.1f), Color.white);
        MakeBlockout(westVoid.transform, "Candle_2", PrimitiveType.Cylinder, new Vector3(-0.8f, 0.1f, -2.2f), new Vector3(0.1f, 0.1f, 0.1f), Color.white);
        MakeBlockout(westVoid.transform, "Candle_3", PrimitiveType.Cylinder, new Vector3(0.5f, 0.05f, -2.8f), new Vector3(0.1f, 0.05f, 0.1f), Color.white);
        MakeBlockout(westVoid.transform, "Candle_4", PrimitiveType.Cylinder, new Vector3(-0.2f, 0.15f, -1.8f), new Vector3(0.1f, 0.15f, 0.1f), Color.white);

        GameObject elLobby = new GameObject("EL_Lobby_Decor");
        elLobby.transform.SetParent(root.transform, false);
        elLobby.transform.localPosition = new Vector3(ElLobbyCenter_X, 0f, ElLobbyCenter_Z);

        MakeBlockout(elLobby.transform, "Chained_Trunk_1", PrimitiveType.Cube, new Vector3(-1.5f, 0.3f, -1.5f), new Vector3(1f, 0.6f, 0.8f), Color.white);
        MakeBlockout(elLobby.transform, "Chained_Trunk_2", PrimitiveType.Cube, new Vector3(-1.4f, 0.8f, -1.6f), new Vector3(0.8f, 0.4f, 0.6f), Color.white);

        GameObject carpet5F = new GameObject("Bloody_Carpet");
        carpet5F.transform.SetParent(root.transform, false);
        MakeBlockout(carpet5F.transform, "Main_Drag_Mark", PrimitiveType.Cube, new Vector3(11f, 0.01f, 2f), new Vector3(14f, 0.02f, 1.2f), new Color(0.3f, 0f, 0f));
        MakeBlockout(carpet5F.transform, "Altar_Drag_Mark", PrimitiveType.Cube, new Vector3(18f, 0.01f, 5f), new Vector3(1.2f, 0.02f, 6f), new Color(0.3f, 0f, 0f));

        Color light5F = new Color(0.8f, 0.1f, 0.1f);
        AddWallSconce(root.transform, "Sconce_Room1", new Vector3(42.1f, 1.8f, 3.75f), 0f, light5F);
        AddWallSconce(root.transform, "Sconce_Room2", new Vector3(31.9f, 1.8f, 3.75f), 0f, light5F);
        AddWallSconce(root.transform, "Sconce_Room3", new Vector3(11.9f, 1.8f, 3.75f), 0f, light5F);
        AddWallSconce(root.transform, "Sconce_Room4", new Vector3(36.3f, 1.8f, 0.25f), 180f, light5F);
        AddWallSconce(root.transform, "Sconce_Room5", new Vector3(6.1f, 1.8f, 0.25f), 180f, light5F);

        // 💡 5층 스태프룸 입구 벽부등 추가
        AddWallSconce(root.transform, "Sconce_StaffRoom", new Vector3(29.8f, 1.8f, 0.25f), 180f, light5F);

        AddWelcomeMats(root.transform, "Bloody_Mat", new Color(0.4f, 0f, 0f));
        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), light5F, 5f, 8f);

        // 💡 5층 스태프룸 통합 함수 호출
        BuildStandardStaffRoom(root.transform);

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

    // 파일 상단 변수 선언부에 에셋 경로 설정 (실제 에셋 위치에 맞춰 변경하세요)
    // 1단계에서 완성한 프리팹 경로 지정
    private const string SconcePrefabPath = "Assets/3rdParty/Sconce/Prefabs/PF_BrassWallLamp.prefab";
    static void AddWallSconce(Transform parent, string name, Vector3 pos, float yRotation, Color lightColor)
    {
        GameObject sconceObj = null;
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SconcePrefabPath);

        if (prefab != null)
        {
            // 1. 프리팹 생성 및 위치/회전 적용 (3D 모델 원본 재질 유지)
            sconceObj = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            sconceObj.name = name;
            sconceObj.transform.localPosition = pos;
            sconceObj.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        else
        {
            // Fallback (3D 에셋 없을 경우 기본 도형 생성)
            sconceObj = new GameObject(name);
            sconceObj.transform.SetParent(parent, false);
            sconceObj.transform.localPosition = pos;
            sconceObj.transform.localRotation = Quaternion.Euler(0, yRotation, 0);

            MakeBlockout(sconceObj.transform, "Base", PrimitiveType.Cube, Vector3.zero, new Vector3(0.2f, 0.4f, 0.1f), Color.black);
            MakeBlockout(sconceObj.transform, "Bulb", PrimitiveType.Sphere, new Vector3(0, 0.1f, -0.1f), new Vector3(0.2f, 0.2f, 0.2f), lightColor);
        }

        // 2. 층별 조명 빛 색상(Point Light)만 변경
        Light l = sconceObj.GetComponentInChildren<Light>();
        if (l == null)
        {
            GameObject lightObj = new GameObject("Sconce_Light_Source");
            lightObj.transform.SetParent(sconceObj.transform, false);
            
            // 전구 높이에 맞는 광원 위치 지정
            lightObj.transform.localPosition = new Vector3(0f, 0.25f, -0.4f);
            l = lightObj.AddComponent<Light>();
        }

        l.type = LightType.Point;
        l.color = lightColor; // 1F/2F/3F/5F 각각의 테마 색상 적용
        l.intensity = 2.5f;
        l.range = 5f;
        l.shadows = LightShadows.Soft;
    }

    static void AddWelcomeMats(Transform parent, string matName, Color matColor)
    {
        GameObject matGroup = new GameObject(matName + "_Group");
        matGroup.transform.SetParent(parent, false);

        MakeBlockout(matGroup.transform, $"{matName}_101", PrimitiveType.Cube, new Vector3(43.1f, 0.015f, 3.4f), new Vector3(1.2f, 0.02f, 0.8f), matColor);
        MakeBlockout(matGroup.transform, $"{matName}_102", PrimitiveType.Cube, new Vector3(32.9f, 0.015f, 3.4f), new Vector3(1.2f, 0.02f, 0.8f), matColor);
        MakeBlockout(matGroup.transform, $"{matName}_103", PrimitiveType.Cube, new Vector3(12.9f, 0.015f, 3.4f), new Vector3(1.2f, 0.02f, 0.8f), matColor);

        MakeBlockout(matGroup.transform, $"{matName}_104", PrimitiveType.Cube, new Vector3(35.3f, 0.015f, 0.6f), new Vector3(1.2f, 0.02f, 0.8f), matColor);
        MakeBlockout(matGroup.transform, $"{matName}_105", PrimitiveType.Cube, new Vector3(5.1f, 0.015f, 0.6f), new Vector3(1.2f, 0.02f, 0.8f), matColor);
    }

    // 상단 변수 선언부에 에셋 경로 지정
    private const string LockerPrefabPath = "Assets/3rdParty/metal-cabinet/source/locker.fbx";
    private const string BreakTablePrefabPath = "Assets/3rdParty/Break_table/source/Carver_Coffee_Table.fbx";
    private const string ChairPrefabPath = "Assets/3rdParty/vintage-wooden-chair/source/Chair.obj";
    private const string CotBedPrefabPath = "Assets/3rdParty/fbx/ASSET.fbx";
    private const string BoardPrefabPath = "Assets/3rdParty/Board/source/CorkBulletin.fbx";

    static void BuildStandardStaffRoom(Transform parent)
    {
        GameObject staffRoom = new GameObject("Staff_Room_Decor");
        staffRoom.transform.SetParent(parent, false); 
        
        // 1. 락커 (3D Model)
        GameObject lockerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LockerPrefabPath);
        for (int i = 0; i < 4; i++)
        {
            Vector3 lockerPos = new Vector3(25f + i * 0.8f, 0f, -7.5f); 
            if (lockerPrefab != null)
            {
                GameObject locker = PrefabUtility.InstantiatePrefab(lockerPrefab, staffRoom.transform) as GameObject;
                locker.name = $"Locker_{i}";
                locker.transform.localPosition = lockerPos;
                locker.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); 
                locker.transform.localScale = Vector3.one; 
            }
            else
            {
                MakeBlockout(staffRoom.transform, $"Locker_{i}", PrimitiveType.Cube, new Vector3(25f + i * 0.8f, 1f, -7.5f), new Vector3(0.7f, 2f, 0.6f), new Color(0.25f, 0.25f, 0.25f));
            }
        }

        // 2. 휴게실 테이블 (3D Model)
        GameObject tablePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BreakTablePrefabPath);
        if (tablePrefab != null)
        {
            GameObject table = PrefabUtility.InstantiatePrefab(tablePrefab, staffRoom.transform) as GameObject;
            table.name = "Break_Table";
            table.transform.localPosition = new Vector3(29f, 0.65f, -4f);
            table.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            table.transform.localScale = new Vector3(30f, 15f, 15f);
        }
        else
        {
            MakeBlockout(staffRoom.transform, "Break_Table", PrimitiveType.Cube, new Vector3(29f, 0.4f, -4f), new Vector3(2.5f, 0.8f, 1.5f), new Color(0.3f, 0.2f, 0.15f));
        }

        // 3. 의자 4개 (3D Model)
        GameObject chairPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ChairPrefabPath);
        
        Vector3[] chairPositions = new Vector3[]
        {
            new Vector3(29f, 0f, -2.9f),
            new Vector3(29f, 0f, -5f),
            new Vector3(27.2f, 0f, -4f),
            new Vector3(30.8f, 0f, -4f)
        };

        float[] chairYRotations = new float[] { 180f, 0f, 90f, -90f };

        for (int i = 0; i < chairPositions.Length; i++)
        {
            string chairName = $"Chair_{i + 1}";

            if (chairPrefab != null)
            {
                GameObject chair = PrefabUtility.InstantiatePrefab(chairPrefab, staffRoom.transform) as GameObject;
                chair.name = chairName;
                chair.transform.localPosition = chairPositions[i];
                chair.transform.localRotation = Quaternion.Euler(0f, chairYRotations[i], 0f); 
                chair.transform.localScale = Vector3.one; 
            }
            else
            {
                MakeBlockout(staffRoom.transform, chairName, PrimitiveType.Cylinder, chairPositions[i] + Vector3.up * 0.25f, new Vector3(0.5f, 0.25f, 0.5f), Color.white);
            }
        }

        // 4. 간이 침대 (3D Model)
        GameObject bedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CotBedPrefabPath);
        if (bedPrefab != null)
        {
            GameObject bed = PrefabUtility.InstantiatePrefab(bedPrefab, staffRoom.transform) as GameObject;
            bed.name = "Cot_Bed";
            bed.transform.localPosition = new Vector3(32.5f, 0f, -2f); 
            bed.transform.localRotation = Quaternion.Euler(0f, 180f, 0f); 
            bed.transform.localScale = new Vector3(1f, 1.2f, 1.5f);
        }
        else
        {
            MakeBlockout(staffRoom.transform, "Cot_Bed", PrimitiveType.Cube, new Vector3(32.5f, 0.3f, -2f), new Vector3(1f, 0.4f, 2.5f), new Color(0.2f, 0.3f, 0.2f));
        }

        // 5. 통합 게시판 (3D Model)
        GameObject boardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BoardPrefabPath);
        if (boardPrefab != null)
        {
            GameObject board = PrefabUtility.InstantiatePrefab(boardPrefab, staffRoom.transform) as GameObject;
            board.name = "Notice_Board";
            
            // 벽면 중앙 높이 위치 (필요시 X/Y 오프셋 조정)
            board.transform.localPosition = new Vector3(26.25f, 1.25f, -0.05f); 
            board.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); 
            board.transform.localScale = new Vector3(12f, 10f, 12f);
        }
        else
        {
            // Fallback: 기존 2개 게시판 블록아웃
            MakeBlockout(staffRoom.transform, "Rulebook_Board", PrimitiveType.Cube, new Vector3(25f, 1.5f, -0.05f), new Vector3(2f, 1.2f, 0.1f), Color.white); 
            MakeBlockout(staffRoom.transform, "Status_Board", PrimitiveType.Cube, new Vector3(27.5f, 1.5f, -0.05f), new Vector3(1.5f, 1f, 0.1f), Color.gray); 
        }

        // 조명
        AddLight(staffRoom.transform, new Vector3(29f, 2.4f, -4f), new Color(0.85f, 0.9f, 1f), 3.5f, 7f);
    }

    // 안개 파티클 시스템 생성 함수
    static void AddFogEffect(Transform parent)
    {
        GameObject fog = new GameObject("Creepy_Fog_Particles");
        fog.transform.SetParent(parent, false);
        // 복도 중앙, 바닥에 가깝게 배치
        fog.transform.localPosition = new Vector3(20f, 0.5f, 2f);

        ParticleSystem ps = fog.AddComponent<ParticleSystem>();
        
        // 파티클 메인 모듈 설정
        var main = ps.main;
        main.startLifetime = 12f;                  // 안개가 오래 머물도록
        main.startSpeed = 0.2f;                    // 천천히 흘러가게
        main.startSize = 8f;                       // 입자 크기 큼직하게 설정
        main.startColor = new Color(0.7f, 0.75f, 0.8f, 0.05f); // 매우 옅은 반투명 청회색
        main.maxParticles = 400;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        // 파티클 방출량 설정
        var emission = ps.emission;
        emission.rateOverTime = 20f;

        // 파티클 형태 설정 (4층 복도 전체를 덮을 수 있는 넓은 Box 형태)
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(45f, 2f, 8f);

        // 안개가 부드럽게 렌더링되도록 기본 머티리얼 적용
        var renderer = fog.GetComponent<ParticleSystemRenderer>();
        
        // [수정된 부분] 유니티 내장 기본 파티클 머티리얼을 불러와서 할당 (분홍색 에러 방지)
        Material defaultParticleMat = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Default-ParticleSystem.mat");
        
        if (defaultParticleMat != null)
        {
            renderer.sharedMaterial = defaultParticleMat;
        }
        else
        {
            // 혹시라도 기본 머티리얼을 못 찾을 경우 임시 쉐이더 생성
            Shader unlitShader = Shader.Find("Particles/Standard Unlit");
            if (unlitShader == null) unlitShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            
            if (unlitShader != null)
            {
                renderer.sharedMaterial = new Material(unlitShader);
            }
        }
    }
}