// Shared UI components for OmNomNom

// Beer can colors by type
function getBeerCanColors(type) {
  switch (type) {
    case 'IPA':
      return {
        bg: 'linear-gradient(160deg, #2a2218 0%, #1f1a12 100%)',
        can: 'linear-gradient(135deg, #e8a832 0%, #d4912a 40%, #c07a22 100%)',
        accent: '#f4c442'
      };
    case 'Sour':
      return {
        bg: 'linear-gradient(160deg, #261a22 0%, #1e141a 100%)',
        can: 'linear-gradient(135deg, #d44a7a 0%, #c23868 40%, #a82d58 100%)',
        accent: '#e8608a'
      };
    case 'Stout':
      return {
        bg: 'linear-gradient(160deg, #181818 0%, #0e0e10 100%)',
        can: 'linear-gradient(135deg, #3a3035 0%, #2a2428 40%, #1e1a1e 100%)',
        accent: '#8a7a70'
      };
    default:
      return {
        bg: 'linear-gradient(160deg, #1a1a2e 0%, #16213e 100%)',
        can: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
        accent: '#a78bfa'
      };
  }
}

// Render a beer can SVG-like element
function renderBeerCan(beer, size = 'normal') {
  const colors = getBeerCanColors(beer.type);
  const sizeClass = size === 'large' ? 'product-image-main' : '';
  return `
    <div class="beer-can" style="background: ${colors.can}">
      <div class="beer-can-label">${beer.name}</div>
      <div class="beer-can-abv">${beer.abv}% ABV</div>
    </div>
  `;
}

// Render the site header (with or without cart indicator)
function renderHeader(showCart = true) {
  const header = document.getElementById('site-header');
  if (!header) return;

  header.innerHTML = `
    <div class="header-inner">
      <a href="index.html" class="logo">
        <span class="logo-icon">&#127866;</span>
        <span class="logo-text">OmNomNom</span>
        <span class="logo-tagline">Because beer is good, but beers are better</span>
      </a>
      <nav class="header-nav">
        <a href="index.html">Shop</a>
        ${showCart ? `
        <a href="cart.html" class="cart-link">
          <svg class="cart-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="9" cy="21" r="1"/>
            <circle cx="20" cy="21" r="1"/>
            <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"/>
          </svg>
          <span class="cart-badge" style="display:none">0</span>
        </a>` : ''}
      </nav>
    </div>
  `;

  Cart.updateCartIndicator();
}

// Render checkout header (no cart, has back/close)
function renderCheckoutHeader() {
  const header = document.getElementById('site-header');
  if (!header) return;

  header.innerHTML = `
    <div class="header-inner">
      <a href="index.html" class="logo">
        <span class="logo-icon">&#127866;</span>
        <span class="logo-text">OmNomNom</span>
        <span class="logo-tagline">Because beer is good, but beers are better</span>
      </a>
      <div class="checkout-header-label">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="18" height="18">
          <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
          <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
        </svg>
        Secure Checkout
      </div>
    </div>
  `;
}

// Render progress bar for checkout pages
function renderProgressBar(currentStep) {
  const container = document.getElementById('progress-bar');
  if (!container) return;

  const steps = [
    { id: 'cart', label: 'Cart', href: 'cart.html' },
    { id: 'address', label: 'Address', href: 'address.html' },
    { id: 'shipping', label: 'Shipping', href: 'shipping.html' },
    { id: 'payment', label: 'Payment', href: 'payment.html' },
    { id: 'summary', label: 'Review', href: 'summary.html' }
  ];

  const currentIndex = steps.findIndex(s => s.id === currentStep);

  let html = '<div class="progress-steps">';
  steps.forEach((step, i) => {
    const state = i < currentIndex ? 'completed' : i === currentIndex ? 'active' : 'upcoming';
    const clickable = i < currentIndex;
    html += `
      <div class="progress-step ${state}">
        ${clickable ? `<a href="${step.href}">` : '<span>'}
          <div class="step-circle">${i < currentIndex ? '&#10003;' : i + 1}</div>
          <div class="step-label">${step.label}</div>
        ${clickable ? '</a>' : '</span>'}
      </div>
      ${i < steps.length - 1 ? `<div class="step-connector ${i < currentIndex ? 'completed' : ''}"></div>` : ''}
    `;
  });
  html += '</div>';
  container.innerHTML = html;
}

// Render order total sidebar
function renderOrderSidebar(containerId = 'order-sidebar') {
  const container = document.getElementById(containerId);
  if (!container) return;

  const items = Cart.getItems();
  const subtotal = Cart.getSubtotal();
  const tax = Cart.getTax();
  const shippingCost = Checkout.getShippingCost();
  const discount = Checkout.getDiscount();
  const total = Checkout.getTotal();
  const itemCount = Cart.getItemCount();
  const voucher = Checkout.getVoucher();

  container.innerHTML = `
    <div class="sidebar-card">
      <h3 class="sidebar-title">Order Summary</h3>
      <div class="sidebar-row">
        <span>Items (${itemCount})</span>
        <span>${formatPrice(subtotal)}</span>
      </div>
      ${discount > 0 ? `
      <div class="sidebar-row sidebar-discount">
        <span>Voucher (${voucher.code})</span>
        <span>-${formatPrice(discount)}</span>
      </div>` : ''}
      <div class="sidebar-row">
        <span>Shipping</span>
        <span>${shippingCost > 0 ? formatPrice(shippingCost) : '--'}</span>
      </div>
      <div class="sidebar-row">
        <span>Est. Tax</span>
        <span>${formatPrice(tax)}</span>
      </div>
      <div class="sidebar-divider"></div>
      <div class="sidebar-row sidebar-total">
        <span>Order Total</span>
        <span>${formatPrice(total)}</span>
      </div>
    </div>
  `;
}

// Toast notification
function showToast(message, duration = 3000) {
  const existing = document.querySelector('.toast');
  if (existing) existing.remove();

  const toast = document.createElement('div');
  toast.className = 'toast';
  toast.innerHTML = `
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="20" height="20">
      <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
      <polyline points="22 4 12 14.01 9 11.01"/>
    </svg>
    <span>${message}</span>
  `;
  document.body.appendChild(toast);

  requestAnimationFrame(() => {
    toast.classList.add('toast-visible');
  });

  setTimeout(() => {
    toast.classList.remove('toast-visible');
    setTimeout(() => toast.remove(), 400);
  }, duration);
}

// Render a beer card for the product grid
function renderBeerCard(beer, index) {
  const savings = getSavingsPercent(beer.price, beer.discountedPrice);
  const colors = getBeerCanColors(beer.type);
  const delay = (index || 0) * 60;
  return `
    <a href="product.html?id=${beer.id}" class="beer-card animate-in" style="animation-delay: ${delay}ms">
      <div class="beer-image" style="background: ${colors.bg}">
        <span class="beer-type-badge">${beer.type}</span>
        ${savings > 0 ? `<span class="beer-save-badge">-${savings}%</span>` : ''}
        ${beer.image
          ? `<img class="beer-product-img" src="${beer.image}" alt="${beer.name}">`
          : `<div class="beer-can" style="background: ${colors.can}">
              <div class="beer-can-label">${beer.name}</div>
              <div class="beer-can-abv">${beer.abv}% ABV</div>
            </div>`}
      </div>
      <div class="beer-info">
        <h3 class="beer-name">${beer.name}</h3>
        <div class="beer-brewery">${getCountryFlag(beer.country)} ${beer.brewery}</div>
        <div class="beer-meta">${beer.type} &middot; ${beer.volume}</div>
        ${renderStars(beer.rating, beer.reviews)}
        <div class="beer-pricing">
          <span class="beer-price">${formatPrice(beer.discountedPrice)}</span>
          ${savings > 0 ? `<span class="beer-original-price">${formatPrice(beer.price)}</span>` : ''}
        </div>
        <div class="beer-stock ${beer.stock <= 5 ? 'low-stock' : ''}">
          ${beer.stock <= 5 ? `Only ${beer.stock} left!` : 'In Stock'}
        </div>
      </div>
    </a>
  `;
}

// Render a cart item row
function renderCartItemRow(item, beer, editable = true) {
  if (!beer) return '';
  const colors = getBeerCanColors(beer.type);
  return `
    <div class="cart-item" data-beer-id="${beer.id}">
      <div class="cart-item-image" style="background: ${colors.bg}">
        ${beer.image
          ? `<img class="beer-product-img" src="${beer.image}" alt="${beer.name}">`
          : `<div class="beer-can" style="background: ${colors.can}">
              <div class="beer-can-label">${beer.name}</div>
              <div class="beer-can-abv">${beer.abv}%</div>
            </div>`}
      </div>
      <div class="cart-item-details">
        <a href="product.html?id=${beer.id}" class="cart-item-name">${beer.name}</a>
        <div class="cart-item-meta">${beer.type} &middot; ${beer.volume}</div>
        <div class="cart-item-price">${formatPrice(beer.discountedPrice)}</div>
        ${editable ? `
        <div class="cart-item-controls">
          <div class="qty-control">
            <button class="qty-btn" onclick="changeQty(${beer.id}, -1)">-</button>
            <span class="qty-value">${item.quantity}</span>
            <button class="qty-btn" onclick="changeQty(${beer.id}, 1)">+</button>
          </div>
          <button class="cart-remove-btn" onclick="removeFromCart(${beer.id})">Remove</button>
        </div>` : `
        <div class="cart-item-qty">Qty: ${item.quantity}</div>
        `}
      </div>
      <div class="cart-item-subtotal">
        ${formatPrice(beer.discountedPrice * item.quantity)}
      </div>
    </div>
  `;
}

// Initialize page
document.addEventListener('DOMContentLoaded', () => {
  Cart.updateCartIndicator();
});
