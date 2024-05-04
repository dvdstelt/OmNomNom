import { Link } from "react-router-dom";
import { useLoadData } from "../misc";
import { clearCartCache, getCartItems } from "../orderService";
import { useEffect, useState } from "react";
import { subscribeToCart } from "../productService";

import cartIcon from "@/assets/cart.png";
import styles from "./CartIndicator.module.css";

export default function CartIndicator({ id }) {
  const [refreshTrigger, setRefreshTrigger] = useState(crypto.randomUUID());
  const { data: items } = useLoadData(getCartItems, id, { refreshTrigger });
  const cartSize = items?.length ?? 0;

  useEffect(
    () =>
      subscribeToCart(() => {
        clearCartCache();
        setRefreshTrigger(crypto.randomUUID());
      }),
    []
  );

  return cartSize > 0 ? (
    <Link className={styles.cartIndicator} to={`/cart/${id}`}>
      <img src={cartIcon} className={styles.icon} />
      <span className={styles.itemCount}>{cartSize}</span>
    </Link>
  ) : null;
}
