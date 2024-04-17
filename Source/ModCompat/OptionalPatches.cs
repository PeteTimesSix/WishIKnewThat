using HarmonyLib;
using PeteTimesSix.WishIKnewThat.ModCompat.DubsMintMenus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeteTimesSix.WishIKnewThat.ModCompat
{
    public static class OptionalPatches
    {
        public static void Patch(Harmony harmony)
        {
            //Log.Warning("Doing optional patches...");
        }

        public static void PatchDelayed(Harmony harmony)
        {
            //Log.Warning("Doing delayed optional patches...");
            if (_DubsMintMenus.active)
            {
                _DubsMintMenus.PatchDelayed(harmony);
            }
        }

    }
}