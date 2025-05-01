using MelonLoader;
using HarmonyLib;
using ScheduleOne.UI.Shop;
using ScheduleOne.ObjectScripts;
using UnityEngine;
using MelonLoader.Utils;
using S1API.Entities;
using System.Collections;
using Response = S1API.Messaging.Response;
using Player = ScheduleOne.PlayerScripts.Player;
using ScheduleOne.Messaging;
using System.Reflection;

[assembly: MelonInfo(typeof(Toileportation.Core), "Toileportation", "1.0.0", "weedeej", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace Toileportation {
//
    [HarmonyPatch(typeof(ShopListing), nameof(ShopListing.Initialize))]
    public static class GoldenToiletPatch
    {
        public static void Postfix(ShopListing __instance)
        {
            if (__instance.Item.Name != "Golden Toilet") return;
            __instance.Item.Description = "The lost artifact of Zaza.\nWho knows what power resides... \n(Stock resets every week)";
            __instance.OverriddenPrice = 80000f;
            __instance.OverridePrice = true;
            __instance.DefaultStock = 2;
            __instance.RestockRate = ShopListing.ERestockRate.Weekly;
            __instance.SetStock(2, true);
            MelonLogger.Msg("Patched Golden Toilet successfully!");
        }
    }
    [HarmonyPatch(typeof(ShopListing), nameof(ShopListing.Restock))]
    public static class GoldenToiletRestockPatch
    {
        public static void Postfix(ShopListing __instance)
        {
            if (__instance.Item.Name != "Golden Toilet") return;
        }
    }
    [HarmonyPatch(typeof(Toilet), nameof(Toilet.Interacted))]
    public static class ToiletFlushPatch
    {
        public static void Postfix(Toilet __instance)
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
            var toiletNPC = ScheduleOne.NPCs.NPCManager.NPCRegistry.Find((npc) => npc.FirstName == "Golden" && npc.LastName == "Toilet");
            toiletNPC.MSGConversation.SetEntryVisibility(false);
        }

        private static IEnumerator HideResponses()
        {
            yield return new WaitForSeconds(10f);
            var toiletNPC = ScheduleOne.NPCs.NPCManager.NPCRegistry.Find((npc) => npc.FirstName == "Golden" && npc.LastName == "Toilet");
            toiletNPC.MSGConversation.SetResponseContainerVisible(false);
        }
    }
    public class Core : MelonMod
    {
        AssetBundle bundle;
        public override void OnInitializeMelon()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Toileportation.toiletportation.bundle");

            if (stream == null)
            {
                this.Unregister($"AssetBundle stream not found");
                return;
            }
            bundle = AssetBundle.LoadFromStream(stream);
        }
    }
}