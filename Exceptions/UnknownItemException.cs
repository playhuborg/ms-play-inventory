using System;

namespace Play.Inventory.Exceptions
{
    internal class UnknownItemException : Exception
    {
        public Guid ItemId;

        public UnknownItemException(Guid itemId) : base($"Unknown item '{itemId}")
        {
            this.ItemId = itemId;
        }
    }
}