<script>
  import '../Branding/styles/global.css';
  import Layout from '../Branding/Layout.svelte';
  import { onMount } from 'svelte';
  import { orderId } from '$lib/stores/orderId.js';
  import { refreshCartCount } from '$lib/stores/cart.js';
  import { get } from 'svelte/store';

  let { children } = $props();

  onMount(() => {
    refreshCartCount(get(orderId));
    return orderId.subscribe((value) => refreshCartCount(value));
  });
</script>

<Layout>
  {@render children()}
</Layout>
