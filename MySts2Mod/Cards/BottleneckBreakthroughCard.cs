using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MySts2Mod.Cards;

[Pool(typeof(ColorlessCardPool))]
public class BottleneckBreakthroughCard : CustomCardModel
{
    public BottleneckBreakthroughCard() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Buff", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<MySts2Mod.Powers.BottleneckBreakthroughPower>(Owner.Creature, IsUpgraded ? 3 : 2, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // Amount handled in OnPlay
    }
}