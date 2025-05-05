using MelonLoader;
using HarmonyLib;
using UnityEngine;
using S1API.Entities;
using System.Collections;
using Il2CppScheduleOne.UI.Shop;
using Il2CppScheduleOne.ObjectScripts;
using Response = S1API.Messaging.Response;
using Player = Il2CppScheduleOne.PlayerScripts.Player;
using S1NPC = Il2CppScheduleOne.NPCs.NPC;
using S1NPCManager = Il2CppScheduleOne.NPCs.NPCManager;
using MemoryStream = System.IO.MemoryStream;
using Stream = System.IO.Stream;

[assembly: MelonInfo(typeof(Toileportation.Core), "Toileportation", "1.0.0", "weedeej", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace Toileportation {
    //
    [HarmonyPatch(typeof(ShopListing), nameof(ShopListing.Initialize))]
    public static class GoldenToiletPatch
    {
        public static void Postfix(ShopListing __instance)
        {
            if (__instance.name == null) return;
            if (!__instance.name.Contains("Golden Toilet")) return;
            try
            {
                __instance.Item.Description = "The lost artifact of Zaza.\nWho knows what power resides... \n(Stock resets every week)";
                __instance.OverriddenPrice = 80000f;
                __instance.OverridePrice = true;
                __instance.DefaultStock = 2;
                __instance.RestockRate = ShopListing.ERestockRate.Weekly;
                __instance.SetStock(2, true);
                MelonLogger.Msg("Patched Golden Toilet successfully!");
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Failed to patch Golden Toilet: {e}");
            }
        }
    }
    [HarmonyPatch(typeof(Toilet), nameof(Toilet.Interacted))]
    public static class ToiletFlushPatch
    {
        public static void Prefix(Toilet __instance)
        {
            if (__instance.ItemInstance.Definition.Name != "Golden Toilet") return;
            Player player = Player.Local;
            Vector3 playerPosition = player.Avatar.MiddleSpine.position;
            Vector3 toiletPosition = __instance.BuildPoint.position;
            float distance = Vector3.Distance(playerPosition, toiletPosition);
            float yDiff = playerPosition.y - toiletPosition.y;
            if (playerPosition.y < toiletPosition.y || yDiff <= 1.4 || yDiff >= 1.6 || distance > 1.5 || !player.Crouched) return;
            NPC toiletNPC = NPC.Get<ToiletNPC>();
            Response[] responses = {
                new Response()
                {
                    OnTriggered = () => {
                        toiletNPC.SendTextMessage("And you will be guided.");
                        TeleporterUI ui = new TeleporterUI();
                        ui.ShowTeleportationUI();
                    },
                    Text = "Guide me through",
                },
                new Response()
                {
                    OnTriggered = () => {
                        toiletNPC.SendTextMessage("And so it will be.");
                        MelonCoroutines.Start(HideConversation());
                    },
                    Text = "Leave the tale untold",
                }
            };
            toiletNPC.SendTextMessage("Thou hast uncovered the long-lost secret of the Golden Throne — a relic of legend, whispered of in forgotten tongues and hidden beneath veils of time.\n\nWhat fate shall thee choose?", responses);
            MelonCoroutines.Start(HideResponses());
        }

        private static IEnumerator HideConversation()
        {
            yield return new WaitForSeconds(5f);
            foreach (S1NPC npc in S1NPCManager.NPCRegistry)
            {
                if (npc.FirstName == "Golden" && npc.LastName == "Toilet")
                    npc.MSGConversation.SetEntryVisibility(false);
            }
        }

        private static IEnumerator HideResponses()
        {
            yield return new WaitForSeconds(10f);
            foreach (S1NPC npc in S1NPCManager.NPCRegistry)
            {
                if (npc.FirstName == "Golden" && npc.LastName == "Toilet")
                    npc.MSGConversation.SetResponseContainerVisible(false);
            }
        }
    }
    public class Core : MelonMod
    {
        Il2CppAssetBundle bundle;
        public override void OnInitializeMelon()
        {
            try
            {
                Stream bundleStream = MelonAssembly.Assembly.GetManifestResourceStream("Toileportation.toiletportation.bundle");
                if (bundleStream == null)
                {
                    this.Unregister($"AssetBundle stream not found");
                    return;
                }
                byte[] bundleData;
                using (MemoryStream ms = new MemoryStream())
                {
                    bundleStream.CopyTo(ms);
                    bundleData = ms.ToArray();
                }
                Il2CppSystem.IO.Stream stream = new Il2CppSystem.IO.MemoryStream(bundleData);
                bundle = Il2CppAssetBundleManager.LoadFromStream(stream);
            } catch (Exception e)
            {
                this.Unregister($"Failed to load AssetBundle. Please report to dev: {e}");
                return;
            }
            LoggerInstance.Msg("Initialized Toileportation");
        }
    }
}