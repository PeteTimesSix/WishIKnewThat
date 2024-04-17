using HarmonyLib;
using PeteTimesSix.WishIKnewThat.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PeteTimesSix.WishIKnewThat.HarmonyPatches
{
    [HarmonyPatch(typeof(Designator_Build), nameof(Designator_Build.Visible), MethodType.Getter)]
    public static class Designator_Build_Visible_Patches
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Designator_Build_Visible_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            var research_check_instructions = new CodeMatch[] {
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Designator_Build), "entDef")),
                new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(BuildableDef), nameof(BuildableDef.IsResearchFinished))),
                new CodeMatch(OpCodes.Brtrue_S)
            };

            var jump_new = new CodeInstruction(OpCodes.Brtrue_S);
            var add_unlocked_soon_check_instructions = new CodeInstruction[] {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Designator_Build), "entDef")),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Designator_Build_Visible_Patches), nameof(GetIsUnlockedSoon))),
                jump_new
            };

            codeMatcher.MatchEndForward(research_check_instructions);
            if(codeMatcher.IsInvalid)
            {
                Log.Warning("WIKT: Designator_Build_Visible_Patches - failed to apply patch (instructions not found)");
                return instructions;
            }

            jump_new.operand = codeMatcher.Instruction.operand;
            codeMatcher.Advance(1);

            codeMatcher.Insert(add_unlocked_soon_check_instructions);
            return codeMatcher.Instructions();
        }

        private static bool GetIsUnlockedSoon(BuildableDef buildable)
        {
            return buildable.IsUnlockedSoon();
        }
    }
}
