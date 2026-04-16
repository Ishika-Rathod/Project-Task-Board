import { useCallback, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import ProjectForm from '../components/ProjectForm'
import { useApi } from '../hooks/useApi'
import { apiClient } from '../services/apiClient'
import { useAppContext } from '../context/AppContext'

function ProjectListPage() {
  const [projects, setProjects] = useState([])
  const [editing, setEditing] = useState(null)
  const { loading, error, execute } = useApi()
  const { setToast, setSelectedProjectId } = useAppContext()

  const loadProjects = useCallback(() => {
    execute(() => apiClient.get('/projects'))
      .then(setProjects)
      .catch(() => {})
  }, [execute])

  useEffect(() => {
    loadProjects()
  }, [loadProjects])

  const createProject = (payload) => {
    return execute(() => apiClient.post('/projects', payload))
      .then(() => {
        setToast('Project created.')
        loadProjects()
      })
  }

  const updateProject = (payload) => {
    return execute(() => apiClient.put(`/projects/${editing.id}`, payload))
      .then(() => {
        setToast('Project updated.')
        setEditing(null)
        loadProjects()
      })
  }

  const deleteProject = (id) => {
    if (!window.confirm('Delete this project and all tasks/comments?')) return
    execute(() => apiClient.delete(`/projects/${id}`))
      .then(() => {
        setToast('Project deleted.')
        loadProjects()
      })
      .catch(() => {})
  }

  return (
    <section>
      <ProjectForm onSubmit={editing ? updateProject : createProject} initialValue={editing} submitText={editing ? 'Update' : 'Create'} />
      <div>
        <h2>Projects</h2>
        {loading && <p>Loading...</p>}
        {error && <p className="error">{error}</p>}
        {projects.map((project) => (
          <article className="card" key={project.id}>
            <h3>{project.name}</h3>
            <p>{project.description || 'No description'}</p>
            <p>Todo: {project.taskCounts.Todo} | InProgress: {project.taskCounts.InProgress} | Review: {project.taskCounts.Review} | Done: {project.taskCounts.Done}</p>
            <div className="actions">
              <Link className="btn" to={`/projects/${project.id}`} onClick={() => setSelectedProjectId(project.id)}>Open Board</Link>
              <button className="btn" onClick={() => setEditing({ name: project.name, description: project.description, id: project.id })}>Edit</button>
              <button className="btn danger" onClick={() => deleteProject(project.id)}>Delete</button>
            </div>
          </article>
        ))}
      </div>
    </section>
  )
}

export default ProjectListPage
