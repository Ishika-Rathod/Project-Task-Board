import { useCallback, useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import TaskForm from '../components/TaskForm'
import { useApi } from '../hooks/useApi'
import { apiClient } from '../services/apiClient'
import { useAppContext } from '../context/AppContext'

function TaskDetailPage() {
  const { taskId } = useParams()
  const navigate = useNavigate()
  const [task, setTask] = useState(null)
  const [comment, setComment] = useState({ author: '', body: '' })
  const { loading, error, execute } = useApi()
  const { setToast } = useAppContext()

  const loadTask = useCallback(() => {
    execute(() => apiClient.get(`/tasks/${taskId}`)).then(setTask).catch(() => {})
  }, [execute, taskId])

  useEffect(() => {
    loadTask()
  }, [loadTask])

  const updateTask = (payload) => {
    return execute(() => apiClient.put(`/tasks/${taskId}`, payload))
      .then(() => {
        setToast('Task updated.')
        loadTask()
      })
  }

  const deleteTask = () => {
    if (!window.confirm('Delete this task and comments?')) return
    execute(() => apiClient.delete(`/tasks/${taskId}`))
      .then(() => {
        setToast('Task deleted.')
        navigate(task ? `/projects/${task.projectId}` : '/projects')
      })
      .catch(() => {})
  }

  const addComment = (e) => {
    e.preventDefault()
    execute(() => apiClient.post(`/tasks/${taskId}/comments`, comment))
      .then(() => {
        setToast('Comment added.')
        setComment({ author: '', body: '' })
        loadTask()
      })
      .catch(() => {})
  }

  const deleteComment = (id) => {
    if (!window.confirm('Delete this comment?')) return
    execute(() => apiClient.delete(`/comments/${id}`))
      .then(() => {
        setToast('Comment deleted.')
        loadTask()
      })
      .catch(() => {})
  }

  return (
    <section>
      <h2>Task Details</h2>
      {loading && <p>Loading task...</p>}
      {error && <p className="error">{error}</p>}
      {task && (
        <>
          <div style={{ marginBottom: '0.8rem', display: 'flex', gap: '0.6rem' }}>
            <Link className="btn" to={`/projects/${task.projectId}`}>Back to Project</Link>
            <button className="btn danger" onClick={deleteTask}>Delete Task</button>
          </div>
          <TaskForm onSubmit={updateTask} initialValue={task} submitText="Update" />
          <article className="card">
            <h3>Comments</h3>
            <form onSubmit={addComment} className="form">
              <input
                name="author"
                value={comment.author}
                placeholder="Author"
                maxLength={50}
                onChange={(e) => setComment((prev) => ({ ...prev, author: e.target.value }))}
                required
              />
              <textarea
                name="body"
                value={comment.body}
                placeholder="Comment text"
                maxLength={500}
                onChange={(e) => setComment((prev) => ({ ...prev, body: e.target.value }))}
                required
              />
              <button type="submit">Add Comment</button>
            </form>
            {task.comments.map((c) => (
              <div key={c.id} className="comment">
                <strong>{c.author}</strong>
                <p>{c.body}</p>
                <button className="btn danger" onClick={() => deleteComment(c.id)}>Delete</button>
              </div>
            ))}
          </article>
          <article className="card">
            <h3>Status History</h3>
            {task.history?.length ? (
              task.history.map((h) => (
                <div key={h.id} className="comment">
                  <strong>{h.oldStatus} {'->'} {h.newStatus}</strong>
                  <p>{new Date(h.changedAt).toLocaleString()}</p>
                </div>
              ))
            ) : (
              <p>No status changes yet.</p>
            )}
          </article>
        </>
      )}
    </section>
  )
}

export default TaskDetailPage
