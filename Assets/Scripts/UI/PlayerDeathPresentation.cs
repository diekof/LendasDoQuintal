using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LendasDoQuintal.UI
{
    public class PlayerDeathPresentation : MonoBehaviour
    {
        private GameObject overlay;
        private Image hero;
        private Text counter;
        private Sprite[] frames;
        public bool IsVisible => overlay != null && overlay.activeSelf;
        public int CurrentFrame { get; private set; }

        private void Awake()
        {
            Texture2D sheet = Resources.Load<Texture2D>("HeroDeath/pajama_walk");
            if (sheet != null)
            {
                sheet.filterMode = FilterMode.Point;
                frames = new Sprite[8];
                for (int i = 0; i < frames.Length; i++)
                    frames[i] = Sprite.Create(sheet, new Rect(i % 3 * 64, sheet.height - (i / 3 + 1) * 64, 64, 64), new Vector2(0.5f, 0.5f), 64f);
            }
            else Debug.LogError("Death animation missing: Resources/HeroDeath/pajama_walk");
            overlay = new GameObject("DeathAnimation", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            overlay.transform.SetParent(transform, false);
            var canvas = overlay.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 200;
            var scaler = overlay.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = 0.5f;
            var backdrop = MakeImage("Shade", overlay.transform);
            backdrop.color = new Color(0.025f, 0.04f, 0.08f, 0.93f);
            backdrop.rectTransform.anchorMin = Vector2.zero; backdrop.rectTransform.anchorMax = Vector2.one;
            backdrop.rectTransform.offsetMin = backdrop.rectTransform.offsetMax = Vector2.zero;
            MakeText("Title", "Hora de um leitinho...", new Vector2(0, 160), 34);
            counter = MakeText("Lives", "", new Vector2(0, -190), 30);
            hero = MakeImage("PajamaHero", overlay.transform); hero.rectTransform.sizeDelta = new Vector2(256, 256); hero.preserveAspect = true;
            overlay.SetActive(false);
        }
        private static Image MakeImage(string name, Transform parent)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Image)); obj.transform.SetParent(parent, false);
            return obj.GetComponent<Image>();
        }
        private Text MakeText(string name, string text, Vector2 position, int size)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Text)); obj.transform.SetParent(overlay.transform, false);
            var rect = obj.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(900, 70); rect.anchoredPosition = position;
            var label = obj.GetComponent<Text>(); label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = text; label.fontSize = size; label.alignment = TextAnchor.MiddleCenter; label.color = new Color32(244, 223, 164, 255); label.raycastTarget = false;
            return label;
        }
        public IEnumerator Play(int lives)
        {
            overlay.SetActive(true); counter.text = "Vidas: " + lives;
            const float duration = 2.4f;
            float time = 0;
            while (time < duration)
            {
                CurrentFrame = Mathf.FloorToInt(time / 0.12f) % 8;
                if (frames != null) hero.sprite = frames[CurrentFrame];
                hero.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(-180, 180, time / duration), -15);
                time += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        public IEnumerator ShowLifeLost(int before, int after)
        {
            counter.text = "Vidas: " + before + " → " + after + (after == 0 ? "\nFim das tentativas" : "\nVoltando ao checkpoint...");
            yield return new WaitForSecondsRealtime(0.7f);
        }
        public void Hide() { if (overlay != null) overlay.SetActive(false); }
        private void OnDestroy() { if (frames != null) foreach (var frame in frames) if (frame != null) Destroy(frame); }
    }
}
