<script>
  import Stars from './Stars.svelte';
  import ReviewCount from './ReviewCount.svelte';

  let { stars = 0, reviewCount = 0, untappdUrl = null } = $props();
</script>

<div class="star-rating">
  <Stars rating={stars} />
  <span class="rating-text">{stars}</span>
  <ReviewCount count={reviewCount} />
  {#if untappdUrl}
    <a href={untappdUrl} target="_blank" rel="noopener" class="untappd-link" title="View on Untappd">
      <img class="untappd-icon" src="/img/untappd.svg" alt="Untappd" />
    </a>
  {/if}
</div>
