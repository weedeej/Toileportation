using MelonLoader;
using ScheduleOne;
using ScheduleOne.DevUtilities;
using ScheduleOne.PlayerScripts;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Toileportation
{
    public class TeleporterUI
    {
        AssetBundle bundle = null;

        public TeleporterUI()
        {
            var loadedBundles = AssetBundle.GetAllLoadedAssetBundles().ToList();
            foreach (var loaded in loadedBundles)
            {
                if (loaded.name == "toiletportation.bundle")
                {
                    bundle = loaded;
                    break;
                }
            }
        }

        public void ShowTeleportationUI()
        {
            MelonCoroutines.Start(ShowUICoro());
        }

        public IEnumerator ShowUICoro()
        {
            yield return new WaitForEndOfFrame();
            GameObject instance = bundle.LoadAsset<GameObject>("assets/toiletportationui.prefab");
            instance = GameObject.Instantiate(instance);
            UIDocument doc = instance.GetComponent<UIDocument>();
            VisualElement rootVisual = doc.rootVisualElement;
            ScrollView scrollView = rootVisual.Q<ScrollView>("propertiesContainer");
            Button closeBtn = rootVisual.Q<Button>("closeBtn");
            closeBtn.RegisterCallback<ClickEvent>(ev =>
            {
                GameObject.Destroy(instance);
            });

            VisualTreeAsset entryTemplate = bundle.LoadAsset<VisualTreeAsset>("assets/entry.uxml");
            VisualTreeAsset toiletEntryTemplate = bundle.LoadAsset<VisualTreeAsset>("assets/goldentoiletentry.uxml");

            var goldenToilets = ModUtilities.GetAllGoldenToilet();
            Sprite icon = Registry.GetItem("goldentoilet").Icon;

            foreach (var property in goldenToilets)
            {
                if (property.Value.Length == 0) continue;
                var entry = entryTemplate.CloneTree();
                var entryFoldout = entry.Q<Foldout>("entry");
                entryFoldout.text = property.Key;
                foreach (var toilet in property.Value)
                {
                    var toiletEntry = toiletEntryTemplate.CloneTree();
                    var toiletBtn = toiletEntry.Q<Button>("goldenToilet");
                    toiletBtn.style.backgroundImage = new StyleBackground
                    {
                        value = new Background { sprite = icon }
                    };
                    toiletBtn.RegisterCallback<ClickEvent>(ev =>
                    {
                        var teleportationPoint = toilet.BuildPoint.position;
                        teleportationPoint.y += 2f;
                        Player player = Player.Local;
                        PlayerSingleton<PlayerMovement>.Instance.Teleport(teleportationPoint);
                        player.transform.forward = toilet.transform.forward;
                        player.SendCrouched(true);
                        player.SetCrouchedLocal(true);
                        GameObject.Destroy(instance);
                    });
                    entryFoldout.contentContainer.Add(toiletEntry);
                }
                scrollView.contentContainer.Add(entry);
            }
        }
    }
}
