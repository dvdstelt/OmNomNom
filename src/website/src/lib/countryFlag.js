// Country-name → ISO 3166-1 alpha-2 mapping for the `flag-icons`
// CSS classes. The library renders flags via `<span class="fi fi-XX">`
// where XX is the lowercase code, so callers stay agnostic of the
// underlying asset format.

const ISO_BY_COUNTRY = {
  'United States': 'us',
  Belgium: 'be',
  'The Netherlands': 'nl'
};

export function countryIso(country) {
  return ISO_BY_COUNTRY[country] ?? null;
}
