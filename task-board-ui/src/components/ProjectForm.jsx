import { useEffect, useState } from 'react'

const initialState = { name: '', description: '' }

function ProjectForm({ onSubmit, initialValue, submitText = 'Save' }) {
  const [form, setForm] = useState(initialState)

  useEffect(() => {
    if (initialValue) setForm(initialValue)
  }, [initialValue])

  const handleChange = (e) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    try {
      await onSubmit(form)
      if (!initialValue) setForm(initialState)
    } catch (err) {
      // Keep user input if create/update fails.
    }
  }

  return (
    <form className="card form" onSubmit={handleSubmit}>
      <h3>{submitText} Project</h3>
      <input
        name="name"
        placeholder="Project name"
        value={form.name}
        onChange={handleChange}
        required
        maxLength={100}
      />
      <textarea
        name="description"
        placeholder="Description"
        value={form.description || ''}
        onChange={handleChange}
        maxLength={300}
      />
      <button type="submit">{submitText}</button>
    </form>
  )
}

export default ProjectForm
