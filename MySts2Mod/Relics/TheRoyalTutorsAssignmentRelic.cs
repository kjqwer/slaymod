using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MySts2Mod.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MySts2Mod.Relics;

[Pool(typeof(SharedRelicPool))]
public class TheRoyalTutorsAssignmentRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic.png".BigRelicImagePath();
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == Owner.Creature.Side)
        {
            Flash();
            await PlayerCmd.GainEnergy(1, Owner);
            await PlayerCmd.GainStars(1, Owner);
        }
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        var playerCombatState = Owner.PlayerCombatState;
        if (playerCombatState == null)
        {
            return;
        }

        if (side == Owner.Creature.Side)
        {
            if (playerCombatState.Energy > 0 || playerCombatState.Stars > 0)
            {
                Flash();
                await PowerCmd.Apply(new WeakPower(), Owner.Creature, 1, Owner.Creature, null);
                await PowerCmd.Apply(new VulnerablePower(), Owner.Creature, 1, Owner.Creature, null);
            }
        }
    }
}
