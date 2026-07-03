namespace Marketing.Endpoint.Messages.Commands;

// CHAOS / TEST ONLY. The command whose only handler (HangMarketingHandler)
// deliberately hangs the Marketing pump so the endpoint's liveness heartbeat
// goes stale. A plain class is enough: the handler's [Handler] attribute and its
// Handle(HangMarketing, ...) signature are what the NServiceBus source generator
// keys off to register it, so the message needs no marker interface and no
// dedicated *.Messages assembly.
public class HangMarketing;
