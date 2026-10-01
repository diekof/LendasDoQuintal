using System.Collections.Generic;
using LendasDoQuintal.Core;
using LendasDoQuintal.Systems;
using UnityEngine;

namespace LendasDoQuintal.Player
{
    // Run-local progression. A new game starts a fresh tree.
    public class HeroSkillTree : MonoBehaviour
    {
        private readonly HashSet<int> unlocked = new HashSet<int>();
        private readonly string[] names = { "Coragem: +2 vida", "Punho firme: +1 dano", "Passo leve: esquiva rápida", "Tempestade de folhas: especial", "Raízes fortes: +2 vida", "Vendaval: especial ampliado" };
        private readonly int[] parents = { -1, -1, -1, 1, 0, 3 };
        private readonly int[] costs = { 1, 1, 1, 1, 2, 2 };
        private PlayerCombat combat;
        private PlayerPlatformMovement movement;
        private Health health;
        private int baseMaxHealth;
        public void SetBaseHealth(int value) { baseMaxHealth = value; }
        private int experience;
        private bool showingTree;
        private float specialEffectUntil;
        private SpriteRenderer leafEffect;
        public int Points { get; private set; } = 1;
        public float Energy { get; private set; } = 40f;
        public bool ShowingTree => showingTree;
        public bool SpecialUnlocked => unlocked.Contains(3);
        [System.Serializable]
        public class SaveData { public int[] nodes; public int points; public int experience; public float energy; }
        public SaveData Capture() => new SaveData { nodes = new List<int>(unlocked).ToArray(), points = Points, experience = experience, energy = Energy };
        public void Restore(SaveData data)
        {
            if (data == null) return;
            unlocked.Clear();
            if (data.nodes != null) foreach (int id in data.nodes) if (id >= 0 && id < names.Length) unlocked.Add(id);
            Points = Mathf.Max(0, data.points); experience = Mathf.Clamp(data.experience, 0, 2); Energy = Mathf.Clamp(data.energy, 0f, 100f);
            combat.DamageBonus = unlocked.Contains(1) ? 1 : 0;
            movement.RollCooldownMultiplier = unlocked.Contains(2) ? 0.55f : 1f;
            health.SetMaxHealth(baseMaxHealth + (unlocked.Contains(0) ? 2 : 0) + (unlocked.Contains(4) ? 2 : 0));
        }

        private void Awake()
        {
            combat = GetComponent<PlayerCombat>();
            movement = GetComponent<PlayerPlatformMovement>();
            health = GetComponent<Health>();
            baseMaxHealth = health.MaxHealth;
        }

        public void Reward(int xp, float energy)
        {
            experience += xp;
            Energy = Mathf.Min(100f, Energy + energy);
            while (experience >= 3) { experience -= 3; Points++; }
        }

        public bool Unlock(int id)
        {
            if (id < 0 || id >= names.Length || unlocked.Contains(id) || Points < costs[id] ||
                (parents[id] >= 0 && !unlocked.Contains(parents[id]))) return false;
            Points -= costs[id];
            unlocked.Add(id);
            if (id == 0 || id == 4) health.SetMaxHealth(health.MaxHealth + 2, false);
            if (id == 1) combat.DamageBonus = 1;
            if (id == 2) movement.RollCooldownMultiplier = 0.55f;
            return true;
        }

        private void Update()
        {
            if (health.IsDead || !combat.enabled) return;
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.JoystickButton6))
            {
                if (showingTree) { showingTree = false; Time.timeScale = 1f; }
                else if (Time.timeScale > 0f) { showingTree = true; Time.timeScale = 0f; }
            }
            if (showingTree)
            {
                for (int i = 0; i < names.Length; i++)
                    if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i))) Unlock(i);
                return;
            }
            if (Time.timeScale <= 0f) return;
            if (leafEffect != null)
            {
                leafEffect.transform.Rotate(0f, 0f, 400f * Time.deltaTime);
                if (Time.time >= specialEffectUntil) Destroy(leafEffect.gameObject);
            }
            if ((Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.JoystickButton8)) &&
                SpecialUnlocked && Energy >= 40f && !combat.ItemStolen)
            {
                Energy -= 40f;
                float radius = unlocked.Contains(5) ? 4f : 2.7f;
                var damaged = new HashSet<Health>();
                foreach (Collider2D hit in Physics2D.OverlapCircleAll(transform.position, radius))
                    if (hit.TryGetComponent(out Health target) && target != health && damaged.Add(target)) target.TakeDamage(unlocked.Contains(5) ? 4 : 2);
                if (leafEffect != null) Destroy(leafEffect.gameObject);
                GameObject effect = new GameObject("TempestadeDeFolhas");
                effect.transform.SetParent(transform, false);
                leafEffect = effect.AddComponent<SpriteRenderer>();
                leafEffect.sprite = LendasDoQuintal.Enemy.FireProjectile.CreateFireSprite();
                leafEffect.color = new Color(0.3f, 1f, 0.35f, 0.5f);
                leafEffect.sortingOrder = 20;
                effect.transform.localScale = Vector3.one * radius * 3f;
                specialEffectUntil = Time.time + 0.4f;
            }
        }

        private void OnGUI()
        {
            if (!showingTree) return;
            float w = Mathf.Min(680f, Screen.width - 24f);
            Rect box = new Rect((Screen.width - w) / 2, 30, w, 420);
            GUI.Box(box, "Árvore do herói — pontos: " + Points + " — Tab para voltar");
            int[] columns = { 0, 1, 2, 1, 0, 1 };
            int[] rows = { 0, 0, 0, 1, 1, 2 };
            float nodeWidth = (w - 60f) / 3f;
            for (int i = 0; i < names.Length; i++)
            {
                Rect node = new Rect(box.x + 15 + columns[i] * (nodeWidth + 15), box.y + 48 + rows[i] * 110f, nodeWidth, 84);
                if (parents[i] >= 0) GUI.Label(new Rect(node.center.x - 8f, node.y - 25f, 20f, 24f), "↓");
                bool available = !unlocked.Contains(i) && Points >= costs[i] && (parents[i] < 0 || unlocked.Contains(parents[i]));
                GUI.enabled = available;
                if (GUI.Button(node,
                    (i + 1) + ". " + names[i].Replace(": ", ":\n") + "\n" +
                    (unlocked.Contains(i) ? "Desbloqueado" : costs[i] + " ponto(s)"))) Unlock(i);
            }
            GUI.enabled = true;
            GUI.Label(new Rect(box.x + 15, box.y + 384, w - 30, 24), "Escolha as raízes acima; as setas indicam os pré-requisitos. Teclas 1–6 compram nós.");
        }
    }
}
