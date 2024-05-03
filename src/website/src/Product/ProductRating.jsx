import fullStar from "@/assets/fullstar.png";
import halfStar from "@/assets/halfstar.png";
import emptyStar from "@/assets/emptystar.png";
import { getProductRating } from "../productService";
import { useLoadData } from "@/misc";

import styles from "./ProductRating.module.css";

export default function ProductRating({ id, rating, className, long = false }) {
  const { data: ratingData } = useLoadData(
    rating ? () => rating : getProductRating,
    id
  );
  const starItems =
    ratingData &&
    Array.from({ length: 5 }).map((_, i) => {
      if (i + 1 < ratingData.stars + 0.3) return fullStar;
      if (i + 1 < ratingData.stars + 0.8) return halfStar;
      return emptyStar;
    });

  return (
    ratingData && (
      <div className={`${styles.rating} ${className ?? ""}`}>
        <div className={styles.stars} title={`${ratingData.stars} out of 5`}>
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
