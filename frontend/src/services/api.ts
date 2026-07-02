import axios from "axios";

const api = axios.create({
    baseURL: 'http://localhost:5073/api'
})

//Auto assign token to all request
api.interceptors.request.use((config)=>{
    const token = localStorage.getItem('token');
    if(token){
        config.headers.Authorization = `Bearer ${token}`
    }

    return config
})

//Auth
export const authApi = {
    register: (data:{username:string; email:string; password:string})=>api.post('/auth/register',data),
    login: (data:{email:string; password:string})=>api.post('/auth/login', data)
}

//Posts
export const postsApi = {
    getFeed:(page = 1, limit = 20) =>
        api.get(`/feed?page=${page}&limit=${limit}`),

    getPost:(id:string) => 
        api.get(`/posts/${id}`),

    createPost:(data: {content:string;language:string|null}) =>
        api.post('/posts',data),
    
    deletePost:(id: string) => 
        api.delete(`/posts/${id}`),
}

// Likes
export const likesApi = {
    likePost:(postId:string) =>
        api.post(`/likes/${postId}`),

    unlikePost:(postId:string) => 
        api.delete(`/likes/${postId}`)
}

//Comments
export const commentsApi = {
  getComments: (postId: string) =>
    api.get(`/comments/${postId}`),

  createComment: (postId: string, content: string) =>
    api.post(`/comments/${postId}`, { content }),

  deleteComment: (commentId: string) =>
    api.delete(`/comments/${commentId}`),
}

// Follow
export const followApi = {
  follow: (userId: string) =>
    api.post(`/follow/${userId}`),

  unfollow: (userId: string) =>
    api.delete(`/follow/${userId}`),

  getFollowers: (userId: string) =>
    api.get(`/follow/${userId}/followers`),
}

export default api