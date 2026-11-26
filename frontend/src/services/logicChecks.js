import { buildActivityAlerts, scoreOverTen } from './gradeRules.js'
import { approveAndNotify, draftsFromGradebook, mergePlans, saveReview } from './studyPlans.js'

function check(name, passed, detail) {
  return { name, passed: Boolean(passed), detail }
}

function sampleBook() {
  return {
    columns: [
      { itemId: 57, item: 'APE1', escalaMinima: 0, escalaMaxima: 10 },
      { itemId: 58, item: 'APE2', escalaMinima: 0, escalaMaxima: 10 },
    ],
    rows: [
      {
        student: { moodleId: 3, nombre: 'Ana', email: 'ana@example.com' },
        notes: new Map([[57, { nota: 4, itemId: 57 }], [58, { nota: 9, itemId: 58 }]]),
      },
      {
        student: { moodleId: 4, nombre: 'Luis', email: '' },
        notes: new Map([[57, { nota: 4, itemId: 57 }], [58, { nota: 8, itemId: 58 }]]),
      },
    ],
  }
}

export function runLogicChecks() {
  const book = sampleBook()
  const alerts = buildActivityAlerts(book.columns, book.rows, { threshold: 7, states: {} })
  const anaAlert = alerts.find((alert) => alert.estudiante === 'Ana' && alert.actividad === 'APE1')
  const drafts = draftsFromGradebook({
    courseName: 'Algoritmos',
    columns: book.columns,
    rows: book.rows,
    threshold: 7,
  })
  const ana = drafts.find((plan) => plan.estudiante === 'Ana')
  const reviewed = saveReview(drafts, ana.id, 'Plan ajustado por la docente.')
  const refreshed = mergePlans(reviewed.plans, drafts)
  const kept = refreshed.find((plan) => plan.id === ana.id)
  const blocked = approveAndNotify(drafts, 'plan-4')
  const approved = approveAndNotify(reviewed.plans, ana.id, '2026-11-25T22:00:00.000Z')
  const sent = approved.plans.find((plan) => plan.id === ana.id)
  const repeated = approveAndNotify(approved.plans, ana.id)

  return [
    check(
      'Una nota de 4 sobre 10 genera alerta y una de 9 no',
      anaAlert?.nota === 4 && !alerts.some((alert) => alert.actividad === 'APE2'),
      anaAlert ? 'APE1 de Ana quedó en alerta con nota 4. APE2, con 9, no entró.' : 'No se generó la alerta de Ana.',
    ),
    check(
      'Una actividad sin nota no genera alerta',
      scoreOverTen(null, 0, 10) == null && alerts.filter((alert) => alert.estudiante === 'Luis').every((alert) => alert.actividad !== 'APE2'),
      'Sin nota el puntaje queda vacío. Luis solo alerta en APE1, que tiene 4/10.',
    ),
    check(
      'El agente arma el borrador solo para quien está bajo el corte',
      drafts.length === 2 && ana?.texto.includes('APE1') && ana.estado === 'Borrador',
      ana ? 'Ana y Luis tienen borrador por APE1. Una nota de 9 no abre otro plan.' : 'No se armó el borrador de Ana.',
    ),
    check(
      'La revisión de la docente se conserva si el agente vuelve a calcular',
      reviewed.error === '' && kept?.estado === 'En revisión' && kept.texto === 'Plan ajustado por la docente.',
      'El texto ajustado sigue igual después de mezclarlo con un borrador nuevo.',
    ),
    check(
      'Sin visto bueno no hay notificación',
      ana?.estado === 'Borrador' && !ana.destino && !ana.notificadoEn,
      'El borrador no tiene destino ni fecha de aviso.',
    ),
    check(
      'Sin correo de Moodle el visto bueno no notifica',
      blocked.error.includes('correo') && blocked.plans.every((plan) => plan.estado !== 'Notificado'),
      blocked.error || 'El envío no fue rechazado.',
    ),
    check(
      'Con el visto bueno el agente registra el aviso al correo del estudiante',
      sent?.estado === 'Notificado' && sent.destino === 'ana@example.com' && sent.notificadoEn === '2026-11-25T22:00:00.000Z',
      'El aviso de prueba quedó para ana@example.com con la fecha de la aprobación.',
    ),
    check(
      'Un plan ya notificado no se envía otra vez',
      repeated.error.includes('ya fue notificado') && repeated.plans.filter((plan) => plan.estado === 'Notificado').length === 1,
      repeated.error || 'El segundo envío no fue rechazado.',
    ),
  ]
}

export function formatReport(results, { title, generatedAt }) {
  const passed = results.filter((item) => item.passed)
  const failed = results.filter((item) => !item.passed)
  const lines = (items) => (items.length === 0
    ? ['Ninguno.']
    : items.flatMap((item) => [`${item.passed ? 'PASÓ' : 'FALLÓ'} — ${item.name}`, item.detail, '']))
  return [
    title,
    `Generado: ${generatedAt}`,
    `Total: ${results.length}`,
    `Pasaron: ${passed.length}`,
    `Fallaron: ${failed.length}`,
    '',
    'PASARON',
    ...lines(passed),
    'FALLARON',
    ...lines(failed),
  ].join('\n').trim()
}
