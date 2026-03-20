using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class GreshamsLawCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(8m),
        new ExtraDamageVar(4m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => CountDebrisAndStatusInHand(card))
    };

    public GreshamsLawCard() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Debris>(Owner), PileType.Discard, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CalculationBase"].UpgradeValueBy(2m);
        DynamicVars["ExtraDamage"].UpgradeValueBy(2m);
    }

    private static int CountDebrisAndStatusInHand(CardModel card)
    {
        var hand = card.Owner.PlayerCombatState?.Hand?.Cards;
        if (hand == null) return 0;
        return hand.Count(c => c.Type == CardType.Status || c.Id.Entry == "Debris");
    }
}
