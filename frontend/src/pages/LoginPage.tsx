import { useState } from "react";
import { useNavigate, Link } from 'react-router-dom';
import { authApi } from "../services/api";
import { useAuthStore } from "../store/authStore";

export default function LoginPage(){
    const navigate = useNavigate();
    const login = useAuthStore((s)=>s.login);

}