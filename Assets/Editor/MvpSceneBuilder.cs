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
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LendasDoQuintal.Editor
{
    public static class MvpSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/LendasDoQuintal_MVP.unity";
        private const string GeneratedPath = "Assets/Generated";
        private const string BackgroundPath = "Assets/Art/Backgrounds/Phase1_Backyard_Background_Wide.png";
        private const string HeroSpritePath = "Assets/Art/Characters/Hero/hero_idle_side_64.png";
        private const string HeroWalkPath = "Assets/Art/Characters/Hero/Walk";
        private const string HeroAttackPath = "Assets/Art/Characters/Hero/Attack";
        private const string HeroJumpPath = "Assets/Art/Characters/Hero/Jump";
        private const string ImpactPath = "Assets/Art/Effects/Impact";
        private const string LoadingArtPath = "Assets/Art/Title/lendas_quintal_title_key_art_pixel.png";
        private const float TargetAspect = 16f / 9f;
        private const float CameraOrthographicSize = 5.4f;
        private const float BackgroundPixelsPerUnit = 20f;

        [MenuItem("Lendas do Quintal/Build MVP Scene")]
        public static void BuildMvpScene()
        {
            EnsureFolders();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "LendasDoQuintal_MVP";

            Sprite playerSprite = LoadProjectSprite(HeroSpritePath, 32f) ?? CreateSprite("Player_Placeholder", PixelSpriteKind.Player);
            Sprite enemySprite = CreateSprite("Enemy_Chicken_Placeholder", PixelSpriteKind.EnemyChicken);
            Sprite clueSprite = CreateSprite("Clue_Placeholder", PixelSpriteKind.Clue);
            Sprite groundSprite = CreateSprite("Ground_Placeholder", PixelSpriteKind.Ground);
            Sprite houseSprite = CreateSprite("House_Placeholder", PixelSpriteKind.House);
            Sprite saciSprite = CreateSprite("Saci_Placeholder", PixelSpriteKind.Saci);
            Sprite backgroundSprite = LoadProjectSprite(BackgroundPath, BackgroundPixelsPerUnit) ?? CreateSprite("Phase1_Backyard_Background_Fallback", PixelSpriteKind.Background);

            GameObject systems = new GameObject("GameFlow");
            ClueSystem clueSystem = systems.AddComponent<ClueSystem>();
            ClueMessageSystem clueMessageSystem = systems.AddComponent<ClueMessageSystem>();
            ObjectiveSystem objectiveSystem = systems.AddComponent<ObjectiveSystem>();
            objectiveSystem.Configure(clueSystem);
            GameFlowController gameFlow = systems.AddComponent<GameFlowController>();

            CreatePhaseBackground(backgroundSprite);
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
                clueMessageSystem,
                "carta_avo",
                "A carta da vovó fala sobre vento no quintal."
            );

            GameObject canvas = CreateHud(
                clueSystem,
                clueMessageSystem,
                objectiveSystem,
                playerHealth,
                out GameObject gameOverPanel,
                out GameObject endPanel,
                out GameObject gameplayHud,
                out GameObject splashPanel,
                out GameObject menuPanel,
                out GameObject difficultyPanel,
                out Button playButton,
                out Button continueButton,
                out Button difficultyButton,
                out Button quitButton,
                out Button easyButton,
                out Button normalButton,
                out Button hardButton,
                out Button insaneButton,
                out Text selectedDifficultyLabel);
            _ = canvas;
            CreateEventSystem();

            gameFlow.Configure(playerHealth, gameOverPanel, endPanel);

            CreateCamera(player.transform);
            GameObject saciEncounter = CreateSaciEncounter(saciSprite, objectiveSystem, gameFlow);
            GameplayMusicProximity gameplayMusic = systems.AddComponent<GameplayMusicProximity>();
            gameplayMusic.Configure(player.transform, saciEncounter.transform);
            gameplayMusic.enabled = false;

            DemoFlowController demoFlow = systems.AddComponent<DemoFlowController>();
            demoFlow.Configure(
                splashPanel,
                menuPanel,
                difficultyPanel,
                gameplayHud,
                playButton,
                continueButton,
                difficultyButton,
                quitButton,
                easyButton,
                normalButton,
                hardButton,
                insaneButton,
                selectedDifficultyLabel,
                playerHealth,
                new[] { enemy.GetComponent<EnemyPatrol>() },
                new[] { enemy.GetComponent<EnemyCombat>() },
                new MonoBehaviour[]
                {
                    player.GetComponent<PlayerPlatformMovement>(),
                    player.GetComponent<PlayerCombat>(),
                    player.GetComponent<PlayerInteraction>(),
                    enemy.GetComponent<EnemyPatrol>(),
                    enemy.GetComponent<EnemyCombat>(),
                    saciEncounter.GetComponent<SaciEncounter>(),
                    gameplayMusic
                });

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
            CreateFolder("Assets/Art", "Backgrounds");
            CreateFolder("Assets/Art", "Title");
            CreateFolder("Assets/Art/Characters/Hero", "Attack");
            CreateFolder("Assets/Art/Characters/Hero", "Jump");
            CreateFolder("Assets/Art", "Effects");
            CreateFolder("Assets/Art/Effects", "Impact");
            CreateFolder("Assets", "Resources");
            CreateFolder("Assets/Resources", "UI");
            CreateFolder("Assets/Resources/UI", "Loading");
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

        private static Sprite LoadProjectSprite(string path, float pixelsPerUnit)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            AssetDatabase.ImportAsset(path);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Sprite[] LoadSpriteSequence(string folder, string prefix, float pixelsPerUnit)
        {
            if (!Directory.Exists(folder))
            {
                return new Sprite[0];
            }

            string[] files = Directory.GetFiles(folder, $"{prefix}*.png");
            System.Array.Sort(files);

            Sprite[] sprites = new Sprite[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                string assetPath = files[i].Replace("\\", "/");
                sprites[i] = LoadProjectSprite(assetPath, pixelsPerUnit);
            }

            return sprites;
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
                case PixelSpriteKind.Background:
                    DrawBackgroundFallback(pixels);
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
            FillRect(p, 0, 0, 16, 16, new Color32(37, 25, 20, 255));
            FillRect(p, 0, 0, 16, 3, new Color32(19, 57, 42, 255));
            FillRect(p, 1, 1, 4, 1, new Color32(45, 102, 57, 255));
            FillRect(p, 8, 1, 5, 1, new Color32(40, 86, 52, 255));
            Set(p, 3, 7, new Color32(78, 48, 33, 255));
            Set(p, 12, 11, new Color32(13, 11, 14, 255));
            Set(p, 7, 14, new Color32(67, 42, 28, 255));
        }

        private static void DrawHouse(Color32[] p)
        {
            FillRect(p, 0, 0, 16, 16, new Color32(48, 29, 24, 255));
            FillRect(p, 0, 0, 16, 2, new Color32(78, 42, 28, 255));
            FillRect(p, 2, 4, 5, 6, new Color32(196, 97, 37, 255));
            FillRect(p, 3, 5, 3, 4, Palette.WarmLight);
            FillRect(p, 9, 3, 2, 13, new Color32(23, 15, 16, 255));
            FillRect(p, 0, 12, 16, 2, new Color32(24, 17, 16, 255));
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

        private static void DrawBackgroundFallback(Color32[] p)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    Color32 sky = y > 8 ? new Color32(13, 19, 36, 255) : new Color32(6, 8, 15, 255);
                    Set(p, x, y, sky);
                }
            }

            FillRect(p, 0, 0, 16, 3, new Color32(8, 10, 14, 255));
            FillRect(p, 1, 3, 5, 5, new Color32(44, 28, 24, 255));
            FillRect(p, 2, 5, 2, 2, Palette.WarmLight);
            FillRect(p, 9, 2, 2, 9, new Color32(11, 23, 18, 255));
            FillRect(p, 12, 2, 2, 11, new Color32(9, 18, 16, 255));
            Set(p, 13, 8, new Color32(194, 30, 51, 255));
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

        private static void CreatePhaseBackground(Sprite backgroundSprite)
        {
            GameObject backgroundRoot = new GameObject("Phase1_Backyard_Background");
            float visibleWidth = CameraOrthographicSize * 2f * TargetAspect;
            float visibleHeight = CameraOrthographicSize * 2f;
            float tileHeight = backgroundSprite.bounds.size.y;
            float scale = visibleHeight / tileHeight;

            GameObject tile = new GameObject("Background_Wide");
            tile.transform.SetParent(backgroundRoot.transform);
            tile.transform.position = new Vector3(visibleWidth * 0.5f, 0f, 8f);
            tile.transform.localScale = new Vector3(scale, scale, 1f);

            SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
            renderer.sprite = backgroundSprite;
            renderer.sortingOrder = -50;
        }

        private static void CreateEnvironment(Sprite groundSprite, Sprite houseSprite)
        {
            GameObject environment = new GameObject("Environment");

            CreateBlock("Casa_Piso", groundSprite, new Vector3(0f, -3f, 0f), new Vector3(14f, 0.8f, 1f), environment.transform);
            CreateBlock("Quintal_Chao", groundSprite, new Vector3(14f, -3f, 0f), new Vector3(22f, 1f, 1f), environment.transform);
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
            combat.Configure(attackPoint, ~0, LoadSpriteSequence(ImpactPath, "impact_punch_", 16f));

            PlayerInteraction interaction = player.AddComponent<PlayerInteraction>();
            interaction.Configure(interactionPoint, ~0);

            PlayerSpriteAnimator spriteAnimator = player.AddComponent<PlayerSpriteAnimator>();
            spriteAnimator.Configure(
                movement,
                combat,
                renderer,
                sprite,
                LoadSpriteSequence(HeroWalkPath, "hero_walk_side_", 32f),
                LoadSpriteSequence(HeroAttackPath, "hero_attack_side_", 32f),
                LoadSpriteSequence(HeroJumpPath, "hero_jump_side_", 32f));

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

        private static void CreateInteractable(string name, Sprite sprite, Vector3 position, ClueSystem clueSystem, ClueMessageSystem clueMessageSystem, string clueId, string message)
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
            interactableObject.Configure(clueSystem, clueMessageSystem, clueId, message);
        }

        private static GameObject CreateHud(
            ClueSystem clueSystem,
            ClueMessageSystem clueMessageSystem,
            ObjectiveSystem objectiveSystem,
            Health playerHealth,
            out GameObject gameOverPanel,
            out GameObject endPanel,
            out GameObject gameplayHud,
            out GameObject splashPanel,
            out GameObject menuPanel,
            out GameObject difficultyPanel,
            out Button playButton,
            out Button continueButton,
            out Button difficultyButton,
            out Button quitButton,
            out Button easyButton,
            out Button normalButton,
            out Button hardButton,
            out Button insaneButton,
            out Text selectedDifficultyLabel)
        {
            GameObject canvas = new GameObject("Canvas");
            Canvas canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvas.AddComponent<GraphicRaycaster>();

            gameplayHud = CreateUiRoot(canvas.transform, "GameplayHud");

            Slider healthSlider = CreateSlider(gameplayHud.transform, "HealthBar", new Vector2(18f, -18f));
            HealthBarUI healthBar = healthSlider.gameObject.AddComponent<HealthBarUI>();
            healthBar.Configure(playerHealth, healthSlider);

            Text objectiveText = CreateText(gameplayHud.transform, "ObjectiveText", new Vector2(18f, -54f), "Descubra o que aconteceu.", 22);
            ObjectiveUI objectiveUI = objectiveText.gameObject.AddComponent<ObjectiveUI>();
            objectiveUI.Configure(objectiveSystem, objectiveText);

            Text clueText = CreateText(gameplayHud.transform, "ClueCounter", new Vector2(18f, -86f), "Pistas: 0", 20);
            ClueCounterUI clueCounter = clueText.gameObject.AddComponent<ClueCounterUI>();
            clueCounter.Configure(clueSystem, clueText);

            GameObject clueMessagePanel = CreateMessagePanel(gameplayHud.transform);
            Text clueMessageText = clueMessagePanel.GetComponentInChildren<Text>();
            ClueMessageUI clueMessageUI = clueMessagePanel.AddComponent<ClueMessageUI>();
            clueMessageUI.Configure(clueMessageSystem, clueMessagePanel, clueMessageText);

            splashPanel = CreateSplashPanel(canvas.transform);
            menuPanel = CreateMenuPanel(canvas.transform, out playButton, out continueButton, out difficultyButton, out quitButton, out selectedDifficultyLabel);
            difficultyPanel = CreateDifficultyPanel(canvas.transform, out easyButton, out normalButton, out hardButton, out insaneButton);

            gameOverPanel = CreatePanel(canvas.transform, "GameOverPanel", "Você se perdeu no mistério.\nPressione R para reiniciar.");
            gameOverPanel.SetActive(false);

            endPanel = CreatePanel(canvas.transform, "PrototypeEndPanel", "O Saci deixou uma pista para a mata.\nPressione R para jogar de novo.");
            endPanel.SetActive(false);

            return canvas;
        }

        private static void CreateEventSystem()
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private static GameObject CreateUiRoot(Transform parent, string name)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent, false);

            RectTransform rect = root.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return root;
        }

        private static GameObject CreateSplashPanel(Transform parent)
        {
            GameObject panel = CreateOverlay(parent, "SplashScreen", new Color32(7, 10, 18, 255));
            Sprite loadingArt = LoadProjectSprite(LoadingArtPath, 100f);
            if (loadingArt != null)
            {
                CreateFullScreenImage(panel.transform, "LoadingKeyArt", loadingArt);
                Text prompt = CreateCenteredText(panel.transform, "Prompt", "Pressione qualquer tecla", new Vector2(0f, 36f), new Vector2(620f, 56f), 26);
                RectTransform promptRect = prompt.GetComponent<RectTransform>();
                promptRect.anchorMin = new Vector2(0.5f, 0f);
                promptRect.anchorMax = new Vector2(0.5f, 0f);
                promptRect.pivot = new Vector2(0.5f, 0f);
                prompt.color = Color.white;
                AddTextOutline(prompt);
            }
            else
            {
                Text title = CreateCenteredText(panel.transform, "Title", "Lendas do Quintal", new Vector2(0f, 80f), new Vector2(900f, 96f), 52);
                title.color = new Color32(255, 181, 45, 255);
                CreateCenteredText(panel.transform, "Subtitle", "O Sumiço da Vovó", new Vector2(0f, 16f), new Vector2(720f, 56f), 30);
                CreateCenteredText(panel.transform, "Prompt", "Pressione qualquer tecla", new Vector2(0f, -96f), new Vector2(520f, 48f), 24);
            }
            return panel;
        }

        private static GameObject CreateMenuPanel(Transform parent, out Button playButton, out Button continueButton, out Button difficultyButton, out Button quitButton, out Text selectedDifficultyLabel)
        {
            GameObject panel = CreateOverlay(parent, "MainMenu", Color.clear);
            Sprite loadingArt = LoadProjectSprite(LoadingArtPath, 100f);
            if (loadingArt != null)
            {
                CreateFullScreenImage(panel.transform, "MenuKeyArt", loadingArt);
                CreateFullScreenShade(panel.transform, "MenuShade", new Color(0f, 0f, 0f, 0.22f));
            }
            else
            {
                Text title = CreateCenteredText(panel.transform, "Title", "Lendas do Quintal", new Vector2(0f, 190f), new Vector2(900f, 82f), 46);
                title.color = new Color32(255, 181, 45, 255);
            }

            selectedDifficultyLabel = CreateCenteredText(panel.transform, "SelectedDifficulty", "Dificuldade: Normal", new Vector2(0f, -118f), new Vector2(520f, 42f), 24);
            selectedDifficultyLabel.color = new Color32(244, 223, 164, 255);
            AddTextOutline(selectedDifficultyLabel);

            playButton = CreateButton(panel.transform, "PlayButton", "Jogar", new Vector2(0f, -182f));
            continueButton = CreateButton(panel.transform, "ContinueButton", "Continuar", new Vector2(0f, -244f));
            difficultyButton = CreateButton(panel.transform, "DifficultyButton", "Dificuldade", new Vector2(0f, -306f));
            quitButton = CreateButton(panel.transform, "QuitButton", "Sair", new Vector2(0f, -368f));

            return panel;
        }

        private static GameObject CreateDifficultyPanel(Transform parent, out Button easyButton, out Button normalButton, out Button hardButton, out Button insaneButton)
        {
            GameObject panel = CreateOverlay(parent, "DifficultyMenu", Color.clear);
            Sprite loadingArt = LoadProjectSprite(LoadingArtPath, 100f);
            if (loadingArt != null)
            {
                CreateFullScreenImage(panel.transform, "DifficultyKeyArt", loadingArt);
                CreateFullScreenShade(panel.transform, "DifficultyShade", new Color(0f, 0f, 0f, 0.34f));
            }

            Text title = CreateCenteredText(panel.transform, "Title", "Selecione a dificuldade", new Vector2(0f, -104f), new Vector2(760f, 54f), 32);
            title.color = new Color32(244, 223, 164, 255);
            AddTextOutline(title);

            easyButton = CreateButton(panel.transform, "EasyButton", "Fácil", new Vector2(0f, -178f));
            normalButton = CreateButton(panel.transform, "NormalButton", "Normal", new Vector2(0f, -240f));
            hardButton = CreateButton(panel.transform, "HardButton", "Difícil", new Vector2(0f, -302f));
            insaneButton = CreateButton(panel.transform, "InsaneButton", "Insano", new Vector2(0f, -364f));

            return panel;
        }

        private static GameObject CreateMessagePanel(Transform parent)
        {
            GameObject panel = new GameObject("ClueMessagePanel");
            panel.transform.SetParent(parent, false);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 34f);
            rect.sizeDelta = new Vector2(960f, 104f);

            Image image = panel.AddComponent<Image>();
            image.color = new Color(0.02f, 0.02f, 0.03f, 0.82f);

            Text text = CreateCenteredText(panel.transform, "Message", string.Empty, Vector2.zero, new Vector2(900f, 76f), 24);
            text.alignment = TextAnchor.MiddleCenter;

            return panel;
        }

        private static GameObject CreateOverlay(Transform parent, string name, Color color)
        {
            GameObject panel = CreateUiRoot(parent, name);
            Image image = panel.AddComponent<Image>();
            image.color = color;
            return panel;
        }

        private static Image CreateFullScreenImage(Transform parent, string name, Sprite sprite)
        {
            GameObject imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent, false);
            imageObject.transform.SetAsFirstSibling();

            RectTransform rect = imageObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = imageObject.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        private static Image CreateFullScreenShade(Transform parent, string name, Color color)
        {
            GameObject shadeObject = new GameObject(name);
            shadeObject.transform.SetParent(parent, false);
            shadeObject.transform.SetSiblingIndex(1);

            RectTransform rect = shadeObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = shadeObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Text CreateCenteredText(Transform parent, string name, string value, Vector2 anchoredPosition, Vector2 size, int fontSize)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Text text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = GetBuiltinUiFont();
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;

            return text;
        }

        private static void AddTextOutline(Text text)
        {
            Outline outline = text.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.85f);
            outline.effectDistance = new Vector2(2f, -2f);
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition)
        {
            GameObject buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(370f, 54f);

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color32(244, 223, 164, 245);

            Button button = buttonObject.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = new Color32(244, 223, 164, 245);
            colors.highlightedColor = new Color32(255, 181, 45, 255);
            colors.pressedColor = new Color32(202, 96, 31, 255);
            colors.selectedColor = new Color32(255, 181, 45, 255);
            button.colors = colors;

            Outline outline = buttonObject.AddComponent<Outline>();
            outline.effectColor = new Color32(7, 10, 18, 255);
            outline.effectDistance = new Vector2(3f, -3f);

            Text text = CreateCenteredText(buttonObject.transform, "Label", label, Vector2.zero, new Vector2(330f, 44f), 24);
            text.color = new Color32(7, 10, 18, 255);

            return button;
        }

        private static Slider CreateSlider(Transform parent, string name, Vector2 anchoredPosition)
        {
            GameObject sliderObject = new GameObject(name);
            sliderObject.transform.SetParent(parent, false);

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
            fill.transform.SetParent(sliderObject.transform, false);
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
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(620f, 32f);

            Text text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = GetBuiltinUiFont();
            text.fontSize = size;
            text.color = Color.white;

            return text;
        }

        private static Font GetBuiltinUiFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static GameObject CreatePanel(Transform parent, string name, string message)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);

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
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            UnityEngine.Camera camera = cameraObject.AddComponent<UnityEngine.Camera>();
            camera.orthographic = true;
            camera.orthographicSize = CameraOrthographicSize;
            camera.aspect = TargetAspect;
            camera.backgroundColor = new Color32(7, 10, 18, 255);
            cameraObject.AddComponent<AudioListener>();

            CameraFollow2D follow = cameraObject.AddComponent<CameraFollow2D>();
            follow.Configure(player, new Vector2(0f, 0f), new Vector2(20f, 0f), new Vector3(4.4f, 1.4f, -10f), CameraOrthographicSize, TargetAspect);
        }

        private static GameObject CreateSaciEncounter(Sprite sprite, ObjectiveSystem objectiveSystem, GameFlowController gameFlow)
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

            return trigger;
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
        Saci,
        Background
    }

    internal static class Palette
    {
        public static readonly Color32 Ink = new Color32(7, 10, 18, 255);
        public static readonly Color32 WarmLight = new Color32(255, 181, 45, 255);
    }
}
