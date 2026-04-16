import { Link, NavLink } from 'react-router-dom'
import { useAppContext } from '../context/AppContext'

function Layout({ children }) {
  const { toast, setToast } = useAppContext()

  return (
    <div className="app-shell">
      <header className="header">
        <Link to="/dashboard" className="brand">
          Project Task Board
        </Link>
        <nav className="nav-links">
          <NavLink to="/dashboard">Dashboard</NavLink>
          <NavLink to="/projects">Projects</NavLink>
        </nav>
      </header>

      {toast && (
        <div className="toast" role="status" onClick={() => setToast('')}>
          {toast}
        </div>
      )}

      <main>{children}</main>
    </div>
  )
}

export default Layout
