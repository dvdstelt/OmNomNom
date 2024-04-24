import { getProductImage } from "../productService";
import { useLoadData } from "@/misc";

export default function ProductImage({ id, imageUrl, className }) {
  const { data: imageSrc, loading } = useLoadData(
    imageUrl ? () => imageUrl : getProductImage,
    id
  );

  return loading ? (
    <div>Loading...</div>
  ) : (
    imageSrc && (
      <img
        className={className}
        src={`/products/${imageSrc}`}
        alt="product image"
      />
    )
  );
}
