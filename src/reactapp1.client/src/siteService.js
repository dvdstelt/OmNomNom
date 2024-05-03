import axios from "axios";

export async function getLocations() {
  const { data } = await axios.get(`https://localhost:7126/locations`);
  return data.locations;
}

export async function getLocation(id) {
  var locations = await getLocations();
  return locations.find((location) => location.id === id);
}
