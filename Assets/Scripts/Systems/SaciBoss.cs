using LendasDoQuintal.Core;
using LendasDoQuintal.Enemy;
using LendasDoQuintal.Player;
using UnityEngine;

namespace LendasDoQuintal.Systems
{
    [RequireComponent(typeof(Health))]
    public class SaciBoss : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private GameFlowController flow;
        [SerializeField] private ClueMessageSystem messages;
        [SerializeField] private GameObject leftBarrier;
        private Health health;
        private SpriteRenderer visual;
        private PlayerCombat combat;
        private bool fighting;
        private float nextAction;
        private float resolveAt;
        private float vulnerableUntil;
        private float stolenUntil;
        private int pattern;
        private bool telegraph;
        private Vector3 origin;
        private Vector3 aimedPosition;
        private int attackDamage = 1;
        public bool Fighting => fighting;
        public void SetDifficulty(int damage) { attackDamage = damage; }

        public void Configure(Transform hero, GameFlowController gameFlow, ClueMessageSystem dialogue, GameObject barrier)
        { player = hero; flow = gameFlow; messages = dialogue; leftBarrier = barrier; }
        private void Awake()
        {
            health = GetComponent<Health>(); visual = GetComponent<SpriteRenderer>(); origin = transform.position;
            health.KeepOnDeath(); health.Died += Won;
        }
        private void OnDestroy() { if (health != null) health.Died -= Won; }
        public void Begin()
        {
            combat = player.GetComponent<PlayerCombat>(); fighting = true; nextAction = Time.time + 3f;
            GetComponent<Collider2D>().enabled = true;
            stolenUntil = 0f; vulnerableUntil = 0f;
            leftBarrier.SetActive(true); messages.Show("Saci: Sua avó guarda um segredo! Desvie do vento, depois venha me pegar!");
        }
        public void ResetEncounter()
        {
            fighting = false; telegraph = false; pattern = 0; health.Restore();
            GetComponent<Collider2D>().enabled = false;
            stolenUntil = 0f;
            transform.position = origin; visual.color = Color.white; leftBarrier.SetActive(false);
            if (combat != null) combat.ItemStolen = false;
        }
        private void Update()
        {
            if (!fighting || Time.timeScale <= 0f || health.IsDead) return;
            combat.ItemStolen = Time.time < stolenUntil;
            bool secondPhase = health.CurrentHealth <= health.MaxHealth / 2;
            if (Time.time >= vulnerableUntil) health.MakeInvulnerable(Time.deltaTime + 0.05f);
            visual.color = telegraph ? Color.yellow : Time.time < vulnerableUntil ? new Color(0.5f, 1f, 0.5f) : Color.white;
            if (!telegraph && Time.time >= nextAction)
            {
                telegraph = true; resolveAt = Time.time + (secondPhase ? 0.65f : 1f);
                aimedPosition = player.position;
                messages.Show(pattern % 3 == 0 ? "Saci: Olha o redemoinho!" : pattern % 3 == 1 ? "Saci: Agora você me vê..." : "Saci: Vou esconder seu amuleto por 3 segundos!");
            }
            if (telegraph && Time.time >= resolveAt)
            {
                telegraph = false;
                if (pattern % 3 == 0) ShootWind(secondPhase);
                else if (pattern % 3 == 1)
                {
                    float arenaStart = Phase1Director.SectionWidth * 7f;
                    float x = Mathf.Clamp(aimedPosition.x + (pattern % 2 == 0 ? -3f : 3f), arenaStart + 7f, arenaStart + 36f);
                    transform.position = new Vector3(x, -1.8f, 0f);
                    if (Vector2.Distance(player.position, transform.position) < 1.5f) DamagePlayer();
                }
                else stolenUntil = Time.time + 3f;
                pattern++; vulnerableUntil = Time.time + 2.8f;
                nextAction = vulnerableUntil + (secondPhase ? 0.6f : 1.4f);
            }
        }
        private void ShootWind(bool strong)
        {
            GameObject wind = new GameObject("RedemoinhoSaci"); wind.transform.position = transform.position + Vector3.up * 0.3f;
            var sr = wind.AddComponent<SpriteRenderer>(); sr.sprite = FireProjectile.CreateFireSprite(); sr.color = Color.green; sr.sortingOrder = 18;
            var collider = wind.AddComponent<CircleCollider2D>(); collider.isTrigger = true; collider.radius = 0.3f;
            Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>());
            wind.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            wind.AddComponent<FireProjectile>().Launch((aimedPosition - wind.transform.position).normalized, strong ? 7f : 4f, attackDamage, 5f);
        }
        private void DamagePlayer()
        { var victim = player.GetComponent<Health>(); victim.TakeDamage(attackDamage); victim.MakeInvulnerable(0.8f); }
        private void Won()
        {
            fighting = false; combat.ItemStolen = false; leftBarrier.SetActive(false);
            messages.Show("Saci: Sua avó é uma guardiã! Siga as vozes do rio. Ela deixou este mapa para você.");
            flow.EndPrototype();
        }
        private void OnGUI()
        {
            if (!fighting || Time.timeScale <= 0f) return;
            GUI.Box(new Rect(Screen.width / 2f - 215, Screen.height - 92, 430, 75),
                "Saci-Pererê — " + health.CurrentHealth + "/" + health.MaxHealth + "\n" +
                (telegraph ? "Ataque chegando: esquive!" : Time.time < vulnerableUntil ? "Cansado: ataque agora!" : "Observe o próximo ataque.") +
                (combat.ItemStolen ? "\nAmuleto escondido: use a esquiva!" : ""));
        }
    }
}
