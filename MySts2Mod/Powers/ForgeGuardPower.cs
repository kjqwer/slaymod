using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MySts2Mod.Powers;

public class ForgeGuardPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            foreach (var tip in HoverTipFactory.FromForge())
                yield return tip;
            yield return HoverTipFactory.Static(StaticHoverTip.Block);
        }
    }

    public override async Task AfterForge(decimal amount, Player forger, AbstractModel? source)
    {
        if (forger != Owner.Player)
        {
            return;
        }

        Flash();
        decimal blockAmount = amount * Amount;
        await CreatureCmd.GainBlock(Owner, blockAmount, ValueProp.Unpowered, null);
    }
}
