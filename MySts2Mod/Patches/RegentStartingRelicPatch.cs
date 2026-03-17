using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using MySts2Mod.Relics;

namespace MySts2Mod.Patches;

[HarmonyPatch(typeof(Regent), nameof(Regent.StartingRelics), MethodType.Getter)]
public static class RegentStartingRelicPatch
{
    public static void Postfix(ref IReadOnlyList<RelicModel> __result)
    {
        var newRelics = new List<RelicModel>(__result);
        newRelics.Add(ModelDb.Relic<PuppyPowerRelic>());
        __result = newRelics;
    }
}
