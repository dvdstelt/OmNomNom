<script>
  let { categories = [], selected = $bindable('All'), onChange = () => {} } = $props();

  const allLabel = 'All';
  let buttons = $derived([allLabel, ...categories]);

  function pick(value) {
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
        onclick={() => pick(value)}
      >
        {value === allLabel ? 'All Beers' : value}
      </button>
    {/each}
  </div>
</div>
