<script>
  import '../Branding/styles/global.css';
  import 'flag-icons/css/flag-icons.min.css';
  import Layout from '../Branding/Layout.svelte';
  import { onMount } from 'svelte';
  import { page } from '$app/state';
  import { orderId } from '$lib/stores/orderId.js';
  import { refreshCartCount } from '$lib/stores/cart.js';

  let { children } = $props();

  // Subscribing to a Svelte store fires synchronously with the current
  // value, so this single subscribe both seeds the cart count on mount
  // and keeps it in sync with later orderId changes. The cart-count
  // badge is hidden under /cart/* and /buy/* (Layout.svelte switches
  // to a "Secure Checkout" header), so skip the fetch on those routes.
  onMount(() =>
    orderId.subscribe((id) => {
      const path = page.url.pathname;
      if (path.startsWith('/cart/') || path.startsWith('/buy/')) return;
      refreshCartCount(id);
    })
  );
</script>

<Layout>
  {@render children()}
</Layout>
