# TASK-0128 - DemoMvc: Sidebar Drawer + Independent Scroll

## Summary
Make the DemoMvc sidebar behave like a drawer (Azure DevOps style) with its own scroll, independent from the main content.

## Context
Users requested a collapsible sidebar that can be hidden and does not interfere with the main content scroll.

## Objective
- Add a toggle to open/close the sidebar.
- Keep sidebar scroll independent.
- Preserve state on desktop.

## Scope
- DemoMvc layout, sidebar styles, and JS only.

## Out of Scope
- Portal/Portaria layouts.
- API changes.

## Acceptance Criteria
- Sidebar can be collapsed/expanded with a toggle button.
- Sidebar content scrolls independently from the main page.
- Desktop remembers collapsed state.
- Mobile opens as overlay/drawer without pushing content.

## How to Test
- UI: Toggle menu and confirm content shifts on desktop.
- UI: Scroll sidebar items without moving main content.
- UI: On small screens, sidebar overlays and can be toggled.

## Status
DONE

## Implementation Notes
- Added a toggle in the topbar to collapse/expand the sidebar.
- Sidebar now uses its own scroll with fixed height.
- Desktop state persists via localStorage; mobile behaves as overlay.
