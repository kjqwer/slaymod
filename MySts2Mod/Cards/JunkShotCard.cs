using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class JunkShotCard : CustomCardModel
{
    private sealed class JunkRepeatVar : DynamicVar
    {
        public JunkRepeatVar() : base("Repeat", 0m) { }

        public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
        {
            var count = CountDebrisAndStatus(card);
            PreviewValue = count;
            EnchantedValue = count;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(0m),
        new ExtraDamageVar(6m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) =>
        {
            return CountDebrisAndStatus(card);
        }),
        new JunkRepeatVar()
    };

    public JunkShotCard() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        int count = CountDebrisAndStatus(this);
        if (count > 0)
        {
            await DamageCmd.Attack(DynamicVars["ExtraDamage"].BaseValue)
                .WithHitCount(count)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Debris>(Owner), PileType.Discard, addedByPlayer: false);

        if (!Keywords.Contains(CardKeyword.Exhaust) && !ExhaustOnNextPlay)
        {
            await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Top);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ExtraDamage"].UpgradeValueBy(2m);
    }

    private static int CountDebrisAndStatus(CardModel card)
    {
        var state = card.Owner.PlayerCombatState;
        if (state == null) return 0;
        return state.AllCards.Count(IsDebrisOrStatus);
    }

    private static bool IsDebrisOrStatus(CardModel card)
    {
        return card.Type == CardType.Status || card.Id.Entry == "Debris";
    }
}
