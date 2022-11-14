using System.Collections;
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

        public void Update()
        {
            if (Time.realtimeSinceStartup > 30f)
                return;
            if (
                Screen.currentResolution.width != OptionsScreen.CurrentWidth
                || Screen.currentResolution.height != OptionsScreen.CurrentHeight
            ) {
                L.LogWarning("Found real solution != options solution. Adjusting.");
                OptionsScreen.SetResolution();
            }
        }
    }
}
