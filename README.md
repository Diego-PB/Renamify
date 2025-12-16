# Renamify

Renamify is a small **Windows Forms** app that helps you **batch rename files** inside a folder by editing a simple mapping:

It scans a folder, groups files by their **base name** (filename without extension), lets you edit the new base name, then **previews** and **applies** the renames safely.

---

## How it works

Renamify assumes files follow the standard pattern:

Example:

### Before
- `sea.png`
- `sea.jpg`
- `sea.gif`

### Mapping
- `sea` → `landscape`

### After
- `landscape.png`
- `landscape.jpg`
- `landscape.gif`

---

## Features

- 📂 Folder picker (Explorer)
- 🔎 Auto-scan right after selecting a folder
- 🧾 Groups files by base name
- ✍️ Edit the new base name directly in the grid
- 👀 Preview changes before applying
- ✅ Conflict detection (prevents overwriting / duplicate targets)
- ↩️ Undo the last rename batch

---

## UI flow

1. Click **Browse...** and select a folder
2. Renamify scans aupeopleatically
3. Fill the **New name (base)** column
4. Click **Preview**
5. Click **Apply**
6. If needed: **Undo** (last batch only)

---

## Requirements

- Windows 10/11
- **.NET 8 SDK** (or newer) installed

---
