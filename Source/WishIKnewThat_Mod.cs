using HarmonyLib;
using PeteTimesSix.WishIKnewThat.ModCompat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PeteTimesSix.WishIKnewThat
{
    public class WishIKnewThat_Mod : Mod
    {
        public static WishIKnewThat_Mod ModSingleton { get; private set; }
        public static WishIKnewThat_Settings Settings { get; internal set; }
        public static Harmony Harmony { get; private set; }

        public WishIKnewThat_Mod(ModContentPack content) : base(content)
        {
            ModSingleton = this;

            Harmony = new Harmony("PeteTimesSix.ResearchReinvented");
            Harmony.PatchAll();

            OptionalPatches.Patch(Harmony);
        }

        public override string SettingsCategory()
        {
            return "WishIKnewThat_ModTitle".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }


    [StaticConstructorOnStartup]
    public static class WishIKnewThat_PostInit
    {
        static WishIKnewThat_PostInit()
        {
            WishIKnewThat_Mod.Settings = WishIKnewThat_Mod.ModSingleton.GetSettings<WishIKnewThat_Settings>();

            OptionalPatches.PatchDelayed(WishIKnewThat_Mod.Harmony);
        }
    }
}
