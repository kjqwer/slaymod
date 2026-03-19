using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;

namespace MySts2Mod.Powers;

public class AstralDirectLinkPower : CustomPowerModel
{
    private const string _cardsLeftKey = "CardsLeft";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => base.DynamicVars[_cardsLeftKey].IntValue;
    public override bool IsInstanced => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar(_cardsLeftKey, 3m)
    };

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
        {
            return;
        }

        // Check if card cost 0
        if (cardPlay.Resources.EnergySpent == 0 && cardPlay.Resources.StarsSpent == 0)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);

            DynamicVars[_cardsLeftKey].BaseValue--;
            InvokeDisplayAmountChanged();

            if (DynamicVars[_cardsLeftKey].IntValue <= 0)
            {
                await PlayerCmd.GainEnergy((int)Amount, Owner.Player);
                DynamicVars[_cardsLeftKey].BaseValue = 3m;
                InvokeDisplayAmountChanged();
            }
        }
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return Task.CompletedTask;
        }

        DynamicVars[_cardsLeftKey].BaseValue = 3m;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}