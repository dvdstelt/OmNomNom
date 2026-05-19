<script>
  // Compact "Order Total: $XX.XX" pill used in places where the full
  // OrderSummaryCard sidebar isn't shown (mobile cart, today). Reuses
  // the same pricing math via effectivePrice so the figure can never
  // drift from what the sidebar shows on desktop.

  import { discountAmount } from './effectivePrice.js';

  let { items = [], shippingPrice = null, totalOverride = null } = $props();

  let total = $derived(
    totalOverride ??
      items.reduce(
        (sum, it) =>
          sum +
          (Number(it?.price ?? 0) - discountAmount(it)) *
            Number(it?.quantity ?? 0),
        0
      ) + Number(shippingPrice ?? 0)
  );

  let format = (value) => '$' + Number(value ?? 0).toFixed(2);
</script>

<div class="order-total-line">
  <span class="order-total-line-label">Order Total</span>
  <span class="order-total-line-amount">{format(total)}</span>
</div>
