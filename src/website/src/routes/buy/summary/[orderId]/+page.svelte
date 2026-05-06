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
        <div class="summary-section">
          <h3>
            Shipping & Billing Address
            <a href="/buy/address/{routeOrderId}">Change</a>
          </h3>
          {#if address?.shippingAddress}
            <div class="summary-detail">
              <div>{address.shippingAddress.fullName}</div>
              <div>{address.shippingAddress.street}</div>
              <div>
                {address.shippingAddress.town}, {address.shippingAddress.country}
              </div>
              <div>{address.shippingAddress.zipCode}</div>
              <div style="margin-top: 12px; font-style: italic;">
                {#if billingSameAsShipping}
                  Billing: same as shipping address
                {:else if address?.billingAddress}
                  Billing:
                  {address.billingAddress.street},
                  {address.billingAddress.town}, {address.billingAddress.country}
                  ({address.billingAddress.zipCode})
                {/if}
              </div>
            </div>
          {/if}
        </div>

        {#if summary.deliveryOption}
          <div class="summary-section">
            <h3>
              Delivery Method
              <a href="/buy/shipping/{routeOrderId}">Change</a>
            </h3>
            <div class="summary-detail">
              <div>{summary.deliveryOption.deliveryOptionName}</div>
              {#if summary.deliveryOption.deliveryOptionDescription}
                <div>{summary.deliveryOption.deliveryOptionDescription}</div>
              {/if}
              <div>${Number(summary.deliveryOption.price ?? 0).toFixed(2)}</div>
            </div>
          </div>
        {/if}

        <div class="summary-section">
          <h3>
            Payment
            <a href="/buy/payment/{routeOrderId}">Change</a>
          </h3>
          <div class="summary-detail">
            <CreditCardSummary
              cardType={summary.creditCardType}
              lastDigits={summary.creditCardLastDigits}
            />
          </div>
        </div>

        <div class="summary-section summary-items">
          <h3>Items</h3>
          <CartItemList items={products} editable={false} />
        </div>
      </div>

      <OrderSummaryCard
        items={products}
        {shippingPrice}
        totalOverride={summary.totalPrice}
      >
        <div class="place-order-section">
          <button
            type="button"
            class="btn-primary"
            style="width: 100%;"
            disabled={placing}
            onclick={placeOrder}
          >
            {placing ? 'Placing order…' : 'Place your order'}
          </button>
          <p>By placing your order, you agree to OmNomNom's terms and conditions.</p>
        </div>
      </OrderSummaryCard>
    </div>
  {/if}
</main>
