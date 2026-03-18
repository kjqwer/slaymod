using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class SpectralGatingCard : CustomCardModel
{
    public SpectralGatingCard() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var toExhaust = Owner.PlayerCombatState.Hand.Cards.Where(c => c.Type == CardType.Status || c.Type == CardType.Curse).ToList();
        int exhaustedCount = toExhaust.Count;

        foreach (var card in toExhaust)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        if (exhaustedCount > 0)
        {
            await PlayerCmd.GainStars(exhaustedCount * 2, Owner);
            await CardPileCmd.Draw(choiceContext, exhaustedCount, Owner, fromHandDraw: false);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}