// Cart management using localStorage
const Cart = {
  STORAGE_KEY: 'omnomnom_cart',

  getItems() {
    const data = localStorage.getItem(this.STORAGE_KEY);
    return data ? JSON.parse(data) : [];
  },

  saveItems(items) {
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(items));
    this.updateCartIndicator();
  },

  addItem(beerId, quantity = 1) {
    const items = this.getItems();
    const existing = items.find(i => i.beerId === parseInt(beerId));
    const beer = getBeerById(beerId);
    if (!beer) return;

    if (existing) {
      existing.quantity = Math.min(existing.quantity + quantity, beer.stock);
    } else {
      items.push({ beerId: parseInt(beerId), quantity: Math.min(quantity, beer.stock) });
    }
    this.saveItems(items);
  },

  removeItem(beerId) {
    let items = this.getItems();
    items = items.filter(i => i.beerId !== parseInt(beerId));
    this.saveItems(items);
  },

  updateQuantity(beerId, quantity) {
    const items = this.getItems();
    const item = items.find(i => i.beerId === parseInt(beerId));
    if (item) {
      const beer = getBeerById(beerId);
      if (quantity <= 0) {
        this.removeItem(beerId);
        return;
      }
      item.quantity = Math.min(quantity, beer ? beer.stock : 99);
      this.saveItems(items);
    }
  },

  getItemCount() {
    return this.getItems().reduce((sum, item) => sum + item.quantity, 0);
  },

  getSubtotal() {
    return this.getItems().reduce((sum, item) => {
      const beer = getBeerById(item.beerId);
      return sum + (beer ? beer.discountedPrice * item.quantity : 0);
    }, 0);
  },

  getTax() {
    return this.getSubtotal() * 0.08; // 8% tax
  },

  clear() {
    localStorage.removeItem(this.STORAGE_KEY);
    this.updateCartIndicator();
  },

  updateCartIndicator() {
    const badge = document.querySelector('.cart-badge');
    if (badge) {
      const count = this.getItemCount();
      badge.textContent = count;
      badge.style.display = count > 0 ? 'flex' : 'none';
    }
  }
};

// Shipping and checkout data stored in sessionStorage
const Checkout = {
  getShippingAddress() {
    const data = sessionStorage.getItem('omnomnom_shipping_address');
    return data ? JSON.parse(data) : null;
  },

  saveShippingAddress(address) {
    sessionStorage.setItem('omnomnom_shipping_address', JSON.stringify(address));
  },

  getBillingAddress() {
    const data = sessionStorage.getItem('omnomnom_billing_address');
    return data ? JSON.parse(data) : null;
  },

  saveBillingAddress(address) {
    sessionStorage.setItem('omnomnom_billing_address', JSON.stringify(address));
  },

  getShippingOption() {
    const data = sessionStorage.getItem('omnomnom_shipping_option');
    return data ? JSON.parse(data) : null;
  },

  saveShippingOption(option) {
    sessionStorage.setItem('omnomnom_shipping_option', JSON.stringify(option));
  },

  getPaymentMethod() {
    const data = sessionStorage.getItem('omnomnom_payment');
    return data ? JSON.parse(data) : { type: 'saved', last4: '4242', brand: 'Visa', expiry: '12/27' };
  },

  savePaymentMethod(payment) {
    sessionStorage.setItem('omnomnom_payment', JSON.stringify(payment));
  },

  getShippingCost() {
    const option = this.getShippingOption();
    if (!option) return 0;
    return option.price;
  },

  getVoucher() {
    const data = sessionStorage.getItem('omnomnom_voucher');
    return data ? JSON.parse(data) : null;
  },

  applyVoucher(code) {
    const voucher = VOUCHERS.find(v => v.code.toLowerCase() === code.toLowerCase());
    if (!voucher) return { success: false, message: 'Invalid voucher code' };
    sessionStorage.setItem('omnomnom_voucher', JSON.stringify(voucher));
    return { success: true, message: `"${voucher.code}" applied: ${voucher.label}` };
  },

  removeVoucher() {
    sessionStorage.removeItem('omnomnom_voucher');
  },

  getDiscount() {
    const voucher = this.getVoucher();
    if (!voucher) return 0;
    if (voucher.type === 'percent') return Cart.getSubtotal() * (voucher.value / 100);
    if (voucher.type === 'fixed') return Math.min(voucher.value, Cart.getSubtotal());
    return 0;
  },

  getTotal() {
    return Cart.getSubtotal() + Cart.getTax() + this.getShippingCost() - this.getDiscount();
  },

  clearAll() {
    Cart.clear();
    sessionStorage.removeItem('omnomnom_shipping_address');
    sessionStorage.removeItem('omnomnom_billing_address');
    sessionStorage.removeItem('omnomnom_shipping_option');
    sessionStorage.removeItem('omnomnom_payment');
    sessionStorage.removeItem('omnomnom_voucher');
  }
};

// Available voucher codes
const VOUCHERS = [
  { code: 'CHEERS10', type: 'percent', value: 10, label: '10% off your order' },
  { code: 'CRAFTLOVER', type: 'fixed', value: 5, label: '$5.00 off your order' },
  { code: 'FREEBIE', type: 'percent', value: 15, label: '15% off your order' }
];

// Shipping options with delivery date calculation
const SHIPPING_OPTIONS = [
  { id: 'standard', name: 'Standard Shipping', days: [5, 7], price: 4.99 },
  { id: 'expedited', name: 'Expedited Shipping', days: [2, 3], price: 12.99 },
  { id: 'priority', name: 'Priority Shipping', days: [1, 1], price: 24.99 }
];

function getDeliveryDates(option) {
  const now = new Date();
  const start = new Date(now);
  start.setDate(start.getDate() + option.days[0]);
  const end = new Date(now);
  end.setDate(end.getDate() + option.days[1]);

  const fmt = { month: 'short', day: 'numeric' };
  if (option.days[0] === option.days[1]) {
    return start.toLocaleDateString('en-US', fmt);
  }
  return `${start.toLocaleDateString('en-US', fmt)} - ${end.toLocaleDateString('en-US', fmt)}`;
}
