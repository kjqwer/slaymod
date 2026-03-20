using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;

namespace MySts2Mod.Cards;

[Pool(typeof(ColorlessCardPool))]
public class EmergencyExtractionCard : CustomCardModel
{
    public EmergencyExtractionCard() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selectableCards = PileType.Discard.GetPile(Owner).Cards.Where(card => card != this).ToList();
        if (selectableCards.Count > 0)
        {
            var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1)
            {
                RequireManualConfirmation = true
            };
            var selected = (await CardSelectCmd.FromSimpleGrid(choiceContext, selectableCards, Owner, prefs)).FirstOrDefault();
            
            if (selected != null)
            {
                selected.EnergyCost.SetThisTurnOrUntilPlayed(0);
                await CardPileCmd.Add(selected, PileType.Hand);
                
                var power = await PowerCmd.Apply<MySts2Mod.Powers.EmergencyExtractionPower>(Owner.Creature, IsUpgraded ? 2 : 5, Owner.Creature, this);
                if (power != null)
                {
                    power.SetTrackedCard(selected);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        // Handled in the IsUpgraded check
    }
}
