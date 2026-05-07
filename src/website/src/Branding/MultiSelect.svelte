<script>
  // Trigger button + checkbox popup. Trigger reuses .filter-select so
  // it sits in the filter row visually consistent with the other
  // selects; popup is absolutely positioned below.
  //
  // Props:
  //   label     button label when nothing is selected
  //   options   array of strings to choose from
  //   selected  bindable array of currently-selected strings
  //   onChange  optional callback fired after a checkbox toggle

  let {
    label,
    options = [],
    selected = $bindable([]),
    onChange = () => {}
  } = $props();

  let open = $state(false);
  let triggerEl;
  let popupEl;

  let count = $derived(selected.length);
  let displayLabel = $derived(count > 0 ? `${label} · ${count}` : label);

  function toggleOption(option, checked) {
    selected = checked
      ? [...selected, option]
      : selected.filter((v) => v !== option);
    onChange(selected);
  }

  function isChecked(option) {
    return selected.includes(option);
  }

  function clear(event) {
    event.stopPropagation();
    selected = [];
    onChange(selected);
  }

  function onWindowMouseDown(event) {
    if (!open) return;
    if (triggerEl?.contains(event.target)) return;
    if (popupEl?.contains(event.target)) return;
    open = false;
  }

  function onKeyDown(event) {
    if (event.key === 'Escape' && open) {
      open = false;
      triggerEl?.focus();
    }
  }
</script>

<svelte:window onmousedown={onWindowMouseDown} onkeydown={onKeyDown} />

<div class="multiselect">
  <button
    type="button"
    class="filter-select multiselect-trigger"
    class:has-selection={count > 0}
    bind:this={triggerEl}
    onclick={() => (open = !open)}
    aria-haspopup="listbox"
    aria-expanded={open}
  >
    {displayLabel}
    {#if count > 0}
      <span
        class="multiselect-clear"
        role="button"
        tabindex="0"
        aria-label="Clear {label} filter"
        onclick={clear}
        onkeydown={(e) => (e.key === 'Enter' || e.key === ' ') && clear(e)}
      >
        ×
      </span>
    {/if}
  </button>

  {#if open}
    <div class="multiselect-popup" bind:this={popupEl} role="listbox">
      {#if options.length === 0}
        <div class="multiselect-empty">No options</div>
      {:else}
        {#each options as option}
          <label class="multiselect-option">
            <input
              type="checkbox"
              checked={isChecked(option)}
              onchange={(e) => toggleOption(option, e.currentTarget.checked)}
            />
            <span>{option}</span>
          </label>
        {/each}
      {/if}
    </div>
  {/if}
</div>
