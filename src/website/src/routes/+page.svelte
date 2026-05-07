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

  // Facet lists are loaded once per session; the products page
  // changes in lockstep with filter/sort/page state.
  let categories = $state([]);
  let breweries = $state([]);
  let countries = $state([]);

  let products = $state([]);
  let loading = $state(true);

  let selectedCategories = $state([]);
  let selectedBreweries = $state([]);
  let selectedCountries = $state([]);
  let selectedSort = $state('default');

  let page = $state(1);
  let pageSize = $state(10);
  let totalCount = $state(0);
  let totalPages = $state(0);

  const sortOptions = [
    { value: 'default', label: 'Default' },
    { value: 'rating', label: 'Top rated' },
    { value: 'orderCount', label: 'Top sellers' },
    { value: 'trending', label: 'Trending now' }
  ];

  async function loadFacets() {
    const facets = await gateway.getFacets();
    categories = facets?.categories ?? [];
    breweries = facets?.breweries ?? [];
    countries = facets?.countries ?? [];
  }

  async function loadPage() {
    loading = true;
    try {
      const data = await gateway.getProducts({
        categories: selectedCategories,
        breweries: selectedBreweries,
        countries: selectedCountries,
        sort: selectedSort,
        page,
        size: pageSize
      });
      products = data?.products ?? [];
      pageSize = data?.pageSize ?? pageSize;
      totalCount = data?.totalCount ?? 0;
      totalPages = data?.totalPages ?? 0;
    } finally {
      loading = false;
    }
  }

  onMount(async () => {
    await loadFacets();
    await loadPage();
  });

  // Any filter or sort change resets to page 1 and refetches.
  function onFilterChange() {
    page = 1;
    loadPage();
  }

  function goToPage(target) {
    if (target < 1 || target > totalPages || target === page) return;
    page = target;
    loadPage();
  }
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
    bind:selectedCategories
    bind:selectedBreweries
    bind:selectedCountries
    bind:selectedSort
    onChange={onFilterChange}
  />

  {#if loading}
    <p style="color: var(--color-text-muted); padding: 48px 0; text-align: center;">
      Loading beers…
    </p>
  {:else if products.length === 0}
    <p style="color: var(--color-text-muted); padding: 48px 0; text-align: center;">
      No beers match your filter.
    </p>
  {:else}
    <div class="beer-grid">
      {#each products as product (product.productId)}
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

    {#if totalPages > 1}
      <nav class="pager" aria-label="Pagination">
        <button
          type="button"
          class="pager-btn"
          disabled={page <= 1}
          onclick={() => goToPage(page - 1)}
        >
          Previous
        </button>
        <span class="pager-status">
          Page {page} of {totalPages} · {totalCount} total
        </span>
        <button
          type="button"
          class="pager-btn"
          disabled={page >= totalPages}
          onclick={() => goToPage(page + 1)}
        >
          Next
        </button>
      </nav>
    {/if}
  {/if}
</main>
