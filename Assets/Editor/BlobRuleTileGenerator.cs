using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;

public class BlobRuleTileGenerator : Editor
{
    private static readonly int[] BitmaskTemplate = {
        0,   4,   92,  112, 28,  124, 116, 64,
        20,  84,  87,  221, 127, 255, 245, 80,
        29,  117, 85,  95,  247, 215, 209, 1,
        23,  213, 81,  31,  253, 125, 113, 16,
        21,  69,  93,  119, 223, 255, 241, 17,
        5,   68,  71,  193, 7,   199, 197, 65
    };

    [MenuItem("Assets/Create/Auto RuleTile (48 Blob)")]
    public static void CreateRuleTile()
    {
        Texture2D selectedTexture = Selection.activeObject as Texture2D;
        if (selectedTexture == null) return;

        string path = AssetDatabase.GetAssetPath(selectedTexture);
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

        List<Sprite> sprites = assets.OfType<Sprite>()
            .OrderByDescending(s => s.rect.y)
            .ThenBy(s => s.rect.x)
            .ToList();

        if (sprites.Count < 47) return;

        RuleTile ruleTile = ScriptableObject.CreateInstance<RuleTile>();
        ruleTile.name = selectedTexture.name + "_RuleTile";

        for (int i = 0; i < Mathf.Min(sprites.Count, BitmaskTemplate.Length); i++)
        {
            int mask = BitmaskTemplate[i];
            RuleTile.TilingRule rule = new RuleTile.TilingRule();
            rule.m_Sprites = new Sprite[] { sprites[i] };

            // En versiones de Lista, necesitamos inicializar los 8 vecinos con 0 (Ignore)
            // El orden estándar de Unity para los índices es:
            // 0:NW, 1:N, 2:NE, 3:W, 4:E, 5:SW, 6:S, 7:SE
            for (int n = 0; n < 8; n++) rule.m_Neighbors.Add(0);

            // Asignamos según los índices de Unity
            rule.m_Neighbors[0] = GetDiagCondition(mask, 128, 1, 64);  // NW
            rule.m_Neighbors[1] = GetOrthoCondition(mask, 1);         // N
            rule.m_Neighbors[2] = GetDiagCondition(mask, 2, 1, 4);     // NE
            rule.m_Neighbors[3] = GetOrthoCondition(mask, 64);        // W
            rule.m_Neighbors[4] = GetOrthoCondition(mask, 4);         // E
            rule.m_Neighbors[5] = GetDiagCondition(mask, 32, 16, 64);  // SW
            rule.m_Neighbors[6] = GetOrthoCondition(mask, 16);        // S
            rule.m_Neighbors[7] = GetDiagCondition(mask, 8, 16, 4);    // SE

            ruleTile.m_TilingRules.Add(rule);
        }

        AssetDatabase.CreateAsset(ruleTile, $"{System.IO.Path.GetDirectoryName(path)}/{ruleTile.name}.asset");
        AssetDatabase.SaveAssets();
        Debug.Log("¡RuleTile generado correctamente!");
    }

    private static int GetOrthoCondition(int mask, int bit) =>
        (mask & bit) != 0 ? RuleTile.TilingRule.Neighbor.This : RuleTile.TilingRule.Neighbor.NotThis;

    private static int GetDiagCondition(int mask, int diagBit, int ortho1, int ortho2)
    {
        if ((mask & ortho1) == 0 || (mask & ortho2) == 0) return 0; // Ignore
        return (mask & diagBit) != 0 ? RuleTile.TilingRule.Neighbor.This : RuleTile.TilingRule.Neighbor.NotThis;
    }
}