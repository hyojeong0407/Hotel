using UnityEditor;
using UnityEngine;

public class ExteriorMaterialApplier : MonoBehaviour
{
    [MenuItem("Tools/Hotel Blockout/Apply Dark Materials (Eye Comfort)")]
    public static void ApplyMaterials()
    {
        GameObject[] targets = Selection.gameObjects;
        if (targets.Length == 0)
        {
            Debug.LogWarning("먼저 색을 칠할 오브젝트들을 하이어라키에서 선택해 주세요! (Shift/Ctrl 클릭으로 다중 선택 가능)");
            return;
        }

        // 1. 기본 건물용 무광 재질
        Material darkWallMat = new Material(Shader.Find("Standard"));
        darkWallMat.color = new Color(0.2f, 0.15f, 0.12f);
        darkWallMat.SetFloat("_Glossiness", 0.0f); 

        Material floorMat = new Material(Shader.Find("Standard"));
        floorMat.color = new Color(0.1f, 0.1f, 0.1f);
        floorMat.SetFloat("_Glossiness", 0.0f);

        Material roofMat = new Material(Shader.Find("Standard"));
        roofMat.color = new Color(0.05f, 0.05f, 0.05f);
        roofMat.SetFloat("_Glossiness", 0.0f);

        // 2. 🛏️ 침대 전용 무광 재질
        Material mattressMat = new Material(Shader.Find("Standard"));
        mattressMat.color = new Color(0.35f, 0.15f, 0.15f); 
        mattressMat.SetFloat("_Glossiness", 0.0f);

        Material bedLegMat = new Material(Shader.Find("Standard"));
        bedLegMat.color = new Color(0.15f, 0.1f, 0.05f); 
        bedLegMat.SetFloat("_Glossiness", 0.0f);

        // 3. 🛁 화장실 벽면 전용 재질 (추가됨)
        Material bathWallMat = new Material(Shader.Find("Standard"));
        bathWallMat.color = new Color(0.18f, 0.2f, 0.22f); // 칙칙하고 차가운 청회색 타일 느낌
        bathWallMat.SetFloat("_Glossiness", 0.0f);

        // 4. 미분류 기본 재질
        Material defaultDarkMat = new Material(Shader.Find("Standard"));
        defaultDarkMat.color = new Color(0.12f, 0.12f, 0.12f);
        defaultDarkMat.SetFloat("_Glossiness", 0.0f);

        int count = 0;

        foreach (GameObject target in targets)
        {
            Undo.RegisterFullObjectHierarchyUndo(target, "Apply Dark Materials");

            MeshRenderer[] renderers = target.GetComponentsInChildren<MeshRenderer>();
            
            foreach (var r in renderers)
            {
                string objName = r.gameObject.name.ToLower();

                // ⭐️ 화장실 벽면(bath_wall_side, bath_wall_front)을 먼저 검사해서 도색 ⭐️
                if (objName.Contains("bath_wall"))
                {
                    r.sharedMaterial = bathWallMat;
                }
                else if (objName.Contains("wall") || objName.Contains("pillar"))
                {
                    r.sharedMaterial = darkWallMat;
                }
                else if (objName.Contains("floor") || objName.Contains("ground") || objName.Contains("stairs"))
                {
                    r.sharedMaterial = floorMat;
                }
                else if (objName.Contains("roof") || objName.Contains("ceiling"))
                {
                    r.sharedMaterial = roofMat;
                }
                else if (objName.Contains("mattress"))
                {
                    r.sharedMaterial = mattressMat;
                }
                else if (objName.Contains("leg"))
                {
                    r.sharedMaterial = bedLegMat;
                }
                else
                {
                    r.sharedMaterial = defaultDarkMat;
                }
                count++;
            }
        }

        Debug.Log($"총 {count}개의 블록 채색 완료! 화장실 벽면(청회색) 분리까지 완벽하게 적용되었습니다 🛁");
    }
}