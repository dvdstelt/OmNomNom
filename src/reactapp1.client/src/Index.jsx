import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getProducts } from "./modelService";
import CarouselItem from "./Product/CarouselItem";

import styles from "./Index.module.css";

export default function Index() {
  const [products, setProducts] = useState([]);

  useEffect(() => {
    async function load() {
      setProducts(await getProducts());
    }

    load();
  }, []);

  return (
    <div className={styles.index}>
      <h2>Welcome to Omnomnom.com</h2>
      <div className={styles.carousel}>
        {products.map((product) => (
          <Link key={product.id} to={`/product/${product.id}`}>
            <CarouselItem
              id={product.id}
              name={product.name}
              price={product.price}
            />
          </Link>
        ))}
      </div>
    </div>
  );
}
