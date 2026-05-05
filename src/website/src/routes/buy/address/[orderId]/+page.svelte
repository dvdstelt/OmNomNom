<script>
  import { onMount } from 'svelte';
  import { page } from '$app/state';
  import { goto } from '$app/navigation';
  import { gateway } from '$lib/api/gateway.js';

  import CheckoutProgress from '../../../../Branding/CheckoutProgress.svelte';
  import AddressFormFields from '../../../../Shipping/AddressFormFields.svelte';
  import OrderSummaryCard from '../../../../Finance/OrderSummaryCard.svelte';

  const blankAddress = {
    id: '',
    fullName: '',
    street: '',
    zipCode: '',
    town: '',
    country: ''
  };

  let shippingAddress = $state({ ...blankAddress });
  let billingAddress = $state({ ...blankAddress });
  let billingSameAsShipping = $state(true);
  let cartItems = $state([]);
  let loading = $state(true);
  let saving = $state(false);

  let routeOrderId = $derived(page.params.orderId);

  function addressesMatch(a, b) {
    return Object.keys(blankAddress)
      .filter((key) => key !== 'id')
      .every((key) => (a?.[key] ?? '') === (b?.[key] ?? ''));
  }

  onMount(async () => {
    try {
      const data = await gateway.getAddress(routeOrderId);
      if (data?.shippingAddress) shippingAddress = { ...blankAddress, ...data.shippingAddress };
      if (data?.billingAddress) billingAddress = { ...blankAddress, ...data.billingAddress };
      billingSameAsShipping = addressesMatch(shippingAddress, billingAddress);
      cartItems = data?.cartItems ?? [];
    } finally {
      loading = false;
    }
  });

  async function saveAndContinue() {
    saving = true;
    try {
      await gateway.saveAddress(routeOrderId, {
        shippingAddress,
        billingAddress: billingSameAsShipping ? shippingAddress : billingAddress
      });
      await goto(`/buy/shipping/${routeOrderId}`);
    } finally {
      saving = false;
    }
  }
</script>

<svelte:head>
  <title>Shipping Address — OmNomNom</title>
</svelte:head>

<CheckoutProgress stage="address" orderId={routeOrderId} />

<main class="page-container">
  <div class="checkout-layout">
    <div class="checkout-main">
      <h1 class="page-title" style="margin-bottom:24px">Shipping &amp; Billing</h1>

      {#if loading}
        <p style="color: var(--color-text-muted); padding: 48px 0; text-align: center;">Loading…</p>
      {:else}
        <div class="form-section">
          <h2>Shipping Address</h2>
          <AddressFormFields bind:address={shippingAddress} idPrefix="ship" />
        </div>

        <div class="form-section">
          <label>
            <input type="checkbox" bind:checked={billingSameAsShipping} />
            Billing address is the same as shipping address
          </label>
        </div>

        {#if !billingSameAsShipping}
          <div class="form-section">
            <h2>Billing Address</h2>
            <AddressFormFields bind:address={billingAddress} idPrefix="bill" />
          </div>
        {/if}

        <div class="btn-group">
          <a href="/cart/{routeOrderId}" class="btn-secondary">Back to Cart</a>
          <button type="button" class="btn-primary" disabled={saving} onclick={saveAndContinue}>
            {saving ? 'Saving…' : 'Ship to this Address'}
          </button>
        </div>
      {/if}
    </div>
    {#if cartItems.length > 0}
      <OrderSummaryCard items={cartItems} />
    {/if}
  </div>
</main>
