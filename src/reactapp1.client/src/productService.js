import axios from "axios";
import memoizeOne from "memoize-one";

// const model = {
//   id: 42,
//   title: "Corpulet Cabbage of Confusion",
//   stars: 4,
//   reviewCount: 42,
//   productImage:
//     "https://www.freshpoint.com/wp-content/uploads/2020/02/Freshpoint-green-cabbage.jpg",
//   stockCount: 7,
//   price: 42,
//   savings: 13.37,
//   shippingLocation: "Haifa, Israel",
//   shipsToLocation: true,
// };

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
  return { price: model.price, savings: model.savings };
}

export function getProductShippingDetails(id) {
  return {
    shippingLocation: "Haifa, Israel", //model.shippingLocation,
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
