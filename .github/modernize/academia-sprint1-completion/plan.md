# Modernization Plan: Complete the AcademIA Sprint 1 Scope

**Project**: AcademIA / EduMonitor

## Technical Framework

- **Backend**: .NET 10, ASP.NET Core, Clean Architecture
- **Frontend**: React 19, Vite, JavaScript
- **Database**: SQL Server
- **External system**: Moodle Web Services REST
- **Build tools**: .NET SDK and npm

## Overview

Complete and verify the seven Sprint 1 user stories documented in
`docs/Informe_Sprint1_AcademIA.md`. The repository already contains the
solution structure, Moodle HTTP client, initial persistence and course sync
path, and a React demonstration UI. The remaining work is to finish the
documented access, synchronization, course-listing, and indicator behaviors
and validate them against the report's acceptance criteria. This plan does
not include Sprint 2/3 features, the AI agent, or deployment/infrastructure
work.

## Planning Evidence and Scope

At planning time, repository inspection found:

- The backend has Moodle client and course/grade use-case code, SQL Server
  persistence, and health endpoints.
- The frontend login navigates without authenticating; course/dashboard views
  still contain demonstration behavior and static data.
- No backend test project was found in the inspected source tree.
- Sprint 1 result checkboxes, test outcomes, retrospective answers, and actual
  metrics in the report are still blank or marked `[Completar]`.
- The current Git branch is `main`; local `develop` and `origin/develop` were
  present. The working tree has pre-existing modifications, deletions, and
  untracked files. Planning did not perform any branch operation or change
  application code.
- No `.github/modernize/rulebook/` folder was found.

These are inspection-time observations, not claims that a feature or test has
passed. Recheck them when execution begins.

## Bounded Task Breakdown

Detailed requirements and dependencies are in
`.metadata/tasks.json`.

| ID | Report scope | Planned result |
|---|---|---|
| 001 | Execution prerequisite | Safe feature-branch gate based on `develop`, preserving existing work |
| 002 | HU-01 | Verified login/logout with role-bearing authentication |
| 003 | HU-02 | Server and UI access restricted by role and course |
| 004 | HU-03 | Safe Moodle configuration and connection verification |
| 005 | HU-04 | Idempotent course/enrollment synchronization and history |
| 006 | HU-05 | Grade/activity synchronization with explicit incomplete/missing-data handling |
| 007 | HU-06 | Authorized, searchable and paginated course/student views |
| 008 | HU-07 | Indicators calculated from synchronized data and transparently labeled |
| 009 | Report testing | Evidence-based verification mapped to the 29 criteria and 14 test cases |
| 010 | Security quality gate | Dependency vulnerability assessment and remediation of confirmed findings |

Do not implement features explicitly deferred beyond Sprint 1 (RF-06 through
RF-12 as scoped in the report), and do not replace static demonstration data
with invented academic outcomes.

## Source-Control Safety Requirement

Before any branch operation, inspect `git status --short --branch` and local
branch availability. Preserve all pre-existing changes exactly. Only proceed
if `develop` is available and the existing working-tree changes can be safely
preserved while creating or switching to a feature branch based on `develop`
whose name includes `joselyn-redroban` (for example,
`feature/joselyn-redroban-academia-sprint1`). If either condition cannot be
confirmed, stop and report the blocker; do not clean, overwrite, discard,
commit, or push any pre-existing work. Recheck the state at execution time;
the planning-time branch snapshot is not authorization to proceed.

## Expected Validation

- Build the .NET solution and frontend; run frontend linting.
- Add and run focused backend unit/API tests for each story, using mocked
  Moodle responses and anonymized fixtures where a live service is not
  available.
- Verify authentication failures, 401/403 access cases, teacher/course
  isolation, repeat-sync idempotency, missing grades, and incomplete-sync
  reporting.
- Verify the final course-to-dashboard flow with SQL Server and a test Moodle
  instance when available. Run and record the 14 report test cases and map
  evidence to all 29 acceptance criteria. Mark environment-dependent checks
  blocked rather than passed when no evidence is available.
- Do not infer retrospective statements or actual sprint metrics. Leave
  report fields that require team recollection or uncollected measurements
  incomplete; only record outcomes supported by execution evidence.

## Open Questions & Questionnaire

- [ ] Availability of a Moodle test instance and a usable test token at execution time.
- [ ] Availability of SQL Server Express or the documented local SQL Server environment at execution time.

