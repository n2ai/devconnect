export interface User{
    id:string,
    userName:string,
    avatarUrl:string|null 
}

export interface Post{
    id:string,
    content:string,
    language:string|null,
    createdAt:string,
    author:User,
    likesCount:number,
    commentsCount:number
}

export interface Comment{
    id:string,
    content:string,
    createdAt:string
}


export interface FeedResponse{
    posts:Post[],
    page:number,
    limit:number,
    totalPosts:number,
    totalPages:number
}

export interface AuthResponse{
    token:string
}