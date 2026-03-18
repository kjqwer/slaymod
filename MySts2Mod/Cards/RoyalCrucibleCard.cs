using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class RoyalCrucibleCard() : CustomCardModel(
    2,                      // 2费
    CardType.Skill,         // 技能牌
    CardRarity.Rare,        // 稀有
    TargetType.Self         // 自身
)
{
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Exhaust
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            foreach (var tip in HoverTipFactory.FromForge())
                yield return tip;
            yield return HoverTipFactory.Static(StaticHoverTip.Block);
            yield return HoverTipFactory.FromKeyword(CardKeyword.Exhaust);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        List<CardModel> handCards = Owner.PlayerCombatState.Hand.Cards.ToList();
        int exhaustedCount = 0;

        foreach (CardModel card in handCards)
        {
            await CardCmd.Exhaust(choiceContext, card);
            exhaustedCount++;
        }

        if (exhaustedCount > 0)
        {
            decimal forgeAmount = exhaustedCount * 10m;
            decimal blockAmount = exhaustedCount * 10m;
            await ForgeCmd.Forge((int)forgeAmount, Owner, this);
            await CreatureCmd.GainBlock(Owner.Creature, blockAmount, ValueProp.Move, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust); // 升级去掉消耗
    }
}
