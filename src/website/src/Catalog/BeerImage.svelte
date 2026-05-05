<script>
  // `children` is a slot for other boundaries (e.g. Finance) to drop
  // overlays inside the absolutely-positioned image container without
  // Catalog needing to know about their data.
  //
  // `hero=true` swaps the wrapper to .product-image-main, which is the
  // 440px hero frame on the product detail page. The default .beer-image
  // wrapper is the 220px card frame on the grid. The structure inside
  // is identical; only the outer class changes.
  let { name, imageUrl, category = '', children, hero = false } = $props();
</script>

<div class={hero ? 'product-image-main' : 'beer-image'}>
  {#if category}
    <span class="beer-type-badge">{category}</span>
  {/if}
  {@render children?.()}
  <img class="beer-product-img" src="/products/{imageUrl}" alt={name} />
</div>
