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
public class WrathOfTheCrownCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(10m),
        new ExtraDamageVar(1m),
        new IntVar("Multiplier", 50m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) =>
        {
            var blade = card.Owner.PlayerCombatState?.Hand.Cards.FirstOrDefault(c => c.Id == ModelDb.Card<SovereignBlade>().Id);
            if (blade != null)
            {
                return Math.Floor(blade.DynamicVars.Damage.PreviewValue * (card.DynamicVars["Multiplier"].PreviewValue / 100m));
            }
            return 0m;
        }),
        new ForgeVar(5)
    };

    public WrathOfTheCrownCard() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        
        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
            
        await ForgeCmd.Forge(DynamicVars.Forge.BaseValue, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Multiplier"].UpgradeValueBy(25m);
    }
}
