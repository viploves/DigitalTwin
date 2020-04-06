using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalTwin.ProvisioningApp
{
    public static partial class Actions
    {
        private static string GetCreationSummary(string itemTypeSingular, string itemTypePlural, List<Guid> createdIds)
            => createdIds.Count == 0
                ? $"Created 0 {itemTypePlural}."
                : createdIds.Count == 1
                    ? $"Created 1 {itemTypeSingular}: {AggregateIdsIntoString(createdIds)}"
                    : $"Created {createdIds.Count} {itemTypePlural}: {AggregateIdsIntoString(createdIds)}";

        private static string AggregateIdsIntoString(IEnumerable<Guid> ids) 
            => ids
                .Select(id => id.ToString())
                .Aggregate((acc, cur) => acc + ", " + cur);
    }
}