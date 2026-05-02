<script>
  let { inStock = 0 } = $props();
  let isLow = $derived(inStock > 0 && inStock <= 5);
  let isOut = $derived(inStock <= 0);
</script>

<div class="beer-stock" class:low-stock={isLow} class:out-of-stock={isOut}>
  {#if isOut}
    Out of Stock
  {:else if isLow}
    Only {inStock} left!
  {:else}
    In Stock
  {/if}
</div>
