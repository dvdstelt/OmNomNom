import { writable } from 'svelte/store';
import { gateway } from '$lib/api/gateway.js';

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
  } catch {
    cartItemCount.set(0);
  }
}
