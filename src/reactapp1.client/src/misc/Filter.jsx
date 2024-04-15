import { useRef } from "react";

import styles from "./Filter.module.css";

export default function Filter({ label, items, onChange }) {
  const itemsRef = useRef([]);

  function filterChanged() {
    onChange(
      itemsRef.current.filter((ref) => ref.checked).map((ref) => ref.id)
    );
  }

  return (
    <div className={styles.filter}>
      <div className={styles.label}>{label}</div>
      {items.map((item, i) => (
        <div>
          <input
            key={item}
            ref={(cb) => (itemsRef.current[i] = cb)}
            id={item}
            type="checkbox"
            onChange={filterChanged}
          />
          <label htmlFor={item}>{item}</label>
        </div>
      ))}
    </div>
  );
}
