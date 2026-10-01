using System;
using System.IO;
using System.Reflection;
using LendasDoQuintal.Core;
using LendasDoQuintal.Enemy;
using LendasDoQuintal.Player;
using LendasDoQuintal.Systems;
using UnityEditor;
using UnityEngine;

namespace LendasDoQuintal.Editor
{
    // Batch entry point without -quit. Tests run after real Awake/Start calls in Play Mode.
    [InitializeOnLoad]
    public static class Phase1Validation
    {
        private const string Pending = "Phase1.Validation.Pending";
        private static double runAfter;
        static Phase1Validation()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(Pending, false))
                { runAfter = EditorApplication.timeSinceStartup + 1d; EditorApplication.update += RunWhenReady; }
            };
        }
        [MenuItem("Lendas do Quintal/Validate Phase 1 Gameplay")]
        public static void BuildAndValidate()
        {
            if (Application.isBatchMode) PlayerSettings.productName = "LendasDoQuintal Phase1 Validation";
            MvpSceneBuilder.BuildPhase1Scene();
            SessionState.SetBool(Pending, true);
            EditorApplication.isPlaying = true;
        }
        private static void Assert(bool condition, string message)
        { if (!condition) throw new Exception(message); }
        private static void Set(object target, string field, object value)
        { target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value); }
        private static void Call(object target, string method)
        { target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null); }
        private static void FinishAnimatedDeath(GameFlowController flow)
        {
            Assert(flow.IsResolvingDeath && Time.timeScale == 0f, "Death did not suspend gameplay for animation");
            int lives = flow.LivesRemaining;
            Call(flow, "OnPlayerDied");
            Assert(flow.LivesRemaining == lives, "Duplicate death consumed a life during animation");
            // Rules test completes the transition explicitly; DeathAnimationValidation covers real playback timing.
            flow.StopAllCoroutines(); flow.SetLives(lives - 1); Call(flow, "FinishPlayerDeath");
        }
        private static void RunWhenReady()
        {
            if (EditorApplication.timeSinceStartup < runAfter) return;
            EditorApplication.update -= RunWhenReady;
            SessionState.SetBool(Pending, false);
            string report;
            int exitCode = 0;
            try
            {
                var phase = UnityEngine.Object.FindAnyObjectByType<Phase1Director>();
                var menu = UnityEngine.Object.FindAnyObjectByType<DemoFlowController>();
                var player = GameObject.FindGameObjectWithTag("Player");
                var health = player.GetComponent<Health>();
                var skills = player.GetComponent<HeroSkillTree>();
                var clues = UnityEngine.Object.FindAnyObjectByType<ClueSystem>();
                var boss = UnityEngine.Object.FindAnyObjectByType<SaciBoss>();
                Assert(phase != null && boss != null, "Scene references missing");
                Call(phase, "Update"); Assert(phase.Elapsed == 0f, "Menu advanced gameplay clock");
                Call(menu, "StartGame");
                Assert(UnityEngine.Object.FindAnyObjectByType<GameplayMusicProximity>().enabled, "Gameplay music was not enabled");
                Assert(!skills.Unlock(3), "Special ignored prerequisite");
                Assert(skills.Unlock(1), "Root skill could not unlock");
                skills.Reward(3, 60f); Assert(skills.Unlock(3), "Special could not unlock after prerequisite");
                Assert(!skills.Unlock(3), "Skill purchased twice");
                skills.Reward(9, 0); Assert(skills.Unlock(0), "Health root did not unlock");
                int upgradedHealth = health.MaxHealth;
                skills.Restore(skills.Capture()); skills.Restore(skills.Capture());
                Assert(health.MaxHealth == upgradedHealth, "Repeated skill restore stacked health bonuses");
                Assert(UnityEngine.Object.FindObjectsByType<SpriteRenderer>().Length > 300, "Phase scenery unexpectedly sparse");
                Call(phase, "Update");
                string[] ids = { "carta_avo", "pegadas", "", "", "objeto_avo", "gorro", "folhas" };
                for (int i = 0; i < 7; i++)
                {
                    var gate = GameObject.Find("Encantamento_" + i);
                    Assert(gate != null && gate.activeSelf, "Section gate missing");
                    float release = 180f * (i + 1) / 7f;
                    Set(phase, "elapsed", release - 1f); Set(phase, "nextWave", float.MaxValue);
                    foreach (var enemy in UnityEngine.Object.FindObjectsByType<Phase1Enemy>())
                        enemy.GetComponent<Health>().TakeDamage(1000);
                    player.transform.position = new Vector3((i + 1) * Phase1Director.SectionWidth + 2f, -1.7f, 0f);
                    Call(phase, "Update"); Assert(gate.activeSelf, "Gate bypassed 180-second pacing");
                    Set(phase, "elapsed", release + 0.1f);
                    if (ids[i] != "")
                    {
                        Call(phase, "Update"); Assert(gate.activeSelf, "Mandatory clue bypassed");
                        Assert(clues.RegisterClue(ids[i]), "Clue registration failed");
                        int points = skills.Points; Assert(!clues.RegisterClue(ids[i]) && points == skills.Points, "Repeated clue farmed points");
                    }
                    Call(phase, "Update"); Assert(!gate.activeSelf && phase.Section == i + 1, "Section did not advance");
                }
                Assert(clues.Count == 5 && phase.Elapsed >= 180f && !boss.Fighting, "Invalid boss arrival state");
                int max = health.MaxHealth;
                var flow = UnityEngine.Object.FindAnyObjectByType<GameFlowController>();
                health.TakeDamage(max); Assert(flow.LivesRemaining == 4, "Life decremented before animation ended");
                FinishAnimatedDeath(flow); Assert(!health.IsDead && health.CurrentHealth == max, "Checkpoint respawn failed");
                Assert(flow.LivesRemaining == 3 && Time.timeScale > 0f, "First death did not preserve gameplay with three lives");
                Vector3 checkpointPosition = player.transform.position;
                for (int death = 0; death < 2; death++)
                {
                    Set(health, "invulnerableUntil", 0f);
                    player.transform.position += Vector3.right * 10f;
                    health.TakeDamage(max);
                    FinishAnimatedDeath(flow);
                    Assert(player.transform.position == checkpointPosition && !health.IsDead && Time.timeScale > 0f, "Remaining lives did not return to checkpoint");
                }
                Assert(flow.LivesRemaining == 1, "Lives were not decremented once per death");
                player.transform.position = new Vector3(Phase1Director.SectionWidth * 7f + 7f, -1.7f, 0f);
                Call(phase, "Update"); Assert(boss.Fighting, "Boss did not begin");
                Call(boss, "Update"); int bossLife = boss.GetComponent<Health>().CurrentHealth;
                boss.GetComponent<Health>().TakeDamage(1);
                Assert(boss.GetComponent<Health>().CurrentHealth == bossLife, "Boss shield failed");
                boss.ResetEncounter(); Assert(!boss.Fighting && !player.GetComponent<PlayerCombat>().ItemStolen, "Boss retry failed");
                phase.ContinueRun(); Assert(phase.Section == 7 && clues.Count == 5 && skills.SpecialUnlocked, "Checkpoint save/load failed");
                Assert(flow.LivesRemaining == 1, "Continue restored consumed lives");
                Set(health, "invulnerableUntil", 0f); health.TakeDamage(health.MaxHealth);
                FinishAnimatedDeath(flow);
                Assert(flow.LivesRemaining == 0 && Time.timeScale == 0f && !player.GetComponent<PlayerCombat>().enabled, "Fourth death did not return to selection");
                var selection = (GameObject)menu.GetType().GetField("difficultyPanel", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(menu);
                Assert(selection.activeSelf, "Difficulty selection did not appear on fourth death");
                var counter = (UnityEngine.UI.Text)flow.GetType().GetField("livesLabel", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(flow);
                Assert(counter != null && counter.text == "Vidas: 0", "HUD lives counter missing or stale");
                Assert(!PlayerPrefs.HasKey("LendasDoQuintal.Phase1.Checkpoint.v1"), "Exhausted run remained available in Continue");
                var pickup = UnityEngine.Object.FindAnyObjectByType<Phase1Pickup>(FindObjectsInactive.Include);
                pickup.gameObject.SetActive(false);
                phase.BeginNewRun();
                Assert(pickup.gameObject.activeSelf && skills.Points == 1 && !skills.SpecialUnlocked && health.MaxHealth == upgradedHealth - 2, "New run retained consumed fruit or skill health bonuses");
                flow.BeginRun(); Time.timeScale = 1f;
                Set(boss.GetComponent<Health>(), "invulnerableUntil", 0f);
                boss.Begin();
                boss.GetComponent<Health>().TakeDamage(1000);
                Assert(!boss.Fighting && Time.timeScale == 0f, "Victory did not finish the phase");
                foreach (SpriteRenderer renderer in UnityEngine.Object.FindObjectsByType<SpriteRenderer>())
                    Assert(renderer.sprite != null, "Missing serialized sprite: " + renderer.name);
                report = "PASS: compilation, scene references, paused clock, skill prerequisites/costs, duplicate purchases, all seven timed gates, mandatory/duplicate clues, 180-second boss arrival, four lives, three checkpoint respawns, fourth-death selection, saved remaining lives, exhausted-save invalidation, boss shield/retry, narrative victory, serialized sprites.\n";
            }
            catch (Exception exception) { report = "FAIL: " + exception + "\n"; exitCode = 1; Debug.LogError(report); }
            Directory.CreateDirectory(".codex-build/phase1-check"); File.WriteAllText(".codex-build/phase1-check/gameplay-validation.txt", report);
            if (Application.isBatchMode) EditorApplication.Exit(exitCode);
            else { Debug.Log(report); EditorApplication.isPlaying = false; }
        }
    }
}
