using UnityEditor;
using UnityEngine;

public static class HotelBlockoutBuilder
{
    const float WallThickness = 0.2f;
    const float CeilingHeight = 2.7f;

    const float RoomWidth = 4.5f;   // X
    const float RoomLength = 6f;    // Z

    const float BathWidth = 2.2f;   // X, corner at (0,0)
    const float BathLength = 2.6f;  // Z

    const float EntryDoorWidth = 0.9f;
    const float EntryDoorCenterX = 3.4f;

    const float BathDoorWidth = 0.8f; // gap sits against the bathroom's inner corner

    [MenuItem("Tools/Hotel Blockout/Build Room 101")]
    public static void BuildRoom101()
    {
        var existing = GameObject.Find("Room_101");
        if (existing != null)
            Object.DestroyImmediate(existing);

        var root = new GameObject("Room_101");
        Undo.RegisterCreatedObjectUndo(root, "Build Room 101 Blockout");

        var floor = MakeBox("Floor", root.transform,
            new Vector3(RoomWidth / 2f, -WallThickness / 2f, RoomLength / 2f),
            new Vector3(RoomWidth, WallThickness, RoomLength));

        var ceiling = MakeBox("Ceiling", root.transform,
            new Vector3(RoomWidth / 2f, CeilingHeight + WallThickness / 2f, RoomLength / 2f),
            new Vector3(RoomWidth, WallThickness, RoomLength));

        // Back wall (Z = RoomLength), no openings
        MakeBox("Wall_Back", root.transform,
            new Vector3(RoomWidth / 2f, CeilingHeight / 2f, RoomLength + WallThickness / 2f),
            new Vector3(RoomWidth + WallThickness * 2f, CeilingHeight, WallThickness));

        // Left wall (X = 0)
        MakeBox("Wall_Left", root.transform,
            new Vector3(-WallThickness / 2f, CeilingHeight / 2f, RoomLength / 2f),
            new Vector3(WallThickness, CeilingHeight, RoomLength + WallThickness * 2f));

        // Right wall (X = RoomWidth)
        MakeBox("Wall_Right", root.transform,
            new Vector3(RoomWidth + WallThickness / 2f, CeilingHeight / 2f, RoomLength / 2f),
            new Vector3(WallThickness, CeilingHeight, RoomLength + WallThickness * 2f));

        // Front wall (Z = 0) with entry door gap
        float doorMinX = EntryDoorCenterX - EntryDoorWidth / 2f;
        float doorMaxX = EntryDoorCenterX + EntryDoorWidth / 2f;

        MakeBox("Wall_Front_Left", root.transform,
            new Vector3(doorMinX / 2f, CeilingHeight / 2f, -WallThickness / 2f),
            new Vector3(doorMinX, CeilingHeight, WallThickness));

        MakeBox("Wall_Front_Right", root.transform,
            new Vector3((doorMaxX + RoomWidth) / 2f, CeilingHeight / 2f, -WallThickness / 2f),
            new Vector3(RoomWidth - doorMaxX, CeilingHeight, WallThickness));

        // Bathroom block in the corner nearest the entrance
        var bathroom = new GameObject("Bathroom");
        bathroom.transform.SetParent(root.transform);

        // Partition separating bathroom from the entry hallway (runs along X = BathWidth)
        MakeBox("Bath_Wall_Side", bathroom.transform,
            new Vector3(BathWidth, CeilingHeight / 2f, BathLength / 2f),
            new Vector3(WallThickness, CeilingHeight, BathLength));

        // Partition separating bathroom from the bedroom (runs along Z = BathLength), with door gap
        float bathDoorMinX = BathWidth - BathDoorWidth;
        MakeBox("Bath_Wall_Front", bathroom.transform,
            new Vector3(bathDoorMinX / 2f, CeilingHeight / 2f, BathLength),
            new Vector3(bathDoorMinX, CeilingHeight, WallThickness));

        Selection.activeGameObject = root;
        SceneView.lastActiveSceneView?.FrameSelected();

        Debug.Log("Room_101 블록아웃 생성 완료: 4.5m x 6m, 천장 2.7m, 화장실 2.2m x 2.6m");
    }

    static GameObject MakeBox(string name, Transform parent, Vector3 position, Vector3 size)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent);
        go.transform.position = position;
        go.transform.localScale = size;
        Undo.RegisterCreatedObjectUndo(go, "Build Room 101 Blockout");
        return go;
    }
}
