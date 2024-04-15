import fullStar from "@/assets/fullstar.png";
import emptyStar from "@/assets/emptystar.png";
import { getProductRating, useGetProduceDetails } from "../modelService";

import styles from "./ProductRating.module.css";

export default function ProductRating({ id, className, long = false }) {
  const { data: ratingData } = useGetProduceDetails(getProductRating, id);
  const starItems =
    ratingData &&
    Array.from({ length: 5 }).map((_, i) =>
      i + 1 < ratingData.stars ? fullStar : emptyStar
    );

  return (
    ratingData && (
      <div className={`${styles.rating} ${className ?? ""}`}>
        <div className={styles.stars}>
          {starItems.map((item, i) => (
            <img className={styles.star} key={i} src={item} alt="star" />
          ))}
        </div>
        {long ? (
          <span>{`${ratingData.reviewCount} customer reviews`}</span>
        ) : (
          <span>{ratingData.reviewCount}</span>
        )}
      </div>
    )
  );
}
