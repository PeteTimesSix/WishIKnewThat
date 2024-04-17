using PeteTimesSix.WishIKnewThat.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PeteTimesSix.WishIKnewThat
{
    public enum VisibilityRequirement
    {
        Off = 0,
        Always = 1 << 0,
        Depth = 1 << 1,
        Count = 1 << 2,
        DepthAndCount = Depth + Count,
    }

    public class WishIKnewThat_Settings : ModSettings
    {
        private const VisibilityRequirement designatorVisibility_DEFAULT = VisibilityRequirement.Count;
        private const int designatorMaxDepth_DEFAULT = 1;
        private const int designatorMaxCount_DEFAULT = 1;
        public static VisibilityRequirement designatorVisibility = designatorVisibility_DEFAULT;
        public static int designatorMaxDepth = designatorMaxDepth_DEFAULT;
        public static int designatorMaxCount = designatorMaxCount_DEFAULT;

        private const VisibilityRequirement recipeVisibility_DEFAULT = VisibilityRequirement.Count;
        private const int recipeMaxDepth_DEFAULT = 1;
        private const int recipeMaxCount_DEFAULT = 1;
        public static VisibilityRequirement recipeVisibility = recipeVisibility_DEFAULT;
        public static int recipeMaxDepth = recipeMaxDepth_DEFAULT;
        public static int recipeMaxCount = recipeMaxCount_DEFAULT;

        private const VisibilityRequirement surgeryVisibility_DEFAULT = VisibilityRequirement.Count;
        private const int surgeryMaxDepth_DEFAULT = 1;
        private const int surgeryMaxCount_DEFAULT = 1;
        public static VisibilityRequirement surgeryVisibility = surgeryVisibility_DEFAULT;
        public static int surgeryMaxDepth = surgeryMaxDepth_DEFAULT;
        public static int surgeryMaxCount = surgeryMaxCount_DEFAULT;

        public void DoSettingsWindowContents(Rect inRect)
        {
            Color preColor = GUI.color;
            var preAnchor = Text.Anchor;

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.EnumSelector("WIKT_settings_designator_visreq", ref designatorVisibility, "WIKT_settings_visreq_");
            if(designatorVisibility.HasFlag(VisibilityRequirement.Depth))
            {
                designatorMaxDepth = (int) listingStandard.FullSlider(designatorMaxDepth, 1, 10, roundTo: 1);
            }
            if (designatorVisibility.HasFlag(VisibilityRequirement.Count))
            {
                designatorMaxCount = (int)listingStandard.FullSlider(designatorMaxCount, 1, 10, roundTo: 1);
            }

            listingStandard.EnumSelector("WIKT_settings_recipe_visreq", ref recipeVisibility, "WIKT_settings_visreq_");
            if (recipeVisibility.HasFlag(VisibilityRequirement.Depth))
            {
                recipeMaxDepth = (int)listingStandard.FullSlider(recipeMaxDepth, 1, 10, roundTo: 1);
            }
            if (recipeVisibility.HasFlag(VisibilityRequirement.Count))
            {
                recipeMaxCount = (int)listingStandard.FullSlider(recipeMaxCount, 1, 10, roundTo: 1);
            }

            listingStandard.EnumSelector("WIKT_settings_surgery_visreq", ref surgeryVisibility, "WIKT_settings_visreq_");
            if (surgeryVisibility.HasFlag(VisibilityRequirement.Depth))
            {
                surgeryMaxDepth = (int)listingStandard.FullSlider(surgeryMaxDepth, 1, 10, roundTo: 1);
            }
            if (surgeryVisibility.HasFlag(VisibilityRequirement.Count))
            {
                surgeryMaxCount = (int)listingStandard.FullSlider(surgeryMaxCount, 1, 10, roundTo: 1);
            }

            listingStandard.End();

            Text.Anchor = preAnchor;
            GUI.color = preColor;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref designatorVisibility, "designatorVisibility", designatorVisibility_DEFAULT);
            Scribe_Values.Look(ref designatorMaxDepth, "designatorMaxDepth", designatorMaxDepth_DEFAULT);
            Scribe_Values.Look(ref designatorMaxCount, "designatorMaxCount", designatorMaxCount_DEFAULT);

            Scribe_Values.Look(ref recipeVisibility, "recipeVisibility", recipeVisibility_DEFAULT);
            Scribe_Values.Look(ref recipeMaxDepth, "recipeMaxDepth", recipeMaxDepth_DEFAULT);
            Scribe_Values.Look(ref recipeMaxCount, "recipeMaxCount", recipeMaxCount_DEFAULT);

            Scribe_Values.Look(ref surgeryVisibility, "surgeryVisibility", surgeryVisibility_DEFAULT);
            Scribe_Values.Look(ref surgeryMaxDepth, "surgeryMaxDepth", surgeryMaxDepth_DEFAULT);
            Scribe_Values.Look(ref surgeryMaxCount, "surgeryMaxCount", surgeryMaxCount_DEFAULT);

        }
    }
}
