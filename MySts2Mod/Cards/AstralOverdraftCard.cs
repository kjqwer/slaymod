using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MySts2Mod.Powers;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class AstralOverdraftCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new StarsVar(5),
        new PowerVar<EnergyDownNextTurnPower>(2m)
    };

    public AstralOverdraftCard() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainStars(DynamicVars.Stars.BaseValue, Owner);
        await PowerCmd.Apply<EnergyDownNextTurnPower>(Owner.Creature, DynamicVars["EnergyDownNextTurnPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Stars.UpgradeValueBy(2m);
    }
}
