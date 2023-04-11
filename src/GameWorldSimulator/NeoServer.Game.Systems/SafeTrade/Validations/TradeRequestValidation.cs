﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Systems.SafeTrade.Trackers;
using NeoServer.Game.World.Algorithms;

namespace NeoServer.Game.Systems.SafeTrade.Validations;

internal static class TradeRequestValidation
{
    private static IMap _map;
    public static void Init(IMap map) => _map = map;
    
    public static bool IsValid(IPlayer firstPlayer, IPlayer secondPlayer, IItem[] items)
    {
        if (Guard.AnyNull(firstPlayer, secondPlayer)) return false;

        // Ensure that the two players are not the same
        if (BothPlayersAreTheSame(firstPlayer, secondPlayer)) return false;

        if (PlayerIsAlreadyTrading(firstPlayer)) return false;

        if (TradeHasNoItems(firstPlayer, items)) return false;

        if (HasNonPickupableItem(firstPlayer, items)) return false;

        if (TradeHasMoreThan255Items(firstPlayer, items)) return false;

        if (ItemIsAlreadyBeingTraded(firstPlayer, items)) return false;

        if (PlayerIsNotNextToItem(firstPlayer, items)) return false;

        // Ensure that both players are close enough to each other
        if (PlayerIsNotCloseEnough(firstPlayer, secondPlayer)) return false;

        if (!HasSightClearToSecondPlayer(firstPlayer, secondPlayer)) return false;

        // Ensure that the second player is not already in a trade with someone else
        if (SecondPlayerIsAlreadyTrading(firstPlayer, secondPlayer)) return false;

        return true;
    }

    private static bool HasNonPickupableItem(IPlayer player, IItem[] items)
    {
        foreach (var tradedItem in items)
        {
            if (!tradedItem.IsPickupable)
            {
                OperationFailService.Send(player.CreatureId, "Item cannot be traded.");
                return true;
            }
        }

        return false;
    }

    private static bool HasSightClearToSecondPlayer(IPlayer firstPlayer, IPlayer secondPlayer)
    {
        var isSightClear = SightClear
            .IsSightClear(_map,
                firstPlayer.Location,
                secondPlayer.Location, checkFloor: false);

        if (isSightClear) return true;
        
        OperationFailService.Send(firstPlayer.CreatureId, $"{secondPlayer.Name} tells you to move close.");
        return false;
    }

    private static bool PlayerIsNotCloseEnough(IPlayer firstPlayer, IPlayer secondPlayer)
    {
        if (firstPlayer.Location.GetMaxSqmDistance(secondPlayer.Location) > 2)
        {
            OperationFailService.Send(firstPlayer.CreatureId, $"{secondPlayer.Name} tells you to move close.");
            return true;
        }

        return false;
    }

    private static bool TradeHasMoreThan255Items(IPlayer player, IItem[] items)
    {
        if (items.Length > 255)
        {
            OperationFailService.Send(player.CreatureId, "You cannot trade more than 255 items at once.");
            return true;
        }

        return false;
    }

    private static bool PlayerIsAlreadyTrading(IPlayer player)
    {
        if (TradeRequestTracker.GetTradeRequest(player) is not null)
        {
            OperationFailService.Send(player.CreatureId, "You are already trading.");
            return true;
        }

        return false;
    }

    private static bool BothPlayersAreTheSame(IPlayer firstPlayer, IPlayer secondPlayer)
    {
        if (firstPlayer == secondPlayer)
        {
            OperationFailService.Send(firstPlayer.CreatureId, "Select a player to trade with.");
            return true;
        }

        return false;
    }

    private static bool SecondPlayerIsAlreadyTrading(IPlayer firstPlayer, IPlayer secondPlayer)
    {
        var tradeRequest = TradeRequestTracker.GetTradeRequest(secondPlayer);

        if (tradeRequest is not null &&
            tradeRequest.PlayerRequested.CreatureId != firstPlayer.CreatureId)
        {
            OperationFailService.Send(firstPlayer.CreatureId, "This person is already trading.");
            return true;
        }

        return false;
    }

    private static bool PlayerIsNotNextToItem(IPlayer player, IItem[] items)
    {
        if (!player.Location.IsNextTo(items[0].Location))
        {
            OperationFailService.Send(player.CreatureId, "You are not close to the item.");
            return true;
        }

        return false;
    }

    private static bool TradeHasNoItems(IPlayer player, IItem[] items)
    {
        if (items is null || !items.Any())
        {
            OperationFailService.Send(player.CreatureId, "You must select an item to trade.");
            return true;
        }

        return false;
    }

    private static bool ItemIsAlreadyBeingTraded(IPlayer firstPlayer, IItem[] items)
    {
        foreach (var item in items)
        {
            if (ItemTradedTracker.GetTradeRequest(item) is not null)
            {
                OperationFailService.Send(firstPlayer.CreatureId, "This item is already being traded.");
                return true;
            }
        }

        return false;
    }
}