import { getProductPriceDetails, useGetProduceDetails } from "../modelService";

import styles from "./PriceDetails.module.css";
import Price from "./Price";

export default function PriceDetails({ id }) {
  const { data: model } = useGetProduceDetails(getProductPriceDetails, id);

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
