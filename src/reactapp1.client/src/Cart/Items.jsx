import ItemRow from "./ItemRow";

import styles from "./Items.module.css";

export default function Items({ items, refreshCart }) {
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
          <ItemRow key={item.productId} item={item} onChange={refreshCart} />
        ))}
      </tbody>
    </table>
  );
}
