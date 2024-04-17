import styles from "./DeliveryOptions.module.css";

export default function DeliveryOptions({
  options,
  selectionId,
  setSelectionId,
}) {
  return (
    <div>
      {(options ?? []).map((option) => (
        <div key={option.id} className={styles.option}>
          <div className={styles.selection}>
            <input
              id={option.id}
              type="radio"
              name="deliveryOption"
              value={option.id}
              checked={selectionId === option.id}
              onChange={() => setSelectionId(option.id)}
            />
            <label htmlFor={option.id}>{option.name}</label>
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
