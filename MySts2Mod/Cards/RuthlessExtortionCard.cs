using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class RuthlessExtortionCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(2),
        new StarsVar(1),
        new CardsVar(0)
    };

    public RuthlessExtortionCard() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var targetCardList = await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), c => c.Type == CardType.Status || c.Id.Entry == "Debris", this);
        var targetCard = targetCardList?.FirstOrDefault();

        if (targetCard != null)
        {
            await CardCmd.Exhaust(choiceContext, targetCard);
            await PlayerCmd.GainEnergy((int)DynamicVars["Energy"].BaseValue, Owner);
            await PlayerCmd.GainStars((int)DynamicVars["Stars"].BaseValue, Owner);
            
            if (DynamicVars["Cards"].BaseValue > 0)
            {
                await CardPileCmd.Draw(choiceContext, (int)DynamicVars["Cards"].BaseValue, Owner, fromHandDraw: false);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}
