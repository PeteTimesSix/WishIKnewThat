using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static HarmonyLib.AccessTools;
using UnityEngine;
using Verse;
using Verse.Sound;
using PeteTimesSix.WishIKnewThat.Extensions;

namespace PeteTimesSix.WishIKnewThat.ModCompat.DubsMintMenus
{
    public static class DMM_BillStack_DoListing_Patches
    {
        public delegate void DoRowDelegate(RecipeDef recipe, HashSet<Building> selectedTables, Precept_Building shittyPrecept = null);

        public static DoRowDelegate method_DoRow;
        public static FieldRef<Rect> GizmoListRect { get; set; }

        public static IEnumerable<CodeInstruction> Doink_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            var type = AccessTools.TypeByName("DubsMintMenus.Patch_BillStack_DoListing");
            var listerField = type.GetField("lister", BindingFlags.Static | BindingFlags.NonPublic);
            method_DoRow = type.GetMethod("DoRow").CreateDelegate(typeof(DoRowDelegate)) as DoRowDelegate;

            var closeout_instructions = new CodeMatch[] {
                new CodeMatch(OpCodes.Ldsfld, listerField),
                new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Listing), nameof(Listing.CurHeight))),
                new CodeMatch(OpCodes.Stsfld, type.GetField("RecipesScrollHeight", BindingFlags.Static | BindingFlags.NonPublic)),
                new CodeMatch(OpCodes.Ldsfld, listerField),
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Listing), nameof(Listing.End))),
                new CodeMatch(OpCodes.Ret),
            };

            var add_unlocked_soon_instructions = new CodeInstruction[] {
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DMM_BillStack_DoListing_Patches), nameof(AddUnlockedSoonRows))),
                new CodeInstruction(OpCodes.Ldsfld, listerField),
            };

            codeMatcher.MatchEndForward(closeout_instructions);

            if (codeMatcher.IsInvalid)
            {
                Log.Warning("WIKT: DMM_BillStack_DoListing_Patches failed to apply Doink patch (instructions not found)");
                return instructions;
            }

            codeMatcher.Advance(1);
            codeMatcher.Insert(add_unlocked_soon_instructions);

            return codeMatcher.Instructions();
        }

        public static IEnumerable<CodeInstruction> DoRow_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            var invisbutton_instructions = new CodeMatch[] {
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Ldc_I4_1),
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Widgets), nameof(Widgets.ButtonInvisible))),
                new CodeMatch(OpCodes.Brfalse)
            };

            var new_jump = new CodeInstruction(OpCodes.Brfalse);
            var check_run_instructions = new CodeInstruction[] {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DMM_BillStack_DoListing_Patches), nameof(ShouldDoNormalButton))),
                new_jump,
            };


            codeMatcher.MatchEndForward(invisbutton_instructions);


            if (codeMatcher.IsInvalid)
            {
                Log.Warning("WIKT: DMM_BillStack_DoListing_Patches failed to apply Doink patch (instructions not found)");
                return instructions;
            }

            //copy jump target
            new_jump.operand = codeMatcher.Instruction.operand;
            codeMatcher.Advance(-(invisbutton_instructions.Count()));
            codeMatcher.Insert(check_run_instructions);

            return codeMatcher.Instructions();
        }

        private static bool staticHack_shouldDoNormalButton = true;
        public static bool ShouldDoNormalButton()
        {
            return staticHack_shouldDoNormalButton;
        }

        private static void AddUnlockedSoonRows(Listing_Standard lister, HashSet<Building> selectedTables)
        {
            HashSet<RecipeDef> prototypeRecipes = new HashSet<RecipeDef>();
            foreach (var building in selectedTables)
            {
                IBillGiver billGiver = building as IBillGiver;
                if (billGiver == null)
                    continue;

                foreach (var recipe in building.def.AllRecipes)
                {
                    if (recipe.IsUnlockedSoon(false) && recipe.AvailableOnNow(building, null))
                    {
                        prototypeRecipes.Add(recipe);
                    }
                }
            }
            foreach (var recipe in prototypeRecipes)
            {
                DoPrototypeRow(lister, selectedTables, recipe, null);
                foreach (Ideo ideo in Faction.OfPlayer.ideos.AllIdeos)
                {
                    foreach (Precept_Building precept_Building in ideo.cachedPossibleBuildings)
                    {
                        if (precept_Building.ThingDef == recipe.ProducedThingDef)
                        {
                            DoPrototypeRow(lister, selectedTables, recipe, precept_Building);
                        }
                    }
                }
            }

            return;
        }

        private static void DoPrototypeRow(Listing_Standard lister, HashSet<Building> selectedTables, RecipeDef recipe, Precept_Building precept_Building)
        {
            var yBefore = lister.CurHeight;
            staticHack_shouldDoNormalButton = false;
            method_DoRow(recipe, selectedTables, null);
            staticHack_shouldDoNormalButton = true;
            var yAfter = lister.CurHeight;
            var height = yAfter - yBefore;

            var rect = lister.GetRect(0);
            var actualRect = new Rect(rect.x, rect.y - height, rect.width, height);

            if (!actualRect.Overlaps(GizmoListRect.Invoke()))
            {
                return;
            }

            var font = Text.Font;
            var anchor = Text.Anchor;
            var color = GUI.color;

            Text.Font = GameFont.Small;
            var protoLabel = "RR_PrototypeLabel".Translate();
            var labelRect = new Rect(actualRect.x + 2f, actualRect.y, actualRect.width - 4f, 20f);
            GUI.color = Colors.OrangishTransparentBG;
            GUI.DrawTexture(labelRect, TexUI.GrayTextBG);
            Text.Anchor = TextAnchor.UpperCenter;
            GUI.color = Colors.OrangishTransparent;
            Widgets.Label(labelRect, protoLabel);

            Text.Font = font;
            Text.Anchor = anchor;
            GUI.color = color;

            if (Widgets.ButtonInvisible(actualRect))
            {
                if (selectedTables.Any())
                {
                    foreach (Building building in selectedTables)
                    {
                        IBillGiver billGiver = building as IBillGiver;
                        if (building.def.AllRecipes.Contains(recipe))
                        {
                            billGiver.BillStack.AddBill(recipe.MakeNewBill(precept_Building));
                        }
                    }
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                }
                else
                {
                    Messages.Message("Mint.SelectABenchToAddBills".Translate(), MessageTypeDefOf.NegativeEvent, false);
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                }
            }

            lister.GapLine(2f);
        }
    }
}
