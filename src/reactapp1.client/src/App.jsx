import "./App.css";
import { createContext } from "react";
import { Link, Outlet } from "react-router-dom";
import CartIndicator from "./Cart/CartIndicator";
import { useLocalStorage } from "./misc";
import LocationSelect from "./misc/LocationSelect";

import logo from "./assets/omnomnom.png";

export const OrderIdContext = createContext();

function App() {
  const [currentOrderId, setCurrentOrderId] = useLocalStorage("orderId");

  return (
    <OrderIdContext.Provider value={{ currentOrderId, setCurrentOrderId }}>
      <div id="header">
        <div>
          <Link to={"/"}>
            <img id="logo" src={logo} alt="logo" />
          </Link>
          <LocationSelect />
        </div>
        {currentOrderId && <CartIndicator id={currentOrderId} />}
      </div>
      <div id="content">
        <Outlet />
      </div>
    </OrderIdContext.Provider>
  );
}

export default App;
