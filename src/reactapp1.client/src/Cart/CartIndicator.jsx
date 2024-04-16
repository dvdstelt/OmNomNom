import { Link } from "react-router-dom";
import { useLoadData } from "../misc";
import { getCartItems } from "../orderService";

import cartIcon from "@/assets/cart.png";
import styles from "./CartIndicator.module.css";

export default function CartIndicator({ id }) {
  const { data: items } = useLoadData(getCartItems, id);

  return (
    <Link className={styles.cartIndicator} to={`/cart/${id}`}>
      <img src={cartIcon} className={styles.icon} />
      <span className={styles.itemCount}>{items?.length ?? 0}</span>
    </Link>
  );
}
