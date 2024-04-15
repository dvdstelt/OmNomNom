import PriceDetails from "./Product/PriceDetails";
import ProductImage from "./Product/ProductImage";
import ProductRating from "./Product/ProductRating";
import ShippingDetails from "./Product/ShippingDetails";
import StockDetails from "./Product/StockDetails";
import { getProductName } from "./productService";
import { useLoadData } from "./misc";
import { useParams } from "react-router-dom";
import { useEffect } from "react";

import styles from "./Product.module.css";

export default function Product() {
  const { productId } = useParams();

  const { data: productName } = useLoadData(getProductName, productId);
  useEffect(() => {
    document.title = `${productName} - omnomnom.com`;
  }, [productName]);

  return (
    <div className={styles.product}>
      <h1>{productName}</h1>
      <ProductRating id={productId} long={true} />
      <div className={styles.imageAndDetails}>
        <ProductImage id={productId} className={styles.productImage} />
        <div className={styles.productDetailsAndInfo}>
          <div className={styles.productDetails}>
            <StockDetails id={productId} />
            <PriceDetails id={productId} />
          </div>
          <div className={styles.buttonGroup}>
            <button type="button">Add to Cart</button>
          </div>
          <ShippingDetails id={productId} className={styles.shippingInfo} />
        </div>
      </div>
    </div>
  );
}
