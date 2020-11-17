﻿using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.World.Map.Tiles
{
    public abstract class BaseTile : ITile
    {
        public Location Location { get; }

        public abstract bool TryGetStackPositionOfThing(IPlayer player, IThing thing, out byte stackPosition);
    }
}
