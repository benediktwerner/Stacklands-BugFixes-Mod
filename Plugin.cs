using System.Collections;
using System.Collections.Generic;
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
        [HarmonyPatch(typeof(Combatable), nameof(Combatable.PerformSpecialHit))]
        public static void FixHealLowestPrefix(SpecialHit specialHit, out SpecialHitTarget __state)
        {
            __state = specialHit.Target;
            if (specialHit.HitType == SpecialHitType.HealLowest && specialHit.Target == SpecialHitTarget.Target)
            {
                specialHit.Target = SpecialHitTarget.RandomFriendly;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combatable), nameof(Combatable.PerformSpecialHit))]
        public static void FixHealLowestPostfix(SpecialHit specialHit, SpecialHitTarget __state)
        {
            specialHit.Target = __state;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Cutscenes), nameof(Cutscenes.IslandIntroPack))]
        public static void NoSecondIslandPackFromDemon(out bool __runOriginal, out IEnumerator __result)
        {
            __result = EndCutscenes();
            __runOriginal = !WorldManager.instance.CurrentSaveGame.GotIslandIntroPack;
        }

        public static IEnumerator EndCutscenes()
        {
            Cutscenes.Stop();
            yield break;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Combatable), nameof(Combatable.StartOrJoinConflictInStack))]
        public static void JoinWholeStackInCombat(Combatable __instance, out bool __runOriginal)
        {
            __runOriginal = false;
            var conflict = __instance.GetConflictInStack();
            if (conflict == null)
                conflict = Conflict.StartConflict(__instance);
            __instance
                .CardsInStackMatchingPredicate((CardData x) => x is Combatable c && !c.InConflict)
                .ForEach(c => conflict.JoinConflict(c as Combatable));
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Combatable), nameof(Combatable.StoppedDragging))]
        public static void JoinWholeStackInCombat2(Combatable __instance, out GameCard __state)
        {
            __state = __instance.MyGameCard.Child;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Combatable), nameof(Combatable.StoppedDragging))]
        public static void JoinWholeStackInCombat3(Combatable __instance, GameCard __state)
        {
            var conflict = __instance.MyConflict;
            if (conflict == null)
                return;

            var child = __state;
            while (child != null)
            {
                var nxt = child.Child;
                if (child.CardData is Combatable c && !c.InConflict)
                    conflict.JoinConflict(c);
                child = nxt;
            }
        }

        public void Update()
        {
            if (Time.realtimeSinceStartup > 30f)
                return;
            if (
                Screen.currentResolution.width != OptionsScreen.CurrentWidth
                || Screen.currentResolution.height != OptionsScreen.CurrentHeight
            )
            {
                L.LogWarning("Found real solution != options solution. Adjusting.");
                OptionsScreen.SetResolution();
            }
        }
    }
}
