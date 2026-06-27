import { create } from 'zustand'

interface AuthState{
    token:string|null,
    username:string|null,
    userId:string|null,
    isAuthenticated:boolean,

    login:(token:string) => void,
    logout:()=>void
}

//Decode JWT Token to get claims
const decodeToken = (token:string) => {
    const payload = token.split('.')[1]
    return JSON.parse(atob(payload))
}

export const useAuthStore = create<AuthState>((set)=>({
    token: localStorage.getItem('token'),
    username: localStorage.getItem('username'),
    userId: localStorage.getItem('userId'),
    isAuthenticated: !!localStorage.getItem('token'),
    
    login:(token:string)=>{
        const decoded = decodeToken(token)
        
        //Save to localstorage
        localStorage.setItem('token', token)
        localStorage.setItem('userId', decoded.userId)
        localStorage.setItem('username', decoded.username)

        set({
            token,
            userId: decoded.userId,
            username: decoded.username,
            isAuthenticated:true
        })
    },

    logout: ()=>{
        localStorage.removeItem('token')
        localStorage.removeItem('userId')
        localStorage.removeItem('username')

        set({
            token:null,
            userId:null,
            username:null,
            isAuthenticated:false
        })
    }
}))