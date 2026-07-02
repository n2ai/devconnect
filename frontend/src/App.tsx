import { Routes, Route, Navigate } from "react-router-dom";
import { useAuthStore } from "./store/authStore";
import LoginPage from './pages/LoginPage.tsx';
import RegisterPage from './pages/RegisterPage.tsx';
import HomePage from './pages/HomePage.tsx';

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

