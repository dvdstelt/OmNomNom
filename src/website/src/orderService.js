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
  if (!orderId || orderId === "null") return [];
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

export async function saveAddress(orderId, addressData) {
  await axios.post(
    `https://localhost:7126/buy/address/${orderId}`,
    addressData
  );
}

async function getShipping(orderId) {
  const { data } = await axios.get(
    `https://localhost:7126/buy/shipping/${orderId}`
  );
  return data;
}
const memoisedShipping = memoizeOne(getShipping);

export async function getSelectedDeliveryOption(orderId) {
  const data = await memoisedShipping(orderId);
  return data.selectedDeliveryOption;
}

export async function getDeliveryOptions(orderId) {
  const data = await memoisedShipping(orderId);
  return data.deliveryOptions;
}

export async function getShippingProducts(orderId) {
  const data = await memoisedShipping(orderId);
  return data.cartItems;
}

export async function saveShipping(orderId, deliveryOptionId) {
  await axios.post(`https://localhost:7126/buy/shipping/${orderId}`, {
    deliveryOptionId,
  });
  memoisedShipping.clear();
}

async function getPayment(orderId) {
  const { data } = await axios.get(
    `https://localhost:7126/buy/payment/${orderId}`
  );
  return data;
}
const memoisedPayment = memoizeOne(getPayment);

export async function getCreditCards(customerId) {
  const { data } = await axios.get(
    `https://localhost:7126/buy/creditcard/${customerId}`
  );
  return data.creditCards;
}

export async function getPaymentInfo(orderId) {
  const data = await memoisedPayment(orderId);
  return data.creditCardId;
}

export async function savePayment(orderId, paymentData) {
  await axios.post(
    `https://localhost:7126/buy/payment/${orderId}`,
    paymentData
  );
  memoisedPayment.clear();
}

async function getSummary(orderId) {
  const { data } = await axios.get(
    `https://localhost:7126/buy/summary/${orderId}`
  );
  return data;
}
const memoisedSummary = memoizeOne(getSummary);

export async function getOrderSummary(orderId) {
  const data = await memoisedSummary(orderId);
  return data;
}
