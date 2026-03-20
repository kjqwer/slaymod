using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySts2Mod.Powers;

public class ScrapArtilleryPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => false;

    public override async Task AfterCardDrawnEarly(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature == Owner && card.Type == CardType.Status)
        {
            var opponents = CombatState.GetOpponentsOf(Owner).Where(c => c.IsHittable).ToList();
            if (opponents.Count == 0) return;

            var target = Owner.Player.RunState.Rng.CombatTargets.NextItem(opponents);
            Flash();
            await CreatureCmd.Damage(choiceContext, target, Amount, ValueProp.Move, Owner);
        }
    }
}
