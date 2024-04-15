import { getProductImage, useGetProduceDetails } from "../productService";

export default function ProductImage({ id, className }) {
  const { data: imageSrc, loading } = useGetProduceDetails(getProductImage, id);

  return loading ? (
    <div>Loading...</div>
  ) : (
    imageSrc && <img className={className} src={imageSrc} alt="product image" />
  );
}
