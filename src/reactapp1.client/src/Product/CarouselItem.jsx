import styles from "./CarouselItem.module.css";
import Price from "./Price";
import ProductImage from "./ProductImage";
import ProductRating from "./ProductRating";

export default function CarouselItem({ id, name, price }) {
  return (
    <div className={styles.carouselItem}>
      <div className={styles.imageContainer}>
        <ProductImage id={id} className={styles.productImage} />
      </div>
      <div>{name}</div>
      <ProductRating id={id} />
      <div className={styles.price}>
        <Price price={price} />
      </div>
    </div>
  );
}
