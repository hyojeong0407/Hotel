using UnityEditor;
using UnityEngine;

public static class HotelBlockoutBuilder
{
    const float WallThickness = 0.2f;
    const float CeilingHeight = 2.7f;

    const float RoomWidth = 4.5f;   // X, per-bay width (101/102/103/104/105/FS all share this)
    const float RoomLength = 6f;    // Z, guest room depth
    const float CorridorDepth = 3f; // Z, hallway between the north and south room rows
    const float ELWidth = 3f;       // X, elevator lobby strip on the west end

    const float BathWidth = 2.2f;
    const float BathLength = 2.6f;
    const float BathDoorWidth = 0.8f;

    const float DoorWidth = 0.9f;

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

    [MenuItem("Tools/Hotel Blockout/Build Floor 1 (Full Layout)")]
    public static void BuildFloor1()
    {
        ClearExisting("Floor1_Hotel");
        var floor = new GameObject("Floor1_Hotel");
        Undo.RegisterCreatedObjectUndo(floor, "Build Floor 1");

        var rooms = new GameObject("Rooms");
        rooms.transform.SetParent(floor.transform);
        var corridor = new GameObject("Corridor");
        corridor.transform.SetParent(floor.transform);
        var elevator = new GameObject("Elevator");
        elevator.transform.SetParent(floor.transform);

        // North row (103, 102, 101), doors face south into the corridor
        BuildGuestRoomSlot(rooms.transform, "Room_103", new Vector3(ELWidth, 0f, CorridorDepth), 0f);
        BuildGuestRoomSlot(rooms.transform, "Room_102", new Vector3(ELWidth + RoomWidth, 0f, CorridorDepth), 0f);
        BuildGuestRoomSlot(rooms.transform, "Room_101", new Vector3(ELWidth + RoomWidth * 2f, 0f, CorridorDepth), 0f);

        // South row (105, Front desk / Staff room, 104), doors face north into the corridor
        BuildGuestRoomSlot(rooms.transform, "Room_105", new Vector3(ELWidth + RoomWidth, 0f, 0f), 180f);
        BuildGuestRoomSlot(rooms.transform, "Room_104", new Vector3(ELWidth + RoomWidth * 3f, 0f, 0f), 180f);

        // The middle south bay (7.5m..12m, between 105 and 104) splits into Front desk (F, next to 105) and Staff room (S, next to 104)
        const float frontDeskWidth = 1.8f;
        float staffRoomWidth = RoomWidth - frontDeskWidth;
        float middleBayWestEdge = ELWidth + RoomWidth * 1f;
        BuildUtilityRoomSlot(rooms.transform, "Front_Desk", middleBayWestEdge, frontDeskWidth);
        BuildUtilityRoomSlot(rooms.transform, "Staff_Room", middleBayWestEdge + frontDeskWidth, staffRoomWidth);

        BuildCorridor(corridor.transform);
        BuildElevatorLobby(elevator.transform);

        FocusOn(floor);
        Debug.Log("Floor1_Hotel 생성 완료: EL 로비 + 복도 + 101~105 + 프론트/스태프룸.");
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

    // Local room space: X 0..width, Z 0..length, entry door on the Z=0 wall.
    static void BuildRoomShell(Transform parent, float width, float length, bool includeBathroom, float doorCenterX)
    {
        MakeBox("Floor", parent,
            new Vector3(width / 2f, -WallThickness / 2f, length / 2f),
            new Vector3(width, WallThickness, length));

        MakeBox("Ceiling", parent,
            new Vector3(width / 2f, CeilingHeight + WallThickness / 2f, length / 2f),
            new Vector3(width, WallThickness, length));

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
    }

    static void BuildCorridor(Transform parent)
    {
        float corridorWidth = RoomWidth * 3f;
        float startX = ELWidth;

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
        float southZ = -RoomLength;
        float northZ = CorridorDepth + RoomLength;
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
