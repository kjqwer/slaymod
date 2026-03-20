using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class OverloadDumpingCard : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, ValueProp.Move),
        new ExtraDamageVar(4m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[] { HoverTipFactory.FromCard<Debris>() };

    public OverloadDumpingCard() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        int stars = Owner.PlayerCombatState?.Stars ?? 0;
        if (stars <= 0)
        {
            return;
        }

        int debrisAddedToHand = 0;
        for (int i = 0; i < stars; i++)
        {
            var addResult = await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Debris>(Owner), PileType.Hand, addedByPlayer: true);
            if (addResult.success && addResult.cardAdded.Pile?.Type == PileType.Hand)
            {
                debrisAddedToHand++;
            }
        }

        if (debrisAddedToHand <= 0)
        {
            return;
        }

        await Cmd.Wait(0.5f);

        await DamageCmd.Attack(DynamicVars["ExtraDamage"].BaseValue)
            .WithHitCount(debrisAddedToHand)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["ExtraDamage"].UpgradeValueBy(2m);
    }
}
