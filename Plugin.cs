using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
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

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Boosterpack), nameof(Boosterpack.Clicked))]
        public static IEnumerable<CodeInstruction> NoWaitForSecondsInSpecialEvents(
            IEnumerable<CodeInstruction> instructions
        )
        {
            var matcher = new CodeMatcher(instructions).MatchForward(
                false,
                new CodeMatch(OpCodes.Ldloc_2),
                new CodeMatch(OpCodes.Ldc_I4_S, (sbyte)10),
                new CodeMatch(OpCodes.Bne_Un)
            );
            if (matcher.IsValid)
                matcher.Advance(2).SetOpcodeAndAdvance(OpCodes.Ble_Un);
            else
                L.LogWarning("Failed to find boughtBoosters == 10 check");
            return matcher.InstructionEnumeration();
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
                L.LogWarning("Found real resolution != options resolution. Adjusting.");
                OptionsScreen.SetResolution();
            }

            if (WorldManager.instance?.CurrentRunVariables != null)
            {
                WorldManager.instance.CurrentRunVariables.CanDropItem = true;
            }
        }
    }
}
