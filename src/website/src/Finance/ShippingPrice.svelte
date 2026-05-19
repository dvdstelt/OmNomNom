<script>
  let { amount = 0 } = $props();
  let format = (value) => {
    const n = Number(value ?? 0);
    return n <= 0 ? 'FREE' : '$' + n.toFixed(2);
  };
</script>{format(amount)}
