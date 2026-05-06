<script>
  import { countryFlagSrc } from '$lib/countryFlag.js';

  let { brewery, country } = $props();
  let flag = $derived(countryFlagSrc(country));
</script>

{#if brewery}
  <div class="beer-brewery">
    {#if flag}
      <img
        class="country-flag"
        src={flag.src}
        srcset={flag.srcset}
        alt={flag.alt}
        width="20"
        height="15"
      />
    {/if}
    {brewery}
  </div>
{/if}
