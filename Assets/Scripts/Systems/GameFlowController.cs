using LendasDoQuintal.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LendasDoQuintal.Systems
{
    public class GameFlowController : MonoBehaviour
    {
        [SerializeField] private Health playerHealth;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject prototypeEndPanel;

        private bool ended;
        public int LivesRemaining { get; private set; } = 4;
        private Vector3 initialSpawn;
        private UnityEngine.UI.Text livesLabel;
        private LendasDoQuintal.UI.PlayerDeathPresentation deathPresentation;
        private MonoBehaviour[] suspendedControllers;
        private bool[] controllerStates;
        private bool bodyWasSimulated;
        private bool spriteWasVisible;
        private float previousTimeScale;
        public bool IsResolvingDeath { get; private set; }

        private void Start()
        {
            if (playerHealth != null) { playerHealth.KeepOnDeath(); initialSpawn = playerHealth.transform.position; }
            var hud = FindAnyObjectByType<LendasDoQuintal.UI.HealthBarUI>(FindObjectsInactive.Include);
            if (hud != null)
            {
                var label = new GameObject("LivesCounter", typeof(RectTransform), typeof(UnityEngine.UI.Text));
                label.transform.SetParent(hud.transform, false);
                var rect = label.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 1f);
                rect.anchoredPosition = new Vector2(400f, -20f); rect.sizeDelta = new Vector2(170f, 40f);
                livesLabel = label.GetComponent<UnityEngine.UI.Text>(); livesLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                livesLabel.fontSize = 24; livesLabel.color = Color.white; livesLabel.raycastTarget = false;
            }
            SetLives(LivesRemaining);
            deathPresentation = gameObject.AddComponent<LendasDoQuintal.UI.PlayerDeathPresentation>();
        }

        public void SetLives(int lives)
        {
            LivesRemaining = Mathf.Clamp(lives, 0, 4);
            if (livesLabel != null) livesLabel.text = "Vidas: " + LivesRemaining;
        }

        public void BeginRun()
        {
            ended = false; SetLives(4);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (prototypeEndPanel != null) prototypeEndPanel.SetActive(false);
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void Update()
        {
            if (ended && InputReader.RestartPressed())
            {
                RestartScene();
            }
        }

        private void OnDisable()
        {
            if (playerHealth != null)
            {
                playerHealth.Died -= OnPlayerDied;
            }
        }

        public void EndPrototype()
        {
            ended = true;
            Time.timeScale = 0f;

            if (prototypeEndPanel != null)
            {
                prototypeEndPanel.SetActive(true);
            }
        }

        private void OnPlayerDied()
        {
            if (IsResolvingDeath || LivesRemaining <= 0 || ended) return;
            IsResolvingDeath = true;
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            suspendedControllers = new MonoBehaviour[] {
                playerHealth.GetComponent<LendasDoQuintal.Player.PlayerPlatformMovement>(),
                playerHealth.GetComponent<LendasDoQuintal.Player.PlayerCombat>(),
                playerHealth.GetComponent<LendasDoQuintal.Player.PlayerInteraction>(),
                playerHealth.GetComponent<LendasDoQuintal.Player.PlayerSpriteAnimator>() };
            controllerStates = new bool[suspendedControllers.Length];
            for (int i = 0; i < suspendedControllers.Length; i++)
                if (suspendedControllers[i] != null) { controllerStates[i] = suspendedControllers[i].enabled; suspendedControllers[i].enabled = false; }
            var body = playerHealth.GetComponent<Rigidbody2D>();
            if (body != null) { bodyWasSimulated = body.simulated; body.linearVelocity = Vector2.zero; body.simulated = false; }
            var sprite = playerHealth.GetComponent<SpriteRenderer>();
            if (sprite != null) { spriteWasVisible = sprite.enabled; sprite.enabled = false; }
            StartCoroutine(PlayDeathSequence());
        }

        private System.Collections.IEnumerator PlayDeathSequence()
        {
            int before = LivesRemaining;
            yield return deathPresentation.Play(before);
            SetLives(before - 1);
            yield return deathPresentation.ShowLifeLost(before, LivesRemaining);
            FinishPlayerDeath();
        }

        private void FinishPlayerDeath()
        {
            if (!IsResolvingDeath) return;
            deathPresentation.Hide();
            for (int i = 0; i < suspendedControllers.Length; i++) if (suspendedControllers[i] != null) suspendedControllers[i].enabled = controllerStates[i];
            var playerBody = playerHealth.GetComponent<Rigidbody2D>(); if (playerBody != null) playerBody.simulated = bodyWasSimulated;
            var sprite = playerHealth.GetComponent<SpriteRenderer>(); if (sprite != null) sprite.enabled = spriteWasVisible;
            IsResolvingDeath = false;
            Time.timeScale = previousTimeScale;
            Phase1Director phase = GetComponent<Phase1Director>();
            if (LivesRemaining > 0)
            {
                if (phase != null && phase.Respawn()) return;
                var checkpoint = GetComponent<CheckpointSystem>();
                if (checkpoint != null) checkpoint.Respawn(); else playerHealth.transform.position = initialSpawn;
                var body = playerHealth.GetComponent<Rigidbody2D>(); if (body != null) body.linearVelocity = Vector2.zero;
                playerHealth.Restore(); playerHealth.MakeInvulnerable(2f);
                return;
            }
            phase?.ClearSavedRun();
            var menu = GetComponent<DemoFlowController>();
            if (menu != null) { menu.ReturnToSelection(); return; }
            ended = true;
            Time.timeScale = 0f;

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }

        private static void RestartScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Configure(Health newPlayerHealth, GameObject newGameOverPanel, GameObject newPrototypeEndPanel)
        {
            if (playerHealth != null)
            {
                playerHealth.Died -= OnPlayerDied;
            }

            playerHealth = newPlayerHealth;
            gameOverPanel = newGameOverPanel;
            prototypeEndPanel = newPrototypeEndPanel;
            ended = false;
            Subscribe();
        }

        private void Subscribe()
        {
            if (playerHealth != null)
            {
                playerHealth.Died -= OnPlayerDied;
                playerHealth.Died += OnPlayerDied;
            }
        }
    }
}
