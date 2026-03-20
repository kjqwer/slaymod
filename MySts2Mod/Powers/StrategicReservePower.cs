using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2Mod.Powers;

public class StrategicReservePower : CustomPowerModel
{
    private class Data
    {
        public CardModel? trackedCard;
    }

    public override PowerType Type => PowerType.Buff;
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

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side) return;
        var playerCombatState = Owner.Player?.PlayerCombatState;
        if (playerCombatState == null) return;

        Data data = GetInternalData<Data>();
        
        if (data.trackedCard != null && data.trackedCard.Pile == playerCombatState.Hand)
        {
            Flash();
            data.trackedCard.EnergyCost.AddThisTurnOrUntilPlayed(-(int)Amount, reduceOnly: true);
        }

        await PowerCmd.Remove(this);
    }
}
