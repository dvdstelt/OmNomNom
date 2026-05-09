namespace Catalog.ServiceComposition.Events;

// Catalog announces the filtered candidate ProductIds. Subscribers that
// own a sort signal (e.g. Marketing) read whatever request parameters
// they need themselves and write the ordered IDs back into OrderedIds.
// Catalog has no opinion on how, or by what, the candidates get sorted.
public class ProductCandidatesAvailable
{
    public required IReadOnlyList<Guid> CandidateIds { get; init; }
    public IReadOnlyList<Guid>? OrderedIds { get; set; }
}
