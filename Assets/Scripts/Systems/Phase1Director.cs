using System.Collections.Generic;
using LendasDoQuintal.Core;
using LendasDoQuintal.Enemy;
using LendasDoQuintal.Player;
using LendasDoQuintal.UI;
using UnityEngine;

namespace LendasDoQuintal.Systems
{
    public class Phase1Director : MonoBehaviour
    {
        public const float BossArrivalSeconds = 180f;
        public const float SectionWidth = 140f;
        public static readonly string[] AreaNames = { "Quarto da criança", "Sala da avó", "Cozinha", "Varanda", "Quintal e galinheiro", "Poço", "Entrada da mata", "Clareira do Saci" };
        private static readonly string[] RequiredClues = { "carta_avo", "pegadas", "", "", "objeto_avo", "gorro", "folhas" };
        [SerializeField] private Transform player;
        [SerializeField] private ClueSystem clues;
        [SerializeField] private ObjectiveSystem objectives;
        [SerializeField] private ClueMessageSystem messages;
        [SerializeField] private GameObject[] gates;
        [SerializeField] private Phase1Enemy[] enemyTemplates;
        [SerializeField] private SaciBoss boss;
        private readonly List<Phase1Enemy> wave = new List<Phase1Enemy>();
        private HeroSkillTree skills;
        private Health health;
        private PlayerCombat combat;
        private PlayerPlatformMovement movement;
        private HealthBarUI hud;
        private int section;
        private float elapsed;
        private float nextWave;
        private bool started;
        private bool bossStarted;
        private int enemyDamage = 1;
        private float enemySpeedScale = 1f;
        private const string SaveKey = "LendasDoQuintal.Phase1.Checkpoint.v1";
        public bool HasSavedRun => PlayerPrefs.HasKey(SaveKey);
        private bool restoring;
        [System.Serializable]
        private class CheckpointSave
        { public int section; public float elapsed; public string[] clues; public HeroSkillTree.SaveData skills; public int lives = 4; }
        public void ClearSavedRun() { PlayerPrefs.DeleteKey(SaveKey); PlayerPrefs.Save(); }
        public void BeginNewRun()
        {
            ClearSavedRun();
            foreach (var pickup in FindObjectsByType<Phase1Pickup>(FindObjectsInactive.Include)) pickup.gameObject.SetActive(true);
            if (!started) return;
            foreach (var enemy in wave) if (enemy != null) Destroy(enemy.gameObject);
            wave.Clear();
            foreach (var gate in gates) gate.SetActive(true);
            boss.ResetEncounter();
            section = 0; elapsed = 0f; started = false; bossStarted = false;
            clues.ResetClues();
            skills.Restore(new HeroSkillTree.SaveData { nodes = new int[0], points = 1, energy = 40f });
            player.position = new Vector3(2f, -1.7f, 0f);
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
        public void ContinueRun()
        {
            if (!PlayerPrefs.HasKey(SaveKey)) return;
            try
            {
                var save = JsonUtility.FromJson<CheckpointSave>(PlayerPrefs.GetString(SaveKey));
                if (save == null || save.skills == null) return;
                GetComponent<GameFlowController>().SetLives(save.lives > 0 ? save.lives : 4);
                section = Mathf.Clamp(save.section, 0, 7); elapsed = Mathf.Max(save.elapsed, section * BossArrivalSeconds / 7f);
                restoring = true;
                if (save.clues != null) foreach (string clue in save.clues) clues.RegisterClue(clue);
                skills.Restore(save.skills);
                restoring = false;
                for (int i = 0; i < section; i++) gates[i].SetActive(false);
                player.position = new Vector3(section * SectionWidth + 2f, -1.7f, 0f);
            }
            catch (System.Exception exception) { restoring = false; Debug.LogWarning("Checkpoint inválido: " + exception.Message); }
        }
        private void SaveCheckpoint()
        {
            var save = new CheckpointSave { section = section, elapsed = elapsed, clues = clues.Capture(), skills = skills.Capture(), lives = GetComponent<GameFlowController>().LivesRemaining };
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(save)); PlayerPrefs.Save();
        }
        public void SetDifficulty(int damage, float speedScale)
        { enemyDamage = damage; enemySpeedScale = speedScale; boss.SetDifficulty(damage); }
        public float Elapsed => elapsed;
        public int Section => section;

        public void Configure(Transform hero, ClueSystem clueSystem, ObjectiveSystem objectiveSystem,
            ClueMessageSystem messageSystem, GameObject[] barriers, Phase1Enemy[] templates, SaciBoss saci)
        { player = hero; clues = clueSystem; objectives = objectiveSystem; messages = messageSystem; gates = barriers; enemyTemplates = templates; boss = saci; }

        private void Start()
        {
            skills = player.GetComponent<HeroSkillTree>(); health = player.GetComponent<Health>();
            health.KeepOnDeath(); combat = player.GetComponent<PlayerCombat>(); movement = player.GetComponent<PlayerPlatformMovement>();
            hud = FindAnyObjectByType<HealthBarUI>(); clues.Changed += OnClue;
        }
        private void OnDestroy() { if (clues != null) clues.Changed -= OnClue; }
        private void OnClue(int count) { if (!restoring && count > 0) skills.Reward(2, 20f); }

        private void Update()
        {
            if (!combat.enabled || health.IsDead || Time.timeScale <= 0f) return;
            elapsed += Time.deltaTime;
            hud?.SetSpecialValue(skills.Energy / 100f);
            if (!started) { started = true; EnterSection(); }
            if (bossStarted) return;
            if (section >= 7)
            {
                if (player.position.x >= SectionWidth * 7 + 5f)
                { bossStarted = true; boss.Begin(); objectives.SetObjective("Saci: desvie dos avisos e ataque quando ele cansar."); }
                return;
            }
            wave.RemoveAll(e => e == null || !e.Alive);
            float release = BossArrivalSeconds * (section + 1) / 7f;
            if (elapsed < release - 5f && Time.time >= nextWave && wave.Count < 2) SpawnWave();
            bool clueReady = RequiredClues[section] == "" || clues.Contains(RequiredClues[section]);
            if (elapsed >= release && wave.Count == 0 && clueReady)
            {
                gates[section].SetActive(false);
                objectives.SetObjective("Passagem liberada. Siga para " + AreaNames[section + 1] + ".");
                if (player.position.x > (section + 1) * SectionWidth)
                { section++; EnterSection(); }
            }
            else
            {
                string task = !clueReady ? "Examine a pista brilhante com E." : wave.Count > 0 ? "Afaste os encantamentos (" + wave.Count + ")." : "Explore: os encantamentos estão se dissipando.";
                objectives.SetObjective(AreaNames[section] + ": " + task);
            }
        }

        private void EnterSection()
        {
            Vector3 checkpoint = new Vector3(section * SectionWidth + 2f, -1.7f, 0f);
            movement.SetRecoveryPoint(checkpoint);
            health.Heal(2);
            messages.Show(section == 0 ? "Vovó sumiu! Examine as pistas com E. Tab: habilidades; L: especial." :
                "Checkpoint: " + AreaNames[section] + ".");
            if (section < 7) SpawnWave();
            SaveCheckpoint();
        }
        private void SpawnWave()
        {
            int type = section < 2 ? 0 : section < 5 ? 1 : section == 5 ? 2 : 3;
            Phase1Enemy enemy = Instantiate(enemyTemplates[type], new Vector3(
                Mathf.Clamp(player.position.x + 6f, section * SectionWidth + 6f, (section + 1) * SectionWidth - 4f), -1.8f, 0f), Quaternion.identity);
            enemy.Configure((Phase1EnemyKind)type, player, section);
            enemy.SetDifficulty(enemyDamage, enemySpeedScale);
            enemy.gameObject.SetActive(true); wave.Add(enemy);
            nextWave = Time.time + 6f;
        }
        public bool Respawn()
        {
            if (!started || player == null) return false;
            player.position = new Vector3(section * SectionWidth + 2f, -1.7f, 0f);
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            health.Restore(); health.MakeInvulnerable(2f);
            combat.ItemStolen = false;
            foreach (FireProjectile projectile in FindObjectsByType<FireProjectile>()) Destroy(projectile.gameObject);
            if (bossStarted) { boss.ResetEncounter(); bossStarted = false; }
            messages.Show("Você voltou ao checkpoint de " + AreaNames[section] + ".");
            SaveCheckpoint();
            return true;
        }
        private void OnGUI()
        {
            if (!started || !combat.enabled || skills.ShowingTree || Time.timeScale <= 0f) return;
            GUI.Box(new Rect(Screen.width - 330, 15, 315, 112), AreaNames[section] + "\n" +
                "Até o Saci: " + Mathf.CeilToInt(Mathf.Max(0f, 180f - elapsed)) + "s | Pistas " + clues.Count + "/5\n" +
                "Vidas " + GetComponent<GameFlowController>().LivesRemaining + " | Saúde " + health.CurrentHealth + "/" + health.MaxHealth + " | Energia " + Mathf.RoundToInt(skills.Energy) + "/100\n" +
                "Tab: árvore | L: folhas | E: examinar");
        }
    }
}
