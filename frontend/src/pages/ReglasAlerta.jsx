import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getAlertRules, saveAlertRules } from '../services/api.js'

function ReglasAlerta() {
  const { id } = useParams()
  const [original, setOriginal] = useState([])
  const [rules, setRules] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [message, setMessage] = useState(null)

  useEffect(() => {
    let active = true
    getAlertRules(id)
      .then((data) => {
        if (!active) return
        setOriginal(data)
        setRules(data)
      })
      .catch((error) => active && setMessage({ type: 'error', text: error.message }))
      .finally(() => active && setLoading(false))
    return () => { active = false }
  }, [id])

  const update = (tipo, changes) => {
    setMessage(null)
    setRules((current) => current.map((rule) => (rule.tipo === tipo ? { ...rule, ...changes } : rule)))
  }

  const invalid = rules.some((rule) => !(Number(rule.umbral) > 0))

  const save = async (event) => {
    event.preventDefault()
    if (invalid) {
      setMessage({ type: 'error', text: 'Cada umbral debe ser un número mayor que cero.' })
      return
    }

    setSaving(true)
    setMessage(null)
    try {
      const saved = await saveAlertRules(
        id,
        rules.map((rule) => ({ tipo: rule.tipo, umbral: Number(rule.umbral), activa: rule.activa })),
      )
      setOriginal(saved)
      setRules(saved)
      setMessage({ type: 'success', text: 'Reglas guardadas. Rigen desde la siguiente evaluación de alertas.' })
    } catch (error) {
      setMessage({ type: 'error', text: error.message })
    } finally {
      setSaving(false)
    }
  }

  return (
    <>
      <div className="page-heading">
        <div>
          <Link className="back-link" to={`/cursos/${id}/alertas`}>← Volver a alertas</Link>
          <span className="eyebrow">CONFIGURACIÓN DE ALERTAS</span>
          <h1>Reglas de alerta académica</h1>
          <p className="muted">Defina los umbrales con los que se identifican situaciones que requieren revisión.</p>
        </div>
      </div>

      {message && (
        <p className={`course-message course-message--${message.type}`} role={message.type === 'error' ? 'alert' : 'status'}>
          {message.text}
        </p>
      )}

      <form className="content-card" onSubmit={save}>
        <div className="table-scroll">
          <table>
            <thead><tr><th>Regla</th><th>Umbral</th><th>Unidad</th><th>Activa</th></tr></thead>
            <tbody>
              {loading && <tr><td colSpan="4">Cargando reglas…</td></tr>}
              {rules.map((rule) => (
                <tr key={rule.tipo}>
                  <td>{rule.nombre}</td>
                  <td>
                    <input
                      aria-label={`Umbral de ${rule.nombre}`}
                      min="0"
                      onChange={(event) => update(rule.tipo, { umbral: event.target.value })}
                      step="any"
                      type="number"
                      value={rule.umbral}
                    />
                  </td>
                  <td>{rule.unidad}</td>
                  <td>
                    <input
                      aria-label={`Activar ${rule.nombre}`}
                      checked={rule.activa}
                      onChange={(event) => update(rule.tipo, { activa: event.target.checked })}
                      type="checkbox"
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <p className="muted">Los cambios rigen desde la siguiente evaluación de alertas.</p>
        <div className="feature-form__footer">
          <button className="button button--primary" disabled={saving || loading} type="submit">
            {saving ? 'Guardando…' : 'Guardar cambios'}
          </button>
          <button
            className="button button--secondary"
            disabled={saving}
            onClick={() => { setRules(original); setMessage(null) }}
            type="button"
          >
            Cancelar
          </button>
        </div>
      </form>
    </>
  )
}

export default ReglasAlerta
