using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MySts2Mod.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MySts2Mod.Cards;

[Pool(typeof(RegentCardPool))]
public class ForgeGuardCard() : CustomCardModel(
    2,                      // 2费
    CardType.Power,         // 能力牌
    CardRarity.Rare,        // 稀有
    TargetType.Self         // 自身
)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new ForgeVar(5)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            foreach (var tip in HoverTipFactory.FromForge())
                yield return tip;
            yield return HoverTipFactory.FromPower<ForgeGuardPower>();
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await ForgeCmd.Forge(DynamicVars.Forge.IntValue, Owner, this);
        await PowerCmd.Apply<ForgeGuardPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1); // 升级后 2费 -> 1费
    }
}
