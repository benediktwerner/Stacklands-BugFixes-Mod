using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace BugFixes
{
    [BepInPlugin("de.benediktwerner.stacklands.bugfixes", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ManualLogSource L;

        private void Awake()
        {
            L = Logger;
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Demon), nameof(Demon.Die))]
        public static void AllowMoreThan1DemonSwordDrop(Demon __instance)
        {
            __instance.Drops.Chances.ForEach(c => c.HasMaxCount = false);
        }

        [HarmonyPatch(typeof(Conveyor), nameof(Conveyor.OutputCardAllowed))]
        [HarmonyPostfix]
        private static void Crane__CheckStatusOnOutput(
            GameCard gameCard,
            CardData inputCardDataPrefab,
            ref bool __result
        )
        {
            if (!__result)
                return;

            GameCard cardWithStatusInStack = gameCard.GetCardWithStatusInStack();
            if (cardWithStatusInStack != null && !cardWithStatusInStack.CardData.CanHaveCardsWhileHasStatus())
                __result = false;
        }

        [HarmonyPatch(typeof(Conveyor), nameof(Conveyor.SendToTargetCard))]
        [HarmonyPostfix]
        private static void Conveyor_BounceToRoot(GameCard card)
        {
            card.BounceTarget = card.BounceTarget.GetRootCard();
        }

        [HarmonyPatch(typeof(Portal), nameof(Portal.RemoveNonHuman))]
        [HarmonyPrefix]
        private static void Portal_RemoveNonHuman_DontRemoveGlue(Portal __instance, out bool __runOriginal)
        {
            // Portals already only allow non-human cards anyway
            __runOriginal = false;
        }

        [HarmonyPatch(typeof(CardData), nameof(CardData.AddStatusEffect), new[] { typeof(StatusEffect) })]
        [HarmonyPrefix]
        private static void CardData_AddStatusEffect_AllowExistingWellFed(CardData __instance, StatusEffect effect)
        {
            if (effect.GetType() == typeof(StatusEffect_WellFed))
                __instance.StatusEffects.RemoveAll(e => e.GetType() == typeof(StatusEffect_WellFed));
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Sawmill), nameof(Sawmill.CompleteMaking))]
        [HarmonyPatch(typeof(Brickyard), nameof(Brickyard.CompleteMaking))]
        private static void DontDetachFromGlueAfterProducing_Prefix(CardData __instance, out GameCard __state)
        {
            __state = __instance.MyGameCard.Parent;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Sawmill), nameof(Sawmill.CompleteMaking))]
        [HarmonyPatch(typeof(Brickyard), nameof(Brickyard.CompleteMaking))]
        private static void DontDetachFromGlueAfterProducing_Postfix(CardData __instance, GameCard __state)
        {
            __instance.MyGameCard.SetParent(__state);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Conveyor), nameof(Conveyor.GetInputCardConveyablePrefab))]
        private static void GetInputCardConveyablePrefab_ReturnOrigInsteadOfPrefabToFixArrows(
            Conveyor __instance,
            CardData card,
            out CardData __result,
            out bool __runOriginal
        )
        {
            __runOriginal = false;

            if (card is ResourceChest resourceChest)
                __result = __instance.GetPrefabForId(resourceChest.HeldCardId);
            else if (card is ResourceMagnet resourceMagnet)
                __result = __instance.GetPrefabForId(resourceMagnet.PullCardId);
            else if (__instance.CanBeConveyed(card))
                __result = card;
            else
                __result = null;
        }

        public void Update()
        {
            if (
                Screen.currentResolution.width != OptionsScreen.CurrentWidth
                || Screen.currentResolution.height != OptionsScreen.CurrentHeight
            )
            {
                L.LogWarning("Found real resolution != options resolution. Adjusting.");
                OptionsScreen.SetResolution();
            }
        }
    }
}
