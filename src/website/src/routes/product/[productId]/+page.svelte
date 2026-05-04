<script>
  import { onMount } from 'svelte';
  import { page } from '$app/state';
  import { goto } from '$app/navigation';
  import { gateway } from '$lib/api/gateway.js';
  import { orderId } from '$lib/stores/orderId.js';
  import { refreshCartCount } from '$lib/stores/cart.js';
  import { get } from 'svelte/store';

  import BeerImage from '../../../Catalog/BeerImage.svelte';
  import ProductDescription from '../../../Catalog/ProductDescription.svelte';
  import StockBadge from '../../../Catalog/StockBadge.svelte';
  import ProductRating from '../../../Marketing/ProductRating.svelte';
  import PriceDetail from '../../../Finance/PriceDetail.svelte';

  let product = $state(null);
  let loading = $state(true);
  let quantity = $state(1);
  let adding = $state(false);

  let productId = $derived(page.params.productId);

  onMount(async () => {
    try {
      const data = await gateway.getProduct(productId);
      product = data.product ?? data;
    } finally {
      loading = false;
    }
  });

  function changeQty(delta) {
    const max = product?.inStock ?? 99;
    quantity = Math.max(1, Math.min(quantity + delta, max));
  }

  async function addToCart() {
    if (adding) return;
    adding = true;
    try {
      const result = await gateway.addProductToCart(get(orderId), {
        id: productId,
        quantity
      });
      if (result?.orderId) orderId.set(result.orderId);
      await refreshCartCount(get(orderId));
      await goto('/');
    } finally {
      adding = false;
    }
  }
</script>

<svelte:head>
  <title>{product?.name ? `${product.name} — OmNomNom` : 'OmNomNom'}</title>
</svelte:head>

<main class="page-container">
  {#if loading}
    <p style="color: var(--color-text-muted); padding: 48px 0; text-align: center;">Loading…</p>
  {:else if !product}
    <div class="cart-empty">
      <h2>Beer not found</h2>
      <a href="/" class="btn-primary">Back to Shop</a>
    </div>
  {:else}
    <div class="product-layout">
      <BeerImage hero name={product.name} imageUrl={product.imageUrl} category={product.category} />
      <div class="product-details">
        <h1>{product.name}</h1>
        <ProductRating stars={product.stars} reviewCount={product.reviewCount} />
        <PriceDetail price={product.price} discount={product.discount} />
        <ProductDescription description={product.description} />
        <StockBadge inStock={product.inStock} />
        <div class="add-to-cart-row">
          <div class="qty-selector">
            <button type="button" onclick={() => changeQty(-1)}>-</button>
            <div class="qty-display">{quantity}</div>
            <button type="button" onclick={() => changeQty(1)}>+</button>
          </div>
          <button
            type="button"
            class="btn-add-to-cart"
            disabled={adding || product.inStock <= 0}
            onclick={addToCart}
          >
            {adding ? 'Adding…' : 'Add to Cart'}
          </button>
        </div>
      </div>
    </div>
  {/if}
</main>
