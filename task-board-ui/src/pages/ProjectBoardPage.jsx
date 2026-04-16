import { useCallback, useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import TaskCard from '../components/TaskCard'
import TaskForm from '../components/TaskForm'
import { useApi } from '../hooks/useApi'
import { apiClient } from '../services/apiClient'
import { useAppContext } from '../context/AppContext'

function ProjectBoardPage() {
  const statuses = ['Todo', 'InProgress', 'Review', 'Done']
  const { projectId } = useParams()
  const [project, setProject] = useState(null)
  const [taskPage, setTaskPage] = useState(null)
  const [searchInput, setSearchInput] = useState('')
  const [debouncedSearch, setDebouncedSearch] = useState('')
  const [filters, setFilters] = useState({ status: '', priority: '', sortBy: 'createdAt', sortDir: 'desc', page: 1 })
  const { loading, error, execute } = useApi()
  const { setToast } = useAppContext()

  const loadProject = useCallback(() => {
    execute(() => apiClient.get(`/projects/${projectId}`)).then(setProject).catch(() => {})
  }, [execute, projectId])

  const loadTasks = useCallback(() => {
    const params = { ...filters, pageSize: 10, title: debouncedSearch }
    if (!params.status) delete params.status
    if (!params.priority) delete params.priority
    if (!params.title) delete params.title
    execute(() => apiClient.get(`/projects/${projectId}/tasks`, { params }))
      .then(setTaskPage)
      .catch(() => {})
  }, [debouncedSearch, execute, filters, projectId])

  useEffect(() => {
    loadProject()
  }, [loadProject])

  useEffect(() => {
    loadTasks()
  }, [loadTasks])

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(searchInput.trim())
      setFilters((prev) => ({ ...prev, page: 1 }))
    }, 400)

    return () => clearTimeout(timer)
  }, [searchInput])

  const createTask = (payload) => {
    return execute(() => apiClient.post(`/projects/${projectId}/tasks`, payload))
      .then(() => {
        setToast('Task created.')
        loadTasks()
      })
  }

  return (
    <section>
      <h2>{project?.name || 'Project Board'}</h2>
      {project?.description && <p>{project.description}</p>}
      <TaskForm onSubmit={createTask} submitText="Create" />

      <div className="card filters">
        <input
          value={searchInput}
          onChange={(e) => setSearchInput(e.target.value)}
          placeholder="Search by title (debounced)"
        />
        <select value={filters.status} onChange={(e) => setFilters((p) => ({ ...p, status: e.target.value, page: 1 }))}>
          <option value="">All Status</option>
          <option value="Todo">Todo</option>
          <option value="InProgress">In Progress</option>
          <option value="Review">Review</option>
          <option value="Done">Done</option>
        </select>
        <select value={filters.priority} onChange={(e) => setFilters((p) => ({ ...p, priority: e.target.value, page: 1 }))}>
          <option value="">All Priority</option>
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
          <option value="Critical">Critical</option>
        </select>
        <select value={filters.sortBy} onChange={(e) => setFilters((p) => ({ ...p, sortBy: e.target.value }))}>
          <option value="createdAt">Created</option>
          <option value="dueDate">Due Date</option>
          <option value="priority">Priority</option>
        </select>
        <select value={filters.sortDir} onChange={(e) => setFilters((p) => ({ ...p, sortDir: e.target.value }))}>
          <option value="desc">Descending</option>
          <option value="asc">Ascending</option>
        </select>
      </div>

      {loading && <p>Loading tasks...</p>}
      {error && <p className="error">{error}</p>}

      <div className="kanban-grid">
        {statuses.map((status) => (
          <section key={status} className="kanban-column">
            <h3>{status}</h3>
            {(taskPage?.data || [])
              .filter((task) => task.status === status)
              .map((task) => (
                <TaskCard key={task.id} task={task} />
              ))}
          </section>
        ))}
      </div>

      {taskPage && (
        <div className="pagination">
          <span>
            Showing {taskPage.data?.length || 0} tasks on this page
            {' '}of {taskPage.totalCount} total.
          </span>
          <button disabled={taskPage.page <= 1} onClick={() => setFilters((p) => ({ ...p, page: p.page - 1 }))}>Previous</button>
          <span>Page {taskPage.page} of {taskPage.totalPages || 1}</span>
          <button disabled={taskPage.page >= taskPage.totalPages} onClick={() => setFilters((p) => ({ ...p, page: p.page + 1 }))}>Next</button>
        </div>
      )}
    </section>
  )
}

export default ProjectBoardPage
