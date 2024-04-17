import { useLoadData } from "../misc";
import Price from "../misc/Price";
import { getCartItems } from "../orderService";

import styles from "./Items.module.css";

export default function Items({ orderId }) {
  const { data: items } = useLoadData(getCartItems, orderId);

  return (
    <div className={styles.items}>
      {(items ?? []).map((item) => (
        <div key={item.productId} className={styles.item}>
          <label>{item.name}</label>
          <div className={styles.itemDetails}>
            <span className={styles.price}>
              <Price price={item.price} />
            </span>
            <span>-</span>
            <span>{`Quantity: ${item.quantity}`}</span>
          </div>
        </div>
      ))}
    </div>
  );
}
