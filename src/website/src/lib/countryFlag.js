// Maps the country names used in the Catalog seed data to ISO-3166-1
// alpha-2 codes for the flagcdn.com flag images. Returns null when the
// country is unknown so the caller can render nothing rather than a
// broken image.

const ISO_BY_COUNTRY = {
  'United States': 'us',
  Belgium: 'be',
  'The Netherlands': 'nl'
};

export function countryFlagSrc(country) {
  const code = ISO_BY_COUNTRY[country];
  if (!code) return null;
  return {
    src: `https://flagcdn.com/w40/${code}.png`,
    srcset: `https://flagcdn.com/w80/${code}.png 2x`,
    alt: country
  };
}
