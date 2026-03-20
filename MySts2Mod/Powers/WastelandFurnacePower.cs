using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace MySts2Mod.Powers;

public class WastelandFurnacePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            foreach (var tip in HoverTipFactory.FromForge())
            {
                yield return tip;
            }
            yield return HoverTipFactory.FromKeyword(CardKeyword.Exhaust);
        }
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        var player = Owner.Player;
        if (player == null)
        {
            return;
        }

        if (card.Owner.Creature != Owner)
        {
            return;
        }

        if (card.Type != CardType.Status && card.Id.Entry != "Debris")
        {
            return;
        }

        Flash();
        await ForgeCmd.Forge(Amount, player, this);
        await PlayerCmd.GainStars(Amount, player);
        await PlayerCmd.GainEnergy((int)Amount, player);
        await CardPileCmd.Draw(choiceContext, Amount, player, fromHandDraw: false);
    }
}
