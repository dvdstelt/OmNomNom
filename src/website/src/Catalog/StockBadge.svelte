<script>
  // The stock count comes from a read model that is eventually
  // consistent with the inventory ledger, so the UI quotes an
  // upper bound ("Less than X left") rather than the exact figure.
  // That way, if the read model is briefly stale and the real
  // count has dropped to zero, we haven't told the customer "Only
  // 3 left" when there are actually none.
  const LOW_STOCK_THRESHOLD = 10;

  let { inStock = 0 } = $props();
  let isOut = $derived(inStock <= 0);
  let isLow = $derived(inStock > 0 && inStock < LOW_STOCK_THRESHOLD);
</script>

<div class="beer-stock" class:low-stock={isLow} class:out-of-stock={isOut}>
  {#if isOut}
    Out of Stock
  {:else if isLow}
    Less than {inStock + 1} left
  {:else}
    In Stock
  {/if}
</div>
