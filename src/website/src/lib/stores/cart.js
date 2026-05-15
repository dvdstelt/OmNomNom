import { writable } from 'svelte/store';
import { gateway } from '$lib/api/gateway.js';
import { orderId as orderIdStore } from '$lib/stores/orderId.js';

export const cartItemCount = writable(0);

export async function refreshCartCount(orderId) {
  if (!orderId) {
    cartItemCount.set(0);
    return;
  }
  try {
    const data = await gateway.getCart(orderId);
    const count = (data?.cartItems ?? []).reduce(
      (sum, item) => sum + (item.quantity ?? 0),
      0
    );
    cartItemCount.set(count);
  } catch (e) {
    cartItemCount.set(0);
    // 410 from the cart composer means the workflow store reaped the
    // slice this orderId points at - it's a dead pointer, so clear it
    // and let the next add-to-cart create a fresh one.
    if (e?.status === 410) orderIdStore.clear();
  }
}
