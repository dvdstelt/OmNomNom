<script>
  import '../Branding/styles/global.css';
  import Layout from '../Branding/Layout.svelte';
  import { onMount } from 'svelte';
  import { orderId } from '$lib/stores/orderId.js';
  import { refreshCartCount } from '$lib/stores/cart.js';

  let { children } = $props();

  // Subscribing to a Svelte store fires synchronously with the current
  // value, so this single subscribe both seeds the cart count on mount
  // and keeps it in sync with later orderId changes.
  onMount(() => orderId.subscribe(refreshCartCount));
</script>

<Layout>
  {@render children()}
</Layout>
