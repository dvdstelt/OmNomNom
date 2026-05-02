<script>
  let { price = 0, discount = 0 } = $props();
  let hasDiscount = $derived(discount > 0 && discount < price);
  let savingsPercent = $derived(hasDiscount ? Math.round(((price - discount) / price) * 100) : 0);
  let format = (value) => '$' + Number(value ?? 0).toFixed(2);
</script>

<div class="product-pricing">
  <span class="beer-price">{format(hasDiscount ? discount : price)}</span>
  {#if hasDiscount}
    <span class="beer-original-price">{format(price)}</span>
    <span class="product-savings">Save {savingsPercent}%</span>
  {/if}
</div>
