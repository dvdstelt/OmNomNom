<script>
  import { goto } from '$app/navigation';
  import { orderId as orderIdStore } from '$lib/stores/orderId.js';

  async function startOver() {
    orderIdStore.clear();
    await goto('/');
  }
</script>

<div style="text-align: center; padding: 64px 24px;">
  <h1 class="page-title">Your session expired</h1>
  <p style="color: var(--color-text-muted); margin: 16px 0 32px;">
    It&rsquo;s been a while since you last touched this cart, so we cleared it.
    Pick your beers again and we&rsquo;ll get you back to checkout.
  </p>
  <button type="button" class="btn-primary" onclick={startOver}>
    Start a new cart
  </button>
</div>
