using UnityEditor;
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

        var bathroom = new GameObject("Bathroom");
        bathroom.transform.SetParent(parent, false);

        MakeBox("Bath_Wall_Side", bathroom.transform,
            new Vector3(BathWidth, CeilingHeight / 2f, BathLength / 2f),
            new Vector3(WallThickness, CeilingHeight, BathLength));

        float bathDoorMinX = BathWidth - BathDoorWidth;
        MakeBox("Bath_Wall_Front", bathroom.transform,
            new Vector3(bathDoorMinX / 2f, CeilingHeight / 2f, BathLength),
            new Vector3(bathDoorMinX, CeilingHeight, WallThickness));

        BuildBathroomFixtures(bathroom.transform);
        BuildBed(parent, width, length);
        BuildRoomFurniture(parent);
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
    }

    static void BuildBathroomFixtures(Transform bathroom)
    {
        MakeBox("Tub", bathroom,
            new Vector3(TubCenterX, TubHeight / 2f, TubCenterZ),
            new Vector3(TubWidth, TubHeight, TubDepth));

        float curtainZ = TubCenterZ + TubDepth / 2f; // hangs along the tub's front (open) edge
        MakeBox("Shower_Curtain", bathroom,
            new Vector3(TubCenterX, CurtainHeight / 2f, curtainZ),
            new Vector3(TubWidth, CurtainHeight, 0.05f));

        MakeBox("Sink", bathroom,
            new Vector3(SinkCenterX, SinkHeight / 2f, SinkCenterZ),
            new Vector3(SinkWidth, SinkHeight, SinkDepth));

        MakeBox("Mirror", bathroom, // mounted on the west wall, above the sink
            new Vector3(0.03f, MirrorElevation + MirrorHeight / 2f, SinkCenterZ),
            new Vector3(0.06f, MirrorHeight, SinkDepth));

        MakeBox("Toilet", bathroom,
            new Vector3(ToiletCenterX, ToiletHeight / 2f, ToiletCenterZ),
            new Vector3(ToiletWidth, ToiletHeight, ToiletDepth));
    }

    static void BuildRoomFurniture(Transform parent)
    {
        var furniture = new GameObject("Furniture");
        furniture.transform.SetParent(parent, false);

        MakeBox("Wardrobe", furniture.transform,
            new Vector3(WardrobeCenterX, WardrobeHeight / 2f, WardrobeCenterZ),
            new Vector3(WardrobeSize, WardrobeHeight, WardrobeSize));

        var nightstand = new GameObject("Nightstand_Phone");
        nightstand.transform.SetParent(furniture.transform, false);
        MakeBox("Nightstand", nightstand.transform,
            new Vector3(NightstandCenterX, NightstandHeight / 2f, NightstandCenterZ),
            new Vector3(NightstandSize, NightstandHeight, NightstandSize));
        MakeBox("Phone", nightstand.transform,
            new Vector3(NightstandCenterX, NightstandHeight + 0.05f, NightstandCenterZ),
            new Vector3(0.3f, 0.1f, 0.2f));

        MakeBox("Sofa_1", furniture.transform,
            new Vector3(SeatingCenterX, SofaHeight / 2f, Sofa1CenterZ),
            new Vector3(SofaSize, SofaHeight, SofaSize));

        MakeBox("Side_Table", furniture.transform,
            new Vector3(SeatingCenterX, TableHeight / 2f, TableCenterZ),
            new Vector3(TableWidth, TableHeight, TableDepth));

        MakeBox("Sofa_2", furniture.transform,
            new Vector3(SeatingCenterX, SofaHeight / 2f, Sofa2CenterZ),
            new Vector3(SofaSize, SofaHeight, SofaSize));

        MakeBox("TV_Shelf", furniture.transform,
            new Vector3(TvCenterX, TvShelfElevation + TvShelfHeight / 2f, TvCenterZ),
            new Vector3(TvThickness, TvShelfHeight, TvSpan));
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
