import { useNavigate, useParams } from "react-router-dom";
import { useLoadData } from "./misc";
import { getDeliveryOptions } from "./orderService";
import ProgressBar, { Stages } from "./misc/ProgressBar";
import DeliveryOptions from "./Shipping/DeliveryOptions";
import Items from "./Shipping/Items";
import { useEffect, useState } from "react";
import axios from "axios";
import Card, { CardType, CurrencyType } from "./Payment/Card";

import styles from "./Payment.module.css";
import masterCardIcon from "@/assets/mastercard.png";
import visaIcon from "@/assets/visa.png";
import discoverIcon from "@/assets/discover.png";
import dinersIcon from "@/assets/diners.png";
import amexIcon from "@/assets/amex.png";
import upayIcon from "@/assets/unionpay.png";

export default function Payment() {
  const { orderId } = useParams();
  const [selectedCard, setSelectedCard] = useState("");
  const navigate = useNavigate();

  // const { data } = useLoadData(getDeliveryOptions, orderId, {
  //   callback: deliveryOptionsLoaded,
  // });
  // function deliveryOptionsLoaded({ selectedId }) {
  //   setSelectedDeliveryOptionId(selectedId);
  // }

  const cards = [
    {
      id: "3102b0fa-e306-48e0-a6c7-a9277dc002fb",
      type: CardType.MasterCard,
      shortNumber: 2464,
      name: "Udi Dahan",
      expiry: "09/2026",
      currencyType: CurrencyType.USD,
    },
    {
      id: "3102b0fa-e306-48e0-a6c7-a9277dc002fc",
      type: CardType.Amex,
      shortNumber: 9999,
      name: "Fakey McFakerson",
      expiry: "01/2099",
      currencyType: CurrencyType.Other,
    },
  ];

  useEffect(() => {
    document.title = "Payment Options - omnomnom.com";
    setSelectedCard(cards[0].id);
  }, []);

  async function saveAndContinue() {
    //await axios.post(`https://localhost:7126/buy/payment/${orderId}`, {??});
    navigate(`/buy/summary/${orderId}`);
  }

  return (
    <div className={styles.payment}>
      <ProgressBar stage={Stages.Payment} />
      <h1>Select a payment option</h1>
      <h2>Your credit and debit cards</h2>
      <div className={styles.cards}>
        {cards.map((card) => (
          <div key={card.id}>
            <input
              type="radio"
              name="selectedCard"
              id={card.id}
              value={card.id}
              checked={selectedCard === card.id}
              onChange={() => setSelectedCard(card.id)}
            />
            <Card details={card} />
          </div>
        ))}
      </div>
      <h2>More payment options</h2>
      <div className={styles.moreOptions}>
        <div>omnomnom.com accepts all major credit and debit cards</div>
        <div className={styles.cardIcons}>
          <img src={masterCardIcon} />
          <img src={visaIcon} />
          <img src={discoverIcon} />
          <img src={dinersIcon} />
          <img src={amexIcon} />
          <img src={upayIcon} />
        </div>
      </div>
      <div className={styles.buttonGroup}>
        <button onClick={saveAndContinue}>Continue</button>
      </div>
    </div>
  );
}
