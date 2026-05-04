<script>
  import { onMount } from 'svelte';
  import { page } from '$app/state';
  import { goto } from '$app/navigation';
  import { gateway } from '$lib/api/gateway.js';

  import CheckoutProgress from '../../../../Branding/CheckoutProgress.svelte';
  import DeliveryOptions from '../../../../Shipping/DeliveryOptions.svelte';
  import CartItemList from '../../../../Catalog/CartItemList.svelte';
  import OrderSummaryCard from '../../../../Finance/OrderSummaryCard.svelte';

  let deliveryOptions = $state([]);
  let cartItems = $state([]);
  let selectedDeliveryOptionId = $state(null);
  let loading = $state(true);
  let saving = $state(false);

  let routeOrderId = $derived(page.params.orderId);

  // Live-update the sidebar's Shipping line as the user picks a
  // delivery option, instead of waiting for a round-trip.
  let selectedDeliveryPrice = $derived(
    deliveryOptions.find((o) => o.deliveryOptionId === selectedDeliveryOptionId)?.price ?? 0
  );

  onMount(async () => {
    try {
      const data = await gateway.getShipping(routeOrderId);
      deliveryOptions = data?.deliveryOptions ?? [];
      cartItems = data?.cartItems ?? [];
      selectedDeliveryOptionId =
        data?.selectedDeliveryOption ?? deliveryOptions[0]?.deliveryOptionId ?? null;
    } finally {
      loading = false;
    }
  });

  async function saveAndContinue() {
    if (!selectedDeliveryOptionId) return;
    saving = true;
    try {
      await gateway.saveShipping(routeOrderId, selectedDeliveryOptionId);
      await goto(`/buy/payment/${routeOrderId}`);
    } finally {
      saving = false;
    }
  }
</script>

<svelte:head>
  <title>Shipping Options — OmNomNom</title>
</svelte:head>

<CheckoutProgress stage="shipping" orderId={routeOrderId} />

<main class="page-container">
  <div class="checkout-layout">
    <div class="checkout-main">
      <h1 class="page-title" style="margin-bottom:24px">Choose Shipping</h1>

      {#if loading}
        <p style="color: var(--color-text-muted); padding: 48px 0; text-align: center;">Loading…</p>
      {:else}
        <div class="form-section">
          <h2>Delivery Speed</h2>
          <DeliveryOptions options={deliveryOptions} bind:selectedId={selectedDeliveryOptionId} />
        </div>

        {#if cartItems.length > 0}
          <div class="form-section">
            <h2>Shipping from OmNomNom</h2>
            <CartItemList items={cartItems} editable={false} />
          </div>
        {/if}

        <div class="btn-group">
          <a href="/buy/address/{routeOrderId}" class="btn-secondary">Back to Address</a>
          <button type="button" class="btn-primary" disabled={saving} onclick={saveAndContinue}>
            {saving ? 'Saving…' : 'Continue to Payment'}
          </button>
        </div>
      {/if}
    </div>
    {#if cartItems.length > 0}
      <OrderSummaryCard items={cartItems} shippingPrice={selectedDeliveryPrice} />
    {/if}
  </div>
</main>
