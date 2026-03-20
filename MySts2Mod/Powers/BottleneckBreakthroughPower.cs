using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2Mod.Powers;

public class BottleneckBreakthroughPower : CustomPowerModel
{
    private const string _triggersLeftKey = "TriggersLeft";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar(_triggersLeftKey, 2m)
    };

    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        DynamicVars[_triggersLeftKey].BaseValue = amount;
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side)
        {
            DynamicVars[_triggersLeftKey].BaseValue = Amount;
            InvokeDisplayAmountChanged();
        }

        return Task.CompletedTask;
    }

    private async Task TryTrigger(PlayerChoiceContext? context)
    {
        var player = Owner.Player;
        if (player == null)
        {
            return;
        }

        if (DynamicVars[_triggersLeftKey].IntValue > 0)
        {
            Flash();
            DynamicVars[_triggersLeftKey].BaseValue--;
            InvokeDisplayAmountChanged();

            await PlayerCmd.GainEnergy(1, player);

            // Hook.AfterCardDrawn 内部无论如何都会调用 choiceContext.PushModel()，
            // 传入 null 必然导致 NullReferenceException。
            // BlockingPlayerChoiceContext 是游戏内置的"无交互"上下文，
            // SignalPlayerChoiceBegun/Ended 均为 no-op，适合自动触发效果使用。
            var drawContext = context ?? new BlockingPlayerChoiceContext();
            await CardPileCmd.Draw(drawContext, 1, player);
        }
    }

    // Trigger when a status or curse card is added to hand or deck
    public override async Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
    {
        if (card.Owner == Owner.Player && (card.Type == CardType.Status || card.Type == CardType.Curse))
        {
            await TryTrigger(null);
        }
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // Handle stack changes for ourselves since AfterAmountChanged doesn't exist
        if (power == this && amount > 0)
        {
            DynamicVars[_triggersLeftKey].BaseValue += amount;
            InvokeDisplayAmountChanged();
        }

        // Trigger when debuff applied to me
        if (power.Owner == Owner && power.Type == PowerType.Debuff && amount > 0)
        {
            await TryTrigger(null);
        }
    }
}
