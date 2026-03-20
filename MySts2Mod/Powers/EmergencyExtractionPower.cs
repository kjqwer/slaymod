using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2Mod.Powers;

public class EmergencyExtractionPower : CustomPowerModel
{
    private class Data
    {
        public bool cardPlayed;
        public CardModel? trackedCard;
    }

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;

    public void SetTrackedCard(CardModel card)
    {
        Data data = GetInternalData<Data>();
        data.trackedCard = card;
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        Data data = GetInternalData<Data>();
        if (data.trackedCard != null && cardPlay.Card == data.trackedCard)
        {
            data.cardPlayed = true;
        }
        return Task.CompletedTask;
    }

    public override async Task BeforeTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return;

        Data data = GetInternalData<Data>();
        if (!data.cardPlayed)
        {
            Flash();
            await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unpowered, Owner);
        }
        
        await PowerCmd.Remove(this);
    }
}
