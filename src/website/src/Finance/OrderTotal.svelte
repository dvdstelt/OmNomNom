<script>
  let {
    label = 'Order Total',
    amount = 0,
    emphasized = false,
    discount = false
  } = $props();
  let format = (value) => {
    const n = Number(value ?? 0);
    return (n < 0 ? '-$' : '$') + Math.abs(n).toFixed(2);
  };
</script>

<div class="sidebar-row" class:sidebar-total={emphasized} class:sidebar-discount={discount}>
  <span>{label}</span>
  <span>{format(amount)}</span>
</div>
