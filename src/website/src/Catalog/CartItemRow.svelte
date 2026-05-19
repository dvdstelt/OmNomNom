<script>
  import Price from '../Finance/Price.svelte';
  import LineSubtotal from '../Finance/LineSubtotal.svelte';

  let { item, editable = true, onQuantityChange = () => {}, onRemove = () => {} } = $props();

  // The cart endpoint surfaces the live in-stock count per line. Cap
  // the + button at that figure so the customer can't queue more units
  // than exist; if the API ever omits inStock fall back to a generous
  // ceiling rather than blocking all increments.
  let stockLimit = $derived(item?.inStock ?? Number.POSITIVE_INFINITY);
  let canIncrease = $derived(item.quantity < stockLimit);
</script>

<div class="cart-item" data-product-id={item.productId}>
  <div class="cart-item-image">
    {#if item.imageUrl}
      <img class="beer-product-img" src="/products/{item.imageUrl}" alt={item.name} />
    {/if}
  </div>
  <div class="cart-item-details">
    <a href="/product/{item.productId}" class="cart-item-name">{item.name}</a>
    <div class="cart-item-price">
      <Price price={item.price} discount={item.discount} />
    </div>
    {#if editable}
      <div class="cart-item-controls">
        <div class="qty-control">
          <button
            type="button"
            class="qty-btn"
            onclick={() => onQuantityChange(item.productId, item.quantity - 1)}
            aria-label="Decrease quantity"
          >
            -
          </button>
          <span class="qty-value">{item.quantity}</span>
          <button
            type="button"
            class="qty-btn"
            onclick={() => onQuantityChange(item.productId, item.quantity + 1)}
            aria-label="Increase quantity"
            disabled={!canIncrease}
          >
            +
          </button>
        </div>
        <button
          type="button"
          class="cart-remove-btn"
          onclick={() => onRemove(item.productId)}
          aria-label="Remove {item.name} from cart"
        >
          <!-- Text label is hidden on mobile via CSS; the trash glyph
               takes its place so the action fits next to the qty stepper. -->
          <span class="cart-remove-text">Remove</span>
          <svg
            class="cart-remove-icon"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            width="20"
            height="20"
            aria-hidden="true"
          >
            <path d="M3 6h18" />
            <path d="M8 6V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
            <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" />
            <path d="M10 11v6" />
            <path d="M14 11v6" />
          </svg>
        </button>
      </div>
    {:else}
      <div class="cart-item-qty">Qty: {item.quantity}</div>
    {/if}
  </div>
  <div class="cart-item-subtotal">
    <LineSubtotal {item} />
  </div>
</div>
