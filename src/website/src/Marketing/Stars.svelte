<script>
  let { rating = 0 } = $props();
  let stars = $derived(
    Array.from({ length: 5 }, (_, i) => {
      const position = i + 1;
      if (position <= Math.floor(rating)) return 'filled';
      if (position - rating < 1 && position - rating > 0) return 'half';
      return 'empty';
    })
  );
</script>

{#each stars as state}
  <span class="star {state}">★</span>
{/each}
