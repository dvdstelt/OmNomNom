<script>
  let { options = [], selectedId = $bindable(null), onSelect = () => {} } = $props();
  let format = (value) => '$' + Number(value ?? 0).toFixed(2);

  function pick(id) {
    selectedId = id;
    onSelect(id);
  }
</script>

<div class="shipping-options">
  {#each options as option (option.deliveryOptionId)}
    {@const isSelected = selectedId === option.deliveryOptionId}
    <label class="shipping-option" class:selected={isSelected}>
      <input
        type="radio"
        name="deliveryOption"
        value={option.deliveryOptionId}
        checked={isSelected}
        onchange={() => pick(option.deliveryOptionId)}
      />
      <div class="shipping-option-info">
        <div class="shipping-option-name">{option.name}</div>
        <div class="shipping-option-delivery">{option.description}</div>
      </div>
      <div class="shipping-option-price">{format(option.price)}</div>
    </label>
  {/each}
</div>
