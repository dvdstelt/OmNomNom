import axios from "axios";
import memoizeOne from "memoize-one";

async function getProduct(id) {
  const { data } = await axios.get(`https://localhost:7126/product/${id}`);
  return data.product;
}
const memoisedProduct = memoizeOne(getProduct);

export async function getProducts() {
  const { data } = await axios.get("https://localhost:7126/products");
  return data.products;
}

export async function getProductName(id) {
  const model = await memoisedProduct(id);
  return model.name;
}

export async function getProductDescription(id) {
  const model = await memoisedProduct(id);
  return model.description;
}

export async function getProductImage(id) {
  const model = await memoisedProduct(id);
  return model.imageUrl;
}

export async function getProductRating(id) {
  const model = await memoisedProduct(id);
  return { stars: model.stars, reviewCount: model.reviewCount };
}

export async function getProductStockDetails(id) {
  const model = await memoisedProduct(id);
  return { stockCount: model.inStock };
}

export async function getProductPriceDetails(id) {
  const model = await memoisedProduct(id);
  return { price: model.price, discount: model.discount };
}

export function getProductShippingDetails(id) {
  return {
    shippingLocation: "Rotterdam, The Netherlands", //model.shippingLocation,
    shipsToLocation: true, //model.shipsToLocation,
  };
}

export async function addProductToCart(orderId, productDetails) {
  const { data } = await axios.post(
    `https://localhost:7126/cart/addproduct/${orderId}`,
    productDetails
  );
  notififyCartSubscribers();
  return data.orderId;
}

const cartSubscriptions = [];
export function subscribeToCart(callback) {
  cartSubscriptions.push(callback);
}

function notififyCartSubscribers() {
  cartSubscriptions.forEach((callback) => callback());
}
