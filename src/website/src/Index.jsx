import { Link } from "react-router-dom";
import { getProducts } from "./productService";
import { useLoadData } from "./misc";
import CarouselItem from "./Product/CarouselItem";
import Filter from "./misc/Filter";
import { useEffect, useState } from "react";

import styles from "./Index.module.css";

export default function Index() {
  const { data: products } = useLoadData(getProducts);
  const [selectedCategories, setSelectedCategories] = useState([]);

  const categories = [
    ...new Set((products ?? []).map((product) => product.category)),
  ];

  useEffect(() => {
    document.title = `omnomnom.com`;
  }, []);

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
              <Link
                key={product.productId}
                to={`/product/${product.productId}`}
              >
                <CarouselItem
                  id={product.productId}
                  imageUrl={product.imageUrl}
                  name={product.name}
                  price={product.price}
                  rating={{
                    stars: product.stars,
                    reviewCount: product.reviewCount,
                  }}
                />
              </Link>
            ))}
        </div>
      </div>
    </div>
  );
}
