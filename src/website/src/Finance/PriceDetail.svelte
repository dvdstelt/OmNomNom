<script>
  import { effectivePrice, hasDiscount } from './effectivePrice.js';

  let { price = 0, discount = 0 } = $props();
  let showDiscount = $derived(hasDiscount(price, discount));
  let savingsPercent = $derived(
    showDiscount ? Math.round(((price - discount) / price) * 100) : 0
  );
  let format = (value) => '$' + Number(value ?? 0).toFixed(2);
</script>

<div class="product-pricing">
  <span class="beer-price">{format(effectivePrice({ price, discount }))}</span>
  {#if showDiscount}
    <span class="beer-original-price">{format(price)}</span>
    <span class="product-savings">Save {savingsPercent}%</span>
  {/if}
</div>
