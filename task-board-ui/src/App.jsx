import { Navigate, Route, Routes } from 'react-router-dom'
import { AppProvider } from './context/AppContext'
import Layout from './components/Layout'
import DashboardPage from './pages/DashboardPage'
import ProjectListPage from './pages/ProjectListPage'
import ProjectBoardPage from './pages/ProjectBoardPage'
import TaskDetailPage from './pages/TaskDetailPage'

function App() {
  return (
    <AppProvider>
      <Layout>
        <Routes>
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/projects" element={<ProjectListPage />} />
          <Route path="/projects/:projectId" element={<ProjectBoardPage />} />
          <Route path="/tasks/:taskId" element={<TaskDetailPage />} />
        </Routes>
      </Layout>
    </AppProvider>
  )
}

export default App
