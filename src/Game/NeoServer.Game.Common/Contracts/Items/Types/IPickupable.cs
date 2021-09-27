﻿using System.Globalization;

namespace NeoServer.Game.Common.Contracts.Items.Types
{
    public interface IPickupable : IMoveableThing, IItem
    {
        float Weight => Metadata.Weight;


    }
}