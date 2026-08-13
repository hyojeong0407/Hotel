using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class HotelBlockoutBuilder
{
    const float WallThickness = 0.2f;
    const float CeilingHeight = 2.5f;

    const float RoomWidth = 10f;    // X, per-bay width (101/102/103/104/105 all share this)
    const float RoomLength = 8f;    // Z, guest room depth
    const float CorridorDepth = 4f; // Z, hallway between the north and south room rows
    const float ELWidth = 4f;       // X, elevator lobby strip on the west end (square: ELWidth == CorridorDepth)

    const float BathWidth = 3f;
    const float BathLength = 3f;
    const float BathDoorWidth = 0.8f;

    const float BedWidth = 4f;
    const float BedDepth = 3f;
    const float BedHeight = 0.5f;
    const float BedClearance = 1f; // open space kept around the bed on every side
    const float BedLegSize = 0.1f;
    const float BedLegHeight = 0.2f;

    const float DoorWidth = 1.3f;
    const float PassageWidth = 7f; // open walkway between Front_Desk and Staff_Room

    // Window in the guest room's back wall (opposite the door)
    const float WindowCenterX = 5f;
    const float WindowWidth = 5f;
    const float WindowSillHeight = 1f;
    const float WindowHeight = 1.2f;

    // Wardrobe — tucked into the corner next to the bathroom
    const float WardrobeCenterX = 3.65f;
    const float WardrobeCenterZ = 0.65f;
    const float WardrobeSize = 1.3f;
    const float WardrobeHeight = 2f;

    // Nightstand + phone — foot of the bed, by the window wall
    const float NightstandCenterX = 0.5f;
    const float NightstandCenterZ = 7.5f;
    const float NightstandSize = 1f;
    const float NightstandHeight = 0.5f;

    // Seating: two armchairs with a side table between them, facing the TV
    const float SofaSize = 1.1f;
    const float SofaHeight = 0.8f;
    const float Sofa1CenterZ = 3.55f;
    const float Sofa2CenterZ = 6.45f;
    const float SeatingCenterX = 6.95f;
    const float TableWidth = 0.7f;
    const float TableDepth = 0.6f;
    const float TableHeight = 0.4f;
    const float TableCenterZ = 5f;

    // TV shelf, mounted on the east wall (벽 X:10 에 완벽하게 밀착되도록 9.825f 로 수정됨)
    const float TvCenterX = 9.825f;
    const float TvCenterZ = 5f;
    const float TvThickness = 0.35f;
    const float TvSpan = 2f;
    const float TvShelfHeight = 0.5f;
    const float TvShelfElevation = 1f;

    // Bathroom fixtures
    const float TubCenterX = 1.5f;
    const float TubCenterZ = 0.65f;
    const float TubWidth = 2.6f;
    const float TubDepth = 0.9f;
    const float TubHeight = 0.5f;
    const float CurtainHeight = 2f;
    const float SinkCenterX = 0.65f;
    const float SinkCenterZ = 2.1f;
    const float SinkWidth = 0.9f;
    const float SinkDepth = 0.6f;
    const float SinkHeight = 0.85f;
    const float MirrorHeight = 0.8f;
    const float MirrorElevation = 1f;
    const float ToiletCenterX = 2.65f;
    const float ToiletCenterZ = 1.7f;
    const float ToiletWidth = 0.7f;
    const float ToiletDepth = 0.8f;
    const float ToiletHeight = 0.4f;

    [MenuItem("Tools/Hotel Blockout/Build Room 101")]
    public static void BuildRoom101()
    {
        ClearExisting("Room_101");
        var root = new GameObject("Room_101");
        Undo.RegisterCreatedObjectUndo(root, "Build Room 101");

        BuildRoomShell(root.transform, RoomWidth, RoomLength, includeBathroom: true, doorCenterX: RoomWidth - 1.1f, roomLabel: "101");

        FocusOn(root);
        Debug.Log("Room_101 블록아웃 생성 완료: 4.5m x 6m, 천장 2.7m, 화장실 2.2m x 2.6m");
    }

    const float StoryHeight = CeilingHeight + WallThickness; // floor-to-floor vertical offset between stories

    [MenuItem("Tools/Hotel Blockout/Build Floor 1 (Full Layout)")]
    public static void BuildFloor1Menu() => BuildFloor(1);

    [MenuItem("Tools/Hotel Blockout/Build Floor 2")]
    public static void BuildFloor2Menu() => BuildFloor(2);

    [MenuItem("Tools/Hotel Blockout/Build Floor 3")]
    public static void BuildFloor3Menu() => BuildFloor(3);

    [MenuItem("Tools/Hotel Blockout/Build Floor 4")]
    public static void BuildFloor4Menu() => BuildFloor(4);

    [MenuItem("Tools/Hotel Blockout/Build Floor 5")]
    public static void BuildFloor5Menu() => BuildFloor(5);

    [MenuItem("Tools/Hotel Blockout/Build All Floors (1-5)")]
    public static void BuildAllFloorsMenu()
    {
        for (int floorNumber = 1; floorNumber <= 5; floorNumber++)
            BuildFloor(floorNumber);
    }

    public static void BuildAllFloorsAndSaveBatch()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
        BuildAllFloorsMenu();
        var floors = FindAllFloorRoots();
        Selection.objects = floors;
        
        EditorApplication.ExecuteMenuItem("Tools/Hotel Blockout/Apply Dark Materials (Eye Comfort)");
        
        ApplyLuxuryFurnitureColors(floors);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("BuildAllFloorsAndSaveBatch 완료: SampleScene에 저장됨.");
    }

    static GameObject[] FindAllFloorRoots()
    {
        var targets = new System.Collections.Generic.List<GameObject>();
        for (int floorNumber = 1; floorNumber <= 5; floorNumber++)
        {
            var go = GameObject.Find($"Floor{floorNumber}_Hotel");
            if (go != null)
                targets.Add(go);
        }
        return targets.ToArray();
    }

    [MenuItem("Tools/Hotel Blockout/Apply Luxury Furniture Colors")]
    public static void ApplyLuxuryFurnitureColorsMenu()
    {
        var targets = Selection.gameObjects;
        if (targets.Length == 0)
        {
            Debug.LogWarning("먼저 색을 칠할 오브젝트들을 하이어라키에서 선택해 주세요!");
            return;
        }
        ApplyLuxuryFurnitureColors(targets);
    }

    static void ApplyLuxuryFurnitureColors(GameObject[] targets)
    {
        Texture2D woodTex = GenerateWoodTexture(new Color(0.22f, 0.11f, 0.06f));
        Texture2D doorWoodTex = GenerateWoodTexture(new Color(0.25f, 0.14f, 0.07f));
        Texture2D velvetTex = GenerateFabricTexture(new Color(0.42f, 0.06f, 0.09f));
        Texture2D goldFabricTex = GenerateFabricTexture(new Color(0.5f, 0.42f, 0.28f));
        Texture2D ceramicTex = GenerateCeramicTexture(new Color(0.16f, 0.16f, 0.17f));
        Texture2D brassTex = GenerateMetalTexture(new Color(0.55f, 0.42f, 0.15f));
        Texture2D vinylTex = GenerateFabricTexture(new Color(0.1f, 0.11f, 0.13f));
        Texture2D marbleTex = GenerateMarbleTexture(new Color(0.08f, 0.08f, 0.09f)); // ★ 바닥용 고급 어두운 대리석 텍스처 생성

        Material velvetRed = NewStandardMaterial(new Color(0.42f, 0.06f, 0.09f), 0.15f, 0f, velvetTex, new Vector2(4f, 4f));
        Material mahogany = NewStandardMaterial(new Color(0.22f, 0.11f, 0.06f), 0.3f, 0f, woodTex, new Vector2(2f, 3f));
        Material doorWood = NewStandardMaterial(new Color(0.25f, 0.14f, 0.07f), 0.32f, 0f, doorWoodTex, new Vector2(2f, 3f));
        Material brassMetal = NewStandardMaterial(new Color(0.55f, 0.42f, 0.15f), 0.6f, 0.75f, brassTex, new Vector2(3f, 1.5f));
        Material mutedGold = NewStandardMaterial(new Color(0.5f, 0.42f, 0.28f), 0.1f, 0f, goldFabricTex, new Vector2(4f, 4f));
        Material blackPlastic = NewStandardMaterial(new Color(0.03f, 0.03f, 0.03f), 0.6f, 0f);      
        Material mirrorGlass = NewStandardMaterial(new Color(0.6f, 0.6f, 0.62f), 0.92f, 0.85f);     
        Material ceramic = NewStandardMaterial(new Color(0.16f, 0.16f, 0.17f), 0.45f, 0f, ceramicTex, new Vector2(3f, 3f));
        Material windowGlass = NewStandardMaterial(new Color(0.5f, 0.62f, 0.58f), 0.85f, 0.1f);     
        Material vinylCurtain = NewStandardMaterial(new Color(0.1f, 0.11f, 0.13f), 0.35f, 0f, vinylTex, new Vector2(2f, 4f));
        Material darkMarble = NewStandardMaterial(new Color(0.08f, 0.08f, 0.09f), 0.7f, 0.1f, marbleTex, new Vector2(8f, 8f)); // ★ 매끄러운 럭셔리 대리석 머티리얼
        Material fontMaterial = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf").material; 

        var redVelvet = new System.Collections.Generic.HashSet<string> { "seat", "backrest", "armrest_near", "armrest_far", "mattress", "blanket_fold" };
        var gold = new System.Collections.Generic.HashSet<string> { "pillow_1", "pillow_2", "shade" };
        var wood = new System.Collections.Generic.HashSet<string> { "cabinet_body", "door_left", "door_right", "cornice", "plinth", "body", "drawer_face", "side_table_top", "headboard", "tv_shelf" };
        var brass = new System.Collections.Generic.HashSet<string> { "handle_left", "handle_right", "drawer_knob", "tub_faucet", "sink_faucet", "mirror_frame_top", "mirror_frame_bottom", "base", "pole", "ceiling_fixture", "plaque_plate" };
        var plastic = new System.Collections.Generic.HashSet<string> { "tv_screen", "phone_base", "phone_handset" };
        var ceramicNames = new System.Collections.Generic.HashSet<string> { "tub", "sink_basin", "sink_pedestal", "sink_backsplash", "toilet_bowl", "toilet_tank", "toilet_seat" };

        int count = 0;
        foreach (var target in targets)
        {
            Undo.RegisterFullObjectHierarchyUndo(target, "Apply Luxury Furniture Colors");
            foreach (var r in target.GetComponentsInChildren<MeshRenderer>())
            {
                string n = r.gameObject.name.ToLower();
                Material chosen =
                    redVelvet.Contains(n) ? velvetRed :
                    gold.Contains(n) ? mutedGold :
                    wood.Contains(n) ? mahogany :
                    brass.Contains(n) ? brassMetal :
                    plastic.Contains(n) ? blackPlastic :
                    ceramicNames.Contains(n) ? ceramic :
                    n == "floor" ? darkMarble : // ★ 바닥(Floor) 큐브에 어두운 럭셔리 대리석 자동 적용
                    n == "mirror" ? mirrorGlass :
                    n == "glass" ? windowGlass :
                    n.StartsWith("curtain_fold") ? vinylCurtain :
                    n.StartsWith("handle_") ? brassMetal :
                    n == "plaque_number" ? fontMaterial :
                    n == "leaf" ? doorWood :
                    null;

                if (chosen == null)
                    continue;

                r.sharedMaterial = chosen;
                count++;
            }
        }

        Debug.Log($"럭셔리 가구 및 바닥 대리석 배색 완료: {count}개 블록 (레드 벨벳/마호가니/브라스/어두운 대리석/도기/유리).");
    }

    // ★ 고급 바닥용 어두운 대리석 마블 패턴 텍스처 생성기
    static Texture2D GenerateMarbleTexture(Color baseColor)
    {
        const int size = 128;
        var tex = new Texture2D(size, size, TextureFormat.RGB24, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float vein = Mathf.PerlinNoise(x * 0.05f, y * 0.05f);
                float detail = Mathf.PerlinNoise(x * 0.2f, y * 0.2f);
                float shade = 0.82f + vein * 0.3f - detail * 0.1f;
                tex.SetPixel(x, y, baseColor * shade);
            }
        }
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    static Material NewStandardMaterial(Color color, float glossiness, float metallic, Texture2D texture = null, Vector2 tiling = default)
    {
        var mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        mat.SetFloat("_Glossiness", glossiness);
        mat.SetFloat("_Metallic", metallic);
        if (texture != null)
        {
            mat.mainTexture = texture;
            mat.mainTextureScale = tiling;
        }
        return mat;
    }

    static Texture2D GenerateWoodTexture(Color baseColor)
    {
        const int size = 128;
        var tex = new Texture2D(size, size, TextureFormat.RGB24, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float grain = Mathf.PerlinNoise(x * 0.06f, y * 0.6f);
                float streak = Mathf.PerlinNoise(x * 0.4f, y * 0.02f);
                float shade = 0.78f + grain * 0.35f - streak * 0.12f;
                tex.SetPixel(x, y, baseColor * shade);
            }
        }
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    static Texture2D GenerateFabricTexture(Color baseColor)
    {
        const int size = 64;
        var tex = new Texture2D(size, size, TextureFormat.RGB24, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float weave = Mathf.PerlinNoise(x * 0.35f, y * 0.35f);
                float fine = Mathf.PerlinNoise(x * 1.6f, y * 1.6f);
                float shade = 0.88f + weave * 0.16f + fine * 0.08f;
                tex.SetPixel(x, y, baseColor * shade);
            }
        }
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    static Texture2D GenerateCeramicTexture(Color baseColor)
    {
        const int size = 64;
        var tex = new Texture2D(size, size, TextureFormat.RGB24, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float mottle = Mathf.PerlinNoise(x * 0.15f, y * 0.15f);
                float shade = 0.94f + mottle * 0.08f;
                tex.SetPixel(x, y, baseColor * shade);
            }
        }
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    static Texture2D GenerateMetalTexture(Color baseColor)
    {
        const int size = 64;
        var tex = new Texture2D(size, size, TextureFormat.RGB24, false);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float streak = Mathf.PerlinNoise(x * 2.2f, y * 0.06f);
                float shade = 0.8f + streak * 0.35f;
                tex.SetPixel(x, y, baseColor * shade);
            }
        }
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    static void BuildFloor(int floorNumber)
    {
        string floorName = $"Floor{floorNumber}_Hotel";
        ClearExisting(floorName);
        var floor = new GameObject(floorName);
        floor.transform.position = new Vector3(0f, (floorNumber - 1) * StoryHeight, 0f);
        Undo.RegisterCreatedObjectUndo(floor, "Build Floor " + floorNumber);

        var rooms = new GameObject("Rooms");
        rooms.transform.SetParent(floor.transform, false);
        var corridor = new GameObject("Corridor");
        corridor.transform.SetParent(floor.transform, false);
        var elevator = new GameObject("Elevator");
        elevator.transform.SetParent(floor.transform, false);

        const float frontDeskWidth = 3f;
        const float staffRoomWidth = 10f;

        float bay1West = ELWidth + RoomWidth;       
        float passageWest = bay1West + frontDeskWidth;
        float staffWest = passageWest + PassageWidth; 
        float bay2South = staffWest + staffRoomWidth + WallThickness;  
        float room1West = staffWest + RoomWidth + WallThickness;
         
        float totalWidth = Mathf.Max(bay2South + RoomWidth, room1West + RoomWidth);

        string RoomName(int d) => "Room_" + (floorNumber * 100 + d);

        BuildGuestRoomSlot(rooms.transform, RoomName(3), new Vector3(ELWidth, 0f, CorridorDepth), 0f);
        BuildOpenBay(rooms.transform, "North_Gap_x03_x02", bay1West, staffWest - bay1West, north: true, includeOuterWall: true);
        BuildGuestRoomSlot(rooms.transform, RoomName(2), new Vector3(staffWest, 0f, CorridorDepth), 0f);
        BuildGuestRoomSlot(rooms.transform, RoomName(1), new Vector3(room1West, 0f, CorridorDepth), 0f);

        BuildGuestRoomSlot(rooms.transform, RoomName(5), new Vector3(bay1West, 0f, 0f), 180f);
        BuildGuestRoomSlot(rooms.transform, RoomName(4), new Vector3(bay2South + RoomWidth, 0f, 0f), 180f);
        BuildUtilityRoomSlot(rooms.transform, "Staff_Room", staffWest, staffRoomWidth);

        if (floorNumber == 1)
        {
            BuildOpenBay(rooms.transform, "Front_Desk", bay1West, frontDeskWidth, north: false, includeOuterWall: false);
            BuildOpenBay(rooms.transform, "Staff_Front_Passage", passageWest, PassageWidth, north: false, includeOuterWall: false);
            BuildSouthEntranceWall(rooms.transform, bay1West, staffWest - bay1West, 4f);
        }
        else
        {
            BuildOpenBay(rooms.transform, "West_Void", bay1West, staffWest - bay1West, north: false, includeOuterWall: true);
        }

        BuildCorridor(corridor.transform, totalWidth);
        BuildElevatorLobby(elevator.transform);

        FocusOn(floor);
        Debug.Log($"{floorName} 생성 완료: EL 로비(정사각) + 복도 + {floorNumber}01~{floorNumber}05 + " +
            (floorNumber == 1 ? "프론트(공란)/통로/스태프룸." : "빈 공간/스태프룸."));
    }

    static void BuildGuestRoomSlot(Transform parent, string name, Vector3 slotPosition, float yRotation)
    {
        var slot = new GameObject(name);
        slot.transform.SetParent(parent);
        slot.transform.localPosition = slotPosition;
        slot.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        Undo.RegisterCreatedObjectUndo(slot, "Build Floor 1");

        BuildRoomShell(slot.transform, RoomWidth, RoomLength, includeBathroom: true, doorCenterX: RoomWidth - 1.1f,
            roomLabel: name.Replace("Room_", ""));
    }

    static void BuildUtilityRoomSlot(Transform parent, string name, float westEdgeX, float width)
    {
        var slot = new GameObject(name);
        slot.transform.SetParent(parent);
        slot.transform.localPosition = new Vector3(westEdgeX + width, 0f, 0f);
        slot.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        Undo.RegisterCreatedObjectUndo(slot, "Build Floor 1");

        BuildRoomShell(slot.transform, width, RoomLength, includeBathroom: false, doorCenterX: width / 2f);
    }

    static void BuildOpenBay(Transform parent, string name, float westEdgeX, float width, bool north, bool includeOuterWall)
    {
        var slot = new GameObject(name);
        slot.transform.SetParent(parent);
        float zStart = north ? CorridorDepth : -RoomLength;
        slot.transform.localPosition = new Vector3(westEdgeX, 0f, zStart);
        Undo.RegisterCreatedObjectUndo(slot, "Build Floor 1");

        MakeBox("Floor", slot.transform,
            new Vector3(width / 2f, -WallThickness / 2f, RoomLength / 2f),
            new Vector3(width, WallThickness, RoomLength));

        MakeBox("Ceiling", slot.transform,
            new Vector3(width / 2f, CeilingHeight + WallThickness / 2f, RoomLength / 2f),
            new Vector3(width, WallThickness, RoomLength));

        if (includeOuterWall)
        {
            float exteriorZ = north ? RoomLength + WallThickness / 2f : -WallThickness / 2f;
            MakeBox("Wall_Outer", slot.transform,
                new Vector3(width / 2f, CeilingHeight / 2f, exteriorZ),
                new Vector3(width + WallThickness * 2f, CeilingHeight, WallThickness));
        }
    }

    static void BuildSouthEntranceWall(Transform parent, float westEdgeX, float spanWidth, float doorWidth)
    {
        var slot = new GameObject("Front_Entrance_Wall");
        slot.transform.SetParent(parent);
        slot.transform.localPosition = new Vector3(westEdgeX, 0f, -RoomLength);
        Undo.RegisterCreatedObjectUndo(slot, "Build Floor 1");

        float doorCenter = spanWidth / 2f;
        float doorMin = doorCenter - doorWidth / 2f;
        float doorMax = doorCenter + doorWidth / 2f;

        MakeBox("Wall_Left", slot.transform,
            new Vector3(doorMin / 2f, CeilingHeight / 2f, -WallThickness / 2f),
            new Vector3(doorMin, CeilingHeight, WallThickness));

        MakeBox("Wall_Right", slot.transform,
            new Vector3((doorMax + spanWidth) / 2f, CeilingHeight / 2f, -WallThickness / 2f),
            new Vector3(spanWidth - doorMax, CeilingHeight, WallThickness));
    }

    static void BuildRoomShell(Transform parent, float width, float length, bool includeBathroom, float doorCenterX, string roomLabel = null)
    {
        MakeBox("Floor", parent,
            new Vector3(width / 2f, -WallThickness / 2f, length / 2f),
            new Vector3(width, WallThickness, length));

        MakeBox("Ceiling", parent,
            new Vector3(width / 2f, CeilingHeight + WallThickness / 2f, length / 2f),
            new Vector3(width, WallThickness, length));

        if (includeBathroom)
            BuildWallWithWindow(parent, width, length);
        else
            MakeBox("Wall_Back", parent,
                new Vector3(width / 2f, CeilingHeight / 2f, length + WallThickness / 2f),
                new Vector3(width + WallThickness * 2f, CeilingHeight, WallThickness));

        MakeBox("Wall_Left", parent,
            new Vector3(-WallThickness / 2f, CeilingHeight / 2f, length / 2f),
            new Vector3(WallThickness, CeilingHeight, length + WallThickness * 2f));

        MakeBox("Wall_Right", parent,
            new Vector3(width + WallThickness / 2f, CeilingHeight / 2f, length / 2f),
            new Vector3(WallThickness, CeilingHeight, length + WallThickness * 2f));

        float doorMinX = doorCenterX - DoorWidth / 2f;
        float doorMaxX = doorCenterX + DoorWidth / 2f;

        MakeBox("Wall_Front_Left", parent,
            new Vector3(doorMinX / 2f, CeilingHeight / 2f, -WallThickness / 2f),
            new Vector3(doorMinX, CeilingHeight, WallThickness));

        MakeBox("Wall_Front_Right", parent,
            new Vector3((doorMaxX + width) / 2f, CeilingHeight / 2f, -WallThickness / 2f),
            new Vector3(width - doorMaxX, CeilingHeight, WallThickness));

        if (!includeBathroom)
            return;

        BuildDoor(parent, "Room_Door", doorMinX, -WallThickness / 2f, DoorWidth, CeilingHeight);

        if (roomLabel != null)
            BuildRoomPlaque(parent, roomLabel, doorCenterX, -WallThickness / 2f);

        var bathroom = new GameObject("Bathroom");
        bathroom.transform.SetParent(parent, false);

        MakeBox("Bath_Wall_Side", bathroom.transform,
            new Vector3(BathWidth, CeilingHeight / 2f, BathLength / 2f),
            new Vector3(WallThickness, CeilingHeight, BathLength));

        float bathDoorMinX = BathWidth - BathDoorWidth;
        MakeBox("Bath_Wall_Front", bathroom.transform,
            new Vector3(bathDoorMinX / 2f, CeilingHeight / 2f, BathLength),
            new Vector3(bathDoorMinX, CeilingHeight, WallThickness));

        BuildDoor(bathroom.transform, "Bath_Door", bathDoorMinX, BathLength, BathDoorWidth, CeilingHeight);

        BuildBathroomFixtures(bathroom.transform);
        BuildBed(parent, width, length);
        BuildRoomFurniture(parent);
        BuildRoomLighting(parent, width, length);
    }

    static void BuildDoor(Transform parent, string name, float hingeX, float wallCenterZ, float doorWidth, float doorHeight)
    {
        var pivot = new GameObject(name);
        pivot.transform.SetParent(parent, false);
        pivot.transform.localPosition = new Vector3(hingeX, 0f, wallCenterZ);

        var leafGroup = new GameObject("Leaf_Group");
        leafGroup.transform.SetParent(pivot.transform, false);

        float leafWidth = doorWidth; 
        MakeBox("Leaf", leafGroup.transform,
            new Vector3(leafWidth / 2f, doorHeight / 2f, 0f),
            new Vector3(leafWidth, doorHeight, WallThickness));

        float handleX = leafWidth - 0.14f;
        float handleY = 1f;
        float faceOffset = WallThickness / 2f;
        BuildDoorHandle(leafGroup.transform, "Front", handleX, handleY, -faceOffset);
        BuildDoorHandle(leafGroup.transform, "Back", handleX, handleY, faceOffset);

        var trigger = new GameObject("InteractZone");
        trigger.transform.SetParent(pivot.transform, false);
        trigger.transform.localPosition = new Vector3(leafWidth / 2f, doorHeight / 2f, 0f);
        var box = trigger.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(doorWidth + 1.6f, doorHeight, 1.6f);
        Undo.RegisterCreatedObjectUndo(trigger, "Build Hotel Blockout");

        System.Type interactableType = System.Type.GetType("DoorInteractable, Assembly-CSharp") ?? System.Type.GetType("DoorInteractable");
        if (interactableType != null)
        {
            pivot.AddComponent(interactableType);
        }
        else
        {
            Debug.LogWarning("DoorInteractable 스크립트를 찾을 수 없어 문에 추가하지 않았습니다.");
        }

        Undo.RegisterCreatedObjectUndo(pivot, "Build Hotel Blockout");
    }

    static void BuildDoorHandle(Transform parent, string faceName, float x, float y, float z)
    {
        MakeBox($"Handle_Plate_{faceName}", parent, new Vector3(x, y, z), new Vector3(0.09f, 0.09f, 0.015f));
        float leverZ = z + Mathf.Sign(z) * 0.02f;
        MakeBox($"Handle_Lever_{faceName}", parent, new Vector3(x, y, leverZ), new Vector3(0.16f, 0.025f, 0.025f));
    }

    static void BuildRoomPlaque(Transform parent, string roomNumber, float centerX, float wallCenterZ)
    {
        float exteriorZ = wallCenterZ - WallThickness / 2f;

        var plaque = new GameObject("Room_Plaque");
        plaque.transform.SetParent(parent, false);
        plaque.transform.localPosition = new Vector3(centerX, 1.65f, exteriorZ);

        MakeBox("Plaque_Plate", plaque.transform, new Vector3(0f, 0f, -0.008f), new Vector3(0.55f, 0.22f, 0.015f));

        var textGO = new GameObject("Plaque_Number");
        textGO.transform.SetParent(plaque.transform, false);
        textGO.transform.localPosition = new Vector3(0f, 0f, -0.02f);

        var textMesh = textGO.AddComponent<TextMesh>();
        textMesh.text = roomNumber;
        textMesh.fontSize = 48;
        textMesh.characterSize = 0.05f; 
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = Color.black;

        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textMesh.font = font;
        textGO.GetComponent<MeshRenderer>().sharedMaterial = font.material;

        Undo.RegisterCreatedObjectUndo(plaque, "Build Hotel Blockout");
    }

    static void BuildWallWithWindow(Transform parent, float width, float length)
    {
        var wall = new GameObject("Wall_Back");
        wall.transform.SetParent(parent, false);

        float windowX0 = WindowCenterX - WindowWidth / 2f;
        float windowX1 = WindowCenterX + WindowWidth / 2f;
        float windowTop = WindowSillHeight + WindowHeight;
        float wallZ = length + WallThickness / 2f;

        MakeBox("Sill", wall.transform,
            new Vector3(width / 2f, WindowSillHeight / 2f, wallZ),
            new Vector3(width + WallThickness * 2f, WindowSillHeight, WallThickness));

        MakeBox("Lintel", wall.transform,
            new Vector3(width / 2f, (windowTop + CeilingHeight) / 2f, wallZ),
            new Vector3(width + WallThickness * 2f, CeilingHeight - windowTop, WallThickness));

        MakeBox("Wall_Left_Of_Window", wall.transform,
            new Vector3(windowX0 / 2f, (WindowSillHeight + windowTop) / 2f, wallZ),
            new Vector3(windowX0, WindowHeight, WallThickness));

        MakeBox("Wall_Right_Of_Window", wall.transform,
            new Vector3((windowX1 + width) / 2f, (WindowSillHeight + windowTop) / 2f, wallZ),
            new Vector3(width - windowX1, WindowHeight, WallThickness));

        MakeBox("Glass", wall.transform,
            new Vector3(WindowCenterX, (WindowSillHeight + windowTop) / 2f, wallZ),
            new Vector3(WindowWidth - 0.1f, WindowHeight - 0.1f, 0.03f));

        MakeBox("Sill_Ledge", wall.transform,
            new Vector3(WindowCenterX, WindowSillHeight - 0.02f, length - 0.1f),
            new Vector3(WindowWidth + 0.1f, 0.04f, 0.3f));
    }

    static void BuildBathroomFixtures(Transform bathroom)
    {
        MakeBox("Tub", bathroom,
            new Vector3(TubCenterX, TubHeight / 2f, TubCenterZ),
            new Vector3(TubWidth, TubHeight, TubDepth));

        float tubFaucetX = TubCenterX - TubWidth / 2f + 0.3f;
        MakeCylinder("Tub_Faucet", bathroom,
            new Vector3(tubFaucetX, TubHeight + 0.12f, TubCenterZ - TubDepth / 2f + 0.05f), 0.04f, 0.22f);

        float curtainZ = TubCenterZ + TubDepth / 2f; 
        const int curtainFolds = 7;
        float foldWidth = TubWidth / curtainFolds;
        for (int i = 0; i < curtainFolds; i++)
        {
            float foldX = TubCenterX - TubWidth / 2f + foldWidth * (i + 0.5f);
            float foldZ = curtainZ + (i % 2 == 0 ? 0.018f : -0.018f);
            MakeBox($"Curtain_Fold_{i:00}", bathroom,
                new Vector3(foldX, CurtainHeight / 2f, foldZ),
                new Vector3(foldWidth * 0.92f, CurtainHeight, 0.04f));
        }

        MakeCylinder("Sink_Pedestal", bathroom,
            new Vector3(SinkCenterX, 0.325f, SinkCenterZ), 0.12f, 0.65f);
        MakeCylinder("Sink_Basin", bathroom,
            new Vector3(SinkCenterX, 0.68f, SinkCenterZ), Mathf.Min(SinkWidth, SinkDepth) * 0.9f, 0.12f);
        MakeBox("Sink_Backsplash", bathroom,
            new Vector3(0.15f, 0.75f, SinkCenterZ),
            new Vector3(0.05f, 0.15f, SinkDepth));
        MakeCylinder("Sink_Faucet", bathroom,
            new Vector3(0.3f, 0.85f, SinkCenterZ), 0.03f, 0.15f);

        MakeBox("Mirror", bathroom, 
            new Vector3(0.03f, MirrorElevation + MirrorHeight / 2f, SinkCenterZ),
            new Vector3(0.06f, MirrorHeight, SinkDepth));
        MakeBox("Mirror_Frame_Top", bathroom,
            new Vector3(0.05f, MirrorElevation + MirrorHeight + 0.02f, SinkCenterZ),
            new Vector3(0.1f, 0.04f, SinkDepth + 0.06f));
        MakeBox("Mirror_Frame_Bottom", bathroom,
            new Vector3(0.05f, MirrorElevation - 0.02f, SinkCenterZ),
            new Vector3(0.1f, 0.04f, SinkDepth + 0.06f));

        float toiletBowlX = ToiletCenterX - 0.1f;
        MakeBox("Toilet_Tank", bathroom,
            new Vector3(ToiletCenterX + ToiletWidth / 2f - 0.08f, 0.55f, ToiletCenterZ),
            new Vector3(0.16f, 0.4f, ToiletDepth * 0.75f));
        MakeCylinder("Toilet_Bowl", bathroom,
            new Vector3(toiletBowlX, 0.19f, ToiletCenterZ), 0.5f, 0.38f);
        MakeBox("Toilet_Seat", bathroom,
            new Vector3(toiletBowlX, 0.4f, ToiletCenterZ),
            new Vector3(0.42f, 0.04f, ToiletDepth * 0.9f));
    }

    static void BuildRoomFurniture(Transform parent)
    {
        var furniture = new GameObject("Furniture");
        furniture.transform.SetParent(parent, false);

        BuildWardrobe(furniture.transform);
        BuildNightstandAndPhone(furniture.transform);
        BuildArmchair(furniture.transform, "Sofa_1", Sofa1CenterZ);
        BuildArmchair(furniture.transform, "Sofa_2", Sofa2CenterZ);

        MakeCylinder("Table_Leg_FL", furniture.transform, new Vector3(SeatingCenterX - TableWidth / 2f + 0.05f, (TableHeight - 0.05f) / 2f, TableCenterZ - TableDepth / 2f + 0.05f), 0.04f, TableHeight - 0.05f);
        MakeCylinder("Table_Leg_FR", furniture.transform, new Vector3(SeatingCenterX + TableWidth / 2f - 0.05f, (TableHeight - 0.05f) / 2f, TableCenterZ - TableDepth / 2f + 0.05f), 0.04f, TableHeight - 0.05f);
        MakeCylinder("Table_Leg_BL", furniture.transform, new Vector3(SeatingCenterX - TableWidth / 2f + 0.05f, (TableHeight - 0.05f) / 2f, TableCenterZ + TableDepth / 2f - 0.05f), 0.04f, TableHeight - 0.05f);
        MakeCylinder("Table_Leg_BR", furniture.transform, new Vector3(SeatingCenterX + TableWidth / 2f - 0.05f, (TableHeight - 0.05f) / 2f, TableCenterZ + TableDepth / 2f - 0.05f), 0.04f, TableHeight - 0.05f);
        MakeBox("Side_Table_Top", furniture.transform,
            new Vector3(SeatingCenterX, TableHeight - 0.025f, TableCenterZ),
            new Vector3(TableWidth, 0.05f, TableDepth));

        BuildTv(furniture.transform);
    }

    const string TvFbxPath = "Assets/3rdParty/tv-and-tv-stand/source/tv.fbx";
    const string TvTexDir = "Assets/3rdParty/tv-and-tv-stand/textures/";
    const string TvAssetDir = "Assets/3rdParty/tv-and-tv-stand/";
    static Material cachedTvBodyMaterial;
    static Material cachedTvWoodMaterial;

    // Real downloaded model if it's present in the project; otherwise the original box placeholder
    // so a room never ends up with no TV at all.
    static void BuildTv(Transform furnitureParent)
    {
        var prefabSource = AssetDatabase.LoadAssetAtPath<GameObject>(TvFbxPath);
        if (prefabSource == null)
        {
            MakeBox("TV_Shelf", furnitureParent,
                new Vector3(TvCenterX, TvShelfElevation + TvShelfHeight / 2f, TvCenterZ),
                new Vector3(TvThickness, TvShelfHeight, TvSpan));
            MakeBox("TV_Screen", furnitureParent,
                new Vector3(TvCenterX + TvThickness / 2f + 0.03f, TvShelfElevation + 0.75f, TvCenterZ),
                new Vector3(0.06f, 0.9f, TvSpan * 0.75f));
            return;
        }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabSource, furnitureParent);
        instance.name = "TV_Model";
        instance.transform.localPosition = Vector3.zero;
        // Guess: model's front (screen) faces -Y in its source app. If it ends up facing the wrong
        // way once mounted, adjust this Y rotation — there was no way to preview it from here.
        instance.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
        instance.transform.localScale = Vector3.one;

        if (cachedTvBodyMaterial == null || cachedTvWoodMaterial == null)
            PrepareTvMaterials(out cachedTvBodyMaterial, out cachedTvWoodMaterial);

        // The model has two material slots (TV body vs. wood stand) — the source material's own name
        // (still intact on the renderer at this point) says which one each slot originally was.
        foreach (var r in instance.GetComponentsInChildren<Renderer>())
        {
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                bool isWood = mats[i] != null && mats[i].name.ToLower().Contains("wood");
                mats[i] = isWood ? cachedTvWoodMaterial : cachedTvBodyMaterial;
            }
            r.sharedMaterials = mats;
        }

        // Auto-fit: scale so the model reads as a ~1m-wide flat screen regardless of the FBX's native
        // scale, then re-anchor so its visual center (not its arbitrary source-app pivot) lands on the
        // wall mount point.
        var bounds = ComputeWorldBounds(instance);
        float currentWidth = Mathf.Max(bounds.size.x, bounds.size.z, 0.001f);
        instance.transform.localScale = Vector3.one * (1f / currentWidth);

        bounds = ComputeWorldBounds(instance);
        Vector3 localPivotOffset = furnitureParent.InverseTransformPoint(bounds.center);
        Vector3 targetLocalCenter = new Vector3(TvCenterX - 0.06f, TvShelfElevation + 0.55f, TvCenterZ);
        instance.transform.localPosition = targetLocalCenter - localPivotOffset;

        Undo.RegisterCreatedObjectUndo(instance, "Build Hotel Blockout");
    }

    static Bounds ComputeWorldBounds(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.zero);
        var b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);
        return b;
    }

    // Two Standard-shader materials (once per batch run, cached as real assets): the TV body/screen
    // (DOOOR_NOB_* textures — metallic-roughness, packed into Standard's metallic-smoothness) and the
    // wood stand (Wood067_* textures — no metallic map since wood isn't metal, plus a parallax map
    // from the displacement texture for cheap extra surface detail).
    static void PrepareTvMaterials(out Material body, out Material wood)
    {
        SetTextureType(TvTexDir + "DOOOR_NOB_Normal.png", TextureImporterType.NormalMap);
        SetTextureType(TvTexDir + "Wood067_1K-JPG_NormalGL.jpg", TextureImporterType.NormalMap);

        var bodyPacked = GetOrCreatePackedTexture(
            TvTexDir + "DOOOR_NOB_Metallic.png", TvTexDir + "DOOOR_NOB_Roughness.png",
            TvAssetDir + "TvBody_MetallicSmoothness.png");
        var woodPacked = GetOrCreatePackedTexture(
            null, TvTexDir + "Wood067_1K-JPG_Roughness.jpg",
            TvAssetDir + "TvStandWood_MetallicSmoothness.png");

        body = GetOrCreateMaterial(TvAssetDir + "TvBody_Mat.mat");
        body.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture2D>(TvTexDir + "DOOOR_NOB_Base_color.png"));
        ApplyNormalAndMetallicGloss(body, TvTexDir + "DOOOR_NOB_Normal.png", bodyPacked);

        wood = GetOrCreateMaterial(TvAssetDir + "TvStandWood_Mat.mat");
        wood.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture2D>(TvTexDir + "Wood067_1K-JPG_Color.jpg"));
        ApplyNormalAndMetallicGloss(wood, TvTexDir + "Wood067_1K-JPG_NormalGL.jpg", woodPacked);

        var displacement = AssetDatabase.LoadAssetAtPath<Texture2D>(TvTexDir + "Wood067_1K-JPG_Displacement.jpg");
        if (displacement != null)
        {
            wood.SetTexture("_ParallaxMap", displacement);
            wood.SetFloat("_Parallax", 0.02f);
            wood.EnableKeyword("_PARALLAXMAP");
        }

        EditorUtility.SetDirty(body);
        EditorUtility.SetDirty(wood);
        AssetDatabase.SaveAssets();
    }

    static void ApplyNormalAndMetallicGloss(Material mat, string normalPath, Texture2D metallicGloss)
    {
        var normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath);
        if (normalTex != null)
        {
            mat.SetTexture("_BumpMap", normalTex);
            mat.EnableKeyword("_NORMALMAP");
        }
        if (metallicGloss != null)
        {
            mat.SetTexture("_MetallicGlossMap", metallicGloss);
            mat.SetFloat("_Metallic", 1f);
            mat.SetFloat("_GlossMapScale", 1f);
            mat.EnableKeyword("_METALLICGLOSSMAP");
        }
    }

    static Material GetOrCreateMaterial(string path)
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(mat, path);
        }
        return mat;
    }

    // Packs metallic (RGB, or flat 0 if there's no metallic map — e.g. wood) and inverted roughness
    // (alpha = smoothness) into one texture, the layout Standard's metallic workflow expects. Caches
    // the result as a real asset so repeat batch runs don't redo the per-pixel work.
    static Texture2D GetOrCreatePackedTexture(string metallicPath, string roughnessPath, string outputPath)
    {
        if (!System.IO.File.Exists(outputPath))
        {
            if (!string.IsNullOrEmpty(metallicPath))
                SetTextureReadableLinear(metallicPath);
            SetTextureReadableLinear(roughnessPath);

            Texture2D metallicTex = string.IsNullOrEmpty(metallicPath) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(metallicPath);
            var roughTex = AssetDatabase.LoadAssetAtPath<Texture2D>(roughnessPath);
            if (roughTex == null)
                return null;

            int w = roughTex.width, h = roughTex.height;
            var packed = new Texture2D(w, h, TextureFormat.RGBA32, false);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float u = x / (float)w, v = y / (float)h;
                    float metallic = metallicTex != null ? metallicTex.GetPixelBilinear(u, v).r : 0f;
                    float rough = roughTex.GetPixel(x, y).r;
                    packed.SetPixel(x, y, new Color(metallic, metallic, metallic, 1f - rough));
                }
            }
            packed.Apply();
            System.IO.File.WriteAllBytes(outputPath, packed.EncodeToPNG());
            Object.DestroyImmediate(packed);
            AssetDatabase.ImportAsset(outputPath, ImportAssetOptions.ForceUpdate);
            SetTextureLinear(outputPath);
        }
        return AssetDatabase.LoadAssetAtPath<Texture2D>(outputPath);
    }

    static void SetTextureType(string path, TextureImporterType type)
    {
        if (AssetImporter.GetAtPath(path) is not TextureImporter importer || importer.textureType == type)
            return;
        importer.textureType = type;
        importer.SaveAndReimport();
    }

    static void SetTextureReadableLinear(string path)
    {
        if (AssetImporter.GetAtPath(path) is not TextureImporter importer)
            return;
        bool changed = false;
        if (!importer.isReadable) { importer.isReadable = true; changed = true; }
        if (importer.sRGBTexture) { importer.sRGBTexture = false; changed = true; }
        if (changed) importer.SaveAndReimport();
    }

    static void SetTextureLinear(string path)
    {
        if (AssetImporter.GetAtPath(path) is not TextureImporter importer || !importer.sRGBTexture)
            return;
        importer.sRGBTexture = false;
        importer.SaveAndReimport();
    }

    static void BuildRoomLighting(Transform parent, float width, float length)
    {
        var lighting = new GameObject("Lighting");
        lighting.transform.SetParent(parent, false);

        float centerX = width / 2f;
        float centerZ = length / 2f;
        MakeCylinder("Ceiling_Fixture", lighting.transform, new Vector3(centerX, CeilingHeight - 0.06f, centerZ), 0.4f, 0.1f);
        BuildPointLight(lighting.transform, "Ceiling_Light", new Vector3(centerX, CeilingHeight - 0.3f, centerZ),
            new Color(1f, 0.82f, 0.6f), 1.2f, 7f);

        BuildLamp(lighting.transform, "Nightstand_Lamp", NightstandCenterX + 0.28f, NightstandHeight, NightstandCenterZ - 0.28f);
        BuildLamp(lighting.transform, "SideTable_Lamp", SeatingCenterX, TableHeight, TableCenterZ);
    }

    static void BuildLamp(Transform parent, string name, float x, float surfaceY, float z)
    {
        var lamp = new GameObject(name);
        lamp.transform.SetParent(parent, false);

        MakeCylinder("Base", lamp.transform, new Vector3(x, surfaceY + 0.02f, z), 0.12f, 0.04f);
        MakeCylinder("Pole", lamp.transform, new Vector3(x, surfaceY + 0.16f, z), 0.03f, 0.24f);
        MakeCylinder("Shade", lamp.transform, new Vector3(x, surfaceY + 0.34f, z), 0.22f, 0.16f);

        BuildPointLight(lamp.transform, "Lamp_Light", new Vector3(x, surfaceY + 0.32f, z),
            new Color(1f, 0.78f, 0.52f), 0.7f, 2.8f);
    }

    static void BuildPointLight(Transform parent, string name, Vector3 localPosition, Color color, float intensity, float range)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;

        var light = go.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;

        Undo.RegisterCreatedObjectUndo(go, "Build Hotel Blockout");
    }

    static void BuildWardrobe(Transform parent)
    {
        var wardrobe = new GameObject("Wardrobe");
        wardrobe.transform.SetParent(parent, false);

        MakeBox("Plinth", wardrobe.transform,
            new Vector3(WardrobeCenterX, 0.05f, WardrobeCenterZ),
            new Vector3(WardrobeSize, 0.1f, WardrobeSize));

        MakeBox("Cabinet_Body", wardrobe.transform,
            new Vector3(WardrobeCenterX, 0.1f + (WardrobeHeight - 0.2f) / 2f, WardrobeCenterZ),
            new Vector3(WardrobeSize - 0.05f, WardrobeHeight - 0.2f, WardrobeSize - 0.05f));

        MakeBox("Cornice", wardrobe.transform,
            new Vector3(WardrobeCenterX, WardrobeHeight - 0.05f, WardrobeCenterZ),
            new Vector3(WardrobeSize + 0.05f, 0.1f, WardrobeSize + 0.05f));

        float doorWidth = (WardrobeSize - 0.08f) / 2f;
        float doorZ = WardrobeCenterZ + WardrobeSize / 2f + 0.02f;
        float doorY = 0.15f + (WardrobeHeight - 0.5f) / 2f;
        float door1X = WardrobeCenterX - doorWidth / 2f - 0.02f;
        float door2X = WardrobeCenterX + doorWidth / 2f + 0.02f;

        MakeBox("Door_Left", wardrobe.transform, new Vector3(door1X, doorY, doorZ), new Vector3(doorWidth, WardrobeHeight - 0.5f, 0.03f));
        MakeBox("Door_Right", wardrobe.transform, new Vector3(door2X, doorY, doorZ), new Vector3(doorWidth, WardrobeHeight - 0.5f, 0.03f));

        MakeBox("Handle_Left", wardrobe.transform, new Vector3(door1X + doorWidth / 2f - 0.03f, doorY, doorZ + 0.03f), new Vector3(0.04f, 0.15f, 0.04f));
        MakeBox("Handle_Right", wardrobe.transform, new Vector3(door2X - doorWidth / 2f + 0.03f, doorY, doorZ + 0.03f), new Vector3(0.04f, 0.15f, 0.04f));
    }

    static void BuildNightstandAndPhone(Transform parent)
    {
        var nightstand = new GameObject("Nightstand_Phone");
        nightstand.transform.SetParent(parent, false);

        const float legHeight = 0.15f;
        float legInsetX0 = NightstandCenterX - NightstandSize / 2f + 0.05f;
        float legInsetX1 = NightstandCenterX + NightstandSize / 2f - 0.05f;
        float legInsetZ0 = NightstandCenterZ - NightstandSize / 2f + 0.05f;
        float legInsetZ1 = NightstandCenterZ + NightstandSize / 2f - 0.05f;

        MakeCylinder("Leg_FL", nightstand.transform, new Vector3(legInsetX0, legHeight / 2f, legInsetZ0), 0.04f, legHeight);
        MakeCylinder("Leg_FR", nightstand.transform, new Vector3(legInsetX1, legHeight / 2f, legInsetZ0), 0.04f, legHeight);
        MakeCylinder("Leg_BL", nightstand.transform, new Vector3(legInsetX0, legHeight / 2f, legInsetZ1), 0.04f, legHeight);
        MakeCylinder("Leg_BR", nightstand.transform, new Vector3(legInsetX1, legHeight / 2f, legInsetZ1), 0.04f, legHeight);

        MakeBox("Body", nightstand.transform,
            new Vector3(NightstandCenterX, legHeight + (NightstandHeight - legHeight) / 2f, NightstandCenterZ),
            new Vector3(NightstandSize, NightstandHeight - legHeight, NightstandSize));

        MakeBox("Drawer_Face", nightstand.transform,
            new Vector3(NightstandCenterX + NightstandSize / 2f - 0.02f, (legHeight + NightstandHeight) / 2f, NightstandCenterZ),
            new Vector3(0.04f, 0.22f, NightstandSize - 0.15f));
        MakeBox("Drawer_Knob", nightstand.transform,
            new Vector3(NightstandCenterX + NightstandSize / 2f + 0.01f, (legHeight + NightstandHeight) / 2f, NightstandCenterZ),
            new Vector3(0.03f, 0.03f, 0.06f));

        MakeBox("Phone_Base", nightstand.transform,
            new Vector3(NightstandCenterX, NightstandHeight + 0.03f, NightstandCenterZ),
            new Vector3(0.28f, 0.06f, 0.2f));
        MakeBox("Phone_Handset", nightstand.transform,
            new Vector3(NightstandCenterX, NightstandHeight + 0.09f, NightstandCenterZ),
            new Vector3(0.22f, 0.05f, 0.09f));
    }

    static void BuildArmchair(Transform parent, string name, float centerZ)
    {
        var chair = new GameObject(name);
        chair.transform.SetParent(parent, false);

        const float legHeight = 0.12f;
        const float seatHeight = 0.15f;
        const float armHeight = 0.35f;
        const float backHeight = 0.55f;
        float x0 = SeatingCenterX - SofaSize / 2f;
        float x1 = SeatingCenterX + SofaSize / 2f;
        float z0 = centerZ - SofaSize / 2f;
        float z1 = centerZ + SofaSize / 2f;

        MakeCylinder("Leg_FL", chair.transform, new Vector3(x0 + 0.08f, legHeight / 2f, z0 + 0.08f), 0.06f, legHeight);
        MakeCylinder("Leg_FR", chair.transform, new Vector3(x1 - 0.08f, legHeight / 2f, z0 + 0.08f), 0.06f, legHeight);
        MakeCylinder("Leg_BL", chair.transform, new Vector3(x0 + 0.08f, legHeight / 2f, z1 - 0.08f), 0.06f, legHeight);
        MakeCylinder("Leg_BR", chair.transform, new Vector3(x1 - 0.08f, legHeight / 2f, z1 - 0.08f), 0.06f, legHeight);

        MakeBox("Seat", chair.transform,
            new Vector3(SeatingCenterX, legHeight + seatHeight / 2f, centerZ),
            new Vector3(SofaSize - 0.2f, seatHeight, SofaSize - 0.05f));

        MakeBox("Backrest", chair.transform,
            new Vector3(x0 + 0.06f, legHeight + backHeight / 2f, centerZ),
            new Vector3(0.12f, backHeight, SofaSize - 0.05f));

        MakeBox("Armrest_Near", chair.transform,
            new Vector3(SeatingCenterX + 0.05f, legHeight + armHeight / 2f, z0 + 0.06f),
            new Vector3(SofaSize - 0.3f, armHeight, 0.12f));
        MakeBox("Armrest_Far", chair.transform,
            new Vector3(SeatingCenterX + 0.05f, legHeight + armHeight / 2f, z1 - 0.06f),
            new Vector3(SofaSize - 0.3f, armHeight, 0.12f));
    }

    static void BuildBed(Transform parent, float width, float length)
    {
        var bed = new GameObject("Bed");
        bed.transform.SetParent(parent, false);

        float bedX0 = 0f;
        float bedZ0 = BathLength + BedClearance;

        MakeBox("Mattress", bed.transform,
            new Vector3(bedX0 + BedWidth / 2f, BedLegHeight + BedHeight / 2f, bedZ0 + BedDepth / 2f),
            new Vector3(BedWidth, BedHeight, BedDepth));

        float legInsetX0 = bedX0 + BedLegSize / 2f;
        float legInsetX1 = bedX0 + BedWidth - BedLegSize / 2f;
        float legInsetZ0 = bedZ0 + BedLegSize / 2f;
        float legInsetZ1 = bedZ0 + BedDepth - BedLegSize / 2f;
        var legSize = new Vector3(BedLegSize, BedLegHeight, BedLegSize);

        MakeBox("Leg_FrontLeft", bed.transform, new Vector3(legInsetX0, BedLegHeight / 2f, legInsetZ0), legSize);
        MakeBox("Leg_FrontRight", bed.transform, new Vector3(legInsetX1, BedLegHeight / 2f, legInsetZ0), legSize);
        MakeBox("Leg_BackLeft", bed.transform, new Vector3(legInsetX0, BedLegHeight / 2f, legInsetZ1), legSize);
        MakeBox("Leg_BackRight", bed.transform, new Vector3(legInsetX1, BedLegHeight / 2f, legInsetZ1), legSize);

        MakeBox("Headboard", bed.transform,
            new Vector3(0.05f, 0.55f, bedZ0 + BedDepth / 2f),
            new Vector3(0.1f, 1.1f, BedDepth));

        float mattressTopY = BedLegHeight + BedHeight;
        MakeBox("Pillow_1", bed.transform,
            new Vector3(0.45f, mattressTopY + 0.09f, bedZ0 + 0.85f),
            new Vector3(0.65f, 0.18f, 1.05f));
        MakeBox("Pillow_2", bed.transform,
            new Vector3(0.45f, mattressTopY + 0.09f, bedZ0 + BedDepth - 0.85f),
            new Vector3(0.65f, 0.18f, 1.05f));

        MakeBox("Blanket_Fold", bed.transform,
            new Vector3(BedWidth - 0.55f, mattressTopY + 0.07f, bedZ0 + BedDepth / 2f),
            new Vector3(1.1f, 0.14f, BedDepth - 0.3f));
    }

    static void BuildCorridor(Transform parent, float totalWidth)
    {
        float startX = ELWidth;
        float corridorWidth = totalWidth - startX;

        MakeBox("Floor", parent,
            new Vector3(startX + corridorWidth / 2f, -WallThickness / 2f, CorridorDepth / 2f),
            new Vector3(corridorWidth, WallThickness, CorridorDepth));

        MakeBox("Ceiling", parent,
            new Vector3(startX + corridorWidth / 2f, CeilingHeight + WallThickness / 2f, CorridorDepth / 2f),
            new Vector3(corridorWidth, WallThickness, CorridorDepth));

        MakeBox("Wall_East", parent,
            new Vector3(startX + corridorWidth + WallThickness / 2f, CeilingHeight / 2f, CorridorDepth / 2f),
            new Vector3(WallThickness, CeilingHeight, CorridorDepth));
    }

    static void BuildElevatorLobby(Transform parent)
    {
        float southZ = 0f;
        float northZ = CorridorDepth;
        float depth = northZ - southZ;
        float centerZ = (southZ + northZ) / 2f;

        MakeBox("Floor", parent,
            new Vector3(ELWidth / 2f, -WallThickness / 2f, centerZ),
            new Vector3(ELWidth, WallThickness, depth));

        MakeBox("Ceiling", parent,
            new Vector3(ELWidth / 2f, CeilingHeight + WallThickness / 2f, centerZ),
            new Vector3(ELWidth, WallThickness, depth));

        MakeBox("Wall_West", parent,
            new Vector3(-WallThickness / 2f, CeilingHeight / 2f, centerZ),
            new Vector3(WallThickness, CeilingHeight, depth + WallThickness * 2f));

        MakeBox("Wall_South", parent,
            new Vector3(ELWidth / 2f, CeilingHeight / 2f, southZ - WallThickness / 2f),
            new Vector3(ELWidth + WallThickness * 2f, CeilingHeight, WallThickness));

        MakeBox("Wall_North", parent,
            new Vector3(ELWidth / 2f, CeilingHeight / 2f, northZ + WallThickness / 2f),
            new Vector3(ELWidth + WallThickness * 2f, CeilingHeight, WallThickness));

        MakeBox("ElevatorDoor_Placeholder", parent,
            new Vector3(0.15f, 1.05f, centerZ),
            new Vector3(0.3f, 2.1f, 1.8f));
    }

    static GameObject MakeBox(string name, Transform parent, Vector3 localPosition, Vector3 size)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localScale = size;
        Undo.RegisterCreatedObjectUndo(go, "Build Hotel Blockout");
        return go;
    }

    static GameObject MakeCylinder(string name, Transform parent, Vector3 localPosition, float diameter, float height)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localScale = new Vector3(diameter, height / 2f, diameter);
        Undo.RegisterCreatedObjectUndo(go, "Build Hotel Blockout");
        return go;
    }

    static void ClearExisting(string name)
    {
        var existing = GameObject.Find(name);
        if (existing != null)
            Object.DestroyImmediate(existing);
    }

    static void FocusOn(GameObject go)
    {
        Selection.activeGameObject = go;
        SceneView.lastActiveSceneView?.FrameSelected();
    }
}