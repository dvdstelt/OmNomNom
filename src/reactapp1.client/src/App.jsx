import "./App.css";
import { Link, Outlet } from "react-router-dom";

import logo from "./assets/omnomnom.png";

function App() {
  // const [forecasts, setForecasts] = useState();

  // useEffect(() => {
  //     populateWeatherData();
  // }, []);

  // const contents = forecasts === undefined
  //     ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
  //     : <table className="table table-striped" aria-labelledby="tabelLabel">
  //         <thead>
  //             <tr>
  //                 <th>Date</th>
  //                 <th>Temp. (C)</th>
  //                 <th>Temp. (F)</th>
  //                 <th>Summary</th>
  //             </tr>
  //         </thead>
  //         <tbody>
  //             {forecasts.map(forecast =>
  //                 <tr key={forecast.date}>
  //                     <td>{forecast.date}</td>
  //                     <td>{forecast.temperatureC}</td>
  //                     <td>{forecast.temperatureF}</td>
  //                     <td>{forecast.summary}</td>
  //                 </tr>
  //             )}
  //         </tbody>
  //     </table>;

  // return (
  //     <div>
  //         <h1 id="tabelLabel">Weather forecast</h1>
  //         <p>This component demonstrates fetching data from the server.</p>
  //         {contents}
  //     </div>
  // );

  // async function populateWeatherData() {
  //     const response = await fetch('weatherforecast');
  //     const data = await response.json();
  //     setForecasts(data);
  // }

  return (
    <>
      <div id="header">
        <Link to={"/"}>
          <img id="logo" src={logo} alt="logo" />
        </Link>
      </div>
      <div id="content">
        <Outlet />
      </div>
    </>
  );
}

export default App;
