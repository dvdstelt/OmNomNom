<script>
  import { onMount } from 'svelte';
  import { page } from '$app/state';
  import { goto } from '$app/navigation';
  import { gateway } from '$lib/api/gateway.js';
  import { orderId } from '$lib/stores/orderId.js';
  import { refreshCartCount } from '$lib/stores/cart.js';

  import CheckoutProgress from '../../../Branding/CheckoutProgress.svelte';
  import CartItemList from '../../../Catalog/CartItemList.svelte';
  import OrderSummaryCard from '../../../Finance/OrderSummaryCard.svelte';

  let items = $state([]);
  let loading = $state(true);

  let routeOrderId = $derived(page.params.orderId);

  async function load() {
    loading = true;
    try {
      const data = await gateway.getCart(routeOrderId);
      items = data?.cartItems ?? [];
    } finally {
      loading = false;
    }
  }

  onMount(load);

  async function changeQuantity(productId, newQuantity) {
    if (newQuantity <= 0) return removeItem(productId);
    const current = items.find((it) => it.productId === productId)?.quantity ?? 0;
    const delta = newQuantity - current;
    if (delta === 0) return;
    items = items.map((it) =>
      it.productId === productId ? { ...it, quantity: newQuantity } : it
    );
    try {
      await gateway.addProductToCart(routeOrderId, { id: productId, quantity: delta });
      await refreshCartCount(routeOrderId);
    } catch {
      await load();
    }
  }

  async function removeItem(productId) {
    const current = items.find((it) => it.productId === productId)?.quantity ?? 0;
    items = items.filter((it) => it.productId !== productId);
    if (current <= 0) return;
    try {
      await gateway.addProductToCart(routeOrderId, { id: productId, quantity: -current });
      await refreshCartCount(routeOrderId);
    } catch {
      await load();
    }
  }

  async function saveAndContinue() {
    await gateway.saveCart(routeOrderId, items);
    await goto(`/buy/address/${routeOrderId}`);
  }

  async function discardCart() {
    orderId.clear();
    await goto('/');
  }
</script>

<svelte:head>
  <title>Shopping Cart — OmNomNom</title>
</svelte:head>

<CheckoutProgress stage="cart" orderId={routeOrderId} />

<main class="page-container">
  {#if loading}
    <p style="color: var(--color-text-muted); padding: 48px 0; text-align: center;">Loading cart…</p>
  {:else}
    <div class="checkout-layout">
      <div class="checkout-main">
        <h1 class="page-title" style="margin-bottom:24px">Shopping Cart</h1>
        <CartItemList
          {items}
          editable={true}
          onQuantityChange={changeQuantity}
          onRemove={removeItem}
        />
        {#if items.length > 0}
          <div class="btn-group">
            <button type="button" class="btn-secondary" onclick={discardCart}>
              Discard Cart
            </button>
            <button type="button" class="btn-primary" onclick={saveAndContinue}>
              Proceed to Checkout
            </button>
          </div>
        {/if}
      </div>
      {#if items.length > 0}
        <OrderSummaryCard {items} />
      {/if}
    </div>
  {/if}
</main>
