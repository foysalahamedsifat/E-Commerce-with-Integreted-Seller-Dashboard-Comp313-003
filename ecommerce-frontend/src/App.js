import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Login from "./Login";
import Register from "./Register";
import PrivateRoute from "./PrivateRoute";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/dashboard" element={<PrivateRoute><h1>Dashboard</h1></PrivateRoute>} />
        <Route path="/" element={<h1>Welcome to Shopify-Style Store</h1>} />
      </Routes>
    </Router>
  );
}

export default App;
