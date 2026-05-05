<script>
  // Red "-N%" bubble used on product grid cards. Lives in the Finance
  // boundary because the percentage comes straight from the Finance
  // pricing rule; Catalog only provides the slot inside .beer-image
  // where this gets rendered.
  import { discountPercent } from './effectivePrice.js';

  let { price = 0, discount = 0 } = $props();
  let percent = $derived(discountPercent({ price, discount }));
</script>

{#if percent > 0}
  <span class="beer-save-badge">-{percent}%</span>
{/if}
