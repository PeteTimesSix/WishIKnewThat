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
using static HarmonyLib.AccessTools;

namespace PeteTimesSix.WishIKnewThat.HarmonyPatches
{
    [HarmonyPatch(typeof(BillStack), nameof(BillStack.DoListing))]
    public static class BillStack_DoListing_Patches
    {
        public delegate FloatMenuOption GenerateSurgeryOptionDelegate(Pawn pawn, Thing thingForMedBills, RecipeDef recipe, IEnumerable<ThingDef> missingIngredients, AcceptanceReport report, int index, BodyPartRecord part = null);

        public static GenerateSurgeryOptionDelegate GenerateSurgeryOption;
        public static FieldRef<FloatMenuOption, string> getFloatMenuOption;

        static BillStack_DoListing_Patches()
        {
            GenerateSurgeryOption = AccessTools.MethodDelegate<GenerateSurgeryOptionDelegate>(AccessTools.Method(typeof(HealthCardUtility), "GenerateSurgeryOption"));
            getFloatMenuOption = AccessTools.FieldRefAccess<string>(typeof(FloatMenuOption), "labelInt");
        }

        [HarmonyPrefix]
        public static void BillStack_DoListing_Prefix(BillStack __instance, ref Func<List<FloatMenuOption>> recipeOptionsMaker)
        {
            var oldRecipeOptionsMaker = recipeOptionsMaker;
            Func<List<FloatMenuOption>> newRecipeOptionsMaker = () =>
            {
                var vanillaResult = oldRecipeOptionsMaker();
                var billGiver = __instance.billGiver;
                if (billGiver is Pawn pawnBillGiver)
                {
                    var surgeries = GetSoonAvailableSurgeryOptions(pawnBillGiver, pawnBillGiver);
                    if (!surgeries.NullOrEmpty())
                    {
                        if (vanillaResult.Count == 1 && vanillaResult[0].Label == "NoneBrackets".Translate())
                            vanillaResult.RemoveAt(0);
                        vanillaResult.AddRange(surgeries);
                    }
                }
                else
                {
                    var recipes = GetSoonAvailableRecipeOptions(billGiver);
                    if (!recipes.NullOrEmpty())
                    {
                        if (vanillaResult.Count == 1 && vanillaResult[0].Label == "NoneBrackets".Translate())
                            vanillaResult.RemoveAt(0);
                        vanillaResult.AddRange(recipes);
                    }
                }
                return vanillaResult;
            };
            recipeOptionsMaker = newRecipeOptionsMaker;
        }

        public static List<FloatMenuOption> GetSoonAvailableSurgeryOptions(Pawn pawn, Thing thingForMedBills)
        {
            if (WishIKnewThat_Settings.surgeryVisibility == VisibilityRequirement.Off)
                return new List<FloatMenuOption>();

            List<FloatMenuOption> list = new List<FloatMenuOption>();
            int index = 0;
            foreach (RecipeDef recipe in thingForMedBills.def.AllRecipes)
            {
                if (recipe.IsUnlockedSoon(true))
                {
                    AcceptanceReport report = recipe.Worker.AvailableReport(pawn, null);
                    if (report.Accepted || !report.Reason.NullOrEmpty())
                    {
                        var missingIngredients = recipe.PotentiallyMissingIngredients(null, thingForMedBills.MapHeld);
                        if (!missingIngredients.Any((ThingDef x) => x.isTechHediff))
                        {
                            if (!missingIngredients.Any((ThingDef x) => x.IsDrug) && (!missingIngredients.Any() || !recipe.dontShowIfAnyIngredientMissing))
                            {
                                if (recipe.targetsBodyPart)
                                {
                                    foreach (var part in recipe.Worker.GetPartsToApplyOn(pawn, recipe))
                                    {
                                        if (recipe.AvailableOnNow(pawn, part))
                                        {
                                            //Log.Message($"pawn {pawn}/{thingForMedBills} adding {recipe} on part {part}");
                                            var option = GenerateSurgeryOption(pawn, thingForMedBills, recipe, missingIngredients, report, index, part);
                                            getFloatMenuOption(option) = "WIKT_gui_unlockedSoonPrefix".Translate() + " " + getFloatMenuOption(option);
                                            option.Disabled = true;
                                            list.Add(option);
                                            index++;
                                        }
                                    }
                                }
                                else
                                {
                                    //Log.Message($"pawn {pawn}/{thingForMedBills} adding {recipe}");
                                    var option = GenerateSurgeryOption(pawn, thingForMedBills, recipe, missingIngredients, report, index, null);
                                    getFloatMenuOption(option) = "WIKT_gui_unlockedSoonPrefix".Translate() + " " + getFloatMenuOption(option);
                                    option.Disabled = true;
                                    list.Add(option);
                                    index++;
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }

        public static List<FloatMenuOption> GetSoonAvailableRecipeOptions(IBillGiver billGiver)
        {
            if (WishIKnewThat_Settings.recipeVisibility == VisibilityRequirement.Off)
                return new List<FloatMenuOption>();

            var asThing = billGiver as Thing;
            if (asThing == null)
                return null;

            var retList = new List<FloatMenuOption>();

            foreach (var recipe in asThing.def.AllRecipes)
            {
                if (recipe.IsUnlockedSoon(false) && recipe.AvailableOnNow(asThing, null))
                {
                    var option = new FloatMenuOption("WIKT_gui_unlockedSoonPrefix".Translate() + " " + recipe.LabelCap, () => OnClick(billGiver, asThing, recipe, null), recipe.UIIconThing, extraPartWidth: 29f, extraPartOnGUI: (Rect rect) => ExtraPartOnGUI(rect, recipe, null));
                    option.Disabled = true;
                    retList.Add(option);
                    foreach (Ideo ideo in Faction.OfPlayer.ideos.AllIdeos)
                    {
                        foreach (Precept_Building precept_Building in ideo.cachedPossibleBuildings)
                        {
                            if (precept_Building.ThingDef == recipe.ProducedThingDef)
                            {
                                var preceptOption = new FloatMenuOption("WIKT_gui_unlockedSoonPrefix".Translate() + " " + "RecipeMake".Translate(precept_Building.def.LabelCap).CapitalizeFirst(), () => OnClick(billGiver, asThing, recipe, precept_Building), recipe.UIIconThing, extraPartWidth: 29f, extraPartOnGUI: (Rect rect) => ExtraPartOnGUI(rect, recipe, precept_Building));
                                option.Disabled = true; 
                                retList.Add(option);
                            }
                        }
                    }
                }
            }

            return retList;
        }

        public static void OnClick(IBillGiver asBillGiver, Thing asThing, RecipeDef recipe, Precept_ThingStyle precept)
        {
            if (recipe.conceptLearned != null)
            {
                PlayerKnowledgeDatabase.KnowledgeDemonstrated(recipe.conceptLearned, KnowledgeAmount.Total);
            }
        }

        public static bool ExtraPartOnGUI(Rect rect, RecipeDef recipe, Precept_ThingStyle precept)
        {
            return Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, recipe, precept);
        }
    }
}
