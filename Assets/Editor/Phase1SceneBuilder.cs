using System.IO;
using LendasDoQuintal.Camera;
using LendasDoQuintal.Core;
using LendasDoQuintal.Enemy;
using LendasDoQuintal.Player;
using LendasDoQuintal.Systems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LendasDoQuintal.Editor
{
    public static partial class MvpSceneBuilder
    {
        private const string Phase1Path = "Assets/Scenes/LendasDoQuintal_Phase1.unity";

        [MenuItem("Lendas do Quintal/Build Phase 1 Scene")]
        public static void BuildPhase1Scene()
        {
            BuildBaseScene(Phase1Path);
            Scene scene = SceneManager.GetActiveScene();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.layer = 8;
            player.AddComponent<HeroSkillTree>();
            player.GetComponent<PlayerPlatformMovement>().Configure(player.transform.Find("GroundCheck"), 1 << 0);
            player.GetComponent<Health>().KeepOnDeath();
            foreach (EnemyPatrol enemy in Object.FindObjectsByType<EnemyPatrol>()) Object.DestroyImmediate(enemy.gameObject);
            Object.DestroyImmediate(Object.FindAnyObjectByType<SaciEncounter>().gameObject);
            Object.DestroyImmediate(GameObject.Find("Environment"));
            // Remove the MVP letter; all five clues are placed explicitly below.
            Object.DestroyImmediate(GameObject.Find("Carta_Rasgada"));
            GameObject systems = GameObject.Find("GameFlow");
            var clues = systems.GetComponent<ClueSystem>();
            var messages = systems.GetComponent<ClueMessageSystem>();
            var objectives = systems.GetComponent<ObjectiveSystem>();
            var flow = systems.GetComponent<GameFlowController>();
            Sprite ground = LoadProjectSprite("Assets/Generated/Ground_Placeholder.png", 16f);
            Sprite house = LoadProjectSprite("Assets/Generated/House_Placeholder.png", 16f);
            Sprite clue = LoadProjectSprite("Assets/Generated/Clue_Placeholder.png", 16f);
            Sprite saci = LoadProjectSprite("Assets/Generated/Saci_Placeholder.png", 16f);
            GameObject world = new GameObject("Phase1_World");
            Sprite[] indoorDecor = ImportDecor("indoor");
            Sprite[] outdoorDecor = ImportDecor("outdoor");
            GameObject[] gates = new GameObject[7];
            for (int area = 0; area < 8; area++)
            {
                float start = area * Phase1Director.SectionWidth;
                MakePhaseBlock(Phase1Director.AreaNames[area] + "_Piso", ground, new Vector2(start + Phase1Director.SectionWidth / 2f - 3f, -3f), new Vector2(Phase1Director.SectionWidth + 6f, 1f), world.transform, Color.white);
                // The first rooms get warm wall panels; outdoor sections reuse the backyard art.
                if (area < 4)
                {
                    GameObject wall = MakePhaseBlock("Parede_" + area, ground, new Vector2(start + Phase1Director.SectionWidth / 2f, 1f), new Vector2(Phase1Director.SectionWidth, 8f), world.transform, new Color(0.32f, 0.24f, 0.20f), false);
                    wall.GetComponent<SpriteRenderer>().sortingOrder = -4;
                }
                else
                {
                    Sprite background = LoadProjectSprite(BackgroundPath, BackgroundPixelsPerUnit);
                    float width = background.bounds.size.x;
                    for (int tile = 0; tile < Mathf.CeilToInt(Phase1Director.SectionWidth / width); tile++)
                    {
                        GameObject backdrop = new GameObject("Quintal_Fundo_" + area + "_" + tile); backdrop.transform.SetParent(world.transform);
                        backdrop.transform.position = new Vector3(start + width * (tile + 0.5f), 0f, 0f);
                        var sr = backdrop.AddComponent<SpriteRenderer>(); sr.sprite = background; sr.sortingOrder = -5;
                    }
                }
                for (int p = 0; p < (area == 7 ? 3 : 10); p++)
                    MakePhaseBlock(area < 4 ? "Móvel" : "Galho", ground, new Vector2(start + (p < 3 ? 10f + p * 9f : 40f + (p - 3) * 14f), p % 2 == 0 ? -1.2f : 0.1f), new Vector2(3.5f, 0.4f), world.transform, new Color(0.65f, 0.8f, 0.45f));
                MakePhaseSign(Phase1Director.AreaNames[area], new Vector2(start + 4, 3f), world.transform);
                GameObject fruit = new GameObject("Fruta_" + area); fruit.transform.SetParent(world.transform); fruit.transform.position = new Vector3(start + 25f, -1.6f, 0f);
                var fruitVisual = fruit.AddComponent<SpriteRenderer>(); fruitVisual.sprite = clue; fruitVisual.color = Color.red; fruitVisual.sortingOrder = 8;
                var fruitCollider = fruit.AddComponent<CircleCollider2D>(); fruitCollider.isTrigger = true; fruitCollider.radius = 0.35f;
                fruit.AddComponent<Phase1Pickup>();
                if (area < 7)
                    gates[area] = MakePhaseBlock("Encantamento_" + area, ground, new Vector2(start + Phase1Director.SectionWidth, 2f), new Vector2(0.6f, 16f), world.transform, new Color(0.3f, 0.8f, 0.55f, 0.65f));
            }
            PopulatePhaseDecor(world.transform, indoorDecor, outdoorDecor);
            MakePhaseBlock("LimiteEsquerdo", ground, new Vector2(-6f, 0f), new Vector2(1f, 16f), world.transform, Color.gray);
            float arenaStart = Phase1Director.SectionWidth * 7f;
            MakePhaseBlock("LimiteArena", ground, new Vector2(arenaStart + 41f, 2f), new Vector2(1f, 16f), world.transform, Color.gray);
            MakePhaseClue("Carta incompleta", 0, "carta_avo", "Neto, se eu sumir, siga as histórias. A mata precisa de mim...", clue, clues, messages);
            MakePhaseClue("Pegadas na janela", 1, "pegadas", "Pegadas pequenas! Parece que alguém pulou em uma perna só.", clue, clues, messages);
            MakePhaseClue("Lembrança da avó", 4, "objeto_avo", "O amuleto da vovó foi levado para o galinheiro. Ela protegia as lendas.", clue, clues, messages);
            MakePhaseClue("Gorro no galho", 5, "gorro", "Um gorro vermelho ficou preso no galho acima do poço. O Saci passou aqui!", clue, clues, messages, true);
            MakePhaseClue("Folhas sem vento", 6, "folhas", "As folhas giram sem vento. O Saci está na clareira!", clue, clues, messages);

            Phase1Enemy[] templates = new Phase1Enemy[4];
            for (int i = 0; i < 4; i++)
            {
                var enemy = new GameObject("Template_" + (Phase1EnemyKind)i); enemy.SetActive(false); enemy.layer = 9;
                var sr = enemy.AddComponent<SpriteRenderer>(); sr.sprite = i == 1 ? LoadProjectSprite("Assets/Art/Enemies/Chicken/Idle/chicken_idle_side_00.png", 32f) : CreatePhaseEnemySprite((Phase1EnemyKind)i); sr.sortingOrder = 9;
                var rb = enemy.AddComponent<Rigidbody2D>(); rb.freezeRotation = true; rb.gravityScale = i == 3 ? 0f : 3f;
                var col = enemy.AddComponent<BoxCollider2D>(); col.size = new Vector2(0.8f, 0.8f); col.sharedMaterial = CreateNoFrictionMaterial();
                enemy.AddComponent<Health>().SetMaxHealth(i == 2 ? 2 : 3);
                templates[i] = enemy.AddComponent<Phase1Enemy>(); templates[i].Configure((Phase1EnemyKind)i, player.transform, 0);
                if (i == 1) templates[i].ConfigureAnimation(LoadSpriteSequence(ChickenWalkPath, "chicken_walk_side_", 32f), LoadSpriteSequence(ChickenSpitPath, "chicken_spit_side_", 32f));
            }
            GameObject arenaGate = MakePhaseBlock("Arena_Entrada", ground, new Vector2(arenaStart + 1f, 2f), new Vector2(0.6f, 16f), world.transform, Color.green);
            arenaGate.SetActive(false);
            GameObject bossObject = new GameObject("Saci_Boss"); bossObject.layer = 9; bossObject.transform.position = new Vector3(arenaStart + 23f, -1.8f, 0f);
            var bossVisual = bossObject.AddComponent<SpriteRenderer>(); bossVisual.sprite = saci; bossVisual.sortingOrder = 12;
            var bossCollider = bossObject.AddComponent<BoxCollider2D>(); bossCollider.size = new Vector2(0.9f, 1.5f); bossCollider.enabled = false;
            bossObject.AddComponent<Health>().SetMaxHealth(18);
            SaciBoss boss = bossObject.AddComponent<SaciBoss>(); boss.Configure(player.transform, flow, messages, arenaGate);
            Phase1Director director = systems.AddComponent<Phase1Director>(); director.Configure(player.transform, clues, objectives, messages, gates, templates, boss);
            systems.GetComponent<GameplayMusicProximity>().Configure(player.transform, boss.transform);
            Object.FindAnyObjectByType<CameraFollow2D>().Configure(player.transform, new Vector2(0f, 0f), new Vector2(arenaStart + 32f, 0f), new Vector3(4.4f, 1.4f, -10f), CameraOrthographicSize, TargetAspect);
            // Inactive panels are found through the canvas hierarchy.
            foreach (Text text in Object.FindObjectsByType<Text>(FindObjectsInactive.Include))
                if (text.transform.parent.name == "PrototypeEndPanel") text.text = "O Saci revelou: a vovó é uma guardiã das lendas!\nO mapa aponta para o Rio das Vozes.\nFase 1 concluída — R para jogar novamente.";
            // Default build launches the complete phase, while the original MVP stays available.
            var buildScenes = new System.Collections.Generic.List<EditorBuildSettingsScene> { new EditorBuildSettingsScene(Phase1Path, true) };
            foreach (var existing in EditorBuildSettings.scenes) if (existing.path != Phase1Path) buildScenes.Add(existing);
            EditorBuildSettings.scenes = buildScenes.ToArray();
            EditorSceneManager.SaveScene(scene, Phase1Path);
            AssetDatabase.SaveAssets();
            Debug.Log("Phase 1 scene generated: " + Phase1Path);
        }

        private static GameObject MakePhaseBlock(string name, Sprite sprite, Vector2 position, Vector2 size, Transform parent, Color color, bool solid = true)
        {
            GameObject obj = new GameObject(name); obj.transform.SetParent(parent); obj.transform.position = position;
            var sr = obj.AddComponent<SpriteRenderer>(); sr.sprite = sprite; sr.color = color; sr.sortingOrder = 2;
            obj.transform.localScale = new Vector3(size.x / sprite.bounds.size.x, size.y / sprite.bounds.size.y, 1f);
            if (solid) { obj.AddComponent<BoxCollider2D>().sharedMaterial = CreateNoFrictionMaterial(); }
            return obj;
        }
        private static void MakePhaseSign(string name, Vector2 position, Transform parent)
        {
            GameObject sign = new GameObject("Placa_" + name); sign.transform.SetParent(parent); sign.transform.position = position;
            TextMesh text = sign.AddComponent<TextMesh>(); text.text = name; text.fontSize = 40; text.characterSize = 0.08f; text.color = new Color(1f, 0.9f, 0.6f);
            sign.GetComponent<MeshRenderer>().sortingOrder = 10;
        }
        private static void MakePhaseClue(string name, int section, string id, string message, Sprite sprite, ClueSystem clues, ClueMessageSystem messages, bool elevated = false)
        {
            Vector3 position = new Vector3(section * Phase1Director.SectionWidth + (elevated ? 28f : 18f), elevated ? 1f : -1.8f, 0f);
            CreateInteractable(name + " [E]", sprite, position, clues, messages, id, message);
            MakePhaseSign("[E] " + name, position + Vector3.up, null);
        }
        private static Sprite CreatePhaseEnemySprite(Phase1EnemyKind kind)
        {
            Texture2D texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[1024];
            for (int y = 0; y < 32; y++) for (int x = 0; x < 32; x++)
            {
                bool shape = kind == Phase1EnemyKind.Toy ? x > 5 && x < 27 && y > 5 && y < 27 :
                    kind == Phase1EnemyKind.Shadow ? Vector2.Distance(new Vector2(x, y), new Vector2(16, 16)) < 13 :
                    Mathf.Abs(x - 16) < (y / 3 + 2) && y > 3 && y < 29;
                Color color = kind == Phase1EnemyKind.Toy ? new Color(0.85f, 0.48f, 0.18f) : kind == Phase1EnemyKind.Shadow ? new Color(0.3f, 0.16f, 0.5f) : new Color(0.5f, 0.85f, 0.65f);
                if (shape) pixels[y * 32 + x] = (y == 21 || y == 22) && (x == 11 || x == 20) ? Color.yellow : color;
            }
            texture.SetPixels(pixels); texture.Apply();
            string path = GeneratedPath + "/Phase1_" + kind + ".png"; File.WriteAllBytes(path, texture.EncodeToPNG()); Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path);
            return LoadProjectSprite(path, 32f);
        }
    }
}
