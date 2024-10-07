import { Home } from "./components/Home";
import YourResult from './components/LiftResult';

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
      path: '/LiftResult',
      element: <YourResult />
  }
];

export default AppRoutes;
