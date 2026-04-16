import { createContext, useContext, useEffect, useMemo, useState } from 'react'

const AppContext = createContext(null)

export function AppProvider({ children }) {
  const [selectedProjectId, setSelectedProjectId] = useState(null)
  const [toast, setToast] = useState('')
  const autoHideMs = 2500

  useEffect(() => {
    if (!toast) return

    const timer = setTimeout(() => {
      setToast('')
    }, autoHideMs)

    return () => clearTimeout(timer)
  }, [toast])

  const value = useMemo(
    () => ({
      selectedProjectId,
      setSelectedProjectId,
      toast,
      setToast,
    }),
    [selectedProjectId, toast],
  )

  return <AppContext.Provider value={value}>{children}</AppContext.Provider>
}

export function useAppContext() {
  const context = useContext(AppContext)
  if (!context) {
    throw new Error('useAppContext must be used inside AppProvider')
  }
  return context
}
