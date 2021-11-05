﻿using System;
using System.Collections.Generic;
using System.IO;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using FluentAssertions;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Tests;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.World.Map;
using Xunit;

namespace NeoServer.Game.World.Tests
{
    public class MapMoveCreatureTest
    {
        [Fact]
        public void TryMoveCreature_Should_Move_Creature()
        {
            var sut = MapTestDataBuilder.Build(1, 101, 1, 101, 6, 9);
            var player = PlayerTestDataBuilder.Build();
            player.SetNewLocation(new Location(50, 50, 7));
            var result = sut.TryMoveCreature(player, new Location(51, 50, 7));

            Assert.True(result);
            Assert.Equal(new Location(51, 50, 7), player.Location);
        }

        [Fact]
        public void TryMoveCreature_when_Teleport_Should_Move_Creature()
        {
            var sut = MapTestDataBuilder.Build(1, 101, 1, 101, 6, 9);
            var player = PlayerTestDataBuilder.Build();
            player.SetNewLocation(new Location(50, 50, 7));
            var result = sut.TryMoveCreature(player, new Location(53, 50, 7));

            Assert.True(result);
            Assert.Equal(new Location(53, 50, 7), player.Location);
        }

        [Fact]
        public void Player_dont_teleport_when_tile_has_teleport_without_destination()
        {
            //arrange
            
            var sut = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 7);
            var pathFinder = new PathFinder(sut);
            
            var player  = PlayerTestDataBuilder.Build(pathFinder: pathFinder );
            player.SetNewLocation(new Location(100,100,7));
            
            var teleportLocation = new Location(101, 100, 7);

            var teleportAttrs = new Dictionary<ItemAttribute, IConvertible>()
            {
                //no destination
            };
            
            sut[teleportLocation].AddItem(new TeleportItem(new ItemType(), teleportLocation, teleportAttrs));

            //act
            
            player.WalkTo(Direction.East);

            //assert
            player.Location.Should().Be(new Location(101, 100, 7));
        }
    }
}