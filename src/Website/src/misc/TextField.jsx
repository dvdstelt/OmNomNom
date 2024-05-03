import { useMemo } from "react";

import styles from "./TextField.module.css";

export default function TextField({ label, value = "", onChange = null }) {
  const id = useMemo(() => crypto.randomUUID(), []);

  function onValueChange(e) {
    const newValue = e.target.value;
    if (onChange) {
      onChange(newValue);
    }
  }

  return (
    <div className={styles.textField}>
      <label htmlFor={id}>{label}</label>
      <input id={id} value={value} onChange={onValueChange} />
    </div>
  );
}
