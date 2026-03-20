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

[Pool(typeof(ColorlessCardPool))]
public class StrategicReserveCard : CustomCardModel
{
    public StrategicReserveCard() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var handCards = Owner.PlayerCombatState?.Hand?.Cards;
        if (handCards == null)
        {
            return;
        }

        var selectableCards = handCards.Where(card => card != this).ToList();
        if (selectableCards.Count > 0)
        {
            var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1)
            {
                RequireManualConfirmation = true
            };
            var selectedList = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, card => card != this, this)).ToList();

            if (selectedList.Count > 0)
            {
                var card = selectedList[0];
                card.GiveSingleTurnRetain();

                var power = await PowerCmd.Apply<MySts2Mod.Powers.StrategicReservePower>(Owner.Creature, IsUpgraded ? 2 : 1, Owner.Creature, this);
                if (power != null)
                {
                    power.SetTrackedCard(card);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        // Handled in OnPlay logic for Amount
    }
}
