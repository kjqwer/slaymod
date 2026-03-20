using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Cards;
using MySts2Mod.Cards;

namespace MySts2Mod.Patches;

[HarmonyPatch(typeof(Regent), nameof(Regent.StartingDeck), MethodType.Getter)]
public static class RegentStartingDeckPatch
{
    public static void Postfix(ref IEnumerable<CardModel> __result)
    {
        var newDeck = new List<CardModel>(__result);
        // newDeck.Add(ModelDb.Card<OverloadDumpingCard>());
        __result = newDeck;
    }
}