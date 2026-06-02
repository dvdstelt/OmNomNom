import { writable } from 'svelte/store';
import { browser } from '$app/environment';

const STORAGE_KEY = 'orderId';

function readInitial() {
  if (!browser) return null;
  const value = localStorage.getItem(STORAGE_KEY);
  return value && value !== 'null' ? value : null;
}

function createOrderIdStore() {
  const { subscribe, set } = writable(readInitial());

  return {
    subscribe,
    set(value) {
      if (browser) {
        if (value) localStorage.setItem(STORAGE_KEY, value);
        else localStorage.removeItem(STORAGE_KEY);
      }
      set(value || null);
    },
    // Mint the cart's id client-side on first use so every back-end
    // composer on the add-to-cart request sees the same id, rather than
    // letting one service mint it server-side and hand it back.
    ensure() {
      const existing = readInitial();
      if (existing) return existing;
      const minted = crypto.randomUUID();
      if (browser) localStorage.setItem(STORAGE_KEY, minted);
      set(minted);
      return minted;
    },
    clear() {
      if (browser) localStorage.removeItem(STORAGE_KEY);
      set(null);
    }
  };
}

export const orderId = createOrderIdStore();
