import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import YourResult from './components/LiftResult';

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    element: <FetchData />
    },
  {
      path: '/LiftResult',
      element: <YourResult />
  }
];

export default AppRoutes;
