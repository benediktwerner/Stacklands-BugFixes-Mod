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
        [HarmonyPatch(typeof(Demon), nameof(Demon.Die))]
        public static void AllowMoreThan1DemonSwordDrop(Demon __instance)
        {
            __instance.Drops.Chances.ForEach(c => c.HasMaxCount = false);
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
