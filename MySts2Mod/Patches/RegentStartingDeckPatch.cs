using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MySts2Mod.Relics;

namespace MySts2Mod.Patches;

[HarmonyPatch(typeof(Regent), nameof(Regent.StartingDeck), MethodType.Getter)]
public static class RegentStartingDeckPatch
{
    public static void Postfix(ref IEnumerable<CardModel> __result)
    {
        var newDeck = new List<CardModel>(__result);
        __result = newDeck;
    }
}

[HarmonyPatch(typeof(Regent), nameof(Regent.StartingRelics), MethodType.Getter)]
public static class RegentStartingRelicsPatch
{
    public static void Postfix(ref IEnumerable<RelicModel> __result)
    {
        var relic = ModelDb.Relic<TheRoyalTutorsAssignmentRelic>();
        var relics = new List<RelicModel>(__result);
        if (!relics.Any(r => r.Id == relic.Id))
        {
            // relics.Add(relic);
        }

        __result = relics;
    }
}
