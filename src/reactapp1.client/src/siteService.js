export async function getLocations() {
  await new Promise((resolve) => setTimeout(resolve, 100));
  return [
    {
      id: "1",
      name: "Dennis",
      town: "Rotterdam",
      zipCode: "1234 A",
    },
    {
      id: "2",
      name: "Phil",
      town: "Perth",
      zipCode: "6056",
    },
  ];
}
