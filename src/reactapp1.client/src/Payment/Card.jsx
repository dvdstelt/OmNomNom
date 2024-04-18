export const CardType = Object.freeze({
  MasterCard: 1,
  Visa: 2,
  Discover: 3,
  Diners: 4,
  Amex: 5,
  UnionPay: 6,
});

export const CurrencyType = Object.freeze({
  USD: 1,
  Other: 2,
});

import styles from "./Card.module.css";
import masterCardIcon from "@/assets/mastercard.png";
import visaIcon from "@/assets/visa.png";
import discoverIcon from "@/assets/discover.png";
import dinersIcon from "@/assets/diners.png";
import amexIcon from "@/assets/amex.png";
import upayIcon from "@/assets/unionpay.png";
import { useState } from "react";

export default function Card({ details }) {
  //TODO: this will need to be hoisted to the parent for saving
  const [selectedCurrencyType, setSelectedCurrencyType] = useState(
    details.currencyType
  );

  const icon = (() => {
    switch (details.type) {
      case CardType.MasterCard:
        return masterCardIcon;
      case CardType.Visa:
        return visaIcon;
      case CardType.Discover:
        return discoverIcon;
      case CardType.Diners:
        return dinersIcon;
      case CardType.Amex:
        return amexIcon;
      case CardType.UnionPay:
        return upayIcon;
    }
  })();

  return (
    <div className={styles.card}>
      <img src={icon} className={styles.cardIcon} />
      <div className={styles.cardDetails}>
        <div>
          <div>
            <span className={styles.cardType}>
              {
                Object.keys(CardType)[
                  Object.values(CardType).indexOf(details.type)
                ]
              }
            </span>
            <span className={styles.cardNumber}>
              {`ending in ${details.shortNumber}`}
            </span>
          </div>
          <span className={styles.cardName}>{details.name}</span>
          <span className={styles.cardExpiry}>{details.expiry}</span>
        </div>
        <div className={styles.currencyDetails}>
          <span>Please tell us the currency of your card</span>
          <div>
            <input
              type="radio"
              id={`${details.id}currencyType${CurrencyType.USD}`}
              value={CurrencyType.USD}
              name={`${details.id}currencyType`}
              checked={selectedCurrencyType === CurrencyType.USD}
              onChange={() => setSelectedCurrencyType(CurrencyType.USD)}
            />
            <label htmlFor={`currencyType${CurrencyType.USD}`}>
              My card is in US Dollars
            </label>
          </div>
          <div>
            <input
              type="radio"
              id={`${details.id}currencyType${CurrencyType.Other}`}
              value={CurrencyType.Other}
              name={`${details.id}currencyType`}
              checked={selectedCurrencyType === CurrencyType.Other}
              onChange={() => setSelectedCurrencyType(CurrencyType.Other)}
            />
            <label htmlFor={`currencyType${CurrencyType.Other}`}>
              My card is in a different currency
            </label>
          </div>
        </div>
      </div>
    </div>
  );
}
