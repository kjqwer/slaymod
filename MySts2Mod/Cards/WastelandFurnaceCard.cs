using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MySts2Mod.Powers;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class WastelandFurnaceCard : CustomCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[] { HoverTipFactory.FromKeyword(CardKeyword.Exhaust) };

    public WastelandFurnaceCard() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WastelandFurnacePower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
