import { useState } from "react";
import LoginPage from "./pages/LoginPage";
import DashboardPage from "./pages/DashboardPage";

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(
    Boolean(localStorage.getItem("token")),
  );

  function handleLogout() {
    localStorage.removeItem("token");
    setIsAuthenticated(false);
  }

  return isAuthenticated ? (
    <DashboardPage onLogout={handleLogout} />
  ) : (
    <LoginPage onLoginSuccess={() => setIsAuthenticated(true)} />
  );
}

export default App;
