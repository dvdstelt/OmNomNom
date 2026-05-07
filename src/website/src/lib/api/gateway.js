// Thin client for the CompositionGateway. The frontend only ever talks to
// this URL — every per-service composer contributes its slice of each
// response server-side, and microviews consume slices passed down by the
// Branding page that owns the route.

const BASE = 'https://localhost:7126';

async function getJson(path) {
  const response = await fetch(`${BASE}${path}`);
  if (!response.ok) throw new Error(`GET ${path} failed: ${response.status}`);
  return response.json();
}

async function postJson(path, body) {
  const response = await fetch(`${BASE}${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body ?? {})
  });
  if (!response.ok) throw new Error(`POST ${path} failed: ${response.status}`);
  return response.status === 204 ? null : response.json().catch(() => null);
}

function buildProductsQuery({
  categories = [],
  breweries = [],
  countries = [],
  inStock = true,
  sort = 'default',
  page = 1,
  size = 10
} = {}) {
  const params = new URLSearchParams();
  if (categories.length) params.set('categories', categories.join(','));
  if (breweries.length) params.set('breweries', breweries.join(','));
  if (countries.length) params.set('countries', countries.join(','));
  // Only emit inStock when it differs from the server-side default.
  if (inStock === false) params.set('inStock', 'false');
  if (sort && sort !== 'default') params.set('sort', sort);
  if (page !== 1) params.set('page', String(page));
  if (size !== 10) params.set('size', String(size));
  const qs = params.toString();
  return qs ? `/products?${qs}` : '/products';
}

export const gateway = {
  getProducts: (options) => getJson(buildProductsQuery(options)),
  getFacets: () => getJson('/products/facets'),
  getProduct: (id) => getJson(`/product/${id}`),
  addProductToCart: (orderId, productDetails) =>
    postJson(`/cart/addproduct/${orderId}`, productDetails),
  getCart: (orderId) => getJson(`/cart/${orderId}`),
  saveCart: (orderId, items) => postJson(`/cart/${orderId}`, items),
  getAddress: (orderId) => getJson(`/buy/address/${orderId}`),
  saveAddress: (orderId, addressData) => postJson(`/buy/address/${orderId}`, addressData),
  getShipping: (orderId) => getJson(`/buy/shipping/${orderId}`),
  saveShipping: (orderId, deliveryOptionId) =>
    postJson(`/buy/shipping/${orderId}`, { deliveryOptionId }),
  getPayment: (orderId) => getJson(`/buy/payment/${orderId}`),
  savePayment: (orderId, creditCardId) =>
    postJson(`/buy/payment/${orderId}`, { creditCardId }),
  getCreditCards: (customerId) => getJson(`/buy/creditcard/${customerId}`),
  getSummary: (orderId) => getJson(`/buy/summary/${orderId}`),
  placeOrder: (orderId) => postJson(`/buy/summary/${orderId}`)
};
