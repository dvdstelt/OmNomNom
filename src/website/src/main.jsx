import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App.jsx";
import "./index.css";
import {
  createBrowserRouter,
  createRoutesFromElements,
  Route,
  RouterProvider,
} from "react-router-dom";
import Index from "./Index.jsx";
import Product from "./Product.jsx";
import Cart from "./Cart.jsx";
import Address from "./Address.jsx";
import Shipping from "./Shipping.jsx";
import Payment from "./Payment.jsx";
import Summary from "./Summary.jsx";

const router = createBrowserRouter(
  createRoutesFromElements(
    <Route path="/" element={<App />}>
      <Route>
        <Route index element={<Index />} />
        <Route path="product/:productId" element={<Product />} />
        <Route path="cart/:orderId" element={<Cart />} />
        <Route path="/buy/address/:orderId" element={<Address />} />
        <Route path="/buy/shipping/:orderId" element={<Shipping />} />
        <Route path="/buy/payment/:orderId" element={<Payment />} />
        <Route path="/buy/summary/:orderId" element={<Summary />} />
      </Route>
    </Route>
  )
);

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
);
