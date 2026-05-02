I want to create the HTML (and Javascript) for a demo website that shows the purchase and check-out process similar to how Amazon does it.

Requirements:

- It can be recognizable and similar to Amazon.com, but it has to be unique at the same time so it's not a 100% copy.
- It must be better looking than the Amazon.com checkout process
- Instead of doing all kinds of products, it's about selling craft beers.
  - Types of beers should include IPA, Sour and Stout beers, on which the visitor should be able to sort

Beers should have

- Name
- Type of beer (Stout, Sour, IPA)
- Price & Discounted amount (in dollars)
- Rating (number of stars)
- Number of reviews
- Amount in stock

Shopping cart

- Can have multiple different beers
- Can have multiple ordered per beer (Quantity)
- Every page that isn't part of the official check-out process should show a shopping cart item with number of beers in it

The pages to order should be

1. Initial page full of beers, where we can filter on type of beer
2. Shopping cart overview, where all beers in shopping cart are shown
3. Shipping address and billing address
   1. By default they are the same, which is made visible via a checkbox.
   2. If checkbox is turned off, it's possible to edit a billing address
4. Type of shipping is selected
   1. At least 3 options are available (Standard, Expedited or priority)
   2. Each option has an expected number of days before it's delivered
   3. Each option has a price
5. Payment options
   1. Credit cards can be selected. There should be at least a default credit card available from previous purchases
   2. A new credit card can be added
6. Order summary
   1. All details are presented, but it's a summary, so not every detail has to be shown
   2. Pressing "Place order" button takes us back to the frontpage