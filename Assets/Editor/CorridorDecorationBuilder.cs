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

    [MenuItem("Tools/Hotel Blockout/Decorate Corridor - 1F (Lobby & Staff)")]
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

        GameObject staffRoom = new GameObject("Staff_Room_Decor");
        staffRoom.transform.SetParent(root.transform, false); 
        
        for (int i = 0; i < 5; i++)
        {
            MakeBlockout(staffRoom.transform, $"Locker_{i}", PrimitiveType.Cube, new Vector3(25f + i * 0.8f, 1f, -7.5f), new Vector3(0.7f, 2f, 0.6f), new Color(0.25f, 0.25f, 0.25f));
        }

        MakeBlockout(staffRoom.transform, "Break_Table", PrimitiveType.Cube, new Vector3(29f, 0.4f, -4f), new Vector3(2.5f, 0.8f, 1.5f), new Color(0.3f, 0.2f, 0.15f));
        
        // 💡 실린더 Y스케일 교정 (0.5 -> 0.25)
        MakeBlockout(staffRoom.transform, "Chair_1", PrimitiveType.Cylinder, new Vector3(28f, 0.25f, -3f), new Vector3(0.5f, 0.25f, 0.5f), Color.white);
        MakeBlockout(staffRoom.transform, "Chair_2", PrimitiveType.Cylinder, new Vector3(30f, 0.25f, -3f), new Vector3(0.5f, 0.25f, 0.5f), Color.white);
        MakeBlockout(staffRoom.transform, "Chair_3", PrimitiveType.Cylinder, new Vector3(29f, 0.25f, -5f), new Vector3(0.5f, 0.25f, 0.5f), Color.white);
        
        MakeBlockout(staffRoom.transform, "Cot_Bed", PrimitiveType.Cube, new Vector3(32.5f, 0.3f, -2f), new Vector3(1f, 0.4f, 2.5f), new Color(0.2f, 0.3f, 0.2f));

        MakeBlockout(staffRoom.transform, "Rulebook_Board", PrimitiveType.Cube, new Vector3(25f, 1.5f, -0.05f), new Vector3(2f, 1.2f, 0.1f), Color.white); 
        MakeBlockout(staffRoom.transform, "Status_Board", PrimitiveType.Cube, new Vector3(27.5f, 1.5f, -0.05f), new Vector3(1.5f, 1f, 0.1f), Color.gray); 

        AddLight(staffRoom.transform, new Vector3(29f, 2.4f, -4f), new Color(0.85f, 0.9f, 1f), 3.5f, 7f);

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
        
        // 💡 꽃병 Y위치 및 스케일 교정 (콘솔 테이블 위에 완벽하게 올라가도록)
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

        AddWelcomeMats(root.transform, "Elegant_Mat", new Color(0.5f, 0.1f, 0.1f));

        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), light2F, 4f, 8f);

        GameObject staffRoom = new GameObject("Staff_Room_Decor");
        staffRoom.transform.SetParent(root.transform, false);
        
        for (int i = 0; i < 5; i++)
        {
            MakeBlockout(staffRoom.transform, $"Wood_Cabinet_{i}", PrimitiveType.Cube, new Vector3(25f + i * 0.8f, 1f, -7.5f), new Vector3(0.7f, 2f, 0.6f), new Color(0.3f, 0.15f, 0.05f));
        }
        
        // 💡 실린더 Y스케일 교정 (테이블과 조각상 모두 바닥에 착 붙도록 수정됨)
        MakeBlockout(staffRoom.transform, "Elegant_TeaTable", PrimitiveType.Cylinder, new Vector3(29f, 0.4f, -4f), new Vector3(2f, 0.4f, 2f), Color.white);
        MakeBlockout(staffRoom.transform, "Armchair_1", PrimitiveType.Cube, new Vector3(27.5f, 0.5f, -4f), new Vector3(1f, 1f, 1f), Color.white);
        MakeBlockout(staffRoom.transform, "Armchair_2", PrimitiveType.Cube, new Vector3(30.5f, 0.5f, -4f), new Vector3(1f, 1f, 1f), Color.white);
        MakeBlockout(staffRoom.transform, "Statue", PrimitiveType.Cylinder, new Vector3(32.5f, 1f, -2f), new Vector3(0.8f, 1f, 0.8f), Color.white);
        
        AddLight(staffRoom.transform, new Vector3(29f, 2.4f, -4f), new Color(1f, 0.8f, 0.5f), 4f, 7f);

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

        AddWelcomeMats(root.transform, "Dirty_Mat", new Color(0.3f, 0.3f, 0.3f));
        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), light3F, 3f, 7f);

        GameObject staffRoom = new GameObject("Staff_Room_Decor");
        staffRoom.transform.SetParent(root.transform, false);
        
        MakeBlockout(staffRoom.transform, "Locker_Broken", PrimitiveType.Cube, new Vector3(25f, 1f, -7.5f), new Vector3(0.7f, 2f, 0.6f), Color.gray);
        GameObject fallenLocker = MakeBlockout(staffRoom.transform, "Locker_Fallen", PrimitiveType.Cube, new Vector3(26.5f, 0.3f, -6f), new Vector3(0.7f, 2f, 0.6f), Color.gray);
        fallenLocker.transform.localRotation = Quaternion.Euler(90f, 15f, 0f);
        MakeBlockout(staffRoom.transform, "Smashed_Table", PrimitiveType.Cube, new Vector3(29f, 0.2f, -4f), new Vector3(2.5f, 0.4f, 1.5f), new Color(0.2f, 0.2f, 0.2f));
        MakeBlockout(staffRoom.transform, "Trash_Pile", PrimitiveType.Sphere, new Vector3(29f, 0.3f, -4f), new Vector3(1.5f, 0.5f, 1.5f), new Color(0.1f, 0.1f, 0.1f));
        MakeBlockout(staffRoom.transform, "Creepy_Linen_Cart", PrimitiveType.Cube, new Vector3(32.5f, 0.8f, -2f), new Vector3(1.2f, 1.6f, 1.8f), Color.white);
        AddLight(staffRoom.transform, new Vector3(29f, 2.4f, -4f), new Color(0.5f, 0.6f, 0.6f), 3f, 7f);

        Debug.Log("3F 복도 장식 및 조명 세팅 완료!");
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
        
        // 💡 화로 및 양초 Y스케일 모두 교정
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

        AddWelcomeMats(root.transform, "Bloody_Mat", new Color(0.4f, 0f, 0f));
        AddLight(root.transform, new Vector3(10f, 2.4f, 2f), light5F, 5f, 8f);

        GameObject staffRoom = new GameObject("Staff_Room_Decor");
        staffRoom.transform.SetParent(root.transform, false);
        
        MakeBlockout(staffRoom.transform, "Trunk_Pile_1", PrimitiveType.Cube, new Vector3(26f, 0.5f, -7f), new Vector3(1.5f, 1f, 1f), Color.black);
        MakeBlockout(staffRoom.transform, "Trunk_Pile_2", PrimitiveType.Cube, new Vector3(26f, 1.2f, -7f), new Vector3(1f, 0.4f, 0.8f), Color.black);
        MakeBlockout(staffRoom.transform, "Bone_Pile", PrimitiveType.Sphere, new Vector3(28f, 0.3f, -7f), new Vector3(1.5f, 0.6f, 1.5f), Color.white);
        MakeBlockout(staffRoom.transform, "Sacrifice_Altar", PrimitiveType.Cube, new Vector3(29f, 0.6f, -4f), new Vector3(3f, 1.2f, 1.5f), new Color(0.1f, 0.1f, 0.1f));
        
        // 💡 제단 위 양초 Y스케일 교정 (제단 위에 들뜨거나 묻히지 않게)
        MakeBlockout(staffRoom.transform, "Altar_Candle_1", PrimitiveType.Cylinder, new Vector3(28f, 1.3f, -4f), new Vector3(0.1f, 0.1f, 0.1f), Color.white);
        MakeBlockout(staffRoom.transform, "Altar_Candle_2", PrimitiveType.Cylinder, new Vector3(30f, 1.3f, -4f), new Vector3(0.1f, 0.1f, 0.1f), Color.white);
        
        MakeBlockout(staffRoom.transform, "Tapestry", PrimitiveType.Cube, new Vector3(33.9f, 1.5f, -4f), new Vector3(0.1f, 2f, 3f), Color.white);
        AddLight(staffRoom.transform, new Vector3(29f, 2.4f, -4f), new Color(0.8f, 0.1f, 0.1f), 5f, 7f);

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
}