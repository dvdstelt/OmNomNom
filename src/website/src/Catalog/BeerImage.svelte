<script>
  // `children` is a slot for other boundaries (e.g. Finance) to drop
  // overlays inside the absolutely-positioned .beer-image container
  // without Catalog needing to know about their data.
  let { name, imageUrl, category = '', children } = $props();
</script>

<div class="beer-image">
  {#if category}
    <span class="beer-type-badge">{category}</span>
  {/if}
  {@render children?.()}
  <img class="beer-product-img" src="/products/{imageUrl}" alt={name} />
</div>
