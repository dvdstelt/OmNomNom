import Price from "../misc/Price";
import { useLoadData } from "../misc";
import { getCartTotal } from "../orderService";
import { subscribeToCart } from "../productService";
import { useState } from "react";

import styles from "./CartAccept.module.css";

export default function CartAccept({ id, className, proceed }) {
  const [refreshTrigger, setRefreshTrigger] = useState(crypto.randomUUID());
  const { data: total } = useLoadData(getCartTotal, id, { refreshTrigger });

  subscribeToCart(() => setRefreshTrigger(crypto.randomUUID()));

  return (
    <div className={`${styles.cartAccept} ${className ?? ""}`}>
      <div className={styles.subtotal}>
        <span>Subtotal:</span>
        <span className={styles.price}>
          <Price price={total} />
        </span>
      </div>
      <button onClick={proceed}>Proceed to Checkout</button>
    </div>
  );
}
