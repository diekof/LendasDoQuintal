using LendasDoQuintal.Core;
using LendasDoQuintal.Enemy;
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
        [SerializeField] private Button easyButton;
        [SerializeField] private Button normalButton;
        [SerializeField] private Button hardButton;
        [SerializeField] private Button insaneButton;
        [SerializeField] private Text selectedDifficultyLabel;
        [SerializeField] private Health playerHealth;
        [SerializeField] private EnemyPatrol[] enemyPatrols;
        [SerializeField] private EnemyCombat[] enemyCombats;
        [SerializeField] private MonoBehaviour[] gameplayControllers;

        private DemoDifficulty selectedDifficulty = DemoDifficulty.Normal;
        private bool waitingForSplashInput;

        private void Start()
        {
            EnsureEventSystem();
            WireButtons();
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
            waitingForSplashInput = true;
            Time.timeScale = 0f;
            SetGameplayEnabled(false);
            SetActive(splashPanel, true);
            SetActive(menuPanel, false);
            SetActive(difficultyPanel, false);
            SetActive(gameplayHud, false);
        }

        private void ShowMenu()
        {
            waitingForSplashInput = false;
            Time.timeScale = 0f;
            SetGameplayEnabled(false);
            SetActive(splashPanel, false);
            SetActive(menuPanel, true);
            SetActive(difficultyPanel, false);
            SetActive(gameplayHud, false);
            UpdateDifficultyLabel();
        }

        private void ShowDifficulty()
        {
            SetActive(menuPanel, false);
            SetActive(difficultyPanel, true);
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
            easyButton?.onClick.AddListener(SelectEasy);
            normalButton?.onClick.AddListener(SelectNormal);
            hardButton?.onClick.AddListener(SelectHard);
            insaneButton?.onClick.AddListener(SelectInsane);
        }

        private void UnwireButtons()
        {
            playButton?.onClick.RemoveListener(StartGame);
            continueButton?.onClick.RemoveListener(StartGame);
            difficultyButton?.onClick.RemoveListener(ShowDifficulty);
            easyButton?.onClick.RemoveListener(SelectEasy);
            normalButton?.onClick.RemoveListener(SelectNormal);
            hardButton?.onClick.RemoveListener(SelectHard);
            insaneButton?.onClick.RemoveListener(SelectInsane);
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
            if (FindFirstObjectByType<EventSystem>() != null)
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
