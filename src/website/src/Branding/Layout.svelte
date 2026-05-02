<script>
  import { page } from '$app/state';
  import CartIndicator from './CartIndicator.svelte';

  let { children } = $props();

  // Checkout flow uses a stripped-down "Secure Checkout" header (no shop nav,
  // no cart link). Anything under /cart/* or /buy/* counts as checkout.
  let isCheckout = $derived(
    page.url.pathname.startsWith('/cart/') || page.url.pathname.startsWith('/buy/')
  );
</script>

<header class="site-header">
  <div class="header-inner">
    <a href="/" class="logo">
      <span class="logo-icon">🍺</span>
      <span class="logo-text">OmNomNom</span>
      <span class="logo-tagline">Because beer is good, but beers are better</span>
    </a>
    {#if isCheckout}
      <div class="checkout-header-label">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="18" height="18">
          <rect x="3" y="11" width="18" height="11" rx="2" ry="2" />
          <path d="M7 11V7a5 5 0 0 1 10 0v4" />
        </svg>
        Secure Checkout
      </div>
    {:else}
      <nav class="header-nav">
        <a href="/">Shop</a>
        <CartIndicator />
      </nav>
    {/if}
  </div>
</header>

{@render children()}

<footer class="site-footer">
  &copy; 2026 OmNomNom Craft Beer Co. All rights reserved. Must be 21+ to purchase.
</footer>
