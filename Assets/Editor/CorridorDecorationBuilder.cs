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

        // 괘종시계 (Grandfather Clock)
        MakeBlockout(northGap.transform, "Grandfather_Clock", PrimitiveType.Cube, new Vector3(0, 1.2f, 3.8f), new Vector3(0.8f, 2.4f, 0.4f), Color.white);
        // 안락의자 2개 (Armchairs)
        MakeBlockout(northGap.transform, "Armchair_Left", PrimitiveType.Cube, new Vector3(-1.5f, 0.4f, 2f), new Vector3(1f, 0.8f, 1f), Color.white);
        MakeBlockout(northGap.transform, "Armchair_Right", PrimitiveType.Cube, new Vector3(1.5f, 0.4f, 2f), new Vector3(1f, 0.8f, 1f), Color.white);
        // 티 테이블
        MakeBlockout(northGap.transform, "Tea_Table", PrimitiveType.Cylinder, new Vector3(0, 0.3f, 2f), new Vector3(1.2f, 0.3f, 1.2f), Color.white);

        // 2. West Void (미니 갤러리)
        GameObject westVoid = new GameObject("West_Void_Decor");
        westVoid.transform.SetParent(root.transform);
        westVoid.transform.localPosition = new Vector3(WestVoidCenter_X, 0f, WestVoidCenter_Z);

        // 대리석 조각상 세트
        MakeBlockout(westVoid.transform, "Statue_Base", PrimitiveType.Cube, new Vector3(0, 0.4f, -2f), new Vector3(1f, 0.8f, 1f), Color.white);
        MakeBlockout(westVoid.transform, "Marble_Statue", PrimitiveType.Cylinder, new Vector3(0, 1.6f, -2f), new Vector3(0.6f, 0.8f, 0.6f), Color.white);
        // 대형 화분
        MakeBlockout(westVoid.transform, "Brass_Pot", PrimitiveType.Cylinder, new Vector3(3f, 0.4f, -3f), new Vector3(0.8f, 0.4f, 0.8f), Color.white);

        // 3. EL 로비 (수하물 대기열)
        GameObject elLobby = new GameObject("EL_Lobby_Decor");
        elLobby.transform.SetParent(root.transform);
        elLobby.transform.localPosition = new Vector3(ElLobbyCenter_X, 0f, ElLobbyCenter_Z);

        // 수하물 카트
        MakeBlockout(elLobby.transform, "Luggage_Cart", PrimitiveType.Cube, new Vector3(-1.5f, 0.8f, -1.5f), new Vector3(1.5f, 1.6f, 0.8f), Color.white);
        
        // [2F 추가] 복도 벽면 콘솔 테이블과 고급 도자기 화병 (North Gap 근처)
        MakeBlockout(northGap.transform, "Console_Table", PrimitiveType.Cube, new Vector3(2.5f, 0.4f, 0f), new Vector3(1.5f, 0.8f, 0.4f), Color.white);
        MakeBlockout(northGap.transform, "Porcelain_Vase", PrimitiveType.Cylinder, new Vector3(2.5f, 0.9f, 0f), new Vector3(0.3f, 0.4f, 0.3f), Color.white);
        
        // [2F 추가] 황동으로 된 클래식 휴지통 (EL 로비 근처)
        MakeBlockout(elLobby.transform, "Brass_TrashBin", PrimitiveType.Cylinder, new Vector3(1.5f, 0.6f, -1f), new Vector3(0.5f, 0.6f, 0.5f), Color.white);

        // 2층 복도 메인 조명 (오렌지빛 전구색)
        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), new Color(1f, 0.8f, 0.5f), 2f);

        Debug.Log("2F 복도 장식(기만적인 우아함) 생성 완료!");
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

        // 삐딱한 괘종시계
        GameObject clock = MakeBlockout(northGap.transform, "Broken_Clock", PrimitiveType.Cube, new Vector3(0, 1.2f, 3.8f), new Vector3(0.8f, 2.4f, 0.4f), Color.white);
        clock.transform.localRotation = Quaternion.Euler(0, 0, 15f); // 쓰러질 듯한 각도
        // 뒤집힌 의자
        GameObject chair = MakeBlockout(northGap.transform, "Overturned_Chair", PrimitiveType.Cube, new Vector3(-1.5f, 0.5f, 2f), new Vector3(1f, 1f, 0.8f), Color.white);
        chair.transform.localRotation = Quaternion.Euler(90f, 30f, 0); 
        // 박살난 테이블 (찌그러진 스케일)
        MakeBlockout(northGap.transform, "Broken_Table", PrimitiveType.Cylinder, new Vector3(0.5f, 0.1f, 2.5f), new Vector3(1.2f, 0.1f, 0.8f), Color.white);

        // 2. West Void (불길한 린넨 카트)
        GameObject westVoid = new GameObject("West_Void_Decor");
        westVoid.transform.SetParent(root.transform);
        westVoid.transform.localPosition = new Vector3(WestVoidCenter_X, 0f, WestVoidCenter_Z);

        // 사람 실루엣이 덮인 카트
        MakeBlockout(westVoid.transform, "Creepy_Linen_Cart", PrimitiveType.Cube, new Vector3(2f, 0.6f, -1f), new Vector3(1.2f, 1.2f, 1.8f), Color.white);
        MakeBlockout(westVoid.transform, "Covered_Figure", PrimitiveType.Sphere, new Vector3(2f, 1.4f, -0.5f), new Vector3(0.8f, 0.8f, 0.8f), Color.white);

        // 3. EL 로비 (오염)
        GameObject elLobby = new GameObject("EL_Lobby_Decor");
        elLobby.transform.SetParent(root.transform);
        elLobby.transform.localPosition = new Vector3(ElLobbyCenter_X, 0f, ElLobbyCenter_Z);

        // 더러운 웰컴 매트 (바닥에 납작하게)
        MakeBlockout(elLobby.transform, "Dirty_Rug", PrimitiveType.Cube, new Vector3(0, 0.01f, 0), new Vector3(3f, 0.02f, 2f), Color.gray);

        // [3F 추가] 엎어진 청소용품 카트와 바닥에 고인 오물 (EL 로비 근처)
        GameObject janitorCart = MakeBlockout(elLobby.transform, "Broken_Janitor_Cart", PrimitiveType.Cube, new Vector3(1.5f, 0.4f, 1f), new Vector3(0.8f, 1.2f, 0.6f), Color.white);
        janitorCart.transform.localRotation = Quaternion.Euler(0, 0, 75f); // 쓰러진 형태
        MakeBlockout(elLobby.transform, "Spilled_Liquid", PrimitiveType.Cube, new Vector3(1.2f, 0.01f, 0.5f), new Vector3(2f, 0.02f, 1.5f), Color.black);
        
        // [3F 추가] 삐딱하게 걸려있는 벽면 액자
        GameObject creepyPainting = MakeBlockout(westVoid.transform, "Crooked_Painting", PrimitiveType.Cube, new Vector3(-2f, 1.5f, -3.9f), new Vector3(1f, 1.2f, 0.05f), Color.white);
        creepyPainting.transform.localRotation = Quaternion.Euler(0, 0, -25f);

        // 3층 복도 깜빡이는 고장난 조명 (차갑고 어두운 빛)
        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), new Color(0.6f, 0.7f, 0.7f), 0.8f);

        Debug.Log("3F 복도 장식(부패하는 화려함) 생성 완료!");
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

        // 제단 탁자
        MakeBlockout(northGap.transform, "Altar_Table", PrimitiveType.Cube, new Vector3(0, 0.5f, 2f), new Vector3(4f, 1f, 1.5f), Color.white);
        // 축음기
        MakeBlockout(northGap.transform, "Phonograph_Base", PrimitiveType.Cube, new Vector3(0, 1.2f, 2f), new Vector3(0.6f, 0.4f, 0.6f), Color.white);
        GameObject horn = MakeBlockout(northGap.transform, "Phonograph_Horn", PrimitiveType.Cylinder, new Vector3(0, 1.6f, 1.8f), new Vector3(0.4f, 0.4f, 0.4f), Color.white);
        horn.transform.localRotation = Quaternion.Euler(45f, 0, 0);

        // 2. West Void (기괴한 상징물)
        GameObject westVoid = new GameObject("West_Void_Decor");
        westVoid.transform.SetParent(root.transform);
        westVoid.transform.localPosition = new Vector3(WestVoidCenter_X, 0f, WestVoidCenter_Z);

        // 기괴한 태피스트리 (벽면에 납작하게 붙임)
        MakeBlockout(westVoid.transform, "Tapestry", PrimitiveType.Cube, new Vector3(0, 1.5f, -3.9f), new Vector3(5f, 2f, 0.1f), Color.white);
        // 동물 뼈 화로
        MakeBlockout(westVoid.transform, "Brazier", PrimitiveType.Cylinder, new Vector3(0, 0.5f, -2f), new Vector3(1f, 0.6f, 1f), Color.white);

        // 3. EL 로비 (봉인된 수하물)
        GameObject elLobby = new GameObject("EL_Lobby_Decor");
        elLobby.transform.SetParent(root.transform);
        elLobby.transform.localPosition = new Vector3(ElLobbyCenter_X, 0f, ElLobbyCenter_Z);

        // 사슬로 감긴 트렁크 무더기
        MakeBlockout(elLobby.transform, "Chained_Trunk_1", PrimitiveType.Cube, new Vector3(-1.5f, 0.3f, -1.5f), new Vector3(1f, 0.6f, 0.8f), Color.white);
        MakeBlockout(elLobby.transform, "Chained_Trunk_2", PrimitiveType.Cube, new Vector3(-1.4f, 0.8f, -1.6f), new Vector3(0.8f, 0.4f, 0.6f), Color.white);

        // [5F 추가] 바닥에 불규칙하게 흩어진 의식용 양초들 (West Void 제단 근처)
        MakeBlockout(westVoid.transform, "Candle_1", PrimitiveType.Cylinder, new Vector3(1f, 0.1f, -1.5f), new Vector3(0.1f, 0.2f, 0.1f), Color.white);
        MakeBlockout(westVoid.transform, "Candle_2", PrimitiveType.Cylinder, new Vector3(-0.8f, 0.1f, -2.2f), new Vector3(0.1f, 0.2f, 0.1f), Color.white);
        MakeBlockout(westVoid.transform, "Candle_3", PrimitiveType.Cylinder, new Vector3(0.5f, 0.05f, -2.8f), new Vector3(0.1f, 0.1f, 0.1f), Color.white);
        MakeBlockout(westVoid.transform, "Candle_4", PrimitiveType.Cylinder, new Vector3(-0.2f, 0.15f, -1.8f), new Vector3(0.1f, 0.3f, 0.1f), Color.white);

        // [5F 추가] 구석에 쌓인 동물 뼈 무더기
        MakeBlockout(northGap.transform, "Bone_Pile", PrimitiveType.Sphere, new Vector3(2f, 0.3f, 2.5f), new Vector3(1.2f, 0.6f, 1f), Color.white);

        // 5층 복도 핏빛 조명
        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), new Color(0.8f, 0.1f, 0.1f), 2.5f);

        Debug.Log("5F 복도 장식(핏빛 펜트하우스) 생성 완료!");
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

        // 추후 진짜 프리팹으로 교체하기 쉽도록 BoxCollider 제거 (선택적)
        Collider col = go.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);

        return go;
    }

    static void AddLight(Transform parent, Vector3 localPos, Color color, float intensity)
    {
        GameObject lightObj = new GameObject("Corridor_MoodLight");
        lightObj.transform.SetParent(parent, false);
        lightObj.transform.localPosition = localPos;

        Light l = lightObj.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = color;
        l.intensity = intensity;
        // 조명 사거리를 8f에서 3.5f 정도로 줄여서 위아래 층 침범 최소화
        l.range = 3.5f; 
        
        // ⭐️ 핵심: 바닥과 천장 메쉬가 빛을 막아주도록 그림자 옵션 활성화
        l.shadows = LightShadows.Soft;
    }
}