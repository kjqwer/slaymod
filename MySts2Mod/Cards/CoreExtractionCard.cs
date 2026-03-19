using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySts2Mod.Cards;

[Pool(typeof(ColorlessCardPool))]
public class CoreExtractionCard : CustomCardModel
{
    public CoreExtractionCard() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var handAttacks = Owner.PlayerCombatState.Hand.Cards.Where(c => c.Type == CardType.Attack).ToList();
        if (handAttacks.Count > 0)
        {
            var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1)
            {
                RequireManualConfirmation = true
            };
            
            var selectedList = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, (card) => card.Type == CardType.Attack, this)).ToList();

            if (selectedList.Count > 0)
            {
                var card = selectedList[0];
                var baseDamage = card.DynamicVars.Damage.BaseValue;
                var rarity = card.Rarity;
                
                await CardCmd.Exhaust(choiceContext, card);
                
                decimal blockAmount = baseDamage;
                if (IsUpgraded)
                {
                    blockAmount += 4;
                }

                await CreatureCmd.GainBlock(Owner.Creature, blockAmount, ValueProp.Unpowered, null);

                // Add power to get a random attack of the same rarity next turn
                var power = await PowerCmd.Apply<MySts2Mod.Powers.CoreExtractionPower>(Owner.Creature, 1, Owner.Creature, this);
                power.SetTargetRarity(rarity);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // Handled in OnPlay
    }
}
