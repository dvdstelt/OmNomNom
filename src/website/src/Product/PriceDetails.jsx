import { getProductPriceDetails } from "../productService";
import { useLoadData } from "@/misc";
import Price from "../misc/Price";

import styles from "./PriceDetails.module.css";

export default function PriceDetails({ id }) {
  const { data: model } = useLoadData(getProductPriceDetails, id);

  return (
    model && (
      <div className={styles.priceDetails}>
        <span className={styles.price}>
          <Price price={model.price} />
        </span>
        {model.discount ? (
          <span>{`save ${model.discount.toLocaleString("en-US", {
            style: "currency",
            currency: "USD",
          })}`}</span>
        ) : null}
      </div>
    )
  );
}
