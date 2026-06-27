import { Routes, Route, Navigate } from "react-router-dom";
import { useAuthStore } from "./store/authStore";
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import HomePage from './pages/homePage';

function App(){
  const {isAuthenticated} = useAuthStore()

  return(
    <Routes>
      <Route path="/login" element={<LoginPage/>}></Route>
      <Route path="/register" element={<RegisterPage/>}></Route>
      <Route
        path="/"
        element={isAuthenticated ? <HomePage/> : <Navigate to="/login" />}/>
    </Routes>
  )
}

export default App

