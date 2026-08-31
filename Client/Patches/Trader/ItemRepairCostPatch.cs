namespace IronManClient.Patches.Trader;

using System;
using System.Reflection;
using EFT.InventoryLogic;
using HarmonyLib;
using Models;
using SPT.Reflection.Patching;
using ProfileStatus = Utils.ProfileStatus;

public class ItemRepairCostPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.PropertyGetter(typeof(Item), nameof(Item.RepairCost));
    }

    [PatchPostfix]
    public static void Postfix(ref int __result)
    {
        var multiplier = GetRepairCostMultiplier(ProfileStatus.ProfileType);

        if (Math.Abs(multiplier - 1f) < 0.001)
            return;

        var originalCost = __result;
        __result = (int)(originalCost * multiplier);

        Plugin.Log.LogInfo($"[Ironman] Repair cost {originalCost} -> {__result} using {multiplier}x multiplier.");
    }

    private static float GetRepairCostMultiplier(ProfileType profileType)
    {
        return profileType switch
        {
            ProfileType.Ultimate => 2f,
            ProfileType.Hardcore => 5f,
            _ => 1f
        };
    }
}
