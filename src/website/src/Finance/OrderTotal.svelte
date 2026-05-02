<script>
  let { label = 'Order Total', amount = 0, emphasized = false } = $props();
  let format = (value) => '$' + Number(value ?? 0).toFixed(2);
</script>

<div class="sidebar-row" class:sidebar-total={emphasized}>
  <span>{label}</span>
  <span>{format(amount)}</span>
</div>
