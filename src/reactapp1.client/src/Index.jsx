import { Link } from "react-router-dom";
import { getProducts, useGetProduceDetails } from "./modelService";
import CarouselItem from "./Product/CarouselItem";
import Filter from "./misc/Filter";
import { useState } from "react";

import styles from "./Index.module.css";

export default function Index() {
  const { data: products } = useGetProduceDetails(getProducts);
  const [selectedCategories, setSelectedCategories] = useState([]);

  const categories = [
    ...new Set((products ?? []).map((product) => product.category)),
  ];

  return (
    <div className={styles.index}>
      <h2>Welcome to Omnomnom.com</h2>
      <div className={styles.filtersAndCarousel}>
        <div className={styles.filters}>
          <Filter
            label="Category"
            items={categories}
            onChange={setSelectedCategories}
          />
        </div>
        <div className={styles.carousel}>
          {(products ?? [])
            .filter(
              (product) =>
                selectedCategories.length === 0 ||
                selectedCategories.includes(product.category)
            )
            .map((product) => (
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
    </div>
  );
}
