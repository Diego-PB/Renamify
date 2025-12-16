using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Renamify
{
    public partial class Form1 : Form
    {
        private string? _currentFolder;
        private List<RenameOp> _previewOps = new();
        private readonly List<RenameBatchEntry> _lastBatch = new();

        public Form1()
        {
            InitializeComponent();
            BuildGridColumns();

            btnBrowse.Click += (_, _) => BrowseFolder();
            btnScan.Click += (_, _) => ScanFolder();
            btnPreview.Click += (_, _) => PreviewRenames();
            btnApply.Click += (_, _) => ApplyRenames();
            btnUndo.Click += (_, _) => UndoLastBatch();

            grid.CellEndEdit += (_, e) =>
            {
                if (e.RowIndex >= 0) ValidateRow(e.RowIndex);
            };

            btnPreview.Enabled = false;
            btnApply.Enabled = false;
            btnUndo.Enabled = false;

            lblStatus.Text = "Pick a folder, then click Scan.";
        }

        private void BuildGridColumns()
        {
            grid.Columns.Clear();

            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "OldBase",
                HeaderText = "Current name (base)",
                ReadOnly = true,
                FillWeight = 28
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NewBase",
                HeaderText = "New name (base)",
                ReadOnly = false,
                FillWeight = 28
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Count",
                HeaderText = "Files",
                ReadOnly = true,
                FillWeight = 10
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Exts",
                HeaderText = "Suffixes",
                ReadOnly = true,
                FillWeight = 18
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                ReadOnly = true,
                FillWeight = 16
            });
        }

        private void BrowseFolder()
        {
            using var dlg = new FolderBrowserDialog
            {
                Description = "Choose the folder to rename",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false
            };

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _currentFolder = dlg.SelectedPath;
                txtFolder.Text = _currentFolder;

                ScanFolder();
            }
        }
        private static FileItem ParseFile(string fullPath)
        {
            var fileName = Path.GetFileName(fullPath);
            var baseName = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName).ToLowerInvariant(); // ".png"
            return new FileItem(fullPath, baseName, ext);
        }


        private void ScanFolder()
        {
            if (string.IsNullOrWhiteSpace(_currentFolder) || !Directory.Exists(_currentFolder))
            {
                MessageBox.Show(this, "Please choose a valid folder.", "Renamify", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            grid.Rows.Clear();
            txtLog.Clear();
            _previewOps.Clear();
            btnApply.Enabled = false;

            var files = Directory.EnumerateFiles(_currentFolder, "*", SearchOption.TopDirectoryOnly).ToList();
            if (files.Count == 0)
            {
                lblStatus.Text = "No files found in this folder.";
                btnPreview.Enabled = false;
                return;
            }

            var items = files.Select(ParseFile).ToList();

            var groups = items
                .GroupBy(i => i.Base, StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var g in groups)
            {
                var exts = g.Select(x => x.Extension.TrimStart('.'))
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();

                var example = Path.GetFileName(g.First().FullPath);

                int rowIndex = grid.Rows.Add(
                    g.Key,
                    "",
                    g.Count().ToString(),
                    string.Join(" ", exts),
                    "OK"
                );

                grid.Rows[rowIndex].Tag = new GroupInfo(g.Key, g.ToList());
                ValidateRow(rowIndex);
            }

            lblStatus.Text = $"{groups.Count} groups found ({files.Count} files). Fill 'New name' then click Preview.";
            btnPreview.Enabled = true;
        }

        private void ValidateRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return;

            var row = grid.Rows[rowIndex];
            var oldBase = Convert.ToString(row.Cells["OldBase"].Value) ?? "";
            var newBaseRaw = Convert.ToString(row.Cells["NewBase"].Value) ?? "";
            var newBase = newBaseRaw.Trim();

            string status;

            if (string.IsNullOrWhiteSpace(newBase))
                status = "Ignored (empty)";
            else if (newBase.Equals(oldBase, StringComparison.OrdinalIgnoreCase))
                status = "No change";
            else
            {
                var sanitized = SanitizeBaseName(newBase);
                if (sanitized.Length == 0)
                    status = "Invalid name";
                else if (sanitized != newBase)
                    status = $"Invalid characters (try \"{sanitized}\")";
                else
                    status = "Ready";
            }

            row.Cells["Status"].Value = status;
        }

        private void PreviewRenames()
        {
            if (_currentFolder is null) return;

            txtLog.Clear();
            _previewOps.Clear();
            btnApply.Enabled = false;

            var ops = new List<RenameOp>();

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Tag is not GroupInfo info) continue;

                var oldBase = info.BaseName;
                var newBase = (Convert.ToString(row.Cells["NewBase"].Value) ?? "").Trim();

                if (string.IsNullOrWhiteSpace(newBase)) continue;
                if (newBase.Equals(oldBase, StringComparison.OrdinalIgnoreCase)) continue;
                if (SanitizeBaseName(newBase) != newBase)
                {
                    row.Cells["Status"].Value = "Invalid name (fix it)";
                    continue;
                }

                foreach (var item in info.Items)
                {
                    var dst = Path.Combine(_currentFolder, newBase + item.Extension);
                    ops.Add(new RenameOp(item.FullPath, dst));
                }
            }

            if (ops.Count == 0)
            {
                lblStatus.Text = "Nothing to rename (or invalid names).";
                return;
            }

            var conflicts = new List<string>();

            // Conflict: multiple sources to same target
            foreach (var dup in ops.GroupBy(o => o.TargetPath, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1))
                conflicts.Add($"Conflict: multiple sources -> {Path.GetFileName(dup.Key)}");

            // Conflict: target already exists (and is not being moved away by this batch)
            var sourcesSet = new HashSet<string>(ops.Select(o => o.SourcePath), StringComparer.OrdinalIgnoreCase);
            foreach (var op in ops)
            {
                if (File.Exists(op.TargetPath) && !sourcesSet.Contains(op.TargetPath))
                    conflicts.Add($"Already exists: {Path.GetFileName(op.TargetPath)}");
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Preview ({ops.Count} files):");
            sb.AppendLine();

            foreach (var op in ops.OrderBy(o => o.SourcePath, StringComparer.OrdinalIgnoreCase))
                sb.AppendLine($"{Path.GetFileName(op.SourcePath)}  ->  {Path.GetFileName(op.TargetPath)}");

            if (conflicts.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("⚠ Conflicts detected:");
                foreach (var c in conflicts) sb.AppendLine(" - " + c);

                txtLog.Text = sb.ToString();
                lblStatus.Text = "Conflicts detected. Fix names before applying.";
                return;
            }

            txtLog.Text = sb.ToString();
            _previewOps = ops;
            lblStatus.Text = "Looks good. Click Apply.";
            btnApply.Enabled = true;
        }

        private void ApplyRenames()
        {
            if (_previewOps.Count == 0)
            {
                MessageBox.Show(this, "Run Preview first.", "Renamify", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _lastBatch.Clear();
                var phase1 = new List<(string src, string tmp)>();
                var phase2 = new List<(string tmp, string dst)>();

                foreach (var op in _previewOps)
                {
                    if (op.SourcePath.Equals(op.TargetPath, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var dir = Path.GetDirectoryName(op.SourcePath)!;
                    var ext = Path.GetExtension(op.SourcePath); 
                    var tmp = Path.Combine(dir, $".renamify_tmp_{Guid.NewGuid():N}{ext}");

                    phase1.Add((op.SourcePath, tmp));
                    phase2.Add((tmp, op.TargetPath));

                    _lastBatch.Add(new RenameBatchEntry(op.SourcePath, op.TargetPath));
                }

                foreach (var (src, tmp) in phase1) File.Move(src, tmp);
                foreach (var (tmp, dst) in phase2) File.Move(tmp, dst);

                lblStatus.Text = $"Done. Renamed {_lastBatch.Count} file(s).";
                btnUndo.Enabled = _lastBatch.Count > 0;
                btnApply.Enabled = false;

                ScanFolder();
                txtLog.AppendText(Environment.NewLine + Environment.NewLine + "✅ Applied.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Rename error:\n\n" + ex.Message, "Renamify", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _previewOps.Clear();
            }
        }

        private void UndoLastBatch()
        {
            if (_lastBatch.Count == 0) return;

            try
            {
                var phase1 = new List<(string src, string tmp)>();
                var phase2 = new List<(string tmp, string dst)>();

                foreach (var e in _lastBatch)
                {
                    if (!File.Exists(e.NewPath)) continue;

                    var dir = Path.GetDirectoryName(e.NewPath)!;
                    var ext = Path.GetExtension(e.NewPath);
                    var tmp = Path.Combine(dir, $".renamify_undo_{Guid.NewGuid():N}{ext}");

                    phase1.Add((e.NewPath, tmp));
                    phase2.Add((tmp, e.OldPath));
                }

                foreach (var (src, tmp) in phase1) File.Move(src, tmp);
                foreach (var (tmp, dst) in phase2) File.Move(tmp, dst);

                lblStatus.Text = "Undo complete.";
                btnUndo.Enabled = false;
                _lastBatch.Clear();

                ScanFolder();
                txtLog.AppendText(Environment.NewLine + Environment.NewLine + "↩️ Undone.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Undo error:\n\n" + ex.Message, "Renamify", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string SanitizeBaseName(string input)
        { 
            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(input.Length);

            foreach (var ch in input)
                if (!invalid.Contains(ch))
                    sb.Append(ch);

            var s = sb.ToString().Trim();
            while (s.EndsWith('.') || s.EndsWith(' '))
                s = s[..^1];

            return s;
        }

        // --- Records / models ---
        private sealed record FileItem(string FullPath, string Base, string Extension);
        private sealed record GroupInfo(string BaseName, List<FileItem> Items);
        private sealed record RenameOp(string SourcePath, string TargetPath);
        private sealed record RenameBatchEntry(string OldPath, string NewPath);

    }
}
