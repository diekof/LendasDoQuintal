using UnityEngine;

namespace LendasDoQuintal.Systems
{
    public class CheckpointSystem : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Vector3 spawnPoint;

        private void Start()
        {
            if (player != null && spawnPoint == Vector3.zero)
            {
                spawnPoint = player.position;
            }
        }

        public void SetCheckpoint(Vector3 position)
        {
            spawnPoint = position;
        }

        public void Respawn()
        {
            if (player != null)
            {
                player.position = spawnPoint;
            }
        }

        public void Configure(Transform newPlayer)
        {
            player = newPlayer;
            spawnPoint = newPlayer.position;
        }
    }
}
