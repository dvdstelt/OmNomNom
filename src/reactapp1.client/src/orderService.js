import axios from "axios";
import memoizeOne from "memoize-one";

async function getCart(orderId) {
  const { data } = await axios.get(`https://localhost:7126/cart/${orderId}`);
  return data;
}
const memoisedCart = memoizeOne(getCart);

export function clearCartCache() {
  memoisedCart.clear();
}

export async function getCartItems(orderId) {
  if (!orderId) return [];
  const data = await memoisedCart(orderId);
  return data.cartItems;
}

export async function getCartTotal(orderId) {
  const data = await memoisedCart(orderId);
  return data.totalCartPrice;
}

export async function getAddress(orderId) {
  const { data } = await axios.get(
    `https://localhost:7126/buy/address/${orderId}`
  );
  return data;
}

async function getShipping(orderId) {
  const { data } = await axios.get(
    `https://localhost:7126/buy/shipping/${orderId}`
  );
  return data;
}
const memoisedShipping = memoizeOne(getShipping);

export async function getDeliveryOptions(orderId) {
  const { data } = await axios.get(`https://localhost:7126/buy/shipping/${orderId}`);
  return data;
}
