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
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class RoyalForesightCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(3),
        new StarsVar(1)
    };

    public RoyalForesightCard() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playerCombatState = Owner.PlayerCombatState;
        if (playerCombatState == null)
        {
            return;
        }

        int amount = (int)DynamicVars.Cards.BaseValue;
        var drawPile = playerCombatState.DrawPile;
        
        if (drawPile.Cards.Count > 0)
        {
            int count = Math.Min(amount, drawPile.Cards.Count);
            List<CardModel> topCards = drawPile.Cards.Take(count).ToList();
            
            // Discard phase
            var toDiscard = (await CardSelectCmd.FromSimpleGrid(choiceContext, topCards, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 0, count))).ToList();
            foreach (var c in toDiscard)
            {
                await CardPileCmd.Add(c, PileType.Discard, CardPilePosition.Top, this);
                topCards.Remove(c);
            }
            
            // Reorder phase
            if (topCards.Count > 1)
            {
                var ordered = await CardSelectCmd.FromSimpleGrid(choiceContext, topCards, Owner, new CardSelectorPrefs(new MegaCrit.Sts2.Core.Localization.LocString("cards", "MYSTS2MOD-ROYAL_FORESIGHT_CARD.reorderPrompt"), topCards.Count, topCards.Count));
                foreach (var c in ordered)
                {
                    await CardPileCmd.Add(c, PileType.Draw, CardPilePosition.Top, this);
                }
            }
        }

        await PlayerCmd.GainStars(DynamicVars.Stars.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2m);
    }
}
