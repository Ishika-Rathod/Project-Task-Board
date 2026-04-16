import { useEffect, useState } from 'react'

const emptyTask = {
  title: '',
  description: '',
  priority: 'Medium',
  status: 'Todo',
  dueDate: '',
}

function TaskForm({ onSubmit, initialValue, submitText = 'Save' }) {
  const [form, setForm] = useState(emptyTask)

  useEffect(() => {
    if (initialValue) {
      setForm({
        ...initialValue,
        dueDate: initialValue.dueDate ? initialValue.dueDate.slice(0, 10) : '',
      })
    }
  }, [initialValue])

  const handleChange = (e) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    try {
      await onSubmit({
        ...form,
        dueDate: form.dueDate || null,
      })
      if (!initialValue) setForm(emptyTask)
    } catch (err) {
      // Keep user input if create/update fails.
    }
  }

  return (
    <form className="card form" onSubmit={handleSubmit}>
      <h3>{submitText} Task</h3>
      <input name="title" value={form.title} onChange={handleChange} required maxLength={150} placeholder="Task title" />
      <textarea name="description" value={form.description || ''} onChange={handleChange} maxLength={1000} placeholder="Description" />
      <div className="form-row">
        <select name="status" value={form.status} onChange={handleChange}>
          <option value="Todo">Todo</option>
          <option value="InProgress">In Progress</option>
          <option value="Review">Review</option>
          <option value="Done">Done</option>
        </select>
        <select name="priority" value={form.priority} onChange={handleChange}>
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
          <option value="Critical">Critical</option>
        </select>
        <input type="date" name="dueDate" value={form.dueDate} onChange={handleChange} />
      </div>
      <button type="submit">{submitText}</button>
    </form>
  )
}

export default TaskForm
