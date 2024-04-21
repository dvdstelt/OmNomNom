import Price from "../misc/Price";
import { useLoadData } from "../misc";
import { getCartTotal } from "../orderService";
import { useNavigate } from "react-router-dom";
import { subscribeToCart } from "../productService";
import { useState } from "react";

import styles from "./CartAccept.module.css";

export default function CartAccept({ id, className }) {
  const [refreshTrigger, setRefreshTrigger] = useState(crypto.randomUUID());
  const { data: total } = useLoadData(getCartTotal, id, { refreshTrigger });
  const navigate = useNavigate();

  subscribeToCart(() => setRefreshTrigger(crypto.randomUUID()));

  function proceed() {
    navigate(`/buy/address/${id}`);
  }

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
