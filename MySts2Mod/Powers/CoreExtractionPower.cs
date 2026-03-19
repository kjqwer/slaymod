using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;

namespace MySts2Mod.Powers;

public class CoreExtractionPower : CustomPowerModel
{
    private class Data
    {
        public CardRarity targetRarity;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;

    public void SetTargetRarity(CardRarity rarity)
    {
        Data data = GetInternalData<Data>();
        data.targetRarity = rarity;
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side) return;

        Data data = GetInternalData<Data>();
        
        Flash();
        
        var options = Owner.Player.Character.CardPool.GetUnlockedCards(Owner.Player.UnlockState, Owner.Player.RunState.CardMultiplayerConstraint)
            .Where(c => c.Type == CardType.Attack && c.Rarity == data.targetRarity);

        if (!options.Any())
        {
            // Fallback if no matching rarity (e.g. Basic/Special)
            options = Owner.Player.Character.CardPool.GetUnlockedCards(Owner.Player.UnlockState, Owner.Player.RunState.CardMultiplayerConstraint)
                .Where(c => c.Type == CardType.Attack);
        }

        var forCombat = CardFactory.GetForCombat(Owner.Player, options, 1, Owner.Player.RunState.Rng.CombatCardGeneration);
        
        foreach (var card in forCombat)
        
        {
            card.EnergyCost.SetThisTurn(0);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
        }

        await PowerCmd.Remove(this);
    }
}