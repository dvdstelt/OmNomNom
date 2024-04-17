import PriceDetails from "./Product/PriceDetails";
import ProductImage from "./Product/ProductImage";
import ProductRating from "./Product/ProductRating";
import ShippingDetails from "./Product/ShippingDetails";
import StockDetails from "./Product/StockDetails";
import { addProductToCart, getProductName } from "./productService";
import { useLoadData, useLocalStorage } from "./misc";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect } from "react";

import styles from "./Product.module.css";

export default function Product() {
  const { productId } = useParams();
  const navigate = useNavigate();
  const [currentOrderId, setCurrentOrderId] = useLocalStorage("orderId");

  const { data: productName } = useLoadData(getProductName, productId);
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
            <StockDetails id={productId} />
            <PriceDetails id={productId} />
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
