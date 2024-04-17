using HarmonyLib;
using PeteTimesSix.WishIKnewThat.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PeteTimesSix.WishIKnewThat.HarmonyPatches
{
    [HarmonyPatch(typeof(Designator_Build), nameof(Designator_Build.GizmoOnGUI))]
    public static class Designator_Build_GizmoOnGUI_Patches
    {
        [HarmonyPrefix]
        public static void Designator_Build_GizmoOnGUI_Prefix(GizmoResult __result, Designator_Build __instance, BuildableDef ___entDef, Vector2 topLeft, float maxWidth)
        {
            if (__instance.PlacingDef.IsUnlockedSoon())
            {
                __instance.Disable("WIKT_gui_message_notUnlockedYet".Translate());
            }
            else
            {
                __instance.Disabled = false;
                __instance.disabledReason = null;
            }
        }

        [HarmonyPostfix]
        public static void Designator_Build_GizmoOnGUI_Postfix(GizmoResult __result, Designator_Build __instance, BuildableDef ___entDef, Vector2 topLeft, float maxWidth)
        {
            if (__instance.PlacingDef.IsUnlockedSoon())
            {
                var width = __instance.GetWidth(maxWidth);
                DrawUnlockedSoonLabel(topLeft, width);
            }
        }

        internal static void DrawUnlockedSoonLabel(Vector2 topLeft, float width)
        {
            Text.Font = GameFont.Tiny;
            var protoLabel = "WIKT_gui_label_unlockedSoon".Translate();
            var height = Text.CalcHeight(protoLabel, width);
            var labelRect = new Rect(topLeft.x + 2f, topLeft.y + 22f, width - 4f, height);
            GUI.color = Colors.OrangishTransparentBG;
            GUI.DrawTexture(labelRect, TexUI.GrayTextBG);
            Text.Anchor = TextAnchor.UpperCenter;
            GUI.color = Colors.OrangishTransparent;
            Widgets.Label(labelRect, protoLabel);
            Text.Anchor = TextAnchor.UpperLeft;

            Text.Font = GameFont.Small;
            GUI.color = Color.white;
        }
    }

    [HarmonyPatch(typeof(Designator_Dropdown), nameof(Designator_Dropdown.GizmoOnGUI))]
    public static class Designator_Dropdown_GizmoOnGUI_Patches
    {
        [HarmonyPrefix]
        public static void Designator_Dropdown_GizmoOnGUI_Prefix(GizmoResult __result, Designator_Dropdown __instance, Designator ___activeDesignator, Vector2 topLeft, float maxWidth)
        {
            if (___activeDesignator != null && ___activeDesignator is Designator_Place placeDesignator)
            {
                if (placeDesignator.PlacingDef.IsUnlockedSoon())
                {
                    __instance.Disable("WIKT_gui_message_notUnlockedYet".Translate());
                }
                else
                {
                    __instance.Disabled = false;
                    __instance.disabledReason = null;
                }
            }
        }


        [HarmonyPostfix]
        public static void Designator_Dropdown_GizmoOnGUI_Postfix(GizmoResult __result, Designator_Dropdown __instance, Designator ___activeDesignator, Vector2 topLeft, float maxWidth)
        {
            if (___activeDesignator != null && ___activeDesignator is Designator_Place placeDesignator)
            {
                if (placeDesignator.PlacingDef.IsUnlockedSoon())
                {
                    var width = __instance.GetWidth(maxWidth);
                    Designator_Build_GizmoOnGUI_Patches.DrawUnlockedSoonLabel(topLeft, width);
                }
            }
        }
    }
}
