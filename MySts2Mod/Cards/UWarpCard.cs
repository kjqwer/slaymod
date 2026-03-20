using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class UWarpCard : CustomCardModel
{
    public UWarpCard() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var handCards = Owner.PlayerCombatState?.Hand?.Cards;
        if (handCards == null)
        {
            return;
        }

        var handSize = handCards.Count;
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, handSize)
        {
            RequireManualConfirmation = true
        };

        var discardList = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this)).ToList();
        
        int discardedCount = discardList.Count;

        if (discardedCount > 0)
        {
            await CardCmd.Discard(choiceContext, discardList);
            var drawnCards = await CardPileCmd.Draw(choiceContext, discardedCount, Owner, fromHandDraw: false);

            foreach (var card in drawnCards)
            {
                card.EnergyCost.AddThisTurn(-1, reduceOnly: true);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
