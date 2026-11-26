function formatDelta(value) {
  if (value == null || !Number.isFinite(Number(value))) return ''
  const number = Number(value)
  const text = Math.abs(number).toFixed(2)
  if (number > 0) return `+${text}`
  if (number < 0) return `−${text}`
  return '0.00'
}

function activityCode(name) {
  const text = String(name ?? 'Actividad')
  const head = text.split(' - ')[0].trim() || text
  return head.length > 18 ? `${head.slice(0, 16)}…` : head
}

function ActivityTrendChart({ points, threshold }) {
  if (points.length === 0) {
    return <p className="muted">No hay actividades calificables para dibujar la tendencia.</p>
  }

  const width = Math.max(680, points.length * 96)
  const height = 300
  const pad = { top: 36, right: 20, bottom: 52, left: 36 }
  const innerWidth = width - pad.left - pad.right
  const innerHeight = height - pad.top - pad.bottom
  const yOf = (value) => pad.top + innerHeight - (Math.min(10, Math.max(0, value)) / 10) * innerHeight
  const xOf = (index) => pad.left + (points.length === 1 ? innerWidth / 2 : (index / (points.length - 1)) * innerWidth)
  const ticks = [0, 2, 4, 6, 8, 10]
  const thresholdValue = Math.min(10, Math.max(0, Number(threshold) || 0))
  const segments = []
  let segment = []
  points.forEach((point, index) => {
    if (point.promedio == null) {
      if (segment.length > 0) segments.push(segment)
      segment = []
      return
    }
    segment.push(`${xOf(index)},${yOf(point.promedio)}`)
  })
  if (segment.length > 0) segments.push(segment)

  const summary = points.map((point) => {
    if (point.promedio == null) return `${point.nombre}: sin calificaciones`
    const sample = point.datosInsuficientes ? ', muestra corta' : ''
    const change = formatDelta(point.delta)
    return `${point.nombre}: ${point.promedio.toFixed(2)} de 10, ${point.numCalificaciones} calificaciones${change ? `, variación ${change}` : ''}${sample}`
  }).join('. ')

  return (
    <figure className="activity-trend" aria-label={summary}>
      <figcaption className="trend-legend">
        <span><i className="trend-swatch trend-swatch--line" /> Promedio del grupo</span>
        <span><i className="trend-swatch trend-swatch--threshold" /> Umbral {thresholdValue.toFixed(2)}/10</span>
        <span><i className="trend-swatch trend-swatch--short" /> Muestra menor al mínimo</span>
      </figcaption>
      <div className="trend-scroll">
        <svg aria-hidden="true" className="activity-trend__svg" style={{ minWidth: points.length > 8 ? width : '100%' }} viewBox={`0 0 ${width} ${height}`}>
          {ticks.map((tick) => (
            <g key={tick}>
              <line className="activity-trend__grid" x1={pad.left} x2={width - pad.right} y1={yOf(tick)} y2={yOf(tick)} />
              <text className="activity-trend__axis" x={pad.left - 8} y={yOf(tick) + 4}>{tick}</text>
            </g>
          ))}
          <line
            className="activity-trend__threshold"
            x1={pad.left}
            x2={width - pad.right}
            y1={yOf(thresholdValue)}
            y2={yOf(thresholdValue)}
          />
          {segments.map((path) => (
            <polyline className="activity-trend__line" key={path.join(' ')} points={path.join(' ')} />
          ))}
          {points.map((point, index) => {
            const x = xOf(index)
            if (point.promedio == null) {
              return (
                <g key={point.key}>
                  <title>{point.nombre}: sin calificaciones</title>
                  <text className="activity-trend__name" textAnchor="middle" x={x} y={height - 28}>{activityCode(point.nombre)}</text>
                  <text className="activity-trend__count" textAnchor="middle" x={x} y={height - 12}>Sin notas</text>
                </g>
              )
            }

            const y = yOf(point.promedio)
            const labelY = point.promedio > 8.2 ? y + 18 : y - 28
            return (
              <g key={point.key}>
                <title>{`${point.nombre}: ${point.promedio.toFixed(2)} / 10, ${point.numCalificaciones} calificaciones`}</title>
                <text className="activity-trend__delta" textAnchor="middle" x={x} y={labelY}>{formatDelta(point.delta)}</text>
                <text className="activity-trend__score" textAnchor="middle" x={x} y={labelY + 13}>{point.promedio.toFixed(2)}</text>
                <circle
                  className={point.datosInsuficientes ? 'activity-trend__point activity-trend__point--short' : 'activity-trend__point'}
                  cx={x}
                  cy={y}
                  r="5.5"
                />
                <text className="activity-trend__name" textAnchor="middle" x={x} y={height - 28}>{activityCode(point.nombre)}</text>
                <text className="activity-trend__count" textAnchor="middle" x={x} y={height - 12}>n={point.numCalificaciones}</text>
              </g>
            )
          })}
        </svg>
      </div>
    </figure>
  )
}

export default ActivityTrendChart
