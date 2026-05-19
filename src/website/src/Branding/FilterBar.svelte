<script>
  // Top-of-page filter strip. Type is multi-select via the popup
  // checkbox component; Brewery and Country are single-select via
  // the existing branded `<select>` chip. Sort sits in its own
  // .sort-group with the separator + label.
  //
  // All four controls are optional - pass an empty array (or no
  // sortOptions) to hide a dropdown.
  //
  // At mobile widths the three filter dropdowns collapse behind a
  // single `Filters` button that opens a bottom sheet containing the
  // same three controls. Sort stays visible in the row. CSS does all
  // the layout work; this component only owns the open/closed state
  // and an active-count badge.

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

  let isSheetOpen = $state(false);

  // One badge across all three dropdowns; "All" is the unselected
  // sentinel for the single-selects.
  let activeFilterCount = $derived(
    selectedCategories.length +
      (selectedBrewery !== 'All' ? 1 : 0) +
      (selectedCountry !== 'All' ? 1 : 0)
  );

  function emit() {
    onChange();
  }

  function openSheet() {
    isSheetOpen = true;
  }

  function closeSheet() {
    isSheetOpen = false;
  }

  function onKeyDown(event) {
    if (event.key === 'Escape' && isSheetOpen) {
      closeSheet();
    }
  }
</script>

<svelte:window onkeydown={onKeyDown} />

<div class="filters-row">
  {#if hasFilterDropdowns}
    <!-- Mobile-only trigger; hidden on desktop via CSS. -->
    <button
      type="button"
      class="filter-toggle"
      onclick={openSheet}
      aria-haspopup="dialog"
      aria-expanded={isSheetOpen}
    >
      Filters{activeFilterCount > 0 ? ` · ${activeFilterCount}` : ''}
    </button>

    <!-- Inline on desktop, bottom-sheet on mobile (toggled via .is-open). -->
    <div
      class="filter-dropdowns"
      class:is-open={isSheetOpen}
      role={isSheetOpen ? 'dialog' : undefined}
      aria-label={isSheetOpen ? 'Filters' : undefined}
    >
      <!-- Sheet header is rendered into the DOM unconditionally but
           only displayed at mobile widths via CSS. -->
      <div class="filter-sheet-head">
        <span class="filter-sheet-title">Filters</span>
        <button
          type="button"
          class="filter-sheet-close"
          onclick={closeSheet}
          aria-label="Close filters"
        >×</button>
      </div>

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
    </div>

    <!-- Backdrop closes the sheet on tap. Only rendered when open so
         it doesn't intercept clicks elsewhere. -->
    {#if isSheetOpen}
      <button
        type="button"
        class="filter-sheet-backdrop"
        onclick={closeSheet}
        aria-label="Close filters"
      ></button>
    {/if}
  {/if}

  {#if hasSortDropdown}
    <!-- Sibling of .filter-dropdowns (not nested) so .filters-row's
         `justify-content: space-between` + `flex-wrap: wrap` can keep
         Sort pinned to the right and let it wrap onto its own row
         independently of how wide the filter chips become. -->
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
