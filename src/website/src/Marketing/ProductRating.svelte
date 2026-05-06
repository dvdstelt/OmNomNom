<script>
  import Stars from './Stars.svelte';
  import ReviewCount from './ReviewCount.svelte';

  let { rating = 0, ratingCount = 0, untappdUrl = null } = $props();
</script>

<div class="star-rating">
  <Stars rating={rating} />
  <span class="rating-text">{rating}</span>
  <ReviewCount count={ratingCount} />
  {#if untappdUrl}
    <a href={untappdUrl} target="_blank" rel="noopener" class="untappd-link" title="View on Untappd">
      <img class="untappd-icon" src="/img/untappd.svg" alt="Untappd" />
    </a>
  {/if}
</div>
