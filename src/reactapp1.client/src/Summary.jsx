import { Link, useNavigate, useParams } from "react-router-dom";
import { useLoadData, useLocalStorage } from "./misc";
import ProgressBar, { Stages } from "./misc/ProgressBar";
import { useContext, useEffect, useState } from "react";
import axios from "axios";
import { OrderIdContext } from "./App";
import Price from "./misc/Price";

import styles from "./Summary.module.css";
import masterCardIcon from "@/assets/mastercard.png";
import visaIcon from "@/assets/visa.png";
import discoverIcon from "@/assets/discover.png";
import dinersIcon from "@/assets/diners.png";
import amexIcon from "@/assets/amex.png";
import upayIcon from "@/assets/unionpay.png";
import Items from "./Shipping/Items";

export default function Summary() {
  const { orderId } = useParams();
  const [selectedCard, setSelectedCard] = useState("");
  const { setCurrentOrderId } = useContext(OrderIdContext);
  const navigate = useNavigate();

  // const { data } = useLoadData(getDeliveryOptions, orderId, {
  //   callback: deliveryOptionsLoaded,
  // });
  // function deliveryOptionsLoaded({ selectedId }) {
  //   setSelectedDeliveryOptionId(selectedId);
  // }

  useEffect(() => {
    document.title = "Order Summary - omnomnom.com";
  }, []);

  async function placeOrder() {
    //await axios.post(`https://localhost:7126/buy/payment/${orderId}`, {??});
    setCurrentOrderId(null);
    navigate(`/`);
  }

  return (
    <div className={styles.summary}>
      <ProgressBar stage={Stages.Summary} />
      <h1>Review your order</h1>
      <div className={styles.content}>
        <div className={styles.shippingPaymentAndItems}>
          <div className={styles.shippingAndPayment}>
            <div className={styles.shipping}>
              <span className={styles.inline}>
                <h2>Shipping Address</h2>
                <Link to={`/buy/shipping/${orderId}`}>change</Link>
              </span>
              <div>Udi Dahan</div>
              <div>20 Uri Tzvi Greenberg Street</div>
              <div>Haifa, Israel</div>
            </div>
            <div className={styles.paymentAndBilling}>
              <div>
                <span className={styles.inline}>
                  <h2>Payment method</h2>
                  <Link to={`/buy/payment/${orderId}`}>change</Link>
                </span>
                <span className={styles.inline}>
                  <img src={masterCardIcon} />
                  <span>ending in 2464</span>
                </span>
              </div>
              <div>
                <span className={styles.inline}>
                  <h2>Billing Address</h2>
                  <Link to={`/buy/shipping/${orderId}`}>change</Link>
                </span>
                <div>Same as shipping address</div>
              </div>
            </div>
          </div>
          <div className={styles.items}>
            <Items orderId={orderId} includeImages={true} />
          </div>
        </div>
        <div className={styles.priceDetails}>
          <button onClick={placeOrder}>Place your order</button>
          <h2>Order Summary</h2>
          <div>
            <label>Items:</label>
            <Price price={42} />
          </div>
          <div>
            <label>Shipping:</label>
            <Price price={7.98} />
          </div>
          <div className={styles.spaceAbove}>
            <label>Total before tax:</label>
            <Price price={49.98} />
          </div>
          <div>
            <label>Estimate tax:</label>
            <Price price={0} />
          </div>
          <div className={styles.orderTotal}>
            <label>Order Total:</label>
            <Price price={49.98} />
          </div>
        </div>
      </div>
    </div>
  );
}
