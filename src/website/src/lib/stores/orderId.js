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
    clear() {
      if (browser) localStorage.removeItem(STORAGE_KEY);
      set(null);
    }
  };
}

export const orderId = createOrderIdStore();
