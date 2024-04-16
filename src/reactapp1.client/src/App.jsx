import "./App.css";
import { Link, Outlet } from "react-router-dom";
import CartIndicator from "./Cart/CartIndicator";

import logo from "./assets/omnomnom.png";

function App() {
  //TODO how to get current cartId for user?
  const currentCartId = "526f0a1d-2900-49ba-9d70-987e9f590b04";
  return (
    <>
      <div id="header">
        <Link to={"/"}>
          <img id="logo" src={logo} alt="logo" />
        </Link>
        {currentCartId && <CartIndicator id={currentCartId} />}
      </div>
      <div id="content">
        <Outlet />
      </div>
    </>
  );
}

export default App;
