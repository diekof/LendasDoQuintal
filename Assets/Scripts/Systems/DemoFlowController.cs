using LendasDoQuintal.Core;
using LendasDoQuintal.Enemy;
using LendasDoQuintal.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LendasDoQuintal.Systems
{
    public class DemoFlowController : MonoBehaviour
    {
        [SerializeField] private GameObject splashPanel;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject difficultyPanel;
        [SerializeField] private GameObject gameplayHud;
        [SerializeField] private Button playButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button difficultyButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button easyButton;
        [SerializeField] private Button normalButton;
        [SerializeField] private Button hardButton;
        [SerializeField] private Button insaneButton;
        [SerializeField] private Text selectedDifficultyLabel;
        [SerializeField] private string loadingArtResourcePath = "UI/Loading/lendas_quintal_title_key_art_pixel";
        [SerializeField] private Health playerHealth;
        [SerializeField] private EnemyPatrol[] enemyPatrols;
        [SerializeField] private EnemyCombat[] enemyCombats;
        [SerializeField] private MonoBehaviour[] gameplayControllers;
        [SerializeField] private GameplayMusicProximity gameplayMusic;
        [Header("Audio")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip menuMusic;
        [SerializeField, Range(0f, 1f)] private float menuMusicVolume = 0.42f;

        private DemoDifficulty selectedDifficulty = DemoDifficulty.Normal;
        private bool waitingForSplashInput;
        private static Sprite loadingArtSprite;

        private void Start()
        {
            EnsureEventSystem();
            EnsureMenuMusic();
            WireButtons();
            EnsureGameplayMusic();
            EnsureSplashKeyArt();
            EnsureMenuPresentation();
            EnsureDifficultyPresentation();
            ShowSplash();
        }

        private void Update()
        {
            if (waitingForSplashInput && Input.anyKeyDown)
            {
                ShowMenu();
                return;
            }

            if (menuPanel != null &&
                menuPanel.activeSelf &&
                (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
            {
                StartGame();
            }
        }

        private void OnDisable()
        {
            UnwireButtons();
            StopMenuMusic();
            Time.timeScale = 1f;
        }

        public void Configure(
            GameObject newSplashPanel,
            GameObject newMenuPanel,
            GameObject newDifficultyPanel,
            GameObject newGameplayHud,
            Button newPlayButton,
            Button newContinueButton,
            Button newDifficultyButton,
            Button newQuitButton,
            Button newEasyButton,
            Button newNormalButton,
            Button newHardButton,
            Button newInsaneButton,
            Text newSelectedDifficultyLabel,
            Health newPlayerHealth,
            EnemyPatrol[] newEnemyPatrols,
            EnemyCombat[] newEnemyCombats,
            MonoBehaviour[] newGameplayControllers)
        {
            UnwireButtons();

            splashPanel = newSplashPanel;
            menuPanel = newMenuPanel;
            difficultyPanel = newDifficultyPanel;
            gameplayHud = newGameplayHud;
            playButton = newPlayButton;
            continueButton = newContinueButton;
            difficultyButton = newDifficultyButton;
            quitButton = newQuitButton;
            easyButton = newEasyButton;
            normalButton = newNormalButton;
            hardButton = newHardButton;
            insaneButton = newInsaneButton;
            selectedDifficultyLabel = newSelectedDifficultyLabel;
            playerHealth = newPlayerHealth;
            enemyPatrols = newEnemyPatrols;
            enemyCombats = newEnemyCombats;
            gameplayControllers = newGameplayControllers;

            WireButtons();
            UpdateDifficultyLabel();
        }

        private void ShowSplash()
        {
            EnsureSplashKeyArt();
            waitingForSplashInput = true;
            Time.timeScale = 0f;
            SetGameplayEnabled(false);
            SetActive(splashPanel, true);
            SetActive(menuPanel, false);
            SetActive(difficultyPanel, false);
            SetActive(gameplayHud, false);
            SetGameplayMusicEnabled(false);
            PlayMenuMusic();
        }

        private void EnsureSplashKeyArt()
        {
            if (splashPanel == null)
            {
                return;
            }

            if (splashPanel.transform.Find("LoadingKeyArt") == null)
            {
                Sprite sprite = LoadLoadingArtSprite();
                if (sprite == null)
                {
                    return;
                }

                GameObject artObject = new GameObject("LoadingKeyArt");
                artObject.transform.SetParent(splashPanel.transform, false);
                artObject.transform.SetAsFirstSibling();

                RectTransform rect = artObject.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                Image image = artObject.AddComponent<Image>();
                image.sprite = sprite;
                image.preserveAspect = true;
                image.raycastTarget = false;
            }

            HideSplashText("Title");
            HideSplashText("Subtitle");
            StyleSplashPrompt();
        }

        private Sprite LoadLoadingArtSprite()
        {
            if (loadingArtSprite != null)
            {
                return loadingArtSprite;
            }

            Texture2D texture = Resources.Load<Texture2D>(loadingArtResourcePath);
            if (texture == null)
            {
                Debug.LogWarning($"Loading screen art not found at Resources/{loadingArtResourcePath}.");
                return null;
            }

            loadingArtSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            loadingArtSprite.name = "LoadingScreenKeyArt";
            return loadingArtSprite;
        }

        private void HideSplashText(string childName)
        {
            Transform child = splashPanel.transform.Find(childName);
            if (child != null)
            {
                child.gameObject.SetActive(false);
            }
        }

        private void StyleSplashPrompt()
        {
            Transform prompt = splashPanel.transform.Find("Prompt");
            if (prompt == null)
            {
                return;
            }

            RectTransform rect = prompt.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(0f, 36f);
                rect.sizeDelta = new Vector2(620f, 56f);
            }

            Text text = prompt.GetComponent<Text>();
            if (text != null)
            {
                text.text = "Pressione qualquer tecla";
                text.fontSize = 26;
                text.color = Color.white;
            }

            Outline outline = prompt.GetComponent<Outline>();
            if (outline == null)
            {
                outline = prompt.gameObject.AddComponent<Outline>();
            }

            outline.effectColor = new Color(0f, 0f, 0f, 0.85f);
            outline.effectDistance = new Vector2(2f, -2f);
        }

        private void ShowMenu()
        {
            EnsureMenuPresentation();
            waitingForSplashInput = false;
            Time.timeScale = 0f;
            SetGameplayEnabled(false);
            SetActive(splashPanel, false);
            SetActive(menuPanel, true);
            SetActive(difficultyPanel, false);
            SetActive(gameplayHud, false);
            SetGameplayMusicEnabled(false);
            UpdateDifficultyLabel();
            PlayMenuMusic();
        }

        private void EnsureMenuPresentation()
        {
            if (menuPanel == null)
            {
                return;
            }

            EnsurePanelKeyArt(menuPanel.transform, "MenuKeyArt");
            EnsurePanelShade(menuPanel.transform, "MenuShade", new Color(0f, 0f, 0f, 0.22f));
            HideMenuText("Title");
            StyleMenuLabel();
            StyleMenuButton(playButton, new Vector2(0f, -182f));
            StyleMenuButton(continueButton, new Vector2(0f, -244f));
            StyleMenuButton(difficultyButton, new Vector2(0f, -306f));
            StyleMenuButton(quitButton, new Vector2(0f, -368f));
        }

        private void EnsureDifficultyPresentation()
        {
            if (difficultyPanel == null)
            {
                return;
            }

            EnsurePanelKeyArt(difficultyPanel.transform, "DifficultyKeyArt");
            EnsurePanelShade(difficultyPanel.transform, "DifficultyShade", new Color(0f, 0f, 0f, 0.34f));
            StylePanelTitle(difficultyPanel.transform, "Title", new Vector2(0f, -104f), new Vector2(760f, 54f), 32);
            StyleMenuButton(easyButton, new Vector2(0f, -178f));
            StyleMenuButton(normalButton, new Vector2(0f, -240f));
            StyleMenuButton(hardButton, new Vector2(0f, -302f));
            StyleMenuButton(insaneButton, new Vector2(0f, -364f));
        }

        private void EnsurePanelKeyArt(Transform parent, string objectName)
        {
            if (parent.Find(objectName) != null)
            {
                return;
            }

            Sprite sprite = LoadLoadingArtSprite();
            if (sprite == null)
            {
                return;
            }

            GameObject artObject = new GameObject(objectName);
            artObject.transform.SetParent(parent, false);
            artObject.transform.SetAsFirstSibling();

            RectTransform rect = artObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = artObject.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
        }

        private static void EnsurePanelShade(Transform parent, string objectName, Color color)
        {
            Transform existing = parent.Find(objectName);
            Image image;
            if (existing == null)
            {
                GameObject shadeObject = new GameObject(objectName);
                shadeObject.transform.SetParent(parent, false);
                shadeObject.transform.SetSiblingIndex(1);

                RectTransform rect = shadeObject.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                image = shadeObject.AddComponent<Image>();
                image.raycastTarget = false;
            }
            else
            {
                image = existing.GetComponent<Image>();
            }

            if (image != null)
            {
                image.color = color;
            }
        }

        private void HideMenuText(string childName)
        {
            Transform child = menuPanel.transform.Find(childName);
            if (child != null)
            {
                child.gameObject.SetActive(false);
            }
        }

        private void StyleMenuLabel()
        {
            if (selectedDifficultyLabel == null)
            {
                return;
            }

            RectTransform rect = selectedDifficultyLabel.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(0f, -118f);
                rect.sizeDelta = new Vector2(520f, 42f);
            }

            selectedDifficultyLabel.fontSize = 24;
            selectedDifficultyLabel.color = new Color32(244, 223, 164, 255);

            Outline outline = selectedDifficultyLabel.GetComponent<Outline>();
            if (outline == null)
            {
                outline = selectedDifficultyLabel.gameObject.AddComponent<Outline>();
            }

            outline.effectColor = new Color(0f, 0f, 0f, 0.88f);
            outline.effectDistance = new Vector2(2f, -2f);
        }

        private static void StyleMenuButton(Button button, Vector2 anchoredPosition)
        {
            if (button == null)
            {
                return;
            }

            RectTransform rect = button.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = anchoredPosition;
                rect.sizeDelta = new Vector2(370f, 54f);
            }

            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = new Color32(244, 223, 164, 245);
            }

            ColorBlock colors = button.colors;
            colors.normalColor = new Color32(244, 223, 164, 245);
            colors.highlightedColor = new Color32(255, 181, 45, 255);
            colors.pressedColor = new Color32(202, 96, 31, 255);
            colors.selectedColor = new Color32(255, 181, 45, 255);
            button.colors = colors;

            Text label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.fontSize = 24;
                label.color = new Color32(7, 10, 18, 255);
            }

            Outline outline = button.GetComponent<Outline>();
            if (outline == null)
            {
                outline = button.gameObject.AddComponent<Outline>();
            }

            outline.effectColor = new Color32(7, 10, 18, 255);
            outline.effectDistance = new Vector2(3f, -3f);
        }

        private static void StylePanelTitle(Transform parent, string childName, Vector2 anchoredPosition, Vector2 size, int fontSize)
        {
            Transform child = parent.Find(childName);
            if (child == null)
            {
                return;
            }

            RectTransform rect = child.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = anchoredPosition;
                rect.sizeDelta = size;
            }

            Text text = child.GetComponent<Text>();
            if (text != null)
            {
                text.fontSize = fontSize;
                text.color = new Color32(244, 223, 164, 255);
            }

            Outline outline = child.GetComponent<Outline>();
            if (outline == null)
            {
                outline = child.gameObject.AddComponent<Outline>();
            }

            outline.effectColor = new Color(0f, 0f, 0f, 0.88f);
            outline.effectDistance = new Vector2(2f, -2f);
        }

        private void ShowDifficulty()
        {
            EnsureDifficultyPresentation();
            SetActive(menuPanel, false);
            SetActive(difficultyPanel, true);
            PlayMenuMusic();
        }

        private void SelectEasy()
        {
            SelectDifficulty(DemoDifficulty.Easy);
        }

        private void SelectNormal()
        {
            SelectDifficulty(DemoDifficulty.Normal);
        }

        private void SelectHard()
        {
            SelectDifficulty(DemoDifficulty.Hard);
        }

        private void SelectInsane()
        {
            SelectDifficulty(DemoDifficulty.Insane);
        }

        private void SelectDifficulty(DemoDifficulty difficulty)
        {
            selectedDifficulty = difficulty;
            UpdateDifficultyLabel();
            ShowMenu();
        }

        private void StartGame()
        {
            waitingForSplashInput = false;
            ApplyDifficulty();
            SetActive(splashPanel, false);
            SetActive(menuPanel, false);
            SetActive(difficultyPanel, false);
            SetActive(gameplayHud, true);
            SetGameplayEnabled(true);
            StopMenuMusic();
            SetGameplayMusicEnabled(true);
            Time.timeScale = 1f;
        }

        private void ApplyDifficulty()
        {
            int health = 5;
            int damage = 1;
            float speed = 2f;

            switch (selectedDifficulty)
            {
                case DemoDifficulty.Easy:
                    health = 7;
                    damage = 1;
                    speed = 1.4f;
                    break;
                case DemoDifficulty.Hard:
                    health = 4;
                    damage = 2;
                    speed = 2.6f;
                    break;
                case DemoDifficulty.Insane:
                    health = 2;
                    damage = 2;
                    speed = 3.4f;
                    break;
            }

            playerHealth?.SetMaxHealth(health);

            if (enemyPatrols != null)
            {
                foreach (EnemyPatrol patrol in enemyPatrols)
                {
                    patrol?.SetSpeed(speed);
                }
            }

            if (enemyCombats != null)
            {
                foreach (EnemyCombat combat in enemyCombats)
                {
                    combat?.SetContactDamage(damage);
                }
            }
        }

        private void SetGameplayEnabled(bool enabled)
        {
            if (gameplayControllers == null)
            {
                return;
            }

            foreach (MonoBehaviour controller in gameplayControllers)
            {
                if (controller != null)
                {
                    controller.enabled = enabled;
                }
            }
        }

        private void EnsureGameplayMusic()
        {
            if (gameplayMusic != null)
            {
                return;
            }

            Transform playerTransform = FindPlayerTransform();
            SaciEncounter saciEncounter = FindAnyObjectByType<SaciEncounter>();
            if (playerTransform == null || saciEncounter == null)
            {
                return;
            }

            gameplayMusic = GetComponent<GameplayMusicProximity>();
            if (gameplayMusic == null)
            {
                gameplayMusic = gameObject.AddComponent<GameplayMusicProximity>();
            }

            gameplayMusic.Configure(playerTransform, saciEncounter.transform);
            gameplayMusic.enabled = false;
        }

        private void SetGameplayMusicEnabled(bool enabled)
        {
            EnsureGameplayMusic();

            if (gameplayMusic != null)
            {
                gameplayMusic.enabled = enabled;
            }
        }

        private static Transform FindPlayerTransform()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            return playerObject != null ? playerObject.transform : null;
        }

        private void UpdateDifficultyLabel()
        {
            if (selectedDifficultyLabel != null)
            {
                selectedDifficultyLabel.text = $"Dificuldade: {DifficultyName(selectedDifficulty)}";
            }
        }

        private static string DifficultyName(DemoDifficulty difficulty)
        {
            switch (difficulty)
            {
                case DemoDifficulty.Easy:
                    return "Fácil";
                case DemoDifficulty.Hard:
                    return "Difícil";
                case DemoDifficulty.Insane:
                    return "Insano";
                default:
                    return "Normal";
            }
        }

        private void WireButtons()
        {
            playButton?.onClick.AddListener(StartGame);
            continueButton?.onClick.AddListener(StartGame);
            difficultyButton?.onClick.AddListener(ShowDifficulty);
            quitButton?.onClick.AddListener(QuitGame);
            easyButton?.onClick.AddListener(SelectEasy);
            normalButton?.onClick.AddListener(SelectNormal);
            hardButton?.onClick.AddListener(SelectHard);
            insaneButton?.onClick.AddListener(SelectInsane);

            EnsureButtonAudio(playButton);
            EnsureButtonAudio(continueButton);
            EnsureButtonAudio(difficultyButton);
            EnsureButtonAudio(quitButton);
            EnsureButtonAudio(easyButton);
            EnsureButtonAudio(normalButton);
            EnsureButtonAudio(hardButton);
            EnsureButtonAudio(insaneButton);
        }

        private void UnwireButtons()
        {
            playButton?.onClick.RemoveListener(StartGame);
            continueButton?.onClick.RemoveListener(StartGame);
            difficultyButton?.onClick.RemoveListener(ShowDifficulty);
            quitButton?.onClick.RemoveListener(QuitGame);
            easyButton?.onClick.RemoveListener(SelectEasy);
            normalButton?.onClick.RemoveListener(SelectNormal);
            hardButton?.onClick.RemoveListener(SelectHard);
            insaneButton?.onClick.RemoveListener(SelectInsane);
        }

        private static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private static void EnsureButtonAudio(Button button)
        {
            if (button != null && button.GetComponent<MenuButtonAudioFeedback>() == null)
            {
                button.gameObject.AddComponent<MenuButtonAudioFeedback>();
            }
        }

        private void EnsureMenuMusic()
        {
            if (menuMusic == null)
            {
                menuMusic = Resources.Load<AudioClip>("Audio/Sombrio Horizonte");
            }

            if (musicSource == null)
            {
                musicSource = GetComponent<AudioSource>();
            }

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
            }

            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.ignoreListenerPause = true;
            musicSource.spatialBlend = 0f;
            musicSource.volume = menuMusicVolume;
        }

        private void PlayMenuMusic()
        {
            EnsureMenuMusic();

            if (musicSource == null || menuMusic == null)
            {
                return;
            }

            if (musicSource.clip != menuMusic)
            {
                musicSource.clip = menuMusic;
            }

            musicSource.volume = menuMusicVolume;

            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }

        private void StopMenuMusic()
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
        }

        private static void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private enum DemoDifficulty
        {
            Easy,
            Normal,
            Hard,
            Insane
        }
    }
}
