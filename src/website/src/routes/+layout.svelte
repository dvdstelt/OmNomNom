<script>
  import '../Branding/styles/global.css';
  import 'flag-icons/css/flag-icons.min.css';
  import Layout from '../Branding/Layout.svelte';
  import { page } from '$app/state';
  import { orderId } from '$lib/stores/orderId.js';
  import { refreshCartCount } from '$lib/stores/cart.js';

  let { children } = $props();

  // The cart-count badge is hidden under /cart/* and /buy/* (Layout.svelte
  // switches to a "Secure Checkout" header), so skip the fetch on those
  // routes. Track BOTH page.url.pathname and $orderId so the effect re-runs
  // on navigation as well as orderId changes; reacting only to orderId
  // misses the case where the user lands directly on /cart/* (initial mount
  // skips the fetch and the count then stays at 0 forever) and the case
  // where Discard Cart clears the orderId while still on /cart/* (the
  // path-skip swallows the clear and the badge keeps the old count after
  // we navigate home).
  $effect(() => {
    const path = page.url.pathname;
    const id = $orderId;
    if (path.startsWith('/cart/') || path.startsWith('/buy/')) return;
    refreshCartCount(id);
  });
</script>

<Layout>
  {@render children()}
</Layout>
