import axios from "axios";
import memoizeOne from "memoize-one";

async function getCart(id) {
  const { data } = await axios.get(`https://localhost:7126/cart/${id}`);
  return data;
}
const memoisedCart = memoizeOne(getCart);

export async function getCartItems(id) {
  const data = await memoisedCart(id);
  return data.cartItems;
}

export async function getCartTotal(id) {
  const data = await memoisedCart(id);
  return data.totalPrice;
}
