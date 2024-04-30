import { useLoadData } from "../misc";
import { getDeliveryOptions } from "../orderService";

import styles from "./DeliveryOptions.module.css";

export default function DeliveryOptions({
  orderId,
  selectionId,
  setSelectionId,
}) {
  const { data: options } = useLoadData(getDeliveryOptions, orderId);

  return (
    <div>
      {(options ?? []).map((option) => (
        <div key={option.deliveryOptionId} className={styles.option}>
          <div className={styles.selection}>
            <input
              id={option.deliveryOptionId}
              type="radio"
              name="deliveryOption"
              value={option.deliveryOptionId}
              checked={
                (selectionId ?? options[0].deliveryOptionId) ===
                option.deliveryOptionId
              }
              onChange={() => setSelectionId(option.deliveryOptionId)}
            />
            <label htmlFor={option.deliveryOptionId}>{option.name}</label>
          </div>
          <div className={styles.priceAndDescription}>
            <span>
              {option.price.toLocaleString("en-US", {
                style: "currency",
                currency: "USD",
              })}
            </span>
            <span> - </span>
            <span>{option.description}</span>
          </div>
        </div>
      ))}
    </div>
  );
}
