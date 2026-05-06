<script>
  import { onMount } from 'svelte';
  import { gateway } from '$lib/api/gateway.js';
  import FilterBar from '../Branding/FilterBar.svelte';
  import BeerImage from '../Catalog/BeerImage.svelte';
  import BeerName from '../Catalog/BeerName.svelte';
  import BreweryLine from '../Catalog/BreweryLine.svelte';
  import StockBadge from '../Catalog/StockBadge.svelte';
  import ProductRating from '../Marketing/ProductRating.svelte';
  import Price from '../Finance/Price.svelte';
  import SaveBadge from '../Finance/SaveBadge.svelte';

  let products = $state([]);
  let loading = $state(true);
  let selectedCategory = $state('All');
  let selectedBrewery = $state('All');
  let selectedCountry = $state('All');
  let selectedSort = $state('default');

  const sortOptions = [
    { value: 'default', label: 'Sort: Default' },
    { value: 'rating', label: 'Top rated' },
    { value: 'orderCount', label: 'Top sellers' },
    { value: 'trending', label: 'Trending now' }
  ];

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
  let breweries = $derived(
    [...new Set(products.map((p) => p.brewery).filter(Boolean))].sort()
  );
  let countries = $derived(
    [...new Set(products.map((p) => p.country).filter(Boolean))].sort()
  );

  let filtered = $derived(
    products.filter(
      (p) =>
        (selectedCategory === 'All' || p.category === selectedCategory) &&
        (selectedBrewery === 'All' || p.brewery === selectedBrewery) &&
        (selectedCountry === 'All' || p.country === selectedCountry)
    )
  );

  // Each sort key maps to a numeric reader; "default" preserves the
  // gateway's order (no reorder).
  const sortReaders = {
    default: () => 0,
    rating: (p) => p.rating ?? 0,
    orderCount: (p) => p.orderCount ?? 0,
    trending: (p) => p.trending ?? 0
  };

  let visible = $derived.by(() => {
    if (selectedSort === 'default') return filtered;
    const reader = sortReaders[selectedSort] ?? sortReaders.default;
    return [...filtered].sort((a, b) => reader(b) - reader(a));
  });
</script>

<svelte:head>
  <title>OmNomNom — Craft Beers</title>
</svelte:head>

<main class="page-container">
  <h1 class="page-title">Discover Craft Beers</h1>
  <p class="page-subtitle">Hand-picked selections for the discerning palate</p>

  <FilterBar
    {categories}
    {breweries}
    {countries}
    {sortOptions}
    bind:selected={selectedCategory}
    bind:selectedBrewery
    bind:selectedCountry
    bind:selectedSort
  />

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
          >
            <SaveBadge price={product.price} discount={product.discount} />
          </BeerImage>
          <div class="beer-info">
            <BeerName name={product.name} />
            <BreweryLine brewery={product.brewery} country={product.country} />
            <ProductRating rating={product.rating} ratingCount={product.ratingCount} />
            <Price price={product.price} discount={product.discount} />
            <StockBadge inStock={product.inStock} />
          </div>
        </a>
      {/each}
    </div>
  {/if}
</main>
