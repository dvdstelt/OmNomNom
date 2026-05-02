<script>
  import { cartItemCount } from '$lib/stores/cart.js';
</script>

<a href="/cart" class="cart-link" aria-label="View cart">
  <svg class="cart-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
    <circle cx="9" cy="21" r="1" />
    <circle cx="20" cy="21" r="1" />
    <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6" />
  </svg>
  {#if $cartItemCount > 0}
    <span class="cart-badge">{$cartItemCount}</span>
  {/if}
</a>
