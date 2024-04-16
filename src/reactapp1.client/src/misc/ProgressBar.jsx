import styles from "./ProgressBar.module.css";

import cartIcon from "@/assets/cartitems.png";

export const Stages = Object.freeze({
  Address: 1,
  Shipping: 2,
  Payment: 3,
  Summary: 4,
});

export default function ProgressBar({ stage }) {
  const progressWidth = (() => {
    switch (stage) {
      case Stages.Address:
        return "calc(5% - 0.5em)";
      case Stages.Shipping:
        return "calc(35% - 0.5em)";
      case Stages.Payment:
        return "calc(65% - 0.5em)";
      case Stages.Summary:
        return "calc(95% - 0.5em)";
      default:
        return "calc(0% - 0.5em)";
    }
  })();

  return (
    <div className={styles.progressBar}>
      <div className={styles.progress} style={{ width: progressWidth }} />
      <img
        className={styles.cartIcon}
        src={cartIcon}
        style={{ left: progressWidth }}
      />
      <div
        className={`${styles.address} ${
          stage >= Stages.Address ? styles.active : ""
        }`}
      >
        <span>Address</span>
      </div>
      <div
        className={`${styles.shipping} ${
          stage >= Stages.Shipping ? styles.active : ""
        }`}
      >
        <span>Shipping</span>
      </div>
      <div
        className={`${styles.payment} ${
          stage >= Stages.Payment ? styles.active : ""
        }`}
      >
        <span>Payment</span>
      </div>
      <div
        className={`${styles.summary} ${
          stage >= Stages.Summary ? styles.active : ""
        }`}
      >
        <span>Summary</span>
      </div>
    </div>
  );
}
