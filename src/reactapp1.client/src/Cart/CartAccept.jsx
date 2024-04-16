import Price from "../misc/Price";
import { useLoadData } from "../misc";
import { getCartTotal } from "../orderService";
import { useNavigate } from "react-router-dom";

import styles from "./CartAccept.module.css";

export default function CartAccept({ id, className }) {
  const { data: total } = useLoadData(getCartTotal, id);
  const navigate = useNavigate();

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
      <div>
        <input id="gift" type="checkbox" />
        <label htmlFor="gift">This list contains a gift</label>
      </div>
      <button onClick={proceed}>Proceed to Checkout</button>
    </div>
  );
}
