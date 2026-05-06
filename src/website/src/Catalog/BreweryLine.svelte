<script>
  import { countryIso } from '$lib/countryFlag.js';

  let { brewery, country } = $props();
  let iso = $derived(countryIso(country));
</script>

{#if brewery}
  <div class="beer-brewery">
    {#if iso}
      <span class="fi fi-{iso} country-flag" title={country} aria-label={country}></span>
    {/if}
    {brewery}
  </div>
{/if}
