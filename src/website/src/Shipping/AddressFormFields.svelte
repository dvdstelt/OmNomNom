<script>
  let { address = $bindable({}), idPrefix = 'addr' } = $props();
</script>

<div class="form-grid">
  <div class="form-group full-width">
    <label for="{idPrefix}-name">Full Name</label>
    <input
      type="text"
      id="{idPrefix}-name"
      placeholder="John Doe"
      bind:value={address.fullName}
    />
  </div>
  <div class="form-group full-width">
    <label for="{idPrefix}-street">Street Address</label>
    <input
      type="text"
      id="{idPrefix}-street"
      placeholder="123 Main St, Apt 4"
      bind:value={address.street}
    />
  </div>
  <div class="form-group">
    <label for="{idPrefix}-zip">Zip Code</label>
    <input type="text" id="{idPrefix}-zip" bind:value={address.zipCode} />
  </div>
  <div class="form-group">
    <label for="{idPrefix}-town">City</label>
    <input type="text" id="{idPrefix}-town" bind:value={address.town} />
  </div>
  <div class="form-group full-width">
    <label for="{idPrefix}-country">Country</label>
    <input type="text" id="{idPrefix}-country" bind:value={address.country} />
  </div>
</div>
