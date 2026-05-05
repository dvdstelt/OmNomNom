<script>
  import CartItemRow from './CartItemRow.svelte';

  let { items = [], editable = true, onQuantityChange = () => {}, onRemove = () => {} } = $props();
</script>

{#if items.length === 0}
  <div class="cart-empty">
    <h2>Your cart is empty</h2>
    <p>Looks like you haven't added any beers yet.</p>
    <a href="/" class="btn-primary">Browse Beers</a>
  </div>
{:else}
  <div class="cart-items">
    {#each items as item (item.productId)}
      <CartItemRow {item} {editable} {onQuantityChange} {onRemove} />
    {/each}
  </div>
{/if}
