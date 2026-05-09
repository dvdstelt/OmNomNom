namespace Catalog.ServiceComposition.Events;

// Catalog announces the filtered candidate ProductIds and the raw sort
// token from the request. Subscribers that own a sort signal (e.g.
// Marketing) write the ordered IDs back into OrderedIds. Catalog
// inspects neither SortBy's value nor which subscriber filled the slot.
public class ProductCandidatesAvailable
{
    public required IReadOnlyList<Guid> CandidateIds { get; init; }
    public required string? SortBy { get; init; }
    public IReadOnlyList<Guid>? OrderedIds { get; set; }
}
