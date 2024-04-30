export const CardType = Object.freeze({
  MasterCard: "MasterCard",
  Visa: "Visa",
  Discover: "Discover",
  Diners: "Diners",
  Amex: "Amex",
  UnionPay: "UnionPay",
});

export const CurrencyType = Object.freeze({
  USD: "USD",
  Other: "Other",
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
    details.currency
  );

  const icon = (() => {
    switch (details.cardType) {
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
                  Object.values(CardType).indexOf(details.cardType)
                ]
              }
            </span>
            <span className={styles.cardNumber}>
              {`ending in ${details.lastDigits}`}
            </span>
          </div>
          <span className={styles.cardName}>{details.cardHolder}</span>
          <span className={styles.cardExpiry}>
            {details.expiryDate.padStart(7, "0")}
          </span>
        </div>
        <div className={styles.currencyDetails}>
          <span>Please tell us the currency of your card</span>
          <div>
            <input
              type="radio"
              id={`${details.cardId}currencyType${CurrencyType.USD}`}
              value={CurrencyType.USD}
              name={`${details.cardId}currencyType`}
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
              id={`${details.cardId}currencyType${CurrencyType.Other}`}
              value={CurrencyType.Other}
              name={`${details.cardId}currencyType`}
              checked={selectedCurrencyType !== CurrencyType.USD}
              onChange={() => setSelectedCurrencyType("")}
            />
            <label htmlFor={`currencyType${CurrencyType.Other}`}>
              My card is in a different currency
            </label>
            <input
              maxLength={3}
              className={`${styles.currencyType} ${
                selectedCurrencyType !== CurrencyType.USD ? styles.visible : ""
              }`}
              value={selectedCurrencyType}
              onChange={(e) => setSelectedCurrencyType(e.currentTarget.value)}
            />
          </div>
        </div>
      </div>
    </div>
  );
}
