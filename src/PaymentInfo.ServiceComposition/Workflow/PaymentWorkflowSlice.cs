using PaymentInfo.Endpoint.Messages.Commands;
using WorkflowComposer;

namespace PaymentInfo.ServiceComposition.Workflow;

// The user's chosen credit card for this in-flight checkout. The
// customer id is environmental (today, hardcoded server-side until
// auth lands), so it isn't stored in the slice — it's filled in by
// BuildSubmitCommand to keep parity with the existing handler.
public sealed record PaymentSlice(Guid CreditCardId);

public class PaymentWorkflowSlice : WorkflowSlice<PaymentSlice>
{
    public const string Key = "PaymentInfo.Card";

    // Hardcoded customer id, mirroring PaymentSubmitHandler. When auth
    // arrives this becomes a real value derived from the session.
    static readonly Guid PlaceholderCustomerId = Guid.Parse("01093176-1308-493a-8f67-da5d278e2375");

    public override string SliceKey => Key;

    protected override IReadOnlyList<string> Validate(PaymentSlice slice) =>
        slice.CreditCardId == Guid.Empty
            ? ["CreditCardId is required."]
            : [];

    protected override object? BuildSubmitCommand(Guid orderId, PaymentSlice slice) =>
        new SubmitPaymentInfo
        {
            OrderId = orderId,
            CreditCardId = slice.CreditCardId,
            CustomerId = PlaceholderCustomerId
        };
}
