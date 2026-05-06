<script>
  // Top-of-page filter strip. Type buttons are always rendered; the
  // brewery and country dropdowns appear only when the consumer
  // passes non-empty arrays for them, so existing call sites that
  // only filter by type stay unchanged.

  let {
    categories = [],
    breweries = [],
    countries = [],
    selected = $bindable('All'),
    selectedBrewery = $bindable('All'),
    selectedCountry = $bindable('All'),
    onChange = () => {}
  } = $props();

  const allLabel = 'All';
  let buttons = $derived([allLabel, ...categories]);

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

  {#if breweries.length > 0 || countries.length > 0}
    <div class="filter-dropdowns">
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
    </div>
  {/if}
</div>
