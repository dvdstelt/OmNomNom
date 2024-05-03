import { useLoadData } from "@/misc";
import { getLocation } from "../siteService";
import { useLocalStorage } from "../misc";

export default function ShippingDetails({ id, className }) {
  const [locationId] = useLocalStorage("locationId");
  const { data: location } = useLoadData(getLocation, locationId);

  return (
    location && (
      <div className={className}>
        {true //model.shipsToLocation
          ? `This item ships to ${location.town}, ${location.country}`
          : `This item does not ship to ${location.town}, ${location.country}`}
      </div>
    )
  );
}
