using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MySts2Mod.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Relics;
using System.Threading.Tasks;

namespace MySts2Mod.Relics;

[Pool(typeof(SharedRelicPool))]
public class PuppyPowerRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

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

    // 战斗第一回合开始时，手牌中所有卡牌本回合免费（能量+星辰均为0）
    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        var owner = Owner;
        if (owner == null)
        {
            return Task.CompletedTask;
        }

        var handCards = owner.PlayerCombatState?.Hand?.Cards;
        if (handCards == null)
        {
            return Task.CompletedTask;
        }

        if (side == owner.Creature.Side && combatState.RoundNumber == 1)
        {
            Flash();
            MainFile.Logger.Info("Puppy Power activated! Make cards free!");
            foreach (var card in handCards)
            {
                card.SetToFreeThisTurn();
            }
        }

        return Task.CompletedTask;
    }
}
