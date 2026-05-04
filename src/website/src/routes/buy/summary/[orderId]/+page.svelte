<script>
  import { onMount } from 'svelte';
  import { page } from '$app/state';
  import { goto } from '$app/navigation';
  import { gateway } from '$lib/api/gateway.js';
  import { orderId as orderIdStore } from '$lib/stores/orderId.js';

  import CheckoutProgress from '../../../../Branding/CheckoutProgress.svelte';
  import CartItemList from '../../../../Catalog/CartItemList.svelte';
  import OrderSummaryCard from '../../../../Finance/OrderSummaryCard.svelte';
  import CreditCardSummary from '../../../../PaymentInfo/CreditCardSummary.svelte';

  let summary = $state(null);
  let address = $state(null);
  let loading = $state(true);
  let placing = $state(false);

  let routeOrderId = $derived(page.params.orderId);

  let products = $derived(Object.values(summary?.products ?? {}));
  let shippingPrice = $derived(summary?.deliveryOption?.price ?? 0);
  let billingSameAsShipping = $derived(
    address &&
      ['street', 'zipCode', 'town', 'country'].every(
        (k) => address.shippingAddress?.[k] === address.billingAddress?.[k]
      )
  );

  onMount(async () => {
    try {
      const [summaryData, addressData] = await Promise.all([
        gateway.getSummary(routeOrderId),
        gateway.getAddress(routeOrderId)
      ]);
      summary = summaryData;
      address = addressData;
    } finally {
      loading = false;
    }
  });

  async function placeOrder() {
    placing = true;
    try {
      await gateway.placeOrder(routeOrderId);
      orderIdStore.clear();
      await goto('/');
    } finally {
      placing = false;
    }
  }
</script>

<svelte:head>
  <title>Order Summary — OmNomNom</title>
</svelte:head>

<CheckoutProgress stage="summary" orderId={routeOrderId} />

<main class="page-container">
  {#if loading}
    <p style="color: var(--color-text-muted); padding: 48px 0; text-align: center;">Loading…</p>
  {:else if !summary}
    <p style="color: var(--color-text-muted); padding: 48px 0; text-align: center;">
      Could not load order.
    </p>
  {:else}
    <h1 class="page-title" style="margin-bottom:24px">Review your order</h1>
    <div class="checkout-layout">
      <div class="checkout-main">
        <div class="form-section">
          <div style="display: flex; justify-content: space-between; align-items: baseline;">
            <h2>Shipping Address</h2>
            <a href="/buy/address/{routeOrderId}">change</a>
          </div>
          {#if address?.shippingAddress}
            <div>{address.shippingAddress.fullName}</div>
            <div>{address.shippingAddress.street}</div>
            <div>{address.shippingAddress.town}, {address.shippingAddress.country}</div>
            <div>{address.shippingAddress.zipCode}</div>
          {/if}
        </div>

        <div class="form-section">
          <div style="display: flex; justify-content: space-between; align-items: baseline;">
            <h2>Payment</h2>
            <a href="/buy/payment/{routeOrderId}">change</a>
          </div>
          <CreditCardSummary
            cardType={summary.creditCardType}
            lastDigits={summary.creditCardLastDigits}
          />
        </div>

        <div class="form-section">
          <div style="display: flex; justify-content: space-between; align-items: baseline;">
            <h2>Billing Address</h2>
            <a href="/buy/address/{routeOrderId}">change</a>
          </div>
          {#if billingSameAsShipping}
            <div>Same as shipping address</div>
          {:else if address?.billingAddress}
            <div>{address.billingAddress.street}</div>
            <div>{address.billingAddress.town}, {address.billingAddress.country}</div>
            <div>{address.billingAddress.zipCode}</div>
          {/if}
        </div>

        <div class="form-section">
          <h2>Items</h2>
          <CartItemList items={products} editable={false} />
        </div>
      </div>

      <OrderSummaryCard
        items={products}
        {shippingPrice}
        totalOverride={summary.totalPrice}
      >
        <button
          type="button"
          class="btn-primary"
          style="width: 100%;"
          disabled={placing}
          onclick={placeOrder}
        >
          {placing ? 'Placing order…' : 'Place your order'}
        </button>
      </OrderSummaryCard>
    </div>
  {/if}
</main>
