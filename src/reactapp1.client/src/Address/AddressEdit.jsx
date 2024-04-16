import TextField from "../misc/TextField";

import styles from "./AddressEdit.module.css";

export default function AddressEdit({ state }) {
  const [address, setAddress] = state;

  function updateAddress(fieldUpdate) {
    return (newValue) => {
      const newAddress = { ...address };
      fieldUpdate(newAddress, newValue);
      setAddress(newAddress);
    };
  }

  return (
    <div className={styles.addressEdit}>
      <TextField
        label="Address Line 1"
        value={address.street}
        onChange={updateAddress((x, value) => (x.street = value))}
      />
      <TextField
        label="Zip Code"
        value={address.zipcode}
        onChange={updateAddress((x, value) => (x.zipcode = value))}
      />
      <TextField
        label="Town/City"
        value={address.town}
        onChange={updateAddress((x, value) => (x.town = value))}
      />
      <TextField
        label="Country"
        value={address.country}
        onChange={updateAddress((x, value) => (x.country = value))}
      />
    </div>
  );
}
