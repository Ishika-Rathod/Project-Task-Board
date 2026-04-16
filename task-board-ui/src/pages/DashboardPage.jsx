import { useEffect, useState } from 'react'
import { useApi } from '../hooks/useApi'
import { apiClient } from '../services/apiClient'

function DashboardPage() {
  const [data, setData] = useState(null)
  const { loading, error, execute } = useApi()

  useEffect(() => {
    execute(() => apiClient.get('/dashboard')).then(setData).catch(() => {})
  }, [execute])

  return (
    <section>
      <h2>Dashboard</h2>
      {loading && <p>Loading dashboard...</p>}
      {error && <p className="error">{error}</p>}
      {data && (
        <div className="grid two">
          <article className="card full">
            <h3>Tasks Overview</h3>
            <div className="dashboard-summary">
              <div className="dashboard-stat">
                <div className="dashboard-stat-label">Total Tasks</div>
                <div className="dashboard-stat-value">{data.totalTasks}</div>
              </div>
              <div className="dashboard-stat">
                <div className="dashboard-stat-label">Overdue</div>
                <div className="dashboard-stat-value overdue">{data.overdueCount}</div>
              </div>
              <div className="dashboard-stat">
                <div className="dashboard-stat-label">Upcoming (7 days)</div>
                <div className="dashboard-stat-value">{data.dueWithin7Days}</div>
              </div>
            </div>

            <div className="bar-list">
              {['Todo', 'InProgress', 'Review', 'Done'].map((status) => {
                const count = data.tasksByStatus?.[status] || 0
                const total = data.totalTasks || 1
                const percent = Math.round((count / total) * 100)
                return (
                  <div key={status} className="bar-row">
                    <div className="bar-row-label">{status} ({count})</div>
                    <div className="bar-track" aria-hidden="true">
                      <div className="bar-fill" style={{ width: `${percent}%` }} />
                    </div>
                  </div>
                )
              })}
            </div>
          </article>
        </div>
      )}
    </section>
  )
}

export default DashboardPage
