<script>
  // Top-of-page filter strip. Type is multi-select via the popup
  // checkbox component; Brewery and Country are single-select via
  // the existing branded `<select>` chip. Sort sits in its own
  // .sort-group with the separator + label.
  //
  // All four controls are optional - pass an empty array (or no
  // sortOptions) to hide a dropdown.

  import MultiSelect from './MultiSelect.svelte';

  let {
    categories = [],
    breweries = [],
    countries = [],
    sortOptions = [],
    selectedCategories = $bindable([]),
    selectedBrewery = $bindable('All'),
    selectedCountry = $bindable('All'),
    selectedSort = $bindable('default'),
    onChange = () => {}
  } = $props();

  let hasFilterDropdowns = $derived(
    categories.length > 0 || breweries.length > 0 || countries.length > 0
  );
  let hasSortDropdown = $derived(sortOptions.length > 0);

  function emit() {
    onChange();
  }
</script>

<div class="filters-row">
  {#if hasFilterDropdowns || hasSortDropdown}
    <div class="filter-dropdowns">
      {#if categories.length > 0}
        <MultiSelect
          label="Type"
          options={categories}
          bind:selected={selectedCategories}
          onChange={emit}
        />
      {/if}
      {#if breweries.length > 0}
        <select
          class="filter-select"
          bind:value={selectedBrewery}
          onchange={emit}
        >
          <option value="All">All Breweries</option>
          {#each breweries as brewery}
            <option value={brewery}>{brewery}</option>
          {/each}
        </select>
      {/if}
      {#if countries.length > 0}
        <select
          class="filter-select"
          bind:value={selectedCountry}
          onchange={emit}
        >
          <option value="All">All Countries</option>
          {#each countries as country}
            <option value={country}>{country}</option>
          {/each}
        </select>
      {/if}
      {#if hasSortDropdown}
        <div class="sort-group" class:has-filters={hasFilterDropdowns}>
          <span class="sort-label">Sort by</span>
          <select
            class="sort-select"
            bind:value={selectedSort}
            onchange={emit}
          >
            {#each sortOptions as option}
              <option value={option.value}>{option.label}</option>
            {/each}
          </select>
        </div>
      {/if}
    </div>
  {/if}
</div>
