import styles from "./Price.module.css";

export default function Price({ price }) {
  const whole = Math.floor(price);
  const fraction = `${(price - whole) * 100}`.padStart(2, "0");

  return (
    <span className={styles.price}>
      <span className={styles.symbol}>$</span>
      <span className={styles.whole}>{whole}</span>
      <span className={styles.fraction}>{fraction}</span>
    </span>
  );
}
