import { getProductPriceDetails } from "../productService";
import { useLoadData } from "@/misc";
import Price from "./Price";

import styles from "./PriceDetails.module.css";

export default function PriceDetails({ id }) {
  const { data: model } = useLoadData(getProductPriceDetails, id);

  return (
    model && (
      <div className={styles.priceDetails}>
        <span className={styles.price}>
          <Price price={model.price} />
        </span>
        <span>{`save $${13.37}`}</span>
      </div>
    )
  );
}
