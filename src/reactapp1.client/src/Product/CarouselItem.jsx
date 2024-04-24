import styles from "./CarouselItem.module.css";
import Price from "../misc/Price";
import ProductImage from "./ProductImage";
import ProductRating from "./ProductRating";

export default function CarouselItem({ id, imageUrl, name, price, rating }) {
  return (
    <div className={styles.carouselItem}>
      <div className={styles.imageContainer}>
        <ProductImage
          id={id}
          imageUrl={imageUrl}
          className={styles.productImage}
        />
      </div>
      <div>{name}</div>
      <ProductRating id={id} rating={rating} />
      <div className={styles.price}>
        <Price price={price} />
      </div>
    </div>
  );
}
