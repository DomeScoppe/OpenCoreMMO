﻿using NeoServer.Game.Common.Creatures.Party;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;

namespace NeoServer.Networking.Packets.Outgoing.Party
{
    public class PartyEmblemPacket : OutgoingPacket
    {
        private readonly ICreature creature;
        private readonly PartyEmblem emblem;

        public PartyEmblemPacket(ICreature creature, PartyEmblem emblem)
        {
            this.creature = creature;
            this.emblem = emblem;
        }
        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.CreatureEmblem);
            message.AddUInt32(creature.CreatureId);
            message.AddByte((byte)emblem);
        }
    }
}
