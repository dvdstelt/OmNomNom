import { Link, useNavigate, useParams } from "react-router-dom";
import { useLoadData, useLocalStorage } from "./misc";
import ProgressBar, { Stages } from "./misc/ProgressBar";
import { useContext, useEffect, useState } from "react";
import axios from "axios";
import { OrderIdContext } from "./App";
import Price from "./misc/Price";
import Items from "./Shipping/Items";
import { getAddress, getOrderSummary } from "./orderService";
import { blankAddress } from "./Address";
import { cardIcon } from "./Payment/Card";

import styles from "./Summary.module.css";

export default function Summary() {
  const { orderId } = useParams();
  const { setCurrentOrderId } = useContext(OrderIdContext);
  const navigate = useNavigate();

  const { data: summaryData } = useLoadData(getOrderSummary, orderId);
  const { data: addressData } = useLoadData(getAddress, orderId);

  const paymentCardIcon = cardIcon("MasterCard");

  useEffect(() => {
    document.title = "Order Summary - omnomnom.com";
  }, []);

  async function placeOrder() {
    await axios.post(`https://localhost:7126/buy/summary/${orderId}`, {});
    setCurrentOrderId(null);
    navigate(`/`);
  }

  const billingSameAsShipping =
    addressData &&
    Object.keys(blankAddress)
      .filter((keyName) => keyName !== "id" && keyName !== "fullName")
      .every(
        (keyName) =>
          addressData.shippingAddress[keyName] ===
          addressData.billingAddress[keyName]
      );

  const products = Object.values(summaryData?.products ?? {});
  const productsPrice = (products ?? []).reduce(
    (sum, product) => sum + product.price * product.quantity,
    0
  );
  const shippingPrice = summaryData?.deliveryOption?.price ?? 0;
  const taxAmount = summaryData?.taxPercentage
    ? (summaryData.taxPercentage / 100) * (productsPrice + shippingPrice)
    : summaryData?.taxAmount
    ? summaryData.taxAmount
    : 0;

  return (
    <div className={styles.summary}>
      <ProgressBar stage={Stages.Summary} orderId={orderId} />
      <h1>Review your order</h1>
      <div className={styles.content}>
        <div className={styles.shippingPaymentAndItems}>
          <div className={styles.shippingAndPayment}>
            <div className={styles.shipping}>
              <span className={styles.inline}>
                <h2>Shipping Address</h2>
                <Link to={`/buy/address/${orderId}`}>change</Link>
              </span>
              {addressData ? (
                <>
                  <div>{addressData.shippingAddress.fullName}</div>
                  <div>{addressData.shippingAddress.street}</div>
                  <div>{`${addressData.shippingAddress.town}, ${addressData.shippingAddress.country}`}</div>
                  <div>{addressData.shippingAddress.zipCode}</div>
                </>
              ) : (
                "Loading..."
              )}
            </div>
            <div className={styles.paymentAndBilling}>
              <div>
                <span className={styles.inline}>
                  <h2>Payment method</h2>
                  <Link to={`/buy/payment/${orderId}`}>change</Link>
                </span>
                <span className={styles.inline}>
                  <img src={paymentCardIcon} />
                  <span>ending in 2464</span>
                </span>
              </div>
              <div>
                <span className={styles.inline}>
                  <h2>Billing Address</h2>
                  <Link to={`/buy/address/${orderId}`}>change</Link>
                </span>
                {addressData ? (
                  <div>
                    {billingSameAsShipping ? (
                      "Same as shipping address"
                    ) : (
                      <>
                        <div>{addressData.billingAddress.street}</div>
                        <div>{`${addressData.billingAddress.town}, ${addressData.billingAddress.country}`}</div>
                        <div>{addressData.billingAddress.zipCode}</div>
                      </>
                    )}
                  </div>
                ) : (
                  "Loading..."
                )}
              </div>
            </div>
          </div>
          <div className={styles.items}>
            <Items
              orderId={orderId}
              overrideItems={products}
              includeImages={true}
              showChange={true}
            />
          </div>
        </div>
        <div className={styles.priceDetails}>
          <button onClick={placeOrder}>Place your order</button>
          <h2>Order Summary</h2>
          <div>
            <label>Items:</label>
            <Price price={productsPrice} />
          </div>
          <div>
            <label>Shipping:</label>
            <Price price={shippingPrice} />
          </div>
          <div className={styles.spaceAbove}>
            <label>Total before tax:</label>
            <Price price={productsPrice + shippingPrice} />
          </div>
          <div>
            <label>Estimated tax:</label>
            <Price price={taxAmount} />
          </div>
          <div className={styles.orderTotal}>
            <label>Order Total:</label>
            <Price price={summaryData?.totalPrice ?? 0} />
          </div>
        </div>
      </div>
    </div>
  );
}
