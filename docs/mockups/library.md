# Library start screen

Goals:
- Show folders/files added to the library with cover thumbnails
- Visual indicator for reading status: Unread, In Progress (with progress ring), Completed
- Quick actions: Open, Resume, Remove from library, Reveal in Explorer

Layout (desktop):
- Header: App title, search bar, filters (All, Unread, In Progress, Completed), sort (Title, Recently Added, Progress)
- Content: Responsive grid of cards
  - Card contents:
    - Thumbnail (cover) with aspect-fit
    - Overlay badges:
      - Reading status: dot (green = completed, blue = in progress with %), hollow grey = unread
      - Pages read / total (e.g., 45/210)
      - Last opened date tooltip
    - Title (single line, ellipsized)
    - Series • Volume/Issue (secondary)
    - Location icon when the item represents a folder; file icon for single archive
    - Context menu (right-click / … button)

Empty state:
- Large callout with “Add folders” button
- Drag-and-drop hint

Mobile variation:
- Same information density, 2-column grid
- Pull-down to refresh scans

Keyboard shortcuts:
- Enter/Space: Open/Resume
- Del: Remove
- Ctrl+F: Search
- 1/2/3: Filter tabs (All/Unread/In Progress/Completed)

Visual spec (ASCII wireframe):

```
┌───────────────────────────────────────────────────────────────────────┐
│ Panelverse                             [ Search… ]  Filter  Sort  ☰  │
├───────────────────────────────────────────────────────────────────────┤
│ ┌───────────────┐ ┌───────────────┐ ┌───────────────┐ ┌───────────────┐ │
│ │   THUMBNAIL   │ │   THUMBNAIL   │ │   THUMBNAIL   │ │   THUMBNAIL   │ │
│ │       ▓▓▓▓▓▓▓ │ │       ▓▓▓▓▓▓▓ │ │       ▓▓▓▓▓▓▓ │ │       ▓▓▓▓▓▓▓ │ │
│ │   • 45/210    │ │   ◌ 0/180     │ │   ✓ 210/210   │ │   • 12/300    │ │
│ └───────────────┘ └───────────────┘ └───────────────┘ └───────────────┘ │
│ Title long…         Title              Series • Vol 1       Title        │
│ Series • Vol 12     Series • Vol 5     Folder 📁             File 📄      │
│ ⋯                   ⋯                  ⋯                    ⋯           │
└───────────────────────────────────────────────────────────────────────┘
```

Legend:
- Status dot:
  - • Blue = In Progress (show percentage as tooltip and on overlay text)
  - ◌ Grey = Unread
  - ✓ Green = Completed
- For in-progress, show a thin progress ring overlay around the thumbnail corner badge.

Notes:
- Thumbnails are generated on scan; missing covers fallback to first page preview.
- Folder cards aggregate child progress (e.g., sum of pages read / total across contained books).
- Right-click actions: Open/Resume, Continue from…, Mark as unread/completed, Remove from library, Reveal in Explorer/Finder.


