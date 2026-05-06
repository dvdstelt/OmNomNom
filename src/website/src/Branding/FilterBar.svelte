<script>
  // Top-of-page filter strip. Type buttons are always rendered; the
  // brewery, country, and sort dropdowns appear only when the
  // consumer opts in, so existing call sites that only filter by
  // type stay unchanged. The sort control sits in its own group
  // with a separator + label so it visually reads as a sort rather
  // than another filter.

  let {
    categories = [],
    breweries = [],
    countries = [],
    sortOptions = [],            // [{ value, label }, ...]
    selected = $bindable('All'),
    selectedBrewery = $bindable('All'),
    selectedCountry = $bindable('All'),
    selectedSort = $bindable('default'),
    onChange = () => {}
  } = $props();

  const allLabel = 'All';
  let buttons = $derived([allLabel, ...categories]);
  let hasFilterDropdowns = $derived(breweries.length > 0 || countries.length > 0);
  let hasSortDropdown = $derived(sortOptions.length > 0);

  function pickType(value) {
    selected = value;
    onChange(value);
  }
</script>

<div class="filters-row">
  <div class="filter-bar">
    {#each buttons as value}
      <button
        type="button"
        class="filter-btn"
        class:active={selected === value}
        onclick={() => pickType(value)}
      >
        {value === allLabel ? 'All Beers' : value}
      </button>
    {/each}
  </div>

  {#if hasFilterDropdowns || hasSortDropdown}
    <div class="filter-dropdowns">
      {#if hasFilterDropdowns}
        {#if breweries.length > 0}
          <select class="filter-select" bind:value={selectedBrewery}>
            <option value="All">All Breweries</option>
            {#each breweries as brewery}
              <option value={brewery}>{brewery}</option>
            {/each}
          </select>
        {/if}
        {#if countries.length > 0}
          <select class="filter-select" bind:value={selectedCountry}>
            <option value="All">All Countries</option>
            {#each countries as country}
              <option value={country}>{country}</option>
            {/each}
          </select>
        {/if}
      {/if}
      {#if hasSortDropdown}
        <div class="sort-group" class:has-filters={hasFilterDropdowns}>
          <span class="sort-label">Sort by</span>
          <select class="sort-select" bind:value={selectedSort}>
            {#each sortOptions as option}
              <option value={option.value}>{option.label}</option>
            {/each}
          </select>
        </div>
      {/if}
    </div>
  {/if}
</div>
