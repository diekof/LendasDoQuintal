using System;
using System.IO;
using System.Reflection;
using LendasDoQuintal.Core;
using LendasDoQuintal.Player;
using LendasDoQuintal.Systems;
using LendasDoQuintal.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LendasDoQuintal.Editor
{
    [InitializeOnLoad]
    public static class DeathAnimationValidation
    {
        private static int stage;
        private static int deaths;
        private static double due;
        private static int frameAtStart;
        private static float phaseTime;
        static DeathAnimationValidation()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool("DeathAnimation.Validate", false))
                { stage = 0; deaths = 0; due = EditorApplication.timeSinceStartup + 2; EditorApplication.update += Tick; }
            };
        }
        [MenuItem("Lendas do Quintal/Validate Death Animation")]
        public static void Run()
        {
            if (Application.isBatchMode) PlayerSettings.productName = "Lendas Death Animation Validation";
            EditorSceneManager.OpenScene("Assets/Scenes/LendasDoQuintal_Phase1.unity");
            SessionState.SetBool("DeathAnimation.Validate", true); EditorApplication.isPlaying = true;
        }
        private static void Check(bool condition, string error) { if (!condition) throw new Exception(error); }
        private static void Tick()
        {
            if (EditorApplication.timeSinceStartup < due) return;
            try
            {
                var flow = UnityEngine.Object.FindAnyObjectByType<GameFlowController>();
                var phase = UnityEngine.Object.FindAnyObjectByType<Phase1Director>();
                var menu = UnityEngine.Object.FindAnyObjectByType<DemoFlowController>();
                var player = GameObject.FindGameObjectWithTag("Player");
                var health = player.GetComponent<Health>();
                var presentation = UnityEngine.Object.FindAnyObjectByType<PlayerDeathPresentation>();
                if (stage == 0)
                {
                    var sheet = Resources.Load<Texture2D>("HeroDeath/pajama_walk");
                    Check(sheet != null && sheet.width == 192 && sheet.height == 192 && sheet.filterMode == FilterMode.Point, "PixelLab sheet was resized or filtered: expected 192x192 point sampling");
                    menu.GetType().GetMethod("StartGame", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(menu, null);
                    stage = 1; due = EditorApplication.timeSinceStartup + 1; return;
                }
                if (stage == 1)
                {
                    health.GetType().GetField("invulnerableUntil", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(health, 0f);
                    phaseTime = phase.Elapsed; health.TakeDamage(1000); frameAtStart = presentation.CurrentFrame;
                    Check(flow.IsResolvingDeath && flow.LivesRemaining == 4 - deaths && health.IsDead, "Death did not wait for animation");
                    Check(Time.timeScale == 0 && !player.GetComponent<PlayerCombat>().enabled && !player.GetComponent<Rigidbody2D>().simulated, "Gameplay remained active during death");
                    stage = 2; due = EditorApplication.timeSinceStartup + 1.1; return;
                }
                if (stage == 2)
                {
                    Check(presentation.IsVisible && presentation.CurrentFrame != frameAtStart, "Animation frames did not advance during pause");
                    Check(phase.Elapsed == phaseTime && flow.LivesRemaining == 4 - deaths, "Clock/lives advanced before animation completion");
                    if (deaths == 0 && !SystemInfo.graphicsDeviceType.Equals(UnityEngine.Rendering.GraphicsDeviceType.Null)) Capture();
                    stage = 3; due = EditorApplication.timeSinceStartup + 2.5; return;
                }
                Check(!flow.IsResolvingDeath && !presentation.IsVisible && flow.LivesRemaining == 3 - deaths, "Animation did not finish with exactly one life lost");
                deaths++;
                if (deaths < 4)
                {
                    Check(!health.IsDead && Time.timeScale > 0 && player.GetComponent<PlayerCombat>().enabled && player.GetComponent<Rigidbody2D>().simulated, "Checkpoint did not resume gameplay");
                    stage = 1; due = EditorApplication.timeSinceStartup + 0.1; return;
                }
                var selection = (GameObject)menu.GetType().GetField("difficultyPanel", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(menu);
                Check(selection.activeSelf && Time.timeScale == 0, "Last animation did not return to selection");
                Finish("PASS: four real-time death animations, frame playback while paused, controls/physics suspended, clock frozen, delayed single life decrement, three checkpoint returns, fourth-death selection.", 0);
            }
            catch (Exception exception) { Finish("FAIL: " + exception, 1); }
        }
        private static void Capture()
        {
            var camera = UnityEngine.Camera.main; var target = new RenderTexture(1280, 720, 24); camera.targetTexture = target;
            foreach (var canvas in UnityEngine.Object.FindObjectsByType<Canvas>()) { canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1; }
            Canvas.ForceUpdateCanvases(); camera.Render(); RenderTexture.active = target;
            var texture = new Texture2D(1280, 720, TextureFormat.RGB24, false); texture.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); texture.Apply();
            Directory.CreateDirectory("output/gameplay"); File.WriteAllBytes("output/gameplay/death-transition.png", texture.EncodeToPNG());
            camera.targetTexture = null; RenderTexture.active = null; UnityEngine.Object.Destroy(texture); UnityEngine.Object.Destroy(target);
        }
        private static void Finish(string report, int code)
        {
            EditorApplication.update -= Tick; SessionState.SetBool("DeathAnimation.Validate", false);
            Directory.CreateDirectory(".codex-build/phase1-check"); File.WriteAllText(".codex-build/phase1-check/death-animation-validation.txt", report);
            if (Application.isBatchMode) EditorApplication.Exit(code); else { Debug.Log(report); EditorApplication.isPlaying = false; }
        }
    }
}
