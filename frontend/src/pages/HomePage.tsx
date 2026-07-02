import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { postsApi, likesApi } from '../services/api'
import { useAuthStore } from '../store/authStore'
import type { FeedResponse, Post } from '../types'

export default function HomePage() {
  const username = useAuthStore((s) => s.username)
  const logout = useAuthStore((s) => s.logout)
  const queryClient = useQueryClient()

  const [content, setContent] = useState('')
  const [language, setLanguage] = useState<string | null>(null)

  // Fetch feed
  const { data, isLoading } = useQuery<FeedResponse>({
    queryKey: ['feed'],
    queryFn: () => postsApi.getFeed().then((res) => res.data),
  })

  // Create post
  const createPost = useMutation({
    mutationFn: () => postsApi.createPost({ content, language }),
    onSuccess: () => {
      setContent('')
      setLanguage(null)
      queryClient.invalidateQueries({ queryKey: ['feed'] })
    },
  })

  // Like post
  const likePost = useMutation({
    mutationFn: (postId: string) => likesApi.likePost(postId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['feed'] }),
  })

  return (
    <div className="min-h-screen bg-gray-950 text-white">
      {/* Header */}
      <div className="border-b border-gray-800 px-4 py-3 flex items-center justify-between max-w-2xl mx-auto">
        <h1 className="text-xl font-bold">
          Dev<span className="text-violet-400">Connect</span>
        </h1>
        <div className="flex items-center gap-4">
          <span className="text-gray-400 text-sm">@{username}</span>
          <button
            onClick={logout}
            className="text-sm text-gray-500 hover:text-red-400 transition-colors"
          >
            Logout
          </button>
        </div>
      </div>

      <div className="max-w-2xl mx-auto">
        {/* Compose */}
        <div className="border-b border-gray-800 p-4">
          <textarea
            value={content}
            onChange={(e) => setContent(e.target.value)}
            placeholder="What are you building?"
            rows={3}
            className="w-full bg-gray-900 border border-gray-700 rounded-lg px-4 py-3 text-white placeholder-gray-500 focus:outline-none focus:border-violet-500 resize-none"
          />
          <div className="flex items-center gap-3 mt-3">
            <select
              value={language ?? ''}
              onChange={(e) => setLanguage(e.target.value || null)}
              className="bg-gray-800 border border-gray-700 rounded-lg px-3 py-1.5 text-sm text-gray-300 focus:outline-none"
            >
              <option value="">No language</option>
              <option value="csharp">C#</option>
              <option value="typescript">TypeScript</option>
              <option value="javascript">JavaScript</option>
              <option value="python">Python</option>
              <option value="sql">SQL</option>
            </select>

            <button
              onClick={() => createPost.mutate()}
              disabled={!content.trim() || createPost.isPending}
              className="ml-auto px-5 py-1.5 bg-violet-600 hover:bg-violet-700 disabled:opacity-50 rounded-full text-sm font-medium transition-colors"
            >
              {createPost.isPending ? 'Posting...' : 'Post'}
            </button>
          </div>
        </div>

        {/* Feed */}
        {isLoading ? (
          <div className="text-center text-gray-500 py-12">Loading feed...</div>
        ) : data?.posts.length === 0 ? (
          <div className="text-center text-gray-500 py-12">No posts yet — be the first!</div>
        ) : (
          data?.posts.map((post: Post) => (
            <PostCard
              key={post.id}
              post={post}
              onLike={() => likePost.mutate(post.id)}
            />
          ))
        )}
      </div>
    </div>
  )
}

function PostCard({ post, onLike }: { post: Post; onLike: () => void }) {
  return (
    <div className="border-b border-gray-800 p-4 hover:bg-gray-900 transition-colors">
      {/* Author */}
      <div className="flex items-center gap-3 mb-3">
        <div className="w-9 h-9 rounded-full bg-violet-800 flex items-center justify-center text-sm font-medium">
          {post.author.userName[0].toUpperCase()}
        </div>
        <div>
          <div className="font-medium text-sm">{post.author.userName}</div>
          <div className="text-gray-500 text-xs">
            {new Date(post.createdAt).toLocaleDateString()}
          </div>
        </div>
      </div>

      {/* Content */}
      {post.language ? (
        <>
          <div className="inline-block text-xs px-2 py-0.5 rounded-full bg-violet-900 text-violet-300 mb-2">
            {post.language}
          </div>
          <pre className="bg-gray-900 border border-gray-700 rounded-lg p-3 text-sm font-mono overflow-x-auto mb-3">
            {post.content}
          </pre>
        </>
      ) : (
        <p className="text-gray-100 leading-relaxed mb-3">{post.content}</p>
      )}

      {/* Actions */}
      <div className="flex gap-6 text-gray-500 text-sm">
        <button
          onClick={onLike}
          className="flex items-center gap-1.5 hover:text-pink-400 transition-colors"
        >
          ♥ {post.likesCount}
        </button>
        <span className="flex items-center gap-1.5">
          💬 {post.commentsCount}
        </span>
      </div>
    </div>
  )
}