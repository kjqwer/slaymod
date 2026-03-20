using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2Mod.Patches;

internal static class EnergyCostHelper
{
    private static readonly FieldInfo BaseField =
        typeof(CardEnergyCost).GetField("_base", BindingFlags.NonPublic | BindingFlags.Instance)!;

    public static void SetBaseCost(CardEnergyCost cost, int value)
    {
        BaseField.SetValue(cost, value);
    }
}

[HarmonyPatch(typeof(Squash), MethodType.Constructor)]
public static class SquashCostPatch
{
    public static void Postfix(Squash __instance)
    {
        EnergyCostHelper.SetBaseCost(__instance.EnergyCost, 0);
    }
}

[HarmonyPatch(typeof(Exterminate), MethodType.Constructor)]
public static class ExterminateCostPatch
{
    public static void Postfix(Exterminate __instance)
    {
        EnergyCostHelper.SetBaseCost(__instance.EnergyCost, 0);
    }
}

[HarmonyPatch(typeof(Peck), MethodType.Constructor)]
public static class PeckCostPatch
{
    public static void Postfix(Peck __instance)
    {
        EnergyCostHelper.SetBaseCost(__instance.EnergyCost, 0);
    }
}

[HarmonyPatch(typeof(ToricToughness), MethodType.Constructor)]
public static class ToricToughnessCostPatch
{
    public static void Postfix(ToricToughness __instance)
    {
        EnergyCostHelper.SetBaseCost(__instance.EnergyCost, 1);
    }
}

[HarmonyPatch(typeof(Relax), MethodType.Constructor)]
public static class RelaxCostAndBaseBlockPatch
{
    public static void Postfix(Relax __instance)
    {
        EnergyCostHelper.SetBaseCost(__instance.EnergyCost, 2);
        __instance.DynamicVars["Block"].BaseValue = 20m;
    }
}

[HarmonyPatch(typeof(Relax), "OnUpgrade")]
public static class RelaxUpgradeBlockPatch
{
    public static void Postfix(Relax __instance)
    {
        __instance.DynamicVars["Block"].BaseValue = 30m;
    }
}
