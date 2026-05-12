namespace PaymentInfo.ServiceComposition.Workflow;

// Wire shape of the POST /buy/payment/{orderId} body. The frontend
// posts { creditCardId } when the user picks a stored card on the
// payment screen. The CustomerId is environmental and is filled in
// server-side by PaymentWorkflowSlice.BuildSubmitCommand.
public class PaymentForm
{
    public Guid CreditCardId { get; set; }
}
