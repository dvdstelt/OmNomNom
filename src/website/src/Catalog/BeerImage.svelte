<script>
  let { name, imageUrl, category = '' } = $props();
</script>

<div class="beer-image">
  {#if category}
    <span class="beer-type-badge">{category}</span>
  {/if}
  <img class="beer-product-img" src="/products/{imageUrl}" alt={name} />
</div>
