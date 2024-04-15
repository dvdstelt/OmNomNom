import Price from "../Product/Price";
import ProductImage from "../Product/ProductImage";
import { useLoadData } from "../misc";
import { getProductName, getProductPriceDetails } from "../productService";

import styles from "./Items.module.css";

export default function ItemRow({ item }) {
  //If we want product API endpoint data for consistency, rather than what is returned from the cart API endpoint
  //const { data: productName } = useLoadData(getProductName, item.id);
  //const { data: priceModel } = useLoadData(getProductPriceDetails, item.id);

  return (
    <tr>
      <td className={styles.item}>
        <div className={styles.imageContainer}>
          <ProductImage id={item.id} className={styles.image} />
        </div>
        <span>{item.name}</span>
      </td>
      <td className={styles.price}>
        <Price price={item.price} />
      </td>
      <td>{item.quantity}</td>
    </tr>
  );
}
