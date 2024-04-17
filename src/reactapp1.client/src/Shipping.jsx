import { useNavigate, useParams } from "react-router-dom";
import { useLoadData } from "./misc";
import { getDeliveryOptions } from "./orderService";
import ProgressBar, { Stages } from "./misc/ProgressBar";
import DeliveryOptions from "./Shipping/DeliveryOptions";
import Items from "./Shipping/Items";
import { useEffect, useState } from "react";
import axios from "axios";

import styles from "./Shipping.module.css";

export default function Shipping() {
  const { orderId } = useParams();
  const [selectedDeliveryOptionId, setSelectedDeliveryOptionId] = useState("");
  const navigate = useNavigate();

  const { data } = useLoadData(getDeliveryOptions, orderId, {
    callback: deliveryOptionsLoaded,
  });
  function deliveryOptionsLoaded({ selectedId }) {
    setSelectedDeliveryOptionId(selectedId);
  }

  useEffect(() => {
    document.title = "Shipping - omnomnom.com";
  }, []);

  async function saveAndContinue() {
    await axios.post(`https://localhost:7126/buy/shipping/${orderId}`, {
      deliveryOptionId: selectedDeliveryOptionId,
    });
    navigate(`/buy/payment/${orderId}`);
  }

  return (
    <div className={styles.shipping}>
      <ProgressBar stage={Stages.Shipping} />
      <h1>When do you need it?</h1>
      <div className={styles.columns}>
        <div>
          <h2>Shipping from omnomnom.com</h2>
          <Items orderId={orderId} />
        </div>
        <div>
          <h2>Choose a delivery option</h2>
          <DeliveryOptions
            options={data?.deliveryOptions ?? []}
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
