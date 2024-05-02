import { useEffect, useState } from "react";
import ReactModal from "react-modal";
import { useLoadData, useLocalStorage } from "../misc";
import { getLocations } from "../siteService";

import marker from "@/assets/marker.png";
import styles from "./LocationSelect.module.css";
import "./modal.css";

export default function LocationSelect() {
  const [locationSelectOpen, setLocationSelectOpen] = useState(false);
  const [selectedLocationId, setSelectedLocationId] =
    useLocalStorage("locationId");
  const { data: locations } = useLoadData(getLocations);

  useEffect(() => {
    if (
      locations &&
      (!selectedLocationId ||
        !locations.find((location) => location.id === selectedLocationId))
    )
      setSelectedLocationId(locations[0].id);
  }, [selectedLocationId, locations]);

  function onCloseLocationSelectModal() {
    setLocationSelectOpen(false);
  }

  function selectLocation(id) {
    setSelectedLocationId(id);
    onCloseLocationSelectModal();
  }

  const selectedLocation = locations?.find(
    (location) => location.id === selectedLocationId
  );

  return (
    selectedLocation && (
      <div>
        <div
          className={styles.locationSelect}
          onClick={() => setLocationSelectOpen(true)}
        >
          <img className={styles.marker} src={marker} />
          <div>
            <span>Deliver to {selectedLocation?.name}</span>
            <span
              className={styles.locationName}
            >{`${selectedLocation?.town}, ${selectedLocation?.zipCode}`}</span>
          </div>
        </div>
        <ReactModal
          isOpen={locationSelectOpen}
          onRequestClose={onCloseLocationSelectModal}
          className="modal"
          overlayClassName="modalOverlay"
        >
          <div className={styles.modal}>
            <div className={styles.modalHeader}>Choose your location</div>
            <div className={styles.modalContent}>
              <span className={styles.instructions}>
                Ship options and delivery speeds may vary for different
                locations
              </span>
              <div className={styles.locations}>
                {(locations ?? []).map((location) => (
                  <div
                    key={location.id}
                    className={`${styles.location} ${
                      location.id === selectedLocationId ? styles.active : ""
                    }`}
                    onClick={() => selectLocation(location.id)}
                  >
                    <span className={styles.locationName}>{location.name}</span>
                    <span>{`${location.town}, ${location.zipCode}`}</span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </ReactModal>
      </div>
    )
  );
}
