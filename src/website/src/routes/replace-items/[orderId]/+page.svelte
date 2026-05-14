<script>
  import { page } from '$app/state';

  let routeOrderId = $derived(page.params.orderId);
</script>

<svelte:head>
  <title>Replace items — OmNomNom</title>
</svelte:head>

<main class="page-container">
  <h1 class="page-title">Thanks!</h1>
  <p class="page-subtitle">
    The replacement-items flow isn&rsquo;t implemented yet. We received your
    order ID and we&rsquo;ll wire this up soon.
  </p>
  <p style="color: var(--color-text-muted); margin-top: 24px; font-size: 13px;">
    Order: <code>{routeOrderId}</code>
  </p>
</main>
