import ProgressBar, { Stages } from "./misc/ProgressBar";
import { useLoadData } from "./misc";
import { getAddress, saveAddress } from "./orderService";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import AddressEdit from "./Address/AddressEdit";

import styles from "./Address.module.css";

export const blankAddress = {
  id: "",
  fullName: "",
  street: "",
  zipCode: "",
  town: "",
  country: "",
};

export default function Address() {
  const { orderId } = useParams();
  const shippingAddressState = useState(blankAddress);
  const billingAddressState = useState(blankAddress);
  const [billingSameAsShipping, setBillingSameAsShipping] = useState(true);
  const navigate = useNavigate();

  useLoadData(getAddress, orderId, { callback: addressLoaded });
  function addressLoaded(addressModel) {
    if (addressModel?.shippingAddress) {
      const [_, setShippingAddress] = shippingAddressState;
      setShippingAddress(addressModel.shippingAddress);
    }
    if (addressModel?.billingAddress) {
      const [_, setBillingAddress] = billingAddressState;
      setBillingAddress(addressModel.billingAddress);
    }
    setBillingSameAsShipping(
      Object.keys(blankAddress)
        .filter((keyName) => keyName !== "id")
        .every(
          (keyName) =>
            addressModel.shippingAddress[keyName] ===
            addressModel.billingAddress[keyName]
        )
    );
  }

  useEffect(() => {
    document.title = "Purchase Address - omnomnom.com";
  }, []);

  async function saveAndContinue() {
    const [shippingAddress] = shippingAddressState;
    const [billingAddress] = billingAddressState;

    await saveAddress(orderId, {
      shippingAddress,
      billingAddress: billingSameAsShipping ? shippingAddress : billingAddress,
    });
    navigate(`/buy/shipping/${orderId}`);
  }

  return (
    <div className={styles.address}>
      <ProgressBar stage={Stages.Address} orderId={orderId} />
      <h1>Where should we deliver your order?</h1>
      <h2>Enter a shipping address</h2>
      <AddressEdit state={shippingAddressState} />
      <div>
        <input
          id="isSame"
          type="checkbox"
          checked={billingSameAsShipping}
          onChange={(e) => setBillingSameAsShipping(e.target.checked)}
        />
        <label htmlFor="gift">
          Billing Address is same as Shipping Address?
        </label>
      </div>
      {billingSameAsShipping ? null : (
        <>
          <h2>Enter a billing address</h2>
          <AddressEdit state={billingAddressState} />
        </>
      )}
      <div className={styles.buttonGroup}>
        <button onClick={saveAndContinue}>Ship to this Address</button>
      </div>
    </div>
  );
}
