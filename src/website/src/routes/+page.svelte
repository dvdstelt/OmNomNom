<script>
  import { onMount } from 'svelte';
  import { gateway } from '$lib/api/gateway.js';
  import FilterBar from '../Branding/FilterBar.svelte';
  import BeerImage from '../Catalog/BeerImage.svelte';
  import BeerName from '../Catalog/BeerName.svelte';
  import StockBadge from '../Catalog/StockBadge.svelte';
  import ProductRating from '../Marketing/ProductRating.svelte';
  import Price from '../Finance/Price.svelte';

  let products = $state([]);
  let loading = $state(true);
  let selectedCategory = $state('All');

  onMount(async () => {
    try {
      const data = await gateway.getProducts();
      products = data.products ?? [];
    } finally {
      loading = false;
    }
  });

  let categories = $derived([
    ...new Set(products.map((p) => p.category).filter(Boolean))
  ]);

  let visible = $derived(
    selectedCategory === 'All'
      ? products
      : products.filter((p) => p.category === selectedCategory)
  );
</script>

<svelte:head>
  <title>OmNomNom — Craft Beers</title>
</svelte:head>

<main class="page-container">
  <h1 class="page-title">Discover Craft Beers</h1>
  <p class="page-subtitle">Hand-picked selections for the discerning palate</p>

  <FilterBar {categories} bind:selected={selectedCategory} />

  {#if loading}
    <p style="color: var(--color-text-muted); padding: 48px 0; text-align: center;">
      Loading beers…
    </p>
  {:else if visible.length === 0}
    <p style="color: var(--color-text-muted); padding: 48px 0; text-align: center;">
      No beers match your filter.
    </p>
  {:else}
    <div class="beer-grid">
      {#each visible as product (product.productId)}
        <a class="beer-card" href="/product/{product.productId}">
          <BeerImage
            name={product.name}
            imageUrl={product.imageUrl}
            category={product.category}
          />
          <div class="beer-info">
            <BeerName name={product.name} />
            <ProductRating stars={product.stars} reviewCount={product.reviewCount} />
            <Price price={product.price} discount={product.discount} />
            <StockBadge inStock={product.inStock} />
          </div>
        </a>
      {/each}
    </div>
  {/if}
</main>
