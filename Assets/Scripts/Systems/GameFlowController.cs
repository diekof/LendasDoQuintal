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

        private void OnEnable()
        {
            if (playerHealth != null)
            {
                playerHealth.Died += OnPlayerDied;
            }
        }

        private void Update()
        {
            if (ended && InputReader.RestartPressed())
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

            if (prototypeEndPanel != null)
            {
                prototypeEndPanel.SetActive(true);
            }
        }

        private void OnPlayerDied()
        {
            ended = true;

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }

        public void Configure(Health newPlayerHealth, GameObject newGameOverPanel, GameObject newPrototypeEndPanel)
        {
            playerHealth = newPlayerHealth;
            gameOverPanel = newGameOverPanel;
            prototypeEndPanel = newPrototypeEndPanel;
        }
    }
}
