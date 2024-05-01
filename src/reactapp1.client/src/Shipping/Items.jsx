import ProductImage from "../Product/ProductImage";
import { useLoadData } from "../misc";
import Price from "../misc/Price";
import { getCartItems } from "../orderService";

import styles from "./Items.module.css";

export default function Items({
  orderId,
  overrideItems,
  includeImages = false,
}) {
  const { data: items } = useLoadData(
    overrideItems ? () => overrideItems : getCartItems,
    orderId,
    { refreshTrigger: overrideItems }
  );

  return (
    <div className={styles.items}>
      {(items ?? []).map((item) => (
        <div key={item.productId} className={styles.itemContainer}>
          {includeImages && (
            <div className={styles.imageContainer}>
              <ProductImage
                id={item.productId}
                imageUrl={item.imageUrl}
                className={styles.image}
              />
            </div>
          )}
          <div key={item.productId} className={styles.item}>
            <label>{item.name}</label>
            <div className={styles.itemDetails}>
              <span className={styles.price}>
                <Price price={item.price} />
              </span>
              <span>-</span>
              <span>{`Quantity: ${item.quantity}`}</span>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}
