import { useState } from "react";
import { useLoadData } from "../misc";
import { getCartItems } from "../orderService";
import ItemRow from "./ItemRow";

import styles from "./Items.module.css";

export default function Items({ id }) {
  const [refreshTrigger, setRefreshTrigger] = useState(crypto.randomUUID());
  const { data: items } = useLoadData(getCartItems, id, { refreshTrigger });

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
