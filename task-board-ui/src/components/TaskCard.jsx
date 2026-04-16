import { Link } from 'react-router-dom'
import StatusBadge from './StatusBadge'

function TaskCard({ task }) {
  const dueDate = task.dueDate ? new Date(task.dueDate) : null
  const isOverdue = dueDate && dueDate < new Date() && task.status !== 'Done'

  return (
    <article className="card task-card">
      <div className="task-header">
        <h4>{task.title}</h4>
        <StatusBadge status={task.status} />
      </div>
      <p>{task.description || 'No description.'}</p>
      <div className="task-meta">
        <span className={`priority priority-${task.priority?.toLowerCase()}`}>{task.priority}</span>
        {dueDate && <span className={isOverdue ? 'overdue' : ''}>Due: {dueDate.toLocaleDateString()}</span>}
      </div>
      <Link className="btn task-open" to={`/tasks/${task.id}`}>Open Details</Link>
    </article>
  )
}

export default TaskCard
