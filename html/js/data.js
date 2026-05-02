// Beer product data for OmNomNom craft beer store
const BEERS = [
  // IPAs
  {
    id: 1,
    name: "Hop Odyssey",
    type: "IPA",
    brewery: "Pacific Crest Brewing",
    country: "United States",
    price: 18.99,
    discountedPrice: 14.99,
    rating: 4.5,
    reviews: 342,
    stock: 24,
    description: "A bold West Coast IPA that takes your palate on an epic journey through layers of tropical fruit and resinous pine.",
    tastingNotes: "Hints of grapefruit and mango up front, followed by a piney backbone and a dry, bitter finish that lingers pleasantly.",
    abv: 7.2,
    volume: "16 oz"
  },
  {
    id: 2,
    name: "Citrus Supernova",
    type: "IPA",
    brewery: "Nova Hop Works",
    country: "United States",
    price: 16.99,
    discountedPrice: 16.99,
    image: "../src/website/public/products/susan.png",
    rating: 4.7,
    reviews: 518,
    stock: 36,
    description: "An explosion of citrus in every sip. This hazy New England IPA is juicy, soft, and dangerously drinkable.",
    tastingNotes: "Bursting with orange zest, tangerine, and a touch of peach. Pillowy mouthfeel with low bitterness and a creamy finish.",
    abv: 6.8,
    volume: "16 oz"
  },
  {
    id: 3,
    name: "Pine & Punishment",
    type: "IPA",
    brewery: "Ironwood Ales",
    country: "United States",
    price: 19.99,
    discountedPrice: 15.99,
    rating: 4.3,
    reviews: 189,
    stock: 12,
    description: "A punishingly hoppy double IPA that rewards the brave with complex layers of flavor and a warming alcohol presence.",
    tastingNotes: "Intense pine resin and dank herbal notes layered over a caramel malt sweetness. Finishes with a lingering grapefruit bitterness.",
    abv: 8.5,
    volume: "12 oz"
  },
  {
    id: 4,
    name: "Mosaic Dream",
    type: "IPA",
    brewery: "Dreamstate Brewing",
    country: "United States",
    price: 17.49,
    discountedPrice: 17.49,
    rating: 4.6,
    reviews: 275,
    stock: 18,
    description: "Single-hopped with Mosaic for a kaleidoscope of tropical flavors. This IPA is a hop lover's daydream.",
    tastingNotes: "Lush blueberry, papaya, and floral aromatics. Medium body with a clean, refreshing bitterness that invites another sip.",
    abv: 6.5,
    volume: "16 oz"
  },

  // Sours
  {
    id: 5,
    name: "Pucker Up",
    type: "Sour",
    brewery: "Brouwerij 3 Fonteinen",
    country: "Belgium",
    price: 15.99,
    discountedPrice: 12.99,
    image: "../src/website/public/products/oudegeuze.png",
    rating: 4.4,
    reviews: 412,
    stock: 30,
    description: "A lip-puckering raspberry Berliner Weisse that balances tart intensity with just enough sweetness to keep you coming back.",
    tastingNotes: "Bright raspberry tartness with hints of lemon curd and a refreshing, effervescent finish. Light and sessionable.",
    abv: 4.5,
    volume: "12 oz"
  },
  {
    id: 6,
    name: "Sour Hour",
    type: "Sour",
    brewery: "Tidal Creek Brewing",
    country: "United States",
    price: 14.99,
    discountedPrice: 14.99,
    rating: 4.2,
    reviews: 203,
    stock: 42,
    description: "A kettle-soured gose with sea salt and coriander that tastes like a summer afternoon at the beach.",
    tastingNotes: "Gentle salinity meets bright lime and coriander spice. Crisp, tart, and incredibly thirst-quenching with a clean finish.",
    abv: 4.2,
    volume: "16 oz"
  },
  {
    id: 7,
    name: "Barcode Copper & Wool",
    type: "Stout",
    subtype: "Stout - Imperial / Double Coffee",
    brewery: "Moersleutel Craft Brewery",
    country: "The Netherlands",
    price: 18.49,
    discountedPrice: 15.49,
    image: "../src/website/public/products/moersleutel-copper-wool.png",
    untappd: "https://untappd.com/b/moersleutel-craft-brewery-8720615261970-barcode-copper-and-wool/5327045",
    rating: 4.34,
    reviews: 5797,
    stock: 8,
    description: "This imperial stout is aged to perfection in Willet, Ruby Port, and Carcavelos barrels. Infused with the finest Colombian coffee from Coffee Collective to enhance and accentuate the fruity flavors of the fortified wines. Take a dive into the art that is brewing. Best served at a temperature of 12\u00B0C.",
    tastingNotes: "Rich barrel-aged complexity with Willet bourbon warmth, Ruby Port sweetness, and Colombian coffee depth. Fruity fortified wine notes intertwine with roasted malt and dark chocolate.",
    abv: 14.0,
    volume: "12 oz"
  },
  {
    id: 8,
    name: "Tropical Tango",
    type: "Sour",
    brewery: "Sunset Sour Co.",
    country: "United States",
    price: 16.49,
    discountedPrice: 16.49,
    rating: 4.5,
    reviews: 328,
    stock: 22,
    description: "A fruited sour ale brewed with passion fruit, guava, and a hint of vanilla. It's a tropical dance party in a can.",
    tastingNotes: "Vibrant passion fruit and guava tartness balanced by a subtle vanilla creaminess. Smooth, fruity, and endlessly refreshing.",
    abv: 5.0,
    volume: "16 oz"
  },

  // Stouts
  {
    id: 9,
    name: "Midnight Velvet",
    type: "Stout",
    brewery: "Blackforge Brewing",
    country: "United States",
    price: 20.99,
    discountedPrice: 16.99,
    image: "../src/website/public/products/abraxas.png",
    rating: 4.9,
    reviews: 891,
    stock: 15,
    description: "An imperial stout as dark as midnight and as smooth as velvet. Aged in bourbon barrels for 12 months.",
    tastingNotes: "Rich dark chocolate and espresso meet bourbon warmth and vanilla oak. Decadently thick with a long, warming finish.",
    abv: 11.5,
    volume: "12 oz"
  },
  {
    id: 10,
    name: "Oatmeal Thunder",
    type: "Stout",
    brewery: "Thunderhead Brewing",
    country: "United States",
    price: 15.99,
    discountedPrice: 15.99,
    rating: 4.4,
    reviews: 256,
    stock: 28,
    description: "A silky oatmeal stout with a thunderous roasted character. Comfort in a glass for cold evenings.",
    tastingNotes: "Smooth oat creaminess with roasted coffee, milk chocolate, and a hint of brown sugar. Medium body with a soft, satisfying finish.",
    abv: 5.8,
    volume: "16 oz"
  },
  {
    id: 11,
    name: "Espresso Yourself",
    type: "Stout",
    brewery: "Dark Roast Brewing",
    country: "United States",
    price: 17.99,
    discountedPrice: 17.99,
    rating: 4.6,
    reviews: 445,
    stock: 20,
    description: "Brewed with locally roasted espresso beans, this coffee stout blurs the line between your morning cup and your evening pint.",
    tastingNotes: "Bold espresso and dark roast coffee dominate, with undertones of bittersweet chocolate and toasted marshmallow. Crisp and dry.",
    abv: 6.2,
    volume: "16 oz"
  },
  {
    id: 12,
    name: "S'mores Galore",
    type: "Stout",
    brewery: "Campfire Brewing Co.",
    country: "United States",
    price: 22.99,
    discountedPrice: 18.99,
    image: "../src/website/public/products/bourboncounty.png",
    rating: 4.7,
    reviews: 573,
    stock: 10,
    description: "A pastry stout inspired by campfire nostalgia. Brewed with graham cracker, chocolate, and marshmallow.",
    tastingNotes: "Graham cracker sweetness meets rich milk chocolate and toasted marshmallow. Full-bodied, dessert-like, with a smooth, lingering sweetness.",
    abv: 10.0,
    volume: "12 oz"
  }
];

// Get a beer by ID
function getBeerById(id) {
  return BEERS.find(b => b.id === parseInt(id));
}

// Get beers by type
function getBeersByType(type) {
  if (!type || type === 'All') return BEERS;
  return BEERS.filter(b => b.type === type);
}

// Get related beers (same type, excluding given ID)
function getRelatedBeers(id, limit = 3) {
  const beer = getBeerById(id);
  if (!beer) return [];
  return BEERS.filter(b => b.type === beer.type && b.id !== beer.id).slice(0, limit);
}

// Generate a color for beer "image" placeholder based on type
function getBeerColor(type) {
  switch (type) {
    case 'IPA': return 'linear-gradient(135deg, #f6a623 0%, #e8891c 50%, #d4740e 100%)';
    case 'Sour': return 'linear-gradient(135deg, #e84393 0%, #d63384 50%, #c2185b 100%)';
    case 'Stout': return 'linear-gradient(135deg, #2d1b0e 0%, #3e2723 50%, #1a1a2e 100%)';
    default: return 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)';
  }
}

// Generate star rating HTML
function getStarRatingHTML(rating) {
  let html = '<div class="star-rating">';
  for (let i = 1; i <= 5; i++) {
    if (i <= Math.floor(rating)) {
      html += '<span class="star filled">&#9733;</span>';
    } else if (i - rating < 1 && i - rating > 0) {
      html += '<span class="star half">&#9733;</span>';
    } else {
      html += '<span class="star">&#9733;</span>';
    }
  }
  html += `<span class="rating-text">${rating} (${0} reviews)</span>`;
  html += '</div>';
  return html;
}

// Render star rating with review count and optional untappd link
function renderStars(rating, reviews, untappd) {
  let html = '<div class="star-rating">';
  for (let i = 1; i <= 5; i++) {
    if (i <= Math.floor(rating)) {
      html += '<span class="star filled">&#9733;</span>';
    } else if (i - rating < 1 && i - rating > 0) {
      html += '<span class="star half">&#9733;</span>';
    } else {
      html += '<span class="star empty">&#9733;</span>';
    }
  }
  const label = untappd ? 'check-ins' : 'reviews';
  html += `<span class="rating-text">${rating} (${reviews.toLocaleString()} ${label})</span>`;
  if (untappd) {
    html += `<a href="${untappd}" target="_blank" rel="noopener" class="untappd-link" title="View on Untappd" onclick="event.stopPropagation()">
      <img class="untappd-icon" src="img/untappd.svg" alt="Untappd">
    </a>`;
  }
  html += '</div>';
  return html;
}

// Country flag as inline SVG image (flag CDN)
function getCountryFlag(country) {
  const codes = {
    'United States': 'us',
    'Belgium': 'be',
    'The Netherlands': 'nl'
  };
  const code = codes[country];
  if (!code) return '';
  return `<img class="country-flag" src="https://flagcdn.com/w40/${code}.png" srcset="https://flagcdn.com/w80/${code}.png 2x" alt="${country}" width="20" height="15">`;
}

// Get unique breweries from beer data
function getBreweries() {
  return [...new Set(BEERS.map(b => b.brewery))].sort();
}

// Get unique countries from beer data
function getCountries() {
  return [...new Set(BEERS.map(b => b.country))].sort();
}

// Format price
function formatPrice(price) {
  return '$' + price.toFixed(2);
}

// Calculate savings percentage
function getSavingsPercent(original, discounted) {
  return Math.round(((original - discounted) / original) * 100);
}
