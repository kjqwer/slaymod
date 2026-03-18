using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class RuthlessConscriptionCard : CustomCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<MinionStrike>(false),
        HoverTipFactory.FromCard<MinionSacrifice>(false)
    };

    public RuthlessConscriptionCard() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. Exhaust a card
        List<CardModel> exhaustList = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1, 1), null, this)).ToList();
        if (exhaustList.Count > 0)
        {
            await CardCmd.Exhaust(choiceContext, exhaustList[0]);
        }

        // 2. Choose MinionStrike or MinionSacrifice
        List<CardModel> choices = new List<CardModel>
        {
            CombatState.CreateCard<MinionStrike>(Owner),
            CombatState.CreateCard<MinionSacrifice>(Owner)
        };
        
        var chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, choices, Owner, canSkip: false);
        if (chosen != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, addedByPlayer: true);
        }

        // 3. Draw 1 card
        await CardPileCmd.Draw(choiceContext, 1m, Owner, fromHandDraw: false);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
