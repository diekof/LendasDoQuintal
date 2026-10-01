using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace LendasDoQuintal.Editor
{
    // Keep NPOT sheets at their authored size; death frames are exactly 64 pixels.
    public class Phase1PixelImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Art/Environment/Phase1/") && !assetPath.StartsWith("Assets/Resources/HeroDeath/")) return;
            var importer = (TextureImporter)assetImporter;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
        }
    }

    public static partial class MvpSceneBuilder
    {
        static Sprite[] ImportDecor(string sheet)
        {
            string path = "Assets/Art/Environment/Phase1/" + sheet + ".png";
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = 64;
            importer.isReadable = true;
            importer.SaveAndReimport();
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            float[] edges = sheet == "outdoor" ? new[] { 0f, .285f, .514f, .78f, 1f } : new[] { 0f, .255f, .514f, .75f, 1f };
            var factory = new SpriteDataProviderFactories(); factory.Init();
            var provider = factory.GetSpriteEditorDataProviderFromObject(importer); provider.InitSpriteEditorDataProvider();
            var existing = provider.GetSpriteRects().ToDictionary(s => s.name, s => s.spriteID);
            var slices = new List<SpriteRect>();
            Color32[] pixels = texture.GetPixels32();
            for (int row = 0; row < 4; row++) for (int column = 0; column < 4; column++)
            {
                int left = column * texture.width / 4, right = (column + 1) * texture.width / 4;
                int bottom = texture.height - Mathf.RoundToInt(edges[row + 1] * texture.height);
                int top = texture.height - Mathf.RoundToInt(edges[row] * texture.height);
                int x0 = right, x1 = left, y0 = top, y1 = bottom;
                for (int y = bottom; y < top; y++) for (int x = left; x < right; x++)
                    if (pixels[y * texture.width + x].a > 32) { x0 = Mathf.Min(x0, x); x1 = Mathf.Max(x1, x); y0 = Mathf.Min(y0, y); y1 = Mathf.Max(y1, y); }
                string name = sheet + "_" + (row * 4 + column).ToString("D2");
                slices.Add(new SpriteRect { name = name, spriteID = existing.TryGetValue(name, out var id) ? id : GUID.Generate(), rect = new Rect(x0, y0, x1 - x0 + 1, y1 - y0 + 1), alignment = SpriteAlignment.BottomCenter, pivot = new Vector2(.5f, 0f) });
            }
            provider.SetSpriteRects(slices.ToArray());
            provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(slices.Select(s => new SpriteNameFileIdPair(s.name, s.spriteID)));
            provider.Apply();
            importer.SaveAndReimport();
            var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s => s.name).ToArray();
            if (sprites.Length != 16) throw new System.InvalidOperationException(path + ": expected 16 decorations, imported " + sprites.Length);
            return sprites;
        }

        static GameObject PlaceDecor(Sprite sprite, string name, float x, float y, float height, Transform parent, int order = 1)
        {
            var obj = new GameObject(name); obj.transform.SetParent(parent); obj.transform.position = new Vector3(x, y, 0);
            obj.transform.localScale = Vector3.one * (height / sprite.bounds.size.y);
            var renderer = obj.AddComponent<SpriteRenderer>(); renderer.sprite = sprite; renderer.sortingOrder = order;
            return obj;
        }

        static void PopulatePhaseDecor(Transform world, Sprite[] indoor, Sprite[] outdoor)
        {
            int[][] furniture = { new[] {0,1,2,3}, new[] {4,5,6,7}, new[] {8,9,10,11}, new[] {12,14,7,6} };
            for (int area = 0; area < 8; area++)
            {
                float start = area * LendasDoQuintal.Systems.Phase1Director.SectionWidth;
                float length = area == 7 ? 40f : 136f;
                if (area < 4)
                {
                    // A repeating room rhythm keeps landmarks visible throughout the long route.
                    for (int i = 0; i < 18; i++)
                    {
                        int index = furniture[area][i % 4];
                        float h = index == 2 || index == 5 || index == 10 ? 3.3f : 2.1f;
                        PlaceDecor(indoor[index], "Mobilia_" + area + "_" + i, start + 5 + i * 7.4f, -2.5f, h, world);
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        PlaceDecor(indoor[14], "Janela_" + area + "_" + i, start + 11 + i * 17, .4f, 2.2f, world, -2);
                        PlaceDecor(indoor[i % 2 == 0 ? 12 : 15], "ParedeDecor_" + area + "_" + i, start + 3 + i * 17, 1.5f, 1.4f, world, -1);
                    }
                    PlaceDecor(indoor[13], "Porta_" + area, start + 132, -2.5f, 4f, world, -2);
                }
                else
                {
                    for (int i = 0; i * 7 < length - 3; i++)
                    {
                        bool forest = area >= 6;
                        int index = forest ? new[] { 9,11,10,11 }[i % 4] : new[] { 2,7,8,4,5,6,12,14 }[i % 8];
                        float h = index == 9 || index == 8 ? 5.5f : index == 2 || index == 6 ? 2.6f : 1.8f;
                        PlaceDecor(outdoor[index], "QuintalDecor_" + area + "_" + i, start + 4 + i * 7, -2.5f, h, world, forest ? 0 : 1);
                    }
                    for (int i = 0; i * 18 < length; i++)
                    {
                        var tree = PlaceDecor(outdoor[area >= 6 ? 9 : 3], "ArvoreFundo_" + area + "_" + i, start + 9 + i * 18, -2.5f, 7f, world, -3);
                        tree.GetComponent<SpriteRenderer>().color = new Color(.65f,.72f,.8f);
                    }
                    if (area == 4) PlaceDecor(outdoor[0], "GalinheiroArte", start + 25, -2.5f, 4f, world);
                    if (area == 5) PlaceDecor(outdoor[1], "PocoArte", start + 21, -2.5f, 3.5f, world);
                }
            }
        }
    }
}
