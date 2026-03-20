using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;

namespace MySts2Mod.Powers;

public class CoreExtractionPower : CustomPowerModel
{
    private class Data
    {
        public CardRarity targetRarity;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;

    public void SetTargetRarity(CardRarity rarity)
    {
        Data data = GetInternalData<Data>();
        data.targetRarity = rarity;
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side) return;
        var player = Owner.Player;
        if (player == null) return;

        Data data = GetInternalData<Data>();

        Flash();

        // CardFactory.GetForCombat 内部的 FilterForCombat 会移除 Basic/Ancient/Event 稀有度的卡牌。
        // 若目标稀有度会被过滤（如消耗了基础打击牌），则回退到 Common 以避免空列表导致 NullReferenceException。
        var safeRarity = data.targetRarity;
        if (safeRarity == CardRarity.Basic || safeRarity == CardRarity.Ancient || safeRarity == CardRarity.Event)
        {
            safeRarity = CardRarity.Common;
        }

        var options = player.Character.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => c.Type == CardType.Attack && c.Rarity == safeRarity);

        if (!options.Any())
        {
            // 二次回退：同样排除会被 FilterForCombat 过滤掉的稀有度
            options = player.Character.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
                .Where(c => c.Type == CardType.Attack
                         && c.Rarity != CardRarity.Basic
                         && c.Rarity != CardRarity.Ancient
                         && c.Rarity != CardRarity.Event);
        }

        var forCombat = CardFactory.GetForCombat(player, options, 1, player.RunState.Rng.CombatCardGeneration);

        foreach (var card in forCombat)
        {
            card.EnergyCost.SetThisTurn(0);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
        }

        await PowerCmd.Remove(this);
    }
}
