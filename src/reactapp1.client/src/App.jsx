import "./App.css";
import { Link, Outlet } from "react-router-dom";
import CartIndicator from "./Cart/CartIndicator";
import { useLocalStorage } from "./misc";

import logo from "./assets/omnomnom.png";

function App() {
  const [currentOrderId, _] = useLocalStorage("orderId");

  return (
    <>
      <div id="header">
        <Link to={"/"}>
          <img id="logo" src={logo} alt="logo" />
        </Link>
        {currentOrderId && <CartIndicator id={currentOrderId} />}
      </div>
      <div id="content">
        <Outlet />
      </div>
    </>
  );
}

export default App;
