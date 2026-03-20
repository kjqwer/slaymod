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
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class SweptUnderTheRugCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(3),
        new EnergyVar(2)
    };

    public SweptUnderTheRugCard() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var handCards = Owner.PlayerCombatState?.Hand?.Cards;
        if (handCards == null)
        {
            return;
        }

        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner, fromHandDraw: false);

        handCards = Owner.PlayerCombatState?.Hand?.Cards;
        if (handCards == null || handCards.Count == 0)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1)
        {
            RequireManualConfirmation = true
        };

        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, _ => true, this)).FirstOrDefault();
        if (selected == null)
        {
            return;
        }

        bool isDebris = selected.Id == ModelDb.Card<Debris>().Id || selected.Id.Entry == "Debris";
        await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top, this);

        if (isDebris)
        {
            await PlayerCmd.GainEnergy((int)DynamicVars["Energy"].BaseValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
