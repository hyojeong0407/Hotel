using UnityEditor;
using UnityEngine;

public static class LobbyBarBuilder
{
    [MenuItem("Tools/Hotel Blockout/Build Luxury Horror Bar (1F)")]
    public static void BuildBar()
    {
        var existing = GameObject.Find("Horror_Lobby_Bar");
        if (existing != null) Object.DestroyImmediate(existing);
        
        GameObject barRoot = new GameObject("Horror_Lobby_Bar");
        Undo.RegisterCreatedObjectUndo(barRoot, "Build Horror Bar");

        // 1. 바 카운터 (첫 번째 코드의 웅장한 가로 8m 유지)
        GameObject counter = GameObject.CreatePrimitive(PrimitiveType.Cube);
        counter.name = "Luxury_Bar_Counter";
        counter.transform.SetParent(barRoot.transform);
        // 진열장(벽)으로부터 2m 앞으로 띄워서 배치
        counter.transform.localPosition = new Vector3(0, 0.55f, -2f); 
        counter.transform.localScale = new Vector3(8f, 1.1f, 1f);

        // 2. 바 스툴 6개 (첫 번째 코드 개수 유지)
        for (int i = 0; i < 6; i++)
        {
            GameObject stool = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stool.name = "Velvet_Stool_" + i;
            stool.transform.SetParent(barRoot.transform);
            
            // 2번, 4번 의자 쓰러뜨리기 연출
            if (i == 2 || i == 4) 
            {
                stool.transform.localPosition = new Vector3(-3f + i * 1.2f, 0.2f, -3.2f);
                stool.transform.localRotation = Quaternion.Euler(90f, 45f, 0f);
                stool.transform.localScale = new Vector3(0.5f, 0.4f, 0.5f);
            }
            else
            {
                stool.transform.localPosition = new Vector3(-3f + i * 1.2f, 0.4f, -3f);
                stool.transform.localScale = new Vector3(0.5f, 0.4f, 0.5f);
            }
        }

        // 3. 펜던트 조명 4개 (개수 유지, 2.5m 천장에 맞춰 높이만 조정)
        for (int i = 0; i < 4; i++)
        {
            GameObject lantern = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lantern.name = "Hanging_Lantern_" + i;
            lantern.transform.SetParent(barRoot.transform);
            lantern.transform.localPosition = new Vector3(-3f + i * 2f, 2.1f, -1.8f);
            lantern.transform.localScale = new Vector3(0.6f, 0.8f, 0.6f);

            Light light = lantern.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.6f, 0.05f, 0.05f); // 핏빛 조명
            light.intensity = 2f;
            light.range = 7f;
        }
        
        // 4. 주류 진열장 선반 (가로 8m 유지, 천장에 딱 맞춘 2.5m 높이)
        GameObject shelf = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shelf.name = "Liquor_Shelf";
        shelf.transform.SetParent(barRoot.transform);
        // 뒷벽에 딱 붙도록 로컬 Z축 조정
        shelf.transform.localPosition = new Vector3(0, 1.25f, -0.25f);
        shelf.transform.localScale = new Vector3(8f, 2.5f, 0.5f);

        // 5. 배치: 103호-102호 사이 공간(North Gap)의 가장 안쪽 벽으로 확 밀어넣기
        // Z: 12 (복도 폭 4m + 방 깊이 8m를 더한 지점이 가장 북쪽 끝 벽면입니다)
        barRoot.transform.position = new Vector3(19f, 0f, 12f);

        Selection.activeGameObject = barRoot;
        Debug.Log("가로 8m 대형 바(Bar)를 가장 안쪽 벽에 바짝 붙여 생성 완료!");
    }
}