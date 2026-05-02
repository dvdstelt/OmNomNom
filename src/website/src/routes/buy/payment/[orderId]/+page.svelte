<script>
  import { onMount } from 'svelte';
  import { page } from '$app/state';
  import { goto } from '$app/navigation';
  import { gateway } from '$lib/api/gateway.js';

  import CheckoutProgress from '../../../../Branding/CheckoutProgress.svelte';
  import CreditCardPicker from '../../../../PaymentInfo/CreditCardPicker.svelte';

  // Hard-coded customer for the demo, matching the React app.
  const CUSTOMER_ID = '01093176-1308-493a-8f67-da5d278e2375';

  let cards = $state([]);
  let selectedCardId = $state(null);
  let loading = $state(true);
  let saving = $state(false);

  let routeOrderId = $derived(page.params.orderId);

  onMount(async () => {
    try {
      const [cardsData, paymentData] = await Promise.all([
        gateway.getCreditCards(CUSTOMER_ID),
        gateway.getPayment(routeOrderId)
      ]);
      cards = cardsData?.creditCards ?? [];
      selectedCardId = paymentData?.creditCardId || cards[0]?.cardId || null;
    } finally {
      loading = false;
    }
  });

  async function saveAndContinue() {
    if (!selectedCardId) return;
    saving = true;
    try {
      await gateway.savePayment(routeOrderId, selectedCardId);
      await goto(`/buy/summary/${routeOrderId}`);
    } finally {
      saving = false;
    }
  }
</script>

<svelte:head>
  <title>Payment — OmNomNom</title>
</svelte:head>

<CheckoutProgress stage="payment" orderId={routeOrderId} />

<main class="page-container">
  <div class="checkout-layout">
    <div class="checkout-main">
      <h1 class="page-title" style="margin-bottom:24px">Payment Method</h1>

      {#if loading}
        <p style="color: var(--color-text-muted); padding: 48px 0; text-align: center;">Loading…</p>
      {:else}
        <div class="form-section">
          <h2>Your credit and debit cards</h2>
          <CreditCardPicker {cards} bind:selectedId={selectedCardId} />
        </div>

        <div class="btn-group">
          <a href="/buy/shipping/{routeOrderId}" class="btn-secondary">Back to Shipping</a>
          <button type="button" class="btn-primary" disabled={saving || !selectedCardId} onclick={saveAndContinue}>
            {saving ? 'Saving…' : 'Review Order'}
          </button>
        </div>
      {/if}
    </div>
  </div>
</main>
