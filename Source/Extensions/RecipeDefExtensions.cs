using PeteTimesSix.WishIKnewThat.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PeteTimesSix.WishIKnewThat.Extensions
{
    public static class RecipeDefExtensions
    {
        public static bool IsUnlockedSoon(this RecipeDef def, bool isSurgery)
        {
            var vis = isSurgery ? WishIKnewThat_Settings.surgeryVisibility : WishIKnewThat_Settings.recipeVisibility;
            if (vis == VisibilityRequirement.Off)
                return false;

            var preregs = new List<ResearchProjectDef>();
            if (def.researchPrerequisite != null)
                preregs.Add(def.researchPrerequisite);
            if (def.researchPrerequisites != null)
                preregs.AddRange(def.researchPrerequisites);

            if (preregs.Count > 0)
            {
                var unfinishedPreregsDirect = preregs.Where((ResearchProjectDef r) => !r.IsFinished).ToList();
                if (unfinishedPreregsDirect.Count == 0)
                    return false; //already unlocked

                if (vis.HasFlag(VisibilityRequirement.Always))
                    return true;

                var maxDepth = isSurgery ? WishIKnewThat_Settings.surgeryMaxDepth : WishIKnewThat_Settings.surgeryMaxDepth;
                var maxCount = isSurgery ? WishIKnewThat_Settings.surgeryMaxCount : WishIKnewThat_Settings.surgeryMaxCount;

                if (vis.HasFlag(VisibilityRequirement.Count) && unfinishedPreregsDirect.Count > maxCount) 
                    return false; // early count check

                var unfinishedPreregsIndirect = ResearchPreregUtils.PreregsDFS(unfinishedPreregsDirect, vis.HasFlag(VisibilityRequirement.Depth) ? maxDepth : null, vis.HasFlag(VisibilityRequirement.Count) ? maxCount : null, out bool wentOver);
                if (wentOver)
                    return false; // depth check and total count check

                return true;
            }
            return false;
        }
    }
}
