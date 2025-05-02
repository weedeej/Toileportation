using MelonLoader;
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.PlayerScripts;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Il2CppSystem.IO;

namespace Toileportation
{
    public class TeleporterUI
    {
        Il2CppAssetBundle bundle = null;
        public void ShowTeleportationUI()
        {
            var loadedBundles = Il2CppAssetBundleManager.GetAllLoadedAssetBundles().ToArray();
            if (loadedBundles.Length == 1) bundle = loadedBundles[0];
            else
            {
                foreach (var loaded in loadedBundles)
                {
                    if (loaded.Contains("assets/toiletportationui.prefab"))
                    {
                        bundle = loaded;
                        break;
                    }
                }
            }
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
            closeBtn.RegisterCallback<ClickEvent>(new Action<ClickEvent>(ev =>
            {
                GameObject.Destroy(instance);
            }));

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
                    Image toiletImage = new Image();
                    toiletImage.sprite = icon;
                    toiletBtn.Add(toiletImage);
                    toiletBtn.RegisterCallback<ClickEvent>(new Action<ClickEvent>(ev =>
                    {
                        var teleportationPoint = toilet.BuildPoint.position;
                        teleportationPoint.y += 2f;
                        Player player = Player.Local;
                        PlayerSingleton<PlayerMovement>.Instance.Teleport(teleportationPoint);
                        player.transform.forward = toilet.transform.forward;
                        player.SendCrouched(true);
                        player.SetCrouchedLocal(true);
                        GameObject.Destroy(instance);
                    }));
                    entryFoldout.contentContainer.Add(toiletEntry);
                }
                scrollView.contentContainer.Add(entry);
            }
        }
    }
}
