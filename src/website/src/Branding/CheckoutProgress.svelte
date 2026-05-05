<script>
  let { stage = 'cart', orderId = '' } = $props();

  const steps = [
    { id: 'cart', label: 'Cart', href: (id) => `/cart/${id}` },
    { id: 'address', label: 'Address', href: (id) => `/buy/address/${id}` },
    { id: 'shipping', label: 'Shipping', href: (id) => `/buy/shipping/${id}` },
    { id: 'payment', label: 'Payment', href: (id) => `/buy/payment/${id}` },
    { id: 'summary', label: 'Review', href: (id) => `/buy/summary/${id}` }
  ];

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
