<script>
  import { cardIcon } from './cardIcon.js';

  let { cards = [], selectedId = $bindable(null), onSelect = () => {} } = $props();

  function pick(id) {
    selectedId = id;
    onSelect(id);
  }
</script>

<div class="payment-options">
  {#each cards as card (card.cardId)}
    {@const isSelected = selectedId === card.cardId}
    {@const icon = cardIcon(card.cardType)}
    <label class="payment-option" class:selected={isSelected}>
      <input
        type="radio"
        name="creditCard"
        value={card.cardId}
        checked={isSelected}
        onchange={() => pick(card.cardId)}
      />
      <div class="payment-card-info">
        <div class="payment-card-brand">
          {#if icon}<img src={icon} alt={card.cardType} height="20" />{/if}
          <span>{card.cardType}</span>
        </div>
        <div class="payment-card-number">**** **** **** {card.lastDigits}</div>
        <div class="payment-card-expiry">
          Expires {card.expiryDate?.padStart?.(7, '0') ?? card.expiryDate ?? ''}
          {#if card.cardHolder}&middot; {card.cardHolder}{/if}
        </div>
      </div>
    </label>
  {/each}
</div>
