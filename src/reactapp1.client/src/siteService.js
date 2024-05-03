import axios from "axios";

export async function getLocations() {
  const { data } = await axios.get(`https://localhost:7126/locations`);
  return data.locations;
}
