import { useEffect, useState } from "react";
import { useLoadData } from "../misc";
import { getCartItems } from "../orderService";
import ItemRow from "./ItemRow";
import { useNavigate } from "react-router-dom";

import styles from "./Items.module.css";

export default function Items({ id }) {
  const [refreshTrigger, setRefreshTrigger] = useState(crypto.randomUUID());
  const { data: items } = useLoadData(getCartItems, id, { refreshTrigger });
  const navigate = useNavigate();

  useEffect(() => {
    if (items?.length === 0) navigate("/");
  });

  return (
    <table className={styles.items}>
      <thead>
        <tr>
          <th>Item</th>
          <th>Price</th>
          <th>Quantity</th>
        </tr>
      </thead>
      <tbody>
        {(items ?? []).map((item) => (
          <ItemRow
            key={item.productId}
            item={item}
            onChange={() => setRefreshTrigger(crypto.randomUUID())}
          />
        ))}
      </tbody>
    </table>
  );
}
