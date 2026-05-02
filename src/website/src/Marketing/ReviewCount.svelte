<script>
  let { count = 0, label = 'reviews' } = $props();
</script>

<span class="review-count">({count.toLocaleString()} {label})</span>

<style>
  .review-count {
    color: var(--color-text-secondary);
    font-size: 0.85rem;
    margin-left: 4px;
  }
</style>
