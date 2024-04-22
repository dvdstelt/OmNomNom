import { useNavigate, useParams } from "react-router-dom";
import Items from "./Cart/Items";
import CartAccept from "./Cart/CartAccept";
import { useEffect, useState } from "react";
import { useLoadData } from "./misc";
import { getCartItems } from "./orderService";
import axios from "axios";

import styles from "./Cart.module.css";

export default function Cart() {
  const { orderId } = useParams();
  const [refreshTrigger, setRefreshTrigger] = useState(crypto.randomUUID());
  const { data: items } = useLoadData(getCartItems, orderId, {
    refreshTrigger,
  });
  const navigate = useNavigate();

  useEffect(() => {
    document.title = `Shopping Cart - omnomnom.com`;
    if (items?.length === 0) navigate("/");
  }, []);

  async function saveAndContinue() {
    await axios.post(`https://localhost:7126/cart/${orderId}`, items);
    navigate(`/buy/address/${orderId}`);
  }

  return (
    <div className={styles.cart}>
      <h1>Shopping Cart</h1>
      <div className={styles.contents}>
        <Items
          items={items}
          className={styles.items}
          refreshCart={() => setRefreshTrigger(crypto.randomUUID())}
        />
        <CartAccept
          id={orderId}
          className={styles.cartAccept}
          proceed={saveAndContinue}
        />
      </div>
    </div>
  );
}
