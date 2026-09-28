using System.IO;
using LendasDoQuintal.Camera;
using LendasDoQuintal.Core;
using LendasDoQuintal.Enemy;
using LendasDoQuintal.Player;
using LendasDoQuintal.Systems;
using LendasDoQuintal.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LendasDoQuintal.Editor
{
    public static class MvpSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/LendasDoQuintal_MVP.unity";
        private const string GeneratedPath = "Assets/Generated";

        [MenuItem("Lendas do Quintal/Build MVP Scene")]
        public static void BuildMvpScene()
        {
            EnsureFolders();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "LendasDoQuintal_MVP";

            Sprite playerSprite = CreateSprite("Player_Placeholder", PixelSpriteKind.Player);
            Sprite enemySprite = CreateSprite("Enemy_Chicken_Placeholder", PixelSpriteKind.EnemyChicken);
            Sprite clueSprite = CreateSprite("Clue_Placeholder", PixelSpriteKind.Clue);
            Sprite groundSprite = CreateSprite("Ground_Placeholder", PixelSpriteKind.Ground);
            Sprite houseSprite = CreateSprite("House_Placeholder", PixelSpriteKind.House);
            Sprite saciSprite = CreateSprite("Saci_Placeholder", PixelSpriteKind.Saci);

            GameObject systems = new GameObject("GameFlow");
            ClueSystem clueSystem = systems.AddComponent<ClueSystem>();
            ObjectiveSystem objectiveSystem = systems.AddComponent<ObjectiveSystem>();
            objectiveSystem.Configure(clueSystem);
            GameFlowController gameFlow = systems.AddComponent<GameFlowController>();

            CreateEnvironment(groundSprite, houseSprite);

            GameObject player = CreatePlayer(playerSprite);
            Health playerHealth = player.GetComponent<Health>();

            GameObject enemy = CreateEnemy(enemySprite, new Vector3(12f, -2.3f, 0f));
            _ = enemy;

            CreateInteractable(
                "Carta_Rasgada",
                clueSprite,
                new Vector3(2.5f, -1.45f, 0f),
                clueSystem,
                "carta_avo",
                "A carta da vovó fala sobre vento no quintal."
            );

            GameObject canvas = CreateHud(clueSystem, objectiveSystem, playerHealth, out GameObject gameOverPanel, out GameObject endPanel);
            _ = canvas;

            gameFlow.Configure(playerHealth, gameOverPanel, endPanel);

            CreateCamera(player.transform);
            CreateSaciEncounter(saciSprite, objectiveSystem, gameFlow);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"MVP scene generated at {ScenePath}");
        }

        private static void EnsureFolders()
        {
            CreateFolder("Assets", "Scenes");
            CreateFolder("Assets", "Generated");
            CreateFolder("Assets", "Prefabs");
        }

        private static void CreateFolder(string parent, string child)
        {
            string path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static Sprite CreateSprite(string name, PixelSpriteKind kind)
        {
            string texturePath = $"{GeneratedPath}/{name}.png";
            Texture2D texture = CreatePixelTexture(kind);
            texture.Apply();
            File.WriteAllBytes(texturePath, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(texturePath);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texturePath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 16f;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
        }

        private static Texture2D CreatePixelTexture(PixelSpriteKind kind)
        {
            Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            Color32 clear = new Color32(0, 0, 0, 0);
            Color32[] pixels = new Color32[16 * 16];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = clear;
            }

            switch (kind)
            {
                case PixelSpriteKind.Player:
                    DrawPlayer(pixels);
                    break;
                case PixelSpriteKind.EnemyChicken:
                    DrawChicken(pixels);
                    break;
                case PixelSpriteKind.Clue:
                    DrawClue(pixels);
                    break;
                case PixelSpriteKind.Ground:
                    DrawGround(pixels);
                    break;
                case PixelSpriteKind.House:
                    DrawHouse(pixels);
                    break;
                case PixelSpriteKind.Saci:
                    DrawSaci(pixels);
                    break;
            }

            texture.SetPixels32(pixels);
            texture.filterMode = FilterMode.Point;
            return texture;
        }

        private static void DrawPlayer(Color32[] p)
        {
            Color32 ink = Palette.Ink;
            Color32 skin = new Color32(178, 96, 45, 255);
            Color32 skinLight = new Color32(239, 151, 75, 255);
            Color32 blue = new Color32(18, 88, 202, 255);
            Color32 blueLight = new Color32(21, 174, 255, 255);
            Color32 cap = new Color32(246, 177, 25, 255);

            FillRect(p, 5, 2, 6, 2, cap);
            FillRect(p, 4, 4, 8, 5, skin);
            Set(p, 7, 5, skinLight);
            Set(p, 10, 6, Palette.WarmLight);
            FillRect(p, 3, 9, 10, 5, blue);
            FillRect(p, 4, 10, 3, 1, blueLight);
            FillRect(p, 4, 14, 3, 2, ink);
            FillRect(p, 9, 14, 3, 2, ink);
            Outline(p);
        }

        private static void DrawChicken(Color32[] p)
        {
            FillRect(p, 4, 7, 8, 6, new Color32(232, 213, 155, 255));
            FillRect(p, 6, 5, 5, 4, new Color32(247, 234, 184, 255));
            Set(p, 9, 6, Palette.Ink);
            Set(p, 11, 7, new Color32(245, 87, 35, 255));
            Set(p, 7, 4, new Color32(212, 33, 55, 255));
            Set(p, 8, 4, new Color32(212, 33, 55, 255));
            FillRect(p, 5, 13, 2, 2, new Color32(232, 121, 36, 255));
            FillRect(p, 10, 13, 2, 2, new Color32(232, 121, 36, 255));
            Set(p, 3, 10, new Color32(139, 31, 40, 255));
            Outline(p);
        }

        private static void DrawClue(Color32[] p)
        {
            FillRect(p, 4, 3, 8, 10, new Color32(244, 223, 164, 255));
            FillRect(p, 5, 4, 6, 1, Palette.WarmLight);
            FillRect(p, 6, 7, 4, 1, new Color32(116, 76, 49, 255));
            FillRect(p, 6, 9, 3, 1, new Color32(116, 76, 49, 255));
            Set(p, 10, 11, new Color32(202, 55, 55, 255));
            Outline(p);
        }

        private static void DrawGround(Color32[] p)
        {
            FillRect(p, 0, 0, 16, 16, new Color32(83, 58, 35, 255));
            FillRect(p, 0, 0, 16, 3, new Color32(43, 117, 50, 255));
            FillRect(p, 1, 1, 3, 1, new Color32(111, 190, 35, 255));
            FillRect(p, 8, 1, 4, 1, new Color32(111, 190, 35, 255));
            Set(p, 3, 7, new Color32(141, 91, 47, 255));
            Set(p, 12, 11, new Color32(35, 28, 24, 255));
            Set(p, 7, 14, new Color32(141, 91, 47, 255));
        }

        private static void DrawHouse(Color32[] p)
        {
            FillRect(p, 0, 0, 16, 16, new Color32(95, 53, 30, 255));
            FillRect(p, 0, 0, 16, 2, new Color32(202, 96, 31, 255));
            FillRect(p, 2, 4, 5, 6, Palette.WarmLight);
            FillRect(p, 3, 5, 3, 4, new Color32(255, 205, 82, 255));
            FillRect(p, 9, 3, 2, 13, new Color32(48, 28, 23, 255));
            FillRect(p, 0, 12, 16, 2, new Color32(54, 32, 25, 255));
        }

        private static void DrawSaci(Color32[] p)
        {
            Color32 shadow = new Color32(10, 11, 18, 255);
            FillRect(p, 5, 5, 6, 8, shadow);
            FillRect(p, 4, 8, 2, 3, shadow);
            FillRect(p, 10, 8, 2, 3, shadow);
            FillRect(p, 6, 2, 5, 3, new Color32(194, 30, 51, 255));
            Set(p, 10, 1, new Color32(238, 68, 79, 255));
            Set(p, 7, 7, new Color32(255, 210, 67, 255));
            Set(p, 10, 7, new Color32(255, 210, 67, 255));
            Set(p, 3, 4, new Color32(238, 18, 57, 255));
            Set(p, 2, 5, new Color32(238, 18, 57, 255));
            Set(p, 1, 6, new Color32(111, 190, 35, 255));
        }

        private static void Outline(Color32[] p)
        {
            Color32[] copy = (Color32[])p.Clone();
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    if (copy[y * 16 + x].a == 0)
                    {
                        continue;
                    }

                    TrySetEmpty(p, x - 1, y, Palette.Ink);
                    TrySetEmpty(p, x + 1, y, Palette.Ink);
                    TrySetEmpty(p, x, y - 1, Palette.Ink);
                    TrySetEmpty(p, x, y + 1, Palette.Ink);
                }
            }
        }

        private static void FillRect(Color32[] pixels, int x, int y, int width, int height, Color32 color)
        {
            for (int py = y; py < y + height; py++)
            {
                for (int px = x; px < x + width; px++)
                {
                    Set(pixels, px, py, color);
                }
            }
        }

        private static void Set(Color32[] pixels, int x, int y, Color32 color)
        {
            if (x < 0 || x >= 16 || y < 0 || y >= 16)
            {
                return;
            }

            pixels[y * 16 + x] = color;
        }

        private static void TrySetEmpty(Color32[] pixels, int x, int y, Color32 color)
        {
            if (x < 0 || x >= 16 || y < 0 || y >= 16)
            {
                return;
            }

            int index = y * 16 + x;
            if (pixels[index].a == 0)
            {
                pixels[index] = color;
            }
        }

        private static void CreateEnvironment(Sprite groundSprite, Sprite houseSprite)
        {
            GameObject environment = new GameObject("Environment");

            CreateBlock("Casa_Piso", groundSprite, new Vector3(0f, -3f, 0f), new Vector3(12f, 1f, 1f), environment.transform);
            CreateBlock("Quintal_Chao", groundSprite, new Vector3(14f, -3f, 0f), new Vector3(22f, 1f, 1f), environment.transform);
            CreateBlock("Parede_Casa", houseSprite, new Vector3(0f, 0f, 1f), new Vector3(12f, 5f, 1f), environment.transform, false);
            CreateBlock("Mesa_Obstaculo", houseSprite, new Vector3(-2f, -1.9f, 0f), new Vector3(1.5f, 0.5f, 1f), environment.transform);
            CreateBlock("Plataforma_Poco", groundSprite, new Vector3(8f, -1.5f, 0f), new Vector3(3f, 0.45f, 1f), environment.transform);
            CreateBlock("Plataforma_Galinheiro", groundSprite, new Vector3(15f, -0.6f, 0f), new Vector3(3.2f, 0.45f, 1f), environment.transform);
            CreateBlock("Entrada_Mata", groundSprite, new Vector3(24f, -1.4f, 0f), new Vector3(2.5f, 0.45f, 1f), environment.transform);
        }

        private static GameObject CreateBlock(string name, Sprite sprite, Vector3 position, Vector3 scale, Transform parent, bool solid = true)
        {
            GameObject block = new GameObject(name);
            block.transform.SetParent(parent);
            block.transform.position = position;
            block.transform.localScale = scale;

            SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = position.z > 0f ? -5 : 0;

            if (solid)
            {
                block.AddComponent<BoxCollider2D>();
            }

            return block;
        }

        private static GameObject CreatePlayer(Sprite sprite)
        {
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = new Vector3(-4f, -1.6f, 0f);

            SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 10;

            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.gravityScale = 3f;

            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 1.2f);

            Health health = player.AddComponent<Health>();
            _ = health;

            Transform groundCheck = CreateChild(player.transform, "GroundCheck", new Vector3(0f, -0.66f, 0f));
            Transform attackPoint = CreateChild(player.transform, "AttackPoint", new Vector3(0.75f, 0f, 0f));
            Transform interactionPoint = CreateChild(player.transform, "InteractionPoint", new Vector3(0.75f, 0f, 0f));

            PlayerPlatformMovement movement = player.AddComponent<PlayerPlatformMovement>();
            movement.Configure(groundCheck, ~0);

            PlayerCombat combat = player.AddComponent<PlayerCombat>();
            combat.Configure(attackPoint, ~0);

            PlayerInteraction interaction = player.AddComponent<PlayerInteraction>();
            interaction.Configure(interactionPoint, ~0);

            return player;
        }

        private static GameObject CreateEnemy(Sprite sprite, Vector3 position)
        {
            GameObject enemy = new GameObject("Enemy_Chicken");
            enemy.transform.position = position;

            SpriteRenderer renderer = enemy.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 9;

            Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.gravityScale = 3f;

            BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 0.8f);

            enemy.AddComponent<Health>();
            enemy.AddComponent<EnemyCombat>();

            Transform groundCheck = CreateChild(enemy.transform, "GroundCheck", new Vector3(0.45f, -0.5f, 0f));
            Transform wallCheck = CreateChild(enemy.transform, "WallCheck", new Vector3(0.55f, 0f, 0f));

            EnemyPatrol patrol = enemy.AddComponent<EnemyPatrol>();
            patrol.Configure(groundCheck, wallCheck, ~0);

            return enemy;
        }

        private static void CreateInteractable(string name, Sprite sprite, Vector3 position, ClueSystem clueSystem, string clueId, string message)
        {
            GameObject interactable = new GameObject(name);
            interactable.transform.position = position;

            SpriteRenderer renderer = interactable.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 8;

            CircleCollider2D collider = interactable.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.5f;

            InteractableObject interactableObject = interactable.AddComponent<InteractableObject>();
            interactableObject.Configure(clueSystem, clueId, message);
        }

        private static GameObject CreateHud(ClueSystem clueSystem, ObjectiveSystem objectiveSystem, Health playerHealth, out GameObject gameOverPanel, out GameObject endPanel)
        {
            GameObject canvas = new GameObject("Canvas");
            Canvas canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();

            Slider healthSlider = CreateSlider(canvas.transform, "HealthBar", new Vector2(18f, -18f));
            HealthBarUI healthBar = healthSlider.gameObject.AddComponent<HealthBarUI>();
            healthBar.Configure(playerHealth, healthSlider);

            Text objectiveText = CreateText(canvas.transform, "ObjectiveText", new Vector2(18f, -54f), "Descubra o que aconteceu.", 22);
            ObjectiveUI objectiveUI = objectiveText.gameObject.AddComponent<ObjectiveUI>();
            objectiveUI.Configure(objectiveSystem, objectiveText);

            Text clueText = CreateText(canvas.transform, "ClueCounter", new Vector2(18f, -86f), "Pistas: 0", 20);
            ClueCounterUI clueCounter = clueText.gameObject.AddComponent<ClueCounterUI>();
            clueCounter.Configure(clueSystem, clueText);

            gameOverPanel = CreatePanel(canvas.transform, "GameOverPanel", "Você se perdeu no mistério.\nPressione R para reiniciar.");
            gameOverPanel.SetActive(false);

            endPanel = CreatePanel(canvas.transform, "PrototypeEndPanel", "O Saci deixou uma pista para a mata.\nPressione R para jogar de novo.");
            endPanel.SetActive(false);

            return canvas;
        }

        private static Slider CreateSlider(Transform parent, string name, Vector2 anchoredPosition)
        {
            GameObject sliderObject = new GameObject(name);
            sliderObject.transform.SetParent(parent);

            RectTransform rect = sliderObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(220f, 22f);

            Slider slider = sliderObject.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 5f;
            slider.value = 5f;

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(sliderObject.transform);
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color32(218, 64, 64, 255);
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            slider.fillRect = fillRect;

            return slider;
        }

        private static Text CreateText(Transform parent, string name, Vector2 anchoredPosition, string value, int size)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(620f, 32f);

            Text text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.color = Color.white;

            return text;
        }

        private static GameObject CreatePanel(Transform parent, string name, string message)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = panel.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.72f);

            Text text = CreateText(panel.transform, "Message", Vector2.zero, message, 28);
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = new Vector2(720f, 180f);
            text.alignment = TextAnchor.MiddleCenter;

            return panel;
        }

        private static void CreateCamera(Transform player)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            UnityEngine.Camera camera = cameraObject.AddComponent<UnityEngine.Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 4.5f;
            camera.backgroundColor = new Color32(18, 24, 42, 255);
            cameraObject.AddComponent<AudioListener>();

            CameraFollow2D follow = cameraObject.AddComponent<CameraFollow2D>();
            follow.Configure(player);
        }

        private static void CreateSaciEncounter(Sprite sprite, ObjectiveSystem objectiveSystem, GameFlowController gameFlow)
        {
            GameObject trigger = new GameObject("SaciEncounter");
            trigger.transform.position = new Vector3(24f, -0.5f, 0f);

            BoxCollider2D collider = trigger.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(2f, 4f);

            GameObject visual = new GameObject("Saci_Visual");
            visual.transform.SetParent(trigger.transform);
            visual.transform.localPosition = new Vector3(0.6f, 0.4f, 0f);
            SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 12;
            visual.SetActive(false);

            SaciEncounter encounter = trigger.AddComponent<SaciEncounter>();
            encounter.Configure(objectiveSystem, gameFlow, visual);
        }

        private static Transform CreateChild(Transform parent, string name, Vector3 localPosition)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent);
            child.transform.localPosition = localPosition;
            return child.transform;
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            EditorBuildSettingsScene[] currentScenes = EditorBuildSettings.scenes;
            foreach (EditorBuildSettingsScene buildScene in currentScenes)
            {
                if (buildScene.path == scenePath)
                {
                    return;
                }
            }

            EditorBuildSettingsScene[] updatedScenes = new EditorBuildSettingsScene[currentScenes.Length + 1];
            currentScenes.CopyTo(updatedScenes, 0);
            updatedScenes[updatedScenes.Length - 1] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = updatedScenes;
        }
    }

    internal enum PixelSpriteKind
    {
        Player,
        EnemyChicken,
        Clue,
        Ground,
        House,
        Saci
    }

    internal static class Palette
    {
        public static readonly Color32 Ink = new Color32(7, 10, 18, 255);
        public static readonly Color32 WarmLight = new Color32(255, 181, 45, 255);
    }
}
