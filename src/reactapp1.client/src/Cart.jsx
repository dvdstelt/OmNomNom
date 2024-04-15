import { useParams } from "react-router-dom";
import Items from "./Cart/Items";
import CartAccept from "./Cart/CartAccept";

import styles from "./Cart.module.css";

export default function Cart() {
  const { orderId } = useParams();

  document.title = `Shopping Cart - omnomnom.com`;

  return (
    <div className={styles.cart}>
      <h1>Shopping Cart</h1>
      <div className={styles.contents}>
        <Items id={orderId} className={styles.items} />
        <CartAccept id={orderId} className={styles.cartAccept} />
      </div>
    </div>
  );
}
