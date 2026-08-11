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

    // TV shelf, mounted on the east wall
    const float TvCenterX = 9.78f;
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

        BuildRoomShell(root.transform, RoomWidth, RoomLength, includeBathroom: true, doorCenterX: RoomWidth - 1.1f);

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

    // Headless entry point for `Unity.exe -batchmode -executeMethod HotelBlockoutBuilder.BuildAllFloorsAndSaveBatch`.
    // Opens SampleScene, rebuilds Floor1-5, reapplies hyojeong0407's dark materials plus the luxury furniture
    // palette (rebuilding replaces every object, which would otherwise leave fresh ones with no material), and saves.
    public static void BuildAllFloorsAndSaveBatch()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
        BuildAllFloorsMenu();
        var floors = FindAllFloorRoots();
        Selection.objects = floors;
        ExteriorMaterialApplier.ApplyMaterials();
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

    // Applied on top of ExteriorMaterialApplier's dark walls/floors/ceilings — a warm, rich palette
    // for the furniture pieces only (deep red velvet upholstery, dark mahogany wood, brass accents),
    // matching a reference photo of an old-money hotel suite. Bathroom fixtures are left alone so the
    // bathroom stays colder/starker than the bedroom.
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
        // Procedural textures (no source images in the project) — plain noise shaped per material so
        // it reads as grain/weave/speckle rather than visible static.
        Texture2D woodTex = GenerateWoodTexture(new Color(0.22f, 0.11f, 0.06f));
        Texture2D velvetTex = GenerateFabricTexture(new Color(0.42f, 0.06f, 0.09f));
        Texture2D goldFabricTex = GenerateFabricTexture(new Color(0.5f, 0.42f, 0.28f));
        Texture2D ceramicTex = GenerateCeramicTexture(new Color(0.16f, 0.16f, 0.17f));
        Texture2D brassTex = GenerateMetalTexture(new Color(0.55f, 0.42f, 0.15f));
        Texture2D vinylTex = GenerateFabricTexture(new Color(0.1f, 0.11f, 0.13f));

        // name -> (color, glossiness, metallic, texture). Glossiness/metallic/texture are picked per
        // real-world material, not just per object, so everything reads as what it is, not flat grey.
        Material velvetRed = NewStandardMaterial(new Color(0.42f, 0.06f, 0.09f), 0.15f, 0f, velvetTex, new Vector2(4f, 4f));
        Material mahogany = NewStandardMaterial(new Color(0.22f, 0.11f, 0.06f), 0.3f, 0f, woodTex, new Vector2(2f, 3f));
        Material brassMetal = NewStandardMaterial(new Color(0.55f, 0.42f, 0.15f), 0.6f, 0.75f, brassTex, new Vector2(3f, 1.5f));
        Material mutedGold = NewStandardMaterial(new Color(0.5f, 0.42f, 0.28f), 0.1f, 0f, goldFabricTex, new Vector2(4f, 4f));
        Material blackPlastic = NewStandardMaterial(new Color(0.03f, 0.03f, 0.03f), 0.6f, 0f);      // glossy plastic, stays smooth
        Material mirrorGlass = NewStandardMaterial(new Color(0.6f, 0.6f, 0.62f), 0.92f, 0.85f);     // stays smooth for reflection
        Material ceramic = NewStandardMaterial(new Color(0.16f, 0.16f, 0.17f), 0.45f, 0f, ceramicTex, new Vector2(3f, 3f));
        Material windowGlass = NewStandardMaterial(new Color(0.5f, 0.62f, 0.58f), 0.85f, 0.1f);     // stays smooth, faint blue-green tint
        Material vinylCurtain = NewStandardMaterial(new Color(0.1f, 0.11f, 0.13f), 0.35f, 0f, vinylTex, new Vector2(2f, 4f));

        var redVelvet = new System.Collections.Generic.HashSet<string> { "seat", "backrest", "armrest_near", "armrest_far", "mattress", "blanket_fold" };
        var gold = new System.Collections.Generic.HashSet<string> { "pillow_1", "pillow_2", "shade" };
        var wood = new System.Collections.Generic.HashSet<string> { "cabinet_body", "door_left", "door_right", "cornice", "plinth", "body", "drawer_face", "side_table_top", "headboard", "tv_shelf", "leaf" };
        var brass = new System.Collections.Generic.HashSet<string> { "handle_left", "handle_right", "drawer_knob", "tub_faucet", "sink_faucet", "mirror_frame_top", "mirror_frame_bottom", "base", "pole", "ceiling_fixture" };
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
                    n == "mirror" ? mirrorGlass :
                    n == "glass" ? windowGlass :
                    n.StartsWith("curtain_fold") ? vinylCurtain :
                    null;

                if (chosen == null)
                    continue;

                r.sharedMaterial = chosen;
                count++;
            }
        }

        Debug.Log($"럭셔리 가구 배색 완료: {count}개 블록 (레드 벨벳/마호가니/브라스/도기/유리).");
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

    // Grain stretched hard along Y (thin noise sampling in X, coarse in Y) plus a faint cross streak.
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

    // Fine, even noise for a woven-fabric feel — used for velvet, muted-gold cloth, and vinyl.
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

    // Very subtle low-frequency mottling, glossy ceramic shouldn't look noisy up close.
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

    // Brushed-metal streaks: fine noise stretched along X, near-flat along Y.
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

    // Floor 1 has the ground-level lobby (open front desk/passage behind a single entrance wall).
    // Floors 2+ reuse the exact same EL/room/corridor/Staff_Room layout, just stacked up by StoryHeight,
    // with 205-Staff_Room's zone simplified to one closed-off void (no entrance needed above the lobby).
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

        // South row: x05 — Front desk (open, no walls) — walkway (open) — Staff room (enclosed) — x04.
        const float frontDeskWidth = 3f;
        const float staffRoomWidth = 10f;
        // North row: x03 — [gap mirroring the open zone below] — x02 (moved to sit opposite Staff_Room) — x01.

        float bay1West = ELWidth + RoomWidth;       // Front_Desk's west edge, also Room x03's east edge
        float passageWest = bay1West + frontDeskWidth;
        float staffWest = passageWest + PassageWidth; // also Room x02's west edge (x02 sits opposite Staff_Room)
        float bay2South = staffWest + staffRoomWidth;  // Room x04's west edge

        float room1West = staffWest + RoomWidth; // sits right after x02, no gap
        float totalWidth = Mathf.Max(bay2South + RoomWidth, room1West + RoomWidth);

        string RoomName(int d) => "Room_" + (floorNumber * 100 + d);

        // North row (x03, x02, x01), doors face south into the corridor
        BuildGuestRoomSlot(rooms.transform, RoomName(3), new Vector3(ELWidth, 0f, CorridorDepth), 0f);
        BuildOpenBay(rooms.transform, "North_Gap_x03_x02", bay1West, staffWest - bay1West, north: true, includeOuterWall: true);
        BuildGuestRoomSlot(rooms.transform, RoomName(2), new Vector3(staffWest, 0f, CorridorDepth), 0f);
        BuildGuestRoomSlot(rooms.transform, RoomName(1), new Vector3(room1West, 0f, CorridorDepth), 0f);

        // South row (x05, Front desk / walkway / Staff room, x04), doors face north into the corridor
        BuildGuestRoomSlot(rooms.transform, RoomName(5), new Vector3(bay1West, 0f, 0f), 180f);
        BuildGuestRoomSlot(rooms.transform, RoomName(4), new Vector3(bay2South + RoomWidth, 0f, 0f), 180f);
        BuildUtilityRoomSlot(rooms.transform, "Staff_Room", staffWest, staffRoomWidth);

        if (floorNumber == 1)
        {
            // Front_Desk + passage share one open floor with no wall between them, and are closed off on the
            // building's south (outer) face by a single wall with a ~4m entrance gap centered across that span.
            BuildOpenBay(rooms.transform, "Front_Desk", bay1West, frontDeskWidth, north: false, includeOuterWall: false);
            BuildOpenBay(rooms.transform, "Staff_Front_Passage", passageWest, PassageWidth, north: false, includeOuterWall: false);
            BuildSouthEntranceWall(rooms.transform, bay1West, staffWest - bay1West, 4f);
        }
        else
        {
            // No lobby entrance above the ground floor — just one closed-off void between x05 and Staff_Room.
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

        BuildRoomShell(slot.transform, RoomWidth, RoomLength, includeBathroom: true, doorCenterX: RoomWidth - 1.1f);
    }

    // South-row utility room (no bathroom), placed by its own west edge X and width within the bay.
    static void BuildUtilityRoomSlot(Transform parent, string name, float westEdgeX, float width)
    {
        var slot = new GameObject(name);
        slot.transform.SetParent(parent);
        slot.transform.localPosition = new Vector3(westEdgeX + width, 0f, 0f);
        slot.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        Undo.RegisterCreatedObjectUndo(slot, "Build Floor 1");

        BuildRoomShell(slot.transform, width, RoomLength, includeBathroom: false, doorCenterX: width / 2f);
    }

    // Open floor space with no interior partitioning — used for the front desk void, the staff/front
    // walkway, and the reserved gap between 101 and 102. Side walls come for free from whichever
    // rooms flank it; `includeOuterWall` controls whether the far (exterior-facing) edge gets closed off.
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
            // The exterior edge is local z = RoomLength for north bays (corridor is at z = 0) but
            // local z = 0 for south bays (corridor is at z = RoomLength) — mirror image of each other.
            float exteriorZ = north ? RoomLength + WallThickness / 2f : -WallThickness / 2f;
            MakeBox("Wall_Outer", slot.transform,
                new Vector3(width / 2f, CeilingHeight / 2f, exteriorZ),
                new Vector3(width + WallThickness * 2f, CeilingHeight, WallThickness));
        }
    }

    // South-facing exterior wall for the hotel's main entrance, with a door gap centered across the given span.
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

    // Local room space: X 0..width, Z 0..length, entry door on the Z=0 wall.
    static void BuildRoomShell(Transform parent, float width, float length, bool includeBathroom, float doorCenterX)
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

        // Main entry door — hinged at the left jamb, swings into the room. Centered on Wall_Front_Left/
        // Right's own thickness (they sit at z = -WallThickness/2, not z = 0) so the door lines up with the wall.
        BuildDoor(parent, "Room_Door", doorMinX, -WallThickness / 2f, DoorWidth, CeilingHeight);

        var bathroom = new GameObject("Bathroom");
        bathroom.transform.SetParent(parent, false);

        MakeBox("Bath_Wall_Side", bathroom.transform,
            new Vector3(BathWidth, CeilingHeight / 2f, BathLength / 2f),
            new Vector3(WallThickness, CeilingHeight, BathLength));

        float bathDoorMinX = BathWidth - BathDoorWidth;
        MakeBox("Bath_Wall_Front", bathroom.transform,
            new Vector3(bathDoorMinX / 2f, CeilingHeight / 2f, BathLength),
            new Vector3(bathDoorMinX, CeilingHeight, WallThickness));

        // Bathroom door — hinged at the left jamb, swings out into the bedroom (away from the fixtures).
        // Bath_Wall_Front is centered exactly at z = BathLength, so the pivot goes there directly.
        BuildDoor(bathroom.transform, "Bath_Door", bathDoorMinX, BathLength, BathDoorWidth, CeilingHeight);

        BuildBathroomFixtures(bathroom.transform);
        BuildBed(parent, width, length);
        BuildRoomFurniture(parent);
        BuildRoomLighting(parent, width, length);
    }

    // Door leaf hinged at the opening's left jamb (hingeX), swinging open in the local +Z direction.
    // wallCenterZ is the wall's own thickness center (not just "where the opening is"), height matches
    // the ceiling, and the leaf is as thick as the wall it sits in — so it reads as part of the wall,
    // not a thin panel floating in a full-height gap.
    // The pivot carries the DoorInteractable script; the leaf is offset from it so rotating the pivot
    // swings the door like a real hinge instead of spinning it in place.
    static void BuildDoor(Transform parent, string name, float hingeX, float wallCenterZ, float doorWidth, float doorHeight)
    {
        var pivot = new GameObject(name);
        pivot.transform.SetParent(parent, false);
        pivot.transform.localPosition = new Vector3(hingeX, 0f, wallCenterZ);

        float leafWidth = doorWidth - 0.04f;
        MakeBox("Leaf", pivot.transform,
            new Vector3(leafWidth / 2f, doorHeight / 2f, 0f),
            new Vector3(leafWidth, doorHeight, WallThickness));

        var trigger = new GameObject("InteractZone");
        trigger.transform.SetParent(pivot.transform, false);
        trigger.transform.localPosition = new Vector3(leafWidth / 2f, doorHeight / 2f, 0f);
        var box = trigger.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(doorWidth + 1.6f, doorHeight, 1.6f);
        Undo.RegisterCreatedObjectUndo(trigger, "Build Hotel Blockout");

        pivot.AddComponent<DoorInteractable>();
        Undo.RegisterCreatedObjectUndo(pivot, "Build Hotel Blockout");
    }

    // Window cut into the back wall (opposite the door): a sill strip, a lintel strip, and side
    // strips flanking the opening — the opening itself is left empty, same as the door gaps.
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

        // Glass pane sitting in the opening, and a sill ledge protruding into the room
        MakeBox("Glass", wall.transform,
            new Vector3(WindowCenterX, (WindowSillHeight + windowTop) / 2f, wallZ),
            new Vector3(WindowWidth - 0.1f, WindowHeight - 0.1f, 0.03f));

        MakeBox("Sill_Ledge", wall.transform,
            new Vector3(WindowCenterX, WindowSillHeight - 0.02f, length - 0.1f),
            new Vector3(WindowWidth + 0.1f, 0.04f, 0.3f));
    }

    static void BuildBathroomFixtures(Transform bathroom)
    {
        // Tub shell + a wall-mounted faucet at the head end
        MakeBox("Tub", bathroom,
            new Vector3(TubCenterX, TubHeight / 2f, TubCenterZ),
            new Vector3(TubWidth, TubHeight, TubDepth));

        float tubFaucetX = TubCenterX - TubWidth / 2f + 0.3f;
        MakeCylinder("Tub_Faucet", bathroom,
            new Vector3(tubFaucetX, TubHeight + 0.12f, TubCenterZ - TubDepth / 2f + 0.05f), 0.04f, 0.22f);

        // Curtain as a row of narrow, alternately-offset panels so it reads as hanging fabric, not a slab
        float curtainZ = TubCenterZ + TubDepth / 2f; // hangs along the tub's front (open) edge
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

        // Pedestal sink: round basin + pedestal + wall backsplash + faucet, instead of a solid block
        MakeCylinder("Sink_Pedestal", bathroom,
            new Vector3(SinkCenterX, 0.325f, SinkCenterZ), 0.12f, 0.65f);
        MakeCylinder("Sink_Basin", bathroom,
            new Vector3(SinkCenterX, 0.68f, SinkCenterZ), Mathf.Min(SinkWidth, SinkDepth) * 0.9f, 0.12f);
        MakeBox("Sink_Backsplash", bathroom,
            new Vector3(0.15f, 0.75f, SinkCenterZ),
            new Vector3(0.05f, 0.15f, SinkDepth));
        MakeCylinder("Sink_Faucet", bathroom,
            new Vector3(0.3f, 0.85f, SinkCenterZ), 0.03f, 0.15f);

        // Mirror with a thin frame border
        MakeBox("Mirror", bathroom, // mounted on the west wall, above the sink
            new Vector3(0.03f, MirrorElevation + MirrorHeight / 2f, SinkCenterZ),
            new Vector3(0.06f, MirrorHeight, SinkDepth));
        MakeBox("Mirror_Frame_Top", bathroom,
            new Vector3(0.05f, MirrorElevation + MirrorHeight + 0.02f, SinkCenterZ),
            new Vector3(0.1f, 0.04f, SinkDepth + 0.06f));
        MakeBox("Mirror_Frame_Bottom", bathroom,
            new Vector3(0.05f, MirrorElevation - 0.02f, SinkCenterZ),
            new Vector3(0.1f, 0.04f, SinkDepth + 0.06f));

        // Toilet: tank against the east wall, bowl + seat toward the room
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

        MakeBox("TV_Shelf", furniture.transform,
            new Vector3(TvCenterX, TvShelfElevation + TvShelfHeight / 2f, TvCenterZ),
            new Vector3(TvThickness, TvShelfHeight, TvSpan));
        MakeBox("TV_Screen", furniture.transform,
            new Vector3(TvCenterX + TvThickness / 2f + 0.03f, TvShelfElevation + 0.75f, TvCenterZ),
            new Vector3(0.06f, 0.9f, TvSpan * 0.75f));
    }

    // One overhead pendant for general room fill, plus two warm accent lamps (nightstand, side table)
    // echoing the reference photo's twin table lamps either side of the seating area.
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

        // Two door panels facing +Z (into the room), with a small gap and handles at the inner edges
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

    // Legs + seat + backrest + armrests, facing +X toward the TV shelf.
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

        // Backrest on the -X side (chair faces +X, toward the TV)
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

    // Bed is flush against the left wall (the same side as the bathroom, x = 0 — no clearance needed
    // there since it's already touching the wall), and kept BedClearance past the bathroom's back wall
    // so it stays close to the bathroom without overlapping it.
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

        // Headboard against the wall the bed is flush with, running along the bed's depth
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

        // Folded-back blanket strip near the foot of the bed
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

    // EL lobby is a square matching the corridor depth (ELWidth should equal CorridorDepth), sitting
    // flush with the corridor rather than spanning the whole building — the room rows no longer reach it.
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

        // Elevator door placeholder — swap for a real elevator prefab later
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

    // Unity's cylinder primitive is 2 units tall and 1 unit wide by default (localScale 1,1,1),
    // so height maps to scale.y * 2 and diameter maps to scale.x/scale.z.
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
