import { formatReport, runLogicChecks } from '../src/services/logicChecks.js'
import { mkdirSync, writeFileSync } from 'node:fs'

const results = runLogicChecks()
const generatedAt = new Date().toLocaleString('es-EC')
const report = formatReport(results, {
  title: 'Informe de pruebas automáticas de EduMonitor',
  generatedAt,
})
mkdirSync(new URL('../informes/', import.meta.url), { recursive: true })
const file = new URL('../informes/informe-pruebas.txt', import.meta.url)
writeFileSync(file, `${report}\n`, 'utf8')
console.log(report)
console.log(`\nInforme escrito en ${file.pathname}`)
if (results.some((item) => !item.passed)) process.exitCode = 1
