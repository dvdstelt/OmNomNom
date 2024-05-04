import PriceDetails from "./Product/PriceDetails";
import ProductImage from "./Product/ProductImage";
import ProductRating from "./Product/ProductRating";
import ShippingDetails from "./Product/ShippingDetails";
import StockDetails from "./Product/StockDetails";
import {
  addProductToCart,
  getProductDescription,
  getProductName,
} from "./productService";
import { useLoadData } from "./misc";
import { useNavigate, useParams } from "react-router-dom";
import { useContext, useEffect } from "react";

import styles from "./Product.module.css";
import { OrderIdContext } from "./App";

export default function Product() {
  const { productId } = useParams();
  const navigate = useNavigate();
  const { currentOrderId, setCurrentOrderId } = useContext(OrderIdContext);

  const { data: productName } = useLoadData(getProductName, productId);
  const { data: productDescription } = useLoadData(
    getProductDescription,
    productId
  );
  useEffect(() => {
    document.title = `${productName} - omnomnom.com`;
  }, [productName]);

  async function addToCart() {
    setCurrentOrderId(
      await addProductToCart(currentOrderId, { id: productId, quantity: 1 })
    );
    //TODO what behaviour do we want here?
    navigate("/");
  }

  return (
    <div className={styles.product}>
      <h1>{productName}</h1>
      <ProductRating id={productId} long={true} />
      <div className={styles.imageAndDetails}>
        <ProductImage id={productId} className={styles.productImage} />
        <div className={styles.productDetailsAndInfo}>
          <div className={styles.productDetails}>
            <div className={styles.descriptionAndStock}>
              <span>{productDescription}</span>
              <StockDetails id={productId} />
            </div>
            <div className={styles.priceDetails}>
              <PriceDetails id={productId} />
            </div>
          </div>
          <div className={styles.buttonGroup}>
            <button type="button" onClick={addToCart}>
              Add to Cart
            </button>
          </div>
          <ShippingDetails id={productId} className={styles.shippingInfo} />
        </div>
      </div>
    </div>
  );
}
