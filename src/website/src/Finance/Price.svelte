<script>
  import { effectivePrice, hasDiscount } from './effectivePrice.js';

  let { price = 0, discount = 0 } = $props();
  let showDiscount = $derived(hasDiscount(discount));
  let format = (value) => '$' + Number(value ?? 0).toFixed(2);
</script>

<div class="beer-pricing">
  <span class="beer-price">{format(effectivePrice({ price, discount }))}</span>
  {#if showDiscount}
    <span class="beer-original-price">{format(price)}</span>
  {/if}
</div>
