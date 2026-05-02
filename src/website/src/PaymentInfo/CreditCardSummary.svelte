<script>
  import { cardIcon } from './cardIcon.js';

  let { cardType = '', lastDigits = '' } = $props();
  let icon = $derived(cardIcon(cardType));
</script>

<span class="payment-card-summary">
  {#if icon}<img src={icon} alt={cardType} height="18" />{/if}
  <span>{cardType} ending in {lastDigits}</span>
</span>

<style>
  .payment-card-summary {
    display: inline-flex;
    align-items: center;
    gap: 8px;
  }
</style>
