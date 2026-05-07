<script>
  // Top-of-page filter strip. Three multi-select filters
  // (categories, breweries, countries) and a single-select sort
  // dropdown that's visually distinguished from the filters with
  // its own .sort-group separator + label.
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
    selectedBreweries = $bindable([]),
    selectedCountries = $bindable([]),
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
        <MultiSelect
          label="Brewery"
          options={breweries}
          bind:selected={selectedBreweries}
          onChange={emit}
        />
      {/if}
      {#if countries.length > 0}
        <MultiSelect
          label="Country"
          options={countries}
          bind:selected={selectedCountries}
          onChange={emit}
        />
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
