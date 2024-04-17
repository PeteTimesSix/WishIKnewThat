using PeteTimesSix.WishIKnewThat.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PeteTimesSix.WishIKnewThat.Extensions
{
    public static class BuildableDefExtensions
    {
        public static bool IsUnlockedSoon(this BuildableDef def)
        {
            var vis = WishIKnewThat_Settings.designatorVisibility;
            if (vis == VisibilityRequirement.Off)
                return false;

            if (def.researchPrerequisites != null && def.researchPrerequisites.Count > 0)
            {
                var unfinishedPreregsDirect = def.researchPrerequisites.Where((ResearchProjectDef r) => !r.IsFinished).ToList();
                if (unfinishedPreregsDirect.Count == 0)
                    return false; //already unlocked


                if (vis.HasFlag(VisibilityRequirement.Always))
                    return true;

                var maxDepth = WishIKnewThat_Settings.designatorMaxDepth;
                var maxCount = WishIKnewThat_Settings.designatorMaxCount;

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
