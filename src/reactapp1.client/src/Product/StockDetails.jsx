import {
  getProductStockDetails,
  useGetProduceDetails,
} from "../productService";

import styles from "./StockDetails.module.css";

export default function StockDetails({ id }) {
  const { data: model } = useGetProduceDetails(getProductStockDetails, id);
  const lowStock = (model?.stockCount ?? 0) < 100;

  return (
    model && (
      <div className={styles.stockDetails}>
        {lowStock ? (
          <>
            <span className={styles.lowStock}>
              Only {model.stockCount} left in stock
            </span>
            <span>with more on the way</span>
          </>
        ) : (
          <span>{model.stockCount} in stock</span>
        )}
      </div>
    )
  );
}
