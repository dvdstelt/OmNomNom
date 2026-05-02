<script>
  let { stage = 'cart' } = $props();

  const steps = [
    { id: 'cart', label: 'Cart', href: (orderId) => `/cart/${orderId}` },
    { id: 'address', label: 'Address', href: (orderId) => `/buy/address/${orderId}` },
    { id: 'shipping', label: 'Shipping', href: (orderId) => `/buy/shipping/${orderId}` },
    { id: 'payment', label: 'Payment', href: (orderId) => `/buy/payment/${orderId}` },
    { id: 'summary', label: 'Review', href: (orderId) => `/buy/summary/${orderId}` }
  ];

  let { orderId = '' } = $props();
  let currentIndex = $derived(steps.findIndex((s) => s.id === stage));
</script>

<div class="progress-bar-container">
  <div class="progress-steps">
    {#each steps as step, i}
      {@const state = i < currentIndex ? 'completed' : i === currentIndex ? 'active' : 'upcoming'}
      {@const clickable = i < currentIndex && orderId}
      <div class="progress-step {state}">
        {#if clickable}
          <a href={step.href(orderId)}>
            <div class="step-circle">{'✓'}</div>
            <div class="step-label">{step.label}</div>
          </a>
        {:else}
          <span>
            <div class="step-circle">{i < currentIndex ? '✓' : i + 1}</div>
            <div class="step-label">{step.label}</div>
          </span>
        {/if}
      </div>
      {#if i < steps.length - 1}
        <div class="step-connector {i < currentIndex ? 'completed' : ''}"></div>
      {/if}
    {/each}
  </div>
</div>
