<script>
  import { countryIso } from '$lib/countryFlag.js';

  // `variant` chooses the wrapper class so this microview can sit
  // both on the product card (`.beer-brewery`, no country name) and
  // on the detail page (`.product-brewery`, brewery · country). Both
  // styles already exist in `global.css`, copied verbatim from the
  // HTML reference.
  let { brewery, country, variant = 'card' } = $props();

  let iso = $derived(countryIso(country));
  let wrapperClass = $derived(variant === 'detail' ? 'product-brewery' : 'beer-brewery');
  let showCountry = $derived(variant === 'detail');
</script>

{#if brewery}
  <div class={wrapperClass}>
    {#if iso}
      <span class="fi fi-{iso} country-flag" title={country} aria-label={country}></span>
    {/if}
    {brewery}
    {#if showCountry && country}
      &middot; {country}
    {/if}
  </div>
{/if}
