<script>
  import { effectivePrice } from './effectivePrice.js';

  let { item } = $props();
  let format = (value) => '$' + Number(value ?? 0).toFixed(2);
</script>{format(effectivePrice(item) * Number(item?.quantity ?? 0))}
