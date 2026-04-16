import { useCallback, useState } from 'react'

function parseApiError(err) {
  const data = err?.response?.data

  if (!data) {
    return err?.message || 'Something went wrong.'
  }

  if (typeof data === 'string') {
    return data
  }

  // Handle validation-style responses: { errors: { field: ["message"] } }
  if (data.errors && typeof data.errors === 'object') {
    const fieldMessages = Object.entries(data.errors)
      .flatMap(([field, messages]) => {
        if (Array.isArray(messages)) {
          return messages.map((msg) => `${field}: ${msg}`)
        }
        return [`${field}: ${String(messages)}`]
      })

    if (fieldMessages.length > 0) {
      return fieldMessages.join(' | ')
    }
  }

  return data.message || data.title || err?.message || 'Something went wrong.'
}

export function useApi() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)

  const execute = useCallback(async (requestFn) => {
    setLoading(true)
    setError(null)
    try {
      const response = await requestFn()
      return response.data
    } catch (err) {
      const message = parseApiError(err)
      setError(message)
      throw err
    } finally {
      setLoading(false)
    }
  }, [])

  return { loading, error, execute }
}
