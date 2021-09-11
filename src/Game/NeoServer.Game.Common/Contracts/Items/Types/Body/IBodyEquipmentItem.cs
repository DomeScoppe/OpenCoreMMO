﻿using System.Collections.Immutable;
using System.Text;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Body
{
    public interface IInventoryItem : IItemRequirement
    {
        public Slot Slot => Metadata.BodyPosition;
    }

    public interface IBodyEquipmentItem : IDressable, IPickupable, IInventoryItem
    {
        bool Pickupable => true;

        ushort MinimumLevelRequired => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.MinimumLevel);

        public ImmutableDictionary<SkillType, byte> SkillBonus =>
            Metadata.Attributes.SkillBonuses.ToImmutableDictionary();

        public WeaponType WeaponType => Metadata.WeaponType;

        protected string RequirementText
        {
            get
            {
                var stringBuilder = new StringBuilder();
                var sufix = "\nIt can only be wielded properly by";

                for (var i = 0; i < Vocations?.Length; i++)
                {
                    //stringBuilder.Append($"{VocationTypeParser.Parse(Vocations[i]).ToLower()}s");
                    //if (i + 1 < Vocations.Length)
                    //{
                    //    stringBuilder.Append(", ");
                    //}
                }

                if (MinLevel > 0) stringBuilder.Append($" of level {MinLevel} or higher");

                return $"{sufix} {stringBuilder}";
            }
        }
    }
}