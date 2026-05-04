<script>
  // Reusable Order Summary sidebar for the checkout flow. One copy of
  // the markup, one place that knows how to apply the discount rule.
  //
  // Props:
  //   items          array of { price, discount, quantity } objects
  //                  (cart items, summary products, etc.)
  //   shippingPrice  number, or null/undefined to hide the Shipping
  //                  row (e.g., before a delivery option is chosen)
  //   totalOverride  if present, replaces the locally-computed total.
  //                  Use the server's authoritative figure on the
  //                  summary page (the gateway already adds shipping
  //                  and applies EffectivePrice); leave null on
  //                  cart/shipping where the local sum is enough.
  //   children       optional snippet rendered below the total. Used
  //                  by the summary page to drop in the Place Order
  //                  button without this microview knowing about it.

  import OrderTotal from './OrderTotal.svelte';
  import { discountAmount } from './effectivePrice.js';

  let {
    items = [],
    shippingPrice = null,
    totalOverride = null,
    children
  } = $props();

  let itemsSubtotal = $derived(
    items.reduce((sum, it) => sum + (it.price ?? 0) * (it.quantity ?? 0), 0)
  );
  let discountTotal = $derived(
    items.reduce((sum, it) => sum + discountAmount(it) * (it.quantity ?? 0), 0)
  );
  let computedTotal = $derived(
    itemsSubtotal - discountTotal + (shippingPrice ?? 0)
  );
  let total = $derived(totalOverride ?? computedTotal);
</script>

<aside>
  <div class="sidebar-card">
    <h3 class="sidebar-title">Order Summary</h3>
    <OrderTotal label="Items" amount={itemsSubtotal} />
    {#if discountTotal > 0}
      <OrderTotal label="Discount" amount={-discountTotal} discount />
    {/if}
    {#if shippingPrice !== null}
      <OrderTotal label="Shipping" amount={shippingPrice} />
    {/if}
    <OrderTotal label="Order Total" amount={total} emphasized />
    {#if children}
      <div style="margin-top: 16px;">{@render children()}</div>
    {/if}
  </div>
</aside>
