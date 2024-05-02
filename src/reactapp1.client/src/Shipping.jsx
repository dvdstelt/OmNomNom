import { useNavigate, useParams } from "react-router-dom";
import { useLoadData } from "./misc";
import { getSelectedDeliveryOption, saveShipping } from "./orderService";
import ProgressBar, { Stages } from "./misc/ProgressBar";
import DeliveryOptions from "./Shipping/DeliveryOptions";
import Items from "./Shipping/Items";
import { useEffect, useState } from "react";

import styles from "./Shipping.module.css";

export default function Shipping() {
  const { orderId } = useParams();
  const [selectedDeliveryOptionId, setSelectedDeliveryOptionId] = useState("");
  const navigate = useNavigate();

  useLoadData(getSelectedDeliveryOption, orderId, {
    callback: previousSelectionLoaded,
  });

  function previousSelectionLoaded(loadedOptionId) {
    if (loadedOptionId) setSelectedDeliveryOptionId(loadedOptionId);
  }

  useEffect(() => {
    document.title = "Shipping - omnomnom.com";
  }, []);

  async function saveAndContinue() {
    await saveShipping(orderId, selectedDeliveryOptionId);
    navigate(`/buy/payment/${orderId}`);
  }

  return (
    <div className={styles.shipping}>
      <ProgressBar stage={Stages.Shipping} orderId={orderId} />
      <h1>When do you need it?</h1>
      <div className={styles.columns}>
        <div>
          <h2>Shipping from omnomnom.com</h2>
          <Items orderId={orderId} />
        </div>
        <div>
          <h2>Choose a delivery option</h2>
          <DeliveryOptions
            orderId={orderId}
            selectionId={selectedDeliveryOptionId}
            setSelectionId={setSelectedDeliveryOptionId}
          />
        </div>
      </div>
      <div className={styles.buttonGroup}>
        <button onClick={saveAndContinue}>Continue</button>
      </div>
    </div>
  );
}
