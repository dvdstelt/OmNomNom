import {
  getProductShippingDetails,
  useGetProduceDetails,
} from "../productService";

export default function ShippingDetails({ id, className }) {
  const { data: model } = useGetProduceDetails(getProductShippingDetails, id);

  return (
    model && (
      <div className={className}>
        {model.shipsToLocation
          ? `This item ships to ${model.shippingLocation}`
          : `This item does not ship to ${model.shippingLocation}`}
      </div>
    )
  );
}
