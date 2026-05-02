<script>
  import Price from '../Finance/Price.svelte';

  let { item, editable = true, onQuantityChange = () => {}, onRemove = () => {} } = $props();
  let format = (value) => '$' + Number(value ?? 0).toFixed(2);
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
          >
            +
          </button>
        </div>
        <button type="button" class="cart-remove-btn" onclick={() => onRemove(item.productId)}>
          Remove
        </button>
      </div>
    {:else}
      <div class="cart-item-qty">Qty: {item.quantity}</div>
    {/if}
  </div>
  <div class="cart-item-subtotal">
    {format((item.discount > 0 ? item.discount : item.price) * item.quantity)}
  </div>
</div>
