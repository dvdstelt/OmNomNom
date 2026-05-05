// Maps a credit-card brand to its icon URL served from /static/img/cards/.
const ICONS = {
  MasterCard: '/img/cards/mastercard.png',
  Visa: '/img/cards/visa.png',
  Discover: '/img/cards/discover.png',
  Diners: '/img/cards/diners.png',
  Amex: '/img/cards/amex.png',
  UnionPay: '/img/cards/unionpay.png'
};

export function cardIcon(cardType) {
  return ICONS[cardType] ?? null;
}
