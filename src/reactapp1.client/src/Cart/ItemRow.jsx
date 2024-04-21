import { useContext } from "react";
import Price from "../misc/Price";
import ProductImage from "../Product/ProductImage";
import { useLoadData } from "../misc";
import {
  addProductToCart,
  getProductName,
  getProductPriceDetails,
} from "../productService";
import { OrderIdContext } from "../App";

import styles from "./Items.module.css";

export default function ItemRow({ item, onChange }) {
  //If we want product API endpoint data for consistency, rather than what is returned from the cart API endpoint
  //const { data: productName } = useLoadData(getProductName, item.id);
  //const { data: priceModel } = useLoadData(getProductPriceDetails, item.id);
  const { currentOrderId } = useContext(OrderIdContext);

  async function reduceQuantity() {
    await addProductToCart(currentOrderId, {
      id: item.productId,
      quantity: -1,
    });
    onChange();
  }

  async function increaseQuantity() {
    await addProductToCart(currentOrderId, { id: item.productId, quantity: 1 });
    onChange && onChange();
  }

  return (
    <tr>
      <td className={styles.item}>
        <div className={styles.imageContainer}>
          <ProductImage id={item.productId} className={styles.image} />
        </div>
        <span>{item.name}</span>
      </td>
      <td className={styles.price}>
        <Price price={item.price} />
      </td>
      <td>
        <div className={styles.quantity}>
          <button onClick={reduceQuantity}>-</button>
          <span>{item.quantity}</span>
          <button onClick={increaseQuantity}>+</button>
        </div>
      </td>
    </tr>
  );
}
