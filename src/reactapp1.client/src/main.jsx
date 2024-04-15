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
        <Route
          path="product/:productId"
          element={<Product />}
          // loader={contactLoader}
          // action={contactAction}
        />
        <Route
          path="cart/:orderId"
          element={<Cart />}
          // loader={contactLoader}
          // action={editAction}
        />
        <Route
          path="address/:orderId"
          element={<Address />}
          // loader={contactLoader}
          // action={editAction}
        />
        <Route
          path="shipping/:orderId"
          element={<Shipping />}
          // loader={contactLoader}
          // action={editAction}
        />
        <Route
          path="payment/:orderId"
          element={<Payment />}
          // loader={contactLoader}
          // action={editAction}
        />
        <Route
          path="summary/:orderId"
          element={<Summary />}
          // loader={contactLoader}
          // action={editAction}
        />
      </Route>
    </Route>
  )
);

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
);
