using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class HotelBlockoutBuilder
{
    // ==========================================
    // 건물 치수 및 세팅 상수 정의
    // ==========================================
    const float WallThickness = 0.2f;
    const float CeilingHeight = 2.5f;

    const float RoomWidth = 10f;    // X축 각 방 너비 (101~105 공유)
    const float RoomLength = 8f;    // Z축 방 깊이
    const float CorridorDepth = 4f; // Z축 복도 깊이
    const float ELWidth = 4f;       // X축 엘리베이터 로비 너비

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
    const float PassageWidth = 7f;

    // Window in the guest room's back wall (opposite the door)
    const float WindowCenterX = 5f;
    const float WindowWidth = 5f;
    const float WindowSillHeight = 1f;
    const float WindowHeight = 1.2f;

    const float WardrobeCenterX = 3.8f;
    const float WardrobeCenterZ = 0.5f;
    const float WardrobeSize = 1.3f;
    const float WardrobeHeight = 2f;

    const float NightstandCenterX = 0.6f;
    const float NightstandCenterZ = 7.5f;
    const float NightstandSize = 1f;
    const float NightstandHeight = 0.72f;

    // Seating: two armchairs with a side table between them, facing the TV
    const float SofaSize = 1.1f;
    const float SofaHeight = 0.8f;
    const float Sofa1CenterZ = 3.55f;
    const float Sofa2CenterZ = 6.45f;
    const float SeatingCenterX = 6.95f;
    const float TableWidth = 0.7f;
    const float TableDepth = 0.6f;
    const float TableHeight = 0.43f;
    const float TableCenterZ = 5f;

    const float TvCenterX = 9.825f;
    const float TvCenterZ = 5f;
    const float TvThickness = 0.35f;
    const float TvSpan = 2f;
    const float TvShelfHeight = 0.5f;
    const float TvShelfElevation = 1f;

    const float TubCenterX = 2.8f;
    const float TubCenterZ = 0.8f;
    const float TubWidth = 2.6f;
    const float TubDepth = 0.9f;
    const float TubHeight = 0.5f;
    const float CurtainHeight = 2f;
    const float SinkCenterX = 0.23f;
    const float SinkCenterZ = 2.1f;
    const float SinkWidth = 0.9f;
    const float SinkDepth = 0.6f;
    const float SinkHeight = 0.85f;
    const float MirrorHeight = 0.8f;
    const float MirrorElevation = 1.4f;
    const float ToiletCenterX = 2.9f;
    const float ToiletCenterZ = 1.7f;
    const float ToiletWidth = 0.7f;
    const float ToiletDepth = 0.8f;
    const float ToiletHeight = 0.4f;

    const float StoryHeight = CeilingHeight + WallThickness;

    // 경로 정의
    const string ArmchairSourcePath = "Assets/3rdParty/Furniture/Prefabs/Fotel3.prefab";
    const string TvSourcePath = "Assets/3rdParty/70-tv/source/TV/TV.blend";
    const string TvStandSourcePath = "Assets/3rdParty/tv-stand-roma-by-turri/source/Moble tv roma turri.fbx";
    const string CarpetDir = "Assets/3rdParty/carpet/";
    const string CarpetMatPath = "Assets/3rdParty/carpet/Carpet_Floor_Mat.mat";
    static readonly Vector2 CarpetTiling = new Vector2(10f, 8f);

    const string BedFbxPath = "Assets/3rdParty/bed/source/model.fbx";
    const string BedTexDir = "Assets/3rdParty/bed/textures/";
    const string BedMatPath = "Assets/3rdParty/bed/Bed_Mat.mat";
    const string BedFbxMaterialName = "Material_0";
    const string BedSourcePath = "Assets/3rdParty/bed/source/model.fbx";
    const float BedModelScale = 1.5f;

    static readonly Vector3 Room304TvStandLocalPos = new Vector3(9.725f, 0f, 5f);
    static readonly Quaternion Room304TvStandLocalRot = Quaternion.identity;
    static readonly Vector3 Room304TvLocalPos = new Vector3(9.825f, 0.4f, 5f);
    static readonly Quaternion Room304TvLocalRot = new Quaternion(-0.5f, -0.5f, -0.5f, 0.5f);


    // ==========================================
    // 1. 메인 통합 빌드 버튼 (단 한번 클릭으로 일괄 실행)
    // ==========================================
    [MenuItem("Tools/Hotel Blockout/Build All Floors (1-5)")]
    public static void BuildAllFloorsMenu()
    {
        // Undo 작업 단일 그룹화 (Ctrl+Z 한 번에 전체 되돌리기 가능)
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Build All Hotel Floors with Full Decor");

        try
        {
            // [단계 1] 기본 층 골조 빌드 (1층~5층)
            for (int floorNumber = 1; floorNumber <= 5; floorNumber++)
                BuildFloor(floorNumber);

            // [단계 2] 불필요한 임시 요소를 제거하고 실제 모델로 교체
            RemoveTvPlaceholderBoxes();
            ReplaceBedsWithModel();
            WireBedTextures();
            RemoveBedLightAndCamera();
            RemoveRoomFloors();

            // [단계 3] 304호 기준 레퍼런스 TV 및 스탠드 배치
            AddTvAndStandFromRoom304Reference();

            // [단계 4] 머티리얼, 카펫, 다크 럭셔리 색상 일괄 적용
            ApplyCarpetToRoomFloors();
            var floors = FindAllFloorRoots();
            ApplyLuxuryFurnitureColors(floors);

            Debug.Log("★ [Hotel Builder] 전체 층 빌드 및 후처리 통합 작업 완료!");
        }
        finally
        {
            Undo.CollapseUndoOperations(undoGroup);
        }
    }


    // ==========================================
    // 2. 층별 개별 메뉴 (필요 시 선택적 실행)
    // ==========================================
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


    // ==========================================
    // 3. 커맨드라인 / 배치 모드 전용 실행 엔트리
    // ==========================================
    public static void BuildAllFloorsAndSaveBatch()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
        BuildAllFloorsMenu();
        EditorSceneManager.SaveScene(scene);
        Debug.Log("BuildAllFloorsAndSaveBatch 완료: SampleScene에 저장됨.");
    }

    public static void AddTvAndStandFromRoom304ReferenceAndSaveBatch()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
        AddTvAndStandFromRoom304Reference();
        EditorSceneManager.SaveScene(scene);
        Debug.Log("AddTvAndStandFromRoom304ReferenceAndSaveBatch 완료.");
    }

    public static void RemoveTvPlaceholderBoxesAndSaveBatch()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
        RemoveTvPlaceholderBoxes();
        EditorSceneManager.SaveScene(scene);
        Debug.Log("RemoveTvPlaceholderBoxesAndSaveBatch 완료.");
    }

    public static void ApplyCarpetToRoomFloorsAndSaveBatch()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
        ApplyCarpetToRoomFloors();
        EditorSceneManager.SaveScene(scene);
        Debug.Log("ApplyCarpetToRoomFloorsAndSaveBatch 완료.");
    }

    public static void RemoveRoomFloorsAndSaveBatch()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
        RemoveRoomFloors();
        EditorSceneManager.SaveScene(scene);
        Debug.Log("RemoveRoomFloorsAndSaveBatch 완료.");
    }

    public static void WireBedTexturesAndSaveBatch()
    {
        WireBedTextures();
        Debug.Log("WireBedTexturesAndSaveBatch 완료.");
    }

    public static void ReplaceBedsWithModelAndSaveBatch()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
        ReplaceBedsWithModel();
        EditorSceneManager.SaveScene(scene);
        Debug.Log("ReplaceBedsWithModelAndSaveBatch 완료.");
    }

    public static void RemoveBedLightAndCameraAndSaveBatch()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
        RemoveBedLightAndCamera();
        EditorSceneManager.SaveScene(scene);
        Debug.Log("RemoveBedLightAndCameraAndSaveBatch 완료.");
    }

    // ==========================================
    // 4. 머티리얼 및 다크모드/색상 적용 로직
    // ==========================================
    [MenuItem("Tools/Hotel Blockout/Apply Luxury Furniture Colors")]
    public static void ApplyLuxuryFurnitureColorsMenu()
    {
        // ⭐️ 선택 상태와 상관없이 무조건 1~5층 전체 건물 오브젝트를 타겟팅합니다.
        var targets = FindAllFloorRoots(); 
        if (targets.Length == 0)
        {
            targets = Selection.gameObjects;
        }
        ApplyLuxuryFurnitureColors(targets);
    }

    static void ApplyLuxuryFurnitureColors(GameObject[] targets)
    {
        // 1. 머티리얼 및 텍스처 정의
        Texture2D woodTex = GenerateWoodTexture(new Color(0.22f, 0.11f, 0.06f));
        Texture2D doorWoodTex = GenerateWoodTexture(new Color(0.25f, 0.14f, 0.07f));
        Texture2D velvetTex = GenerateFabricTexture(new Color(0.42f, 0.06f, 0.09f));
        Texture2D goldFabricTex = GenerateFabricTexture(new Color(0.5f, 0.42f, 0.28f));
        Texture2D ceramicTex = GenerateCeramicTexture(new Color(0.16f, 0.16f, 0.17f));
        Texture2D brassTex = GenerateMetalTexture(new Color(0.55f, 0.42f, 0.15f));
        Texture2D vinylTex = GenerateFabricTexture(new Color(0.1f, 0.11f, 0.13f));
        Texture2D marbleTex = GenerateMarbleTexture(new Color(0.08f, 0.08f, 0.09f));

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
        Material darkMarble = NewStandardMaterial(new Color(0.08f, 0.08f, 0.09f), 0.7f, 0.1f, marbleTex, new Vector2(8f, 8f));
        
        // ⭐️ 요청하신 HEX #33261F / RGB(51, 38, 31) 갈색 머티리얼
        Material darkWall = NewStandardMaterial(new Color(51f / 255f, 38f / 255f, 31f / 255f), 0.0f, 0f);
        Material darkCeiling = NewStandardMaterial(new Color(0.06f, 0.06f, 0.07f), 0.05f, 0f);
        Material fontMaterial = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf").material; 

        var redVelvet = new HashSet<string> { "seat", "backrest", "armrest_near", "armrest_far", "mattress", "blanket_fold" };
        var gold = new HashSet<string> { "pillow_1", "pillow_2", "shade" };
        var wood = new HashSet<string> { "cabinet_body", "door_left", "door_right", "cornice", "plinth", "body", "drawer_face", "side_table_top", "headboard", "tv_shelf" };
        var brass = new HashSet<string> { "handle_left", "handle_right", "drawer_knob", "tub_faucet", "sink_faucet", "mirror_frame_top", "mirror_frame_bottom", "base", "pole", "ceiling_fixture", "plaque_plate" };
        var plastic = new HashSet<string> { "tv_screen", "phone_base", "phone_handset" };
        var ceramicNames = new HashSet<string> { "tub", "sink_basin", "sink_pedestal", "sink_backsplash", "toilet_bowl", "toilet_tank", "toilet_seat" };

        int count = 0;
        foreach (var target in targets)
        {
            Undo.RegisterFullObjectHierarchyUndo(target, "Apply Luxury Furniture Colors");
            foreach (var r in target.GetComponentsInChildren<MeshRenderer>())
            {
                // [방어막 1] 프리팹 인스턴스 검사 (3D 에셋 보존)
                if (PrefabUtility.IsPartOfAnyPrefab(r.gameObject))
                    continue;

                // [방어막 2] 외부 FBX/OBJ/Blend 모델 메시는 색상 변경 제외
                MeshFilter mf = r.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(mf.sharedMesh);
                    if (!string.IsNullOrEmpty(assetPath) && 
                    (assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase) || 
                        assetPath.EndsWith(".obj", System.StringComparison.OrdinalIgnoreCase) ||
                        assetPath.EndsWith(".blend", System.StringComparison.OrdinalIgnoreCase) ||
                        assetPath.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                }

                string n = r.gameObject.name.ToLower();

                // ⭐️ 위치 조건 없이 모든 벽체(Wall) 오브젝트에 지정 갈색을 적용하도록 통일
                bool isWall = n.Contains("wall") || n.Contains("pillar") || n.Contains("sill") || n.Contains("lintel");

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
                    n.StartsWith("handle_") ? brassMetal :
                    n == "plaque_number" ? fontMaterial :
                    n == "leaf" ? doorWood :
                    n == "floor" ? darkMarble :
                    n == "ceiling" ? darkCeiling :
                    isWall ? darkWall : // ⭐️ 벽면에 RGB(51, 38, 31) 갈색 강제 할당
                    null;

                if (chosen == null)
                    continue;

                r.sharedMaterial = chosen;
                count++;
            }
        }

        Debug.Log($"건물 외벽 및 내부 배색 완료: 총 {count}개 메시에 RGB(51, 38, 31) 갈색 적용됨!");
    }

    // ==========================================
    // 5. 층 / 방 배치 생성을 위한 절차적 빌더
    // ==========================================
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
    }

    static void BuildGuestRoomSlot(Transform parent, string name, Vector3 slotPosition, float yRotation)
    {
        var slot = new GameObject(name);
        slot.transform.SetParent(parent);
        slot.transform.localPosition = slotPosition;
        slot.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        Undo.RegisterCreatedObjectUndo(slot, "Build Floor");

        BuildRoomShell(slot.transform, RoomWidth, RoomLength, includeBathroom: true, doorCenterX: RoomWidth - 1.1f,
            roomLabel: name.Replace("Room_", ""));
    }

    static void BuildUtilityRoomSlot(Transform parent, string name, float westEdgeX, float width)
    {
        var slot = new GameObject(name);
        slot.transform.SetParent(parent);
        slot.transform.localPosition = new Vector3(westEdgeX + width, 0f, 0f);
        slot.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        Undo.RegisterCreatedObjectUndo(slot, "Build Floor");

        BuildRoomShell(slot.transform, width, RoomLength, includeBathroom: false, doorCenterX: width / 2f);
    }

    static void BuildOpenBay(Transform parent, string name, float westEdgeX, float width, bool north, bool includeOuterWall)
    {
        var slot = new GameObject(name);
        slot.transform.SetParent(parent);
        float zStart = north ? CorridorDepth : -RoomLength;
        slot.transform.localPosition = new Vector3(westEdgeX, 0f, zStart);
        Undo.RegisterCreatedObjectUndo(slot, "Build Floor");

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
        Undo.RegisterCreatedObjectUndo(slot, "Build Floor");

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
        // --- 1. 욕조 및 커튼 일체형 에셋 로드 ---
        string bathPrefabPath = "Assets/3rdParty/bath/Bathtub.blend";
        GameObject bathPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(bathPrefabPath);

        if (bathPrefab == null)
        {
            // .fbx 파일일 경우 대비 예외 처리
            bathPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/3rdParty/bath/Bathtub.fbx");
        }

        if (bathPrefab != null)
        {
            GameObject tubObj = PrefabUtility.InstantiatePrefab(bathPrefab, bathroom) as GameObject;
            tubObj.name = "Bathtub";

            // 스크린샷 인스펙터 수치 반영 (Rotation X: -90, Y: -90, Scale: 1)
            tubObj.transform.localPosition = new Vector3(TubCenterX, 0f, TubCenterZ);
            tubObj.transform.localRotation = Quaternion.Euler(-90f, 90f, 0f);
            tubObj.transform.localScale = new Vector3(1f, 1.7f, 1.1f);

            Undo.RegisterCreatedObjectUndo(tubObj, "Build Hotel Blockout");
        }
        else
        {
            // 예외 처리 (에셋 미로드 시 기존 블록아웃 대체)
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
        }

        // --- 2. 세면대 에셋 로드 ---
        string sinkPrefabPath = "Assets/3rdParty/Sink/Vessel_Sink_with_Armature.prefab";
        GameObject sinkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(sinkPrefabPath);

        if (sinkPrefab == null)
        {
            sinkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/3rdParty/Sink/Vessel_Sink_with_Armature.fbx");
            if (sinkPrefab == null)
            {
                sinkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/3rdParty/Sink/Vessel_Sink_with_Armature.blend");
            }
        }

        if (sinkPrefab != null)
        {
            GameObject sinkObj = PrefabUtility.InstantiatePrefab(sinkPrefab, bathroom) as GameObject;
            sinkObj.name = "Vessel_Sink";

            // 스크린샷 인스펙터 수치 반영 (Rotation 0, Scale 1)
            sinkObj.transform.localPosition = new Vector3(SinkCenterX, -0.2f, SinkCenterZ);
            sinkObj.transform.localRotation = Quaternion.Euler(0f, -180f, 0f);
            sinkObj.transform.localScale = Vector3.one;

            Undo.RegisterCreatedObjectUndo(sinkObj, "Build Hotel Blockout");
        }
        else
        {
            // 예외 처리 (에셋 미로드 시 기존 블록아웃 대체)
            MakeCylinder("Sink_Pedestal", bathroom,
                new Vector3(SinkCenterX, 0.325f, SinkCenterZ), 0.12f, 0.65f);
            MakeCylinder("Sink_Basin", bathroom,
                new Vector3(SinkCenterX, 0.68f, SinkCenterZ), Mathf.Min(SinkWidth, SinkDepth) * 0.9f, 0.12f);
            MakeBox("Sink_Backsplash", bathroom,
                new Vector3(0.15f, 0.75f, SinkCenterZ),
                new Vector3(0.05f, 0.15f, SinkDepth));
            MakeCylinder("Sink_Faucet", bathroom,
                new Vector3(0.3f, 0.85f, SinkCenterZ), 0.03f, 0.15f);
        }

        // --- 3. 거울 에셋 로드 ---
        string mirrorPrefabPath = "Assets/3rdParty/Mirror/Mirror.blend";
        GameObject mirrorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(mirrorPrefabPath);

        if (mirrorPrefab == null)
        {
            mirrorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/3rdParty/Mirror/Mirror.fbx");
        }

        if (mirrorPrefab != null)
        {
            GameObject mirrorObj = PrefabUtility.InstantiatePrefab(mirrorPrefab, bathroom) as GameObject;
            mirrorObj.name = "Mirror";

            // 스크린샷 인스펙터 수치 반영 (Rotation Y: -90, Scale: 1)
            mirrorObj.transform.localPosition = new Vector3(1.05f, MirrorElevation, SinkCenterZ);
            mirrorObj.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            mirrorObj.transform.localScale = Vector3.one;

            Undo.RegisterCreatedObjectUndo(mirrorObj, "Build Hotel Blockout");
        }
        else
        {
            // 예외 처리 (에셋 미로드 시 기존 블록아웃 대체)
            MakeBox("Mirror", bathroom, 
                new Vector3(0.03f, MirrorElevation + MirrorHeight / 2f, SinkCenterZ),
                new Vector3(0.06f, MirrorHeight, SinkDepth));
            MakeBox("Mirror_Frame_Top", bathroom,
                new Vector3(0.05f, MirrorElevation + MirrorHeight + 0.02f, SinkCenterZ),
                new Vector3(0.1f, 0.04f, SinkDepth + 0.06f));
            MakeBox("Mirror_Frame_Bottom", bathroom,
                new Vector3(0.05f, MirrorElevation - 0.02f, SinkCenterZ),
                new Vector3(0.1f, 0.04f, SinkDepth + 0.06f));
        }

        // --- 4. 변기 에셋 로드 ---
        string toiletPrefabPath = "Assets/3rdParty/Toilet/Toilet.prefab";
        GameObject toiletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(toiletPrefabPath);

        if (toiletPrefab == null)
        {
            toiletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/3rdParty/Toilet/Toilet.fbx");
        }

        if (toiletPrefab != null)
        {
            GameObject toiletObj = PrefabUtility.InstantiatePrefab(toiletPrefab, bathroom) as GameObject;
            toiletObj.name = "Toilet";

            toiletObj.transform.localPosition = new Vector3(ToiletCenterX, 0f, ToiletCenterZ);
            toiletObj.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            toiletObj.transform.localScale = Vector3.one;

            Undo.RegisterCreatedObjectUndo(toiletObj, "Build Hotel Blockout");
        }
        else
        {
            // 예외 처리 (에셋 미로드 시 기존 블록아웃 대체)
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
    }

    static void BuildRoomFurniture(Transform parent)
    {
        var furniture = new GameObject("Furniture");
        furniture.transform.SetParent(parent, false);

        BuildWardrobe(furniture.transform);
        BuildNightstandAndPhone(furniture.transform);
        BuildArmchair(furniture.transform, "Sofa_1", Sofa1CenterZ);
        BuildArmchair(furniture.transform, "Sofa_2", Sofa2CenterZ);

        // --- 사이드 테이블 (Table_01 프리팹 로드) ---
        string tablePrefabPath = "Assets/3rdParty/MedievalTavernPack/Prefabs/Furniture/Table_01.prefab";
        GameObject tablePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(tablePrefabPath);

        if (tablePrefab != null)
        {
            GameObject sideTable = PrefabUtility.InstantiatePrefab(tablePrefab, furniture.transform) as GameObject;
            sideTable.name = "Side_Table";
            
            // 위치 지정 (바닥 기준 Y=0f)
            sideTable.transform.localPosition = new Vector3(SeatingCenterX, 0f, TableCenterZ);
            sideTable.transform.localRotation = Quaternion.identity;
            
            // 객실 사이드 테이블 크기에 맞춰 스케일 조절 (필요시 수치 조절)
            sideTable.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

            Undo.RegisterCreatedObjectUndo(sideTable, "Build Hotel Blockout");
        }
        else
        {
            // 예외 처리 (에셋을 못 찾을 경우 기존 블록아웃 생성)
            MakeCylinder("Table_Leg_FL", furniture.transform, new Vector3(SeatingCenterX - TableWidth / 2f + 0.05f, (TableHeight - 0.05f) / 2f, TableCenterZ - TableDepth / 2f + 0.05f), 0.04f, TableHeight - 0.05f);
            MakeCylinder("Table_Leg_FR", furniture.transform, new Vector3(SeatingCenterX + TableWidth / 2f - 0.05f, (TableHeight - 0.05f) / 2f, TableCenterZ - TableDepth / 2f + 0.05f), 0.04f, TableHeight - 0.05f);
            MakeCylinder("Table_Leg_BL", furniture.transform, new Vector3(SeatingCenterX - TableWidth / 2f + 0.05f, (TableHeight - 0.05f) / 2f, TableCenterZ + TableDepth / 2f - 0.05f), 0.04f, TableHeight - 0.05f);
            MakeCylinder("Table_Leg_BR", furniture.transform, new Vector3(SeatingCenterX + TableWidth / 2f - 0.05f, (TableHeight - 0.05f) / 2f, TableCenterZ + TableDepth / 2f - 0.05f), 0.04f, TableHeight - 0.05f);
            MakeBox("Side_Table_Top", furniture.transform,
                new Vector3(SeatingCenterX, TableHeight - 0.025f, TableCenterZ),
                new Vector3(TableWidth, 0.05f, TableDepth));
        }

    }

    static void BuildRoomLighting(Transform parent, float width, float length)
    {
        var lighting = new GameObject("Lighting");
        lighting.transform.SetParent(parent, false);

        float centerX = width / 2f;
        float centerZ = length / 2f;

        // --- 천장 샹들리에 조명 에셋 로드 ---
        string ceilingLampPath = "Assets/3rdParty/Celling_lamp/source/lamp_19.fbx";
        GameObject ceilingLampPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ceilingLampPath);

        if (ceilingLampPrefab == null)
        {
            // .fbx로 못 찾을 경우 .blend 시도
            ceilingLampPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/3rdParty/Ceiling_lamp/source/lamp_19.blend");
        }

        if (ceilingLampPrefab != null)
        {
            GameObject ceilingFixture = PrefabUtility.InstantiatePrefab(ceilingLampPrefab, lighting.transform) as GameObject;
            ceilingFixture.name = "Ceiling_Fixture";
            
            // 피벗(중심점)이 천장 고정부에 위치해 있으므로 Y축을 CeilingHeight(2.5m)에 맞춤
            ceilingFixture.transform.localPosition = new Vector3(centerX, CeilingHeight, centerZ);
            ceilingFixture.transform.localRotation = Quaternion.identity;
            ceilingFixture.transform.localScale = Vector3.one;

            Undo.RegisterCreatedObjectUndo(ceilingFixture, "Build Hotel Blockout");
        }
        else
        {
            // 예외 처리 (에셋을 찾지 못할 경우 기존 실린더 대체)
            MakeCylinder("Ceiling_Fixture", lighting.transform, new Vector3(centerX, CeilingHeight - 0.06f, centerZ), 0.4f, 0.1f);
        }

        // 샹들리에 하단 전구 높이에 맞춰 포인트 라이트 위치 조정 (Y: CeilingHeight - 0.8f)
        BuildPointLight(lighting.transform, "Ceiling_Light", new Vector3(centerX, CeilingHeight - 0.8f, centerZ),
            new Color(1f, 0.82f, 0.6f), 2.0f, 7f);

        // 스탠드 조명 생성
        BuildLamp(lighting.transform, "Nightstand_Lamp", NightstandCenterX, NightstandHeight, NightstandCenterZ - 0.25f);
        BuildLamp(lighting.transform, "SideTable_Lamp", SeatingCenterX, TableHeight, TableCenterZ);
    }

    static void BuildLamp(Transform parent, string name, float x, float surfaceY, float z)
    {
        // 3D 램프 모델 경로 지정
        string lampPrefabPath = "Assets/3rdParty/Lamp/source/LampTurn.blend";
        GameObject lampPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(lampPrefabPath);

        if (lampPrefab != null)
        {
            GameObject lampObj = PrefabUtility.InstantiatePrefab(lampPrefab, parent) as GameObject;
            lampObj.name = name;
            
            // 스크린샷 인스펙터 값 반영 (Position, Rotation X: -90, Scale: 0.1)
            lampObj.transform.localPosition = new Vector3(x, surfaceY, z);
            lampObj.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            lampObj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

            // 은은한 램프 불빛 추가 (자식 오브젝트로 생성)
            BuildPointLight(lampObj.transform, "Lamp_Light", new Vector3(0f, 1.8f, 0f),
                new Color(1f, 0.78f, 0.52f), 1.2f, 3.5f);

            Undo.RegisterCreatedObjectUndo(lampObj, "Build Hotel Blockout");
        }
        else
        {
            // 예외 처리 (에셋 미로드 시 기존 블록아웃 대체)
            var lamp = new GameObject(name);
            lamp.transform.SetParent(parent, false);

            MakeCylinder("Base", lamp.transform, new Vector3(x, surfaceY + 0.02f, z), 0.12f, 0.04f);
            MakeCylinder("Pole", lamp.transform, new Vector3(x, surfaceY + 0.16f, z), 0.03f, 0.24f);
            MakeCylinder("Shade", lamp.transform, new Vector3(x, surfaceY + 0.34f, z), 0.22f, 0.16f);

            BuildPointLight(lamp.transform, "Lamp_Light", new Vector3(x, surfaceY + 0.32f, z),
                new Color(1f, 0.78f, 0.52f), 0.7f, 2.8f);
        }
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
        // 옷장 프리팹 경로 지정
        string closetPrefabPath = "Assets/3rdParty/Furniture/Prefabs/BigCloset.prefab";
        GameObject closetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(closetPrefabPath);

        if (closetPrefab != null)
        {
            GameObject wardrobe = PrefabUtility.InstantiatePrefab(closetPrefab, parent) as GameObject;
            wardrobe.name = "Wardrobe";
            
            // 위치 배치 (바닥 기준 Y=0f)
            wardrobe.transform.localPosition = new Vector3(WardrobeCenterX, 0f, WardrobeCenterZ);
            wardrobe.transform.localRotation = Quaternion.identity;
            wardrobe.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            Undo.RegisterCreatedObjectUndo(wardrobe, "Build Hotel Blockout");
        }
        else
        {
            // 예외 처리 (에셋을 찾지 못할 경우 기존 블록아웃 생성)
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
    }

    static void BuildNightstandAndPhone(Transform parent)
    {
        var nightstand = new GameObject("Nightstand_Phone");
        nightstand.transform.SetParent(parent, false);

        // 협탁(SmallCloset) 프리팹 로드
        string nightstandPrefabPath = "Assets/3rdParty/Furniture/Prefabs/SmallCloset.prefab";
        GameObject nightstandPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(nightstandPrefabPath);

        if (nightstandPrefab != null)
        {
            GameObject standObj = PrefabUtility.InstantiatePrefab(nightstandPrefab, nightstand.transform) as GameObject;
            standObj.name = "Nightstand_Model";
            
            // 바닥 기준 배치
            standObj.transform.localPosition = new Vector3(NightstandCenterX, 0f, NightstandCenterZ);
            standObj.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            standObj.transform.localScale = new Vector3(1f, 0.7f, 1.2f);

            Undo.RegisterCreatedObjectUndo(standObj, "Build Hotel Blockout");
        }
        else
        {
            // 예외 처리 (에셋 미로드 시 기존 블록아웃 대체)
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
        }

        // 협탁 위 전화기
        string phonePrefabPath = "Assets/3rdParty/VintageTelephone/VintageTelephone.obj";
        GameObject phonePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(phonePrefabPath);

        if (phonePrefab != null)
        {
            GameObject phoneObj = PrefabUtility.InstantiatePrefab(phonePrefab, nightstand.transform) as GameObject;
            phoneObj.name = "Vintage_Telephone";
            
            // 협탁 상단 표면 높이(약 0.65m) 및 인스펙터 스케일(0.5) 반영
            phoneObj.transform.localPosition = new Vector3(NightstandCenterX, 0.72f, NightstandCenterZ + 0.2f);
            phoneObj.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            phoneObj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

            Undo.RegisterCreatedObjectUndo(phoneObj, "Build Hotel Blockout");
        }
        else
        {
            // 에셋 미로드 시 예외 처리 (기존 블록아웃)
            MakeBox("Phone_Base", nightstand.transform,
                new Vector3(NightstandCenterX, NightstandHeight + 0.03f, NightstandCenterZ),
                new Vector3(0.28f, 0.06f, 0.2f));
            MakeBox("Phone_Handset", nightstand.transform,
                new Vector3(NightstandCenterX, NightstandHeight + 0.09f, NightstandCenterZ),
                new Vector3(0.22f, 0.05f, 0.09f));
        }
    }

    static void BuildArmchair(Transform parent, string name, float centerZ)
    {
        var source = AssetDatabase.LoadAssetAtPath<GameObject>(ArmchairSourcePath);
        if (source == null)
        {
            source = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/3rdParty/Furniture/Prefabs/Fotel3.fbx");
        }

        if (source == null)
        {
            Debug.LogError($"안락의자 에셋을 찾을 수 없습니다: {ArmchairSourcePath}");
            return;
        }

        var chairInstance = (GameObject)PrefabUtility.InstantiatePrefab(source, parent);
        chairInstance.name = name;

        Vector3 chairPos = new Vector3(SeatingCenterX, 0f, centerZ);
        chairInstance.transform.localPosition = chairPos;

        // 1. TV 중심 위치(TvCenterX, 0, TvCenterZ)를 바라보도록 회전 설정
        Vector3 tvPos = new Vector3(TvCenterX, 0f, TvCenterZ);
        Vector3 lookDir = (tvPos - chairPos).normalized;
        if (lookDir != Vector3.zero)
        {
            chairInstance.transform.localRotation = Quaternion.LookRotation(lookDir, Vector3.up);
        }

        // 2. 스케일(크기) 0.7로 변경
        chairInstance.transform.localScale = Vector3.one * 0.7f;

        Undo.RegisterCreatedObjectUndo(chairInstance, "Build Hotel Blockout");
    }

    static void BuildBed(Transform parent, float width, float length)
    {
        var bedSource = AssetDatabase.LoadAssetAtPath<GameObject>(BedSourcePath);
        if (bedSource == null)
        {
            Debug.LogError($"침대 에셋을 찾을 수 없습니다: {BedSourcePath}");
            return;
        }

        var bedWrapper = new GameObject("Bed");
        bedWrapper.transform.SetParent(parent, false);

        // 침대가 들어갈 위치 중앙값 계산
        float bedX0 = 0f;
        float bedZ0 = BathLength + BedClearance;
        Vector3 targetFootprintCenter = new Vector3(bedX0 + BedWidth / 2f, 0f, bedZ0 + BedDepth / 2f);

        // 프리팹 직접 생성 및 정렬 배치
        PlaceBedInstance(bedSource, bedWrapper.transform, targetFootprintCenter);

        Undo.RegisterCreatedObjectUndo(bedWrapper, "Build Hotel Blockout");
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


    // ==========================================
    // 6. 통합 메인 빌더용 내부 후처리 메서드들
    // ==========================================
    static void AddTvAndStandFromRoom304Reference()
    {
        var tvSource = AssetDatabase.LoadAssetAtPath<GameObject>(TvSourcePath);
        var standSource = AssetDatabase.LoadAssetAtPath<GameObject>(TvStandSourcePath);
        if (tvSource == null || standSource == null)
            return;

        int added = 0;
        for (int floorNumber = 1; floorNumber <= 5; floorNumber++)
        {
            for (int d = 1; d <= 5; d++)
            {
                string roomName = "Room_" + (floorNumber * 100 + d);
                var roomGo = GameObject.Find(roomName);
                if (roomGo == null) continue;

                var furniture = roomGo.transform.Find("Furniture");
                if (furniture == null)
                {
                    var furnitureGo = new GameObject("Furniture");
                    furnitureGo.transform.SetParent(roomGo.transform, false);
                    Undo.RegisterCreatedObjectUndo(furnitureGo, "Add TV+Stand");
                    furniture = furnitureGo.transform;
                }

                bool hasTv = furniture.Find("TV") != null;
                bool hasStand = furniture.Find("Moble tv roma turri") != null;
                if (hasTv && hasStand) continue;

                if (!hasStand)
                {
                    var standInstance = (GameObject)PrefabUtility.InstantiatePrefab(standSource, furniture);
                    standInstance.name = "Moble tv roma turri";
                    standInstance.transform.localPosition = Room304TvStandLocalPos;
                    standInstance.transform.localRotation = Room304TvStandLocalRot;
                    standInstance.transform.localScale = Vector3.one;
                    Undo.RegisterCreatedObjectUndo(standInstance, "Add TV+Stand");
                }

                if (!hasTv)
                {
                    var tvInstance = (GameObject)PrefabUtility.InstantiatePrefab(tvSource, furniture);
                    tvInstance.name = "TV";
                    tvInstance.transform.localPosition = Room304TvLocalPos;
                    tvInstance.transform.localRotation = Room304TvLocalRot;
                    tvInstance.transform.localScale = Vector3.one;
                    Undo.RegisterCreatedObjectUndo(tvInstance, "Add TV+Stand");
                }
                added++;
            }
        }
        Debug.Log($"TV 및 스탠드 {added}개 방에 세팅 완료.");
    }

    static void RemoveTvPlaceholderBoxes()
    {
        for (int floorNumber = 1; floorNumber <= 5; floorNumber++)
        {
            for (int d = 1; d <= 5; d++)
            {
                string roomName = "Room_" + (floorNumber * 100 + d);
                var roomGo = GameObject.Find(roomName);
                if (roomGo == null) continue;

                var furniture = roomGo.transform.Find("Furniture");
                if (furniture == null) continue;

                var shelf = furniture.Find("TV_Shelf");
                if (shelf != null) Undo.DestroyObjectImmediate(shelf.gameObject);

                var screen = furniture.Find("TV_Screen");
                if (screen != null) Undo.DestroyObjectImmediate(screen.gameObject);
            }
        }
    }

    static void ApplyCarpetToRoomFloors()
    {
        var carpetMat = PrepareCarpetMaterial();
        if (carpetMat == null) return;

        for (int floorNumber = 1; floorNumber <= 5; floorNumber++)
        {
            for (int d = 1; d <= 5; d++)
            {
                string roomName = "Room_" + (floorNumber * 100 + d);
                var roomGo = GameObject.Find(roomName);
                if (roomGo == null) continue;

                var floor = roomGo.transform.Find("Floor");
                var renderer = floor != null ? floor.GetComponent<MeshRenderer>() : null;
                if (renderer == null) continue;

                Undo.RecordObject(renderer, "Apply Carpet To Room Floors");
                renderer.sharedMaterial = carpetMat;
            }
        }
    }

    static void RemoveRoomFloors()
    {
        for (int floorNumber = 1; floorNumber <= 5; floorNumber++)
        {
            for (int d = 1; d <= 5; d++)
            {
                string roomName = "Room_" + (floorNumber * 100 + d);
                var roomGo = GameObject.Find(roomName);
                if (roomGo == null) continue;

                var floor = roomGo.transform.Find("Floor");
                if (floor == null) continue;

                Undo.DestroyObjectImmediate(floor.gameObject);
            }
        }
    }

    static void WireBedTextures()
    {
        string albedoPath = BedTexDir + "bed_basecolor.png";
        string normalPath = BedTexDir + "bed_normal.png";
        string combinedMrPath = BedTexDir + "bed_metallic_roughness.png";

        var albedoTex = AssetDatabase.LoadAssetAtPath<Texture2D>(albedoPath);
        if (albedoTex == null) return;

        SetTextureType(normalPath, TextureImporterType.NormalMap);
        var normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath);
        var packed = GetOrCreatePackedTextureFromCombinedMR(combinedMrPath, BedTexDir + "Bed_MetallicSmoothness.png");

        var mat = GetOrCreateCarpetMaterial(BedMatPath);
        mat.mainTexture = albedoTex;
        if (normalTex != null)
        {
            mat.SetTexture("_BumpMap", normalTex);
            mat.EnableKeyword("_NORMALMAP");
        }
        if (packed != null)
        {
            mat.SetTexture("_MetallicGlossMap", packed);
            mat.SetFloat("_Metallic", 1f);
            mat.SetFloat("_GlossMapScale", 1f);
            mat.EnableKeyword("_METALLICGLOSSMAP");
        }
        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();

        var importer = AssetImporter.GetAtPath(BedFbxPath) as ModelImporter;
        if (importer != null)
        {
            var id = new AssetImporter.SourceAssetIdentifier(typeof(Material), BedFbxMaterialName);
            importer.AddRemap(id, mat);
            importer.SaveAndReimport();
        }
    }

    static void ReplaceBedsWithModel()
    {
        var bedSource = AssetDatabase.LoadAssetAtPath<GameObject>(BedSourcePath);
        if (bedSource == null) return;

        float bedX0 = 0f;
        float bedZ0 = BathLength + BedClearance;
        Vector3 targetFootprintCenter = new Vector3(bedX0 + BedWidth / 2f, 0f, bedZ0 + BedDepth / 2f);

        for (int floorNumber = 1; floorNumber <= 5; floorNumber++)
        {
            for (int d = 1; d <= 5; d++)
            {
                string roomName = "Room_" + (floorNumber * 100 + d);
                var roomGo = GameObject.Find(roomName);
                if (roomGo == null) continue;

                var oldBed = roomGo.transform.Find("Bed");
                if (oldBed == null) continue;

                Undo.DestroyObjectImmediate(oldBed.gameObject);

                var bedWrapper = new GameObject("Bed");
                bedWrapper.transform.SetParent(roomGo.transform, false);
                Undo.RegisterCreatedObjectUndo(bedWrapper, "Replace Beds With Model");

                PlaceBedInstance(bedSource, bedWrapper.transform, targetFootprintCenter);
            }
        }
    }

    static void RemoveBedLightAndCamera()
    {
        var importer = AssetImporter.GetAtPath(BedSourcePath) as ModelImporter;
        if (importer == null) return;

        importer.importCameras = false;
        importer.importLights = false;
        importer.SaveAndReimport();
    }


    // ==========================================
    // 7. 유틸리티 및 텍스처/머티리얼 헬퍼 메서드
    // ==========================================
    static GameObject[] FindAllFloorRoots()
    {
        var targets = new List<GameObject>();
        for (int floorNumber = 1; floorNumber <= 5; floorNumber++)
        {
            var go = GameObject.Find($"Floor{floorNumber}_Hotel");
            if (go != null)
                targets.Add(go);
        }
        return targets.ToArray();
    }

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

    static Material PrepareCarpetMaterial()
    {
        string colorPath = CarpetDir + "fabric_0012_color_1k.jpg";
        string normalPath = CarpetDir + "fabric_0012_normal_opengl_1k.png";
        string aoPath = CarpetDir + "fabric_0012_ao_1k.jpg";
        string heightPath = CarpetDir + "fabric_0012_height_1k.png";

        var colorTex = AssetDatabase.LoadAssetAtPath<Texture2D>(colorPath);
        if (colorTex == null) return null;

        SetTextureType(normalPath, TextureImporterType.NormalMap);
        SetTextureLinear(aoPath);

        var mat = GetOrCreateCarpetMaterial(CarpetMatPath);
        mat.mainTexture = colorTex;
        mat.mainTextureScale = CarpetTiling;
        mat.SetFloat("_Metallic", 0f);
        mat.SetFloat("_Glossiness", 0.08f);

        var normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath);
        if (normalTex != null)
        {
            mat.SetTexture("_BumpMap", normalTex);
            mat.SetTextureScale("_BumpMap", CarpetTiling);
            mat.EnableKeyword("_NORMALMAP");
        }

        var aoTex = AssetDatabase.LoadAssetAtPath<Texture2D>(aoPath);
        if (aoTex != null)
        {
            mat.SetTexture("_OcclusionMap", aoTex);
            mat.SetTextureScale("_OcclusionMap", CarpetTiling);
        }

        var heightTex = AssetDatabase.LoadAssetAtPath<Texture2D>(heightPath);
        if (heightTex != null)
        {
            mat.SetTexture("_ParallaxMap", heightTex);
            mat.SetTextureScale("_ParallaxMap", CarpetTiling);
            mat.SetFloat("_Parallax", 0.02f);
            mat.EnableKeyword("_PARALLAXMAP");
        }

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        return mat;
    }

    static Material GetOrCreateCarpetMaterial(string path)
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(mat, path);
        }
        return mat;
    }

    static void SetTextureType(string path, TextureImporterType type)
    {
        if (AssetImporter.GetAtPath(path) is not TextureImporter importer || importer.textureType == type)
            return;
        importer.textureType = type;
        importer.SaveAndReimport();
    }

    static void SetTextureLinear(string path)
    {
        if (AssetImporter.GetAtPath(path) is not TextureImporter importer || !importer.sRGBTexture)
            return;
        importer.sRGBTexture = false;
        importer.SaveAndReimport();
    }

    static Texture2D GetOrCreatePackedTextureFromCombinedMR(string combinedPath, string outputPath)
    {
        if (!System.IO.File.Exists(outputPath))
        {
            SetTextureReadable(combinedPath);
            SetTextureLinear(combinedPath);
            var srcTex = AssetDatabase.LoadAssetAtPath<Texture2D>(combinedPath);
            if (srcTex == null) return null;

            int w = srcTex.width, h = srcTex.height;
            var packed = new Texture2D(w, h, TextureFormat.RGBA32, false);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Color c = srcTex.GetPixel(x, y);
                    float roughness = c.g;
                    float metallic = c.b;
                    packed.SetPixel(x, y, new Color(metallic, metallic, metallic, 1f - roughness));
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

    static void SetTextureReadable(string path)
    {
        if (AssetImporter.GetAtPath(path) is not TextureImporter importer || importer.isReadable)
            return;
        importer.isReadable = true;
        importer.SaveAndReimport();
    }

    static void PlaceBedInstance(GameObject bedSource, Transform parent, Vector3 targetFootprintCenter)
    {
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(bedSource, parent);
        instance.name = "Bed_Model";
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        var rawBounds = ComputeWorldBounds(instance);
        float sizeX = Mathf.Max(rawBounds.size.x, 0.001f);
        float sizeZ = Mathf.Max(rawBounds.size.z, 0.001f);

        bool needsRotation = sizeZ > sizeX;
        instance.transform.localRotation = needsRotation ? Quaternion.Euler(0f, 90f, 0f) : Quaternion.identity;
        instance.transform.localScale = Vector3.one * BedModelScale;

        var bounds = ComputeWorldBounds(instance);
        Vector3 localCenter = parent.InverseTransformPoint(bounds.center);
        float localMinY = parent.InverseTransformPoint(new Vector3(bounds.center.x, bounds.min.y, bounds.center.z)).y;

        instance.transform.localPosition = new Vector3(
            targetFootprintCenter.x - localCenter.x,
            targetFootprintCenter.y - localMinY,
            targetFootprintCenter.z - localCenter.z);

        Undo.RegisterCreatedObjectUndo(instance, "Replace Beds With Model");
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