namespace TinyFSGui
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            OFD = new OpenFileDialog();
            SFD = new SaveFileDialog();
            MainMenu = new MenuStrip();
            FileToolStripMenuItem = new ToolStripMenuItem();
            NewToolStripMenuItem = new ToolStripMenuItem();
            OpenToolStripMenuItem = new ToolStripMenuItem();
            SaveToolStripMenuItem = new ToolStripMenuItem();
            SaveAsToolStripMenuItem = new ToolStripMenuItem();
            ExitToolStripMenuItem = new ToolStripMenuItem();
            EditToolStripMenuItem = new ToolStripMenuItem();
            AddFileToolStripMenuItem = new ToolStripMenuItem();
            SetEncryptionToolStripMenuItem = new ToolStripMenuItem();
            OptimizeCompressionToolStripMenuItem = new ToolStripMenuItem();
            CaseInsensitiveModeToolStripMenuItem = new ToolStripMenuItem();
            HelpToolStripMenuItem = new ToolStripMenuItem();
            OpenWebsiteToolStripMenuItem = new ToolStripMenuItem();
            DocumentationToolStripMenuItem = new ToolStripMenuItem();
            AboutToolStripMenuItem = new ToolStripMenuItem();
            LvContents = new ListView();
            ChFileName = new ColumnHeader();
            ChSize = new ColumnHeader();
            ChAttributes = new ColumnHeader();
            ChPreview = new ColumnHeader();
            CmsList = new ContextMenuStrip(components);
            SaveFileToDiskToolStripMenuItem = new ToolStripMenuItem();
            RenameToolStripMenuItem = new ToolStripMenuItem();
            EnableCompressionToolStripMenuItem = new ToolStripMenuItem();
            DeleteFileToolStripMenuItem = new ToolStripMenuItem();
            MainMenu.SuspendLayout();
            CmsList.SuspendLayout();
            SuspendLayout();
            // 
            // OFD
            // 
            OFD.DefaultExt = "tfs";
            OFD.Filter = "TinyFS files|*.tfs|All files|*.*";
            OFD.Title = "Open TinyFS file";
            // 
            // SFD
            // 
            SFD.DefaultExt = "tfs";
            SFD.Filter = "TinyFS files|*.tfs|All files|*.*";
            SFD.Title = "Save TinyFS file";
            // 
            // MainMenu
            // 
            MainMenu.Items.AddRange(new ToolStripItem[] { FileToolStripMenuItem, EditToolStripMenuItem, HelpToolStripMenuItem });
            MainMenu.Location = new Point(0, 0);
            MainMenu.Name = "MainMenu";
            MainMenu.Size = new Size(624, 24);
            MainMenu.TabIndex = 0;
            MainMenu.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            FileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { NewToolStripMenuItem, OpenToolStripMenuItem, SaveToolStripMenuItem, SaveAsToolStripMenuItem, ExitToolStripMenuItem });
            FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            FileToolStripMenuItem.Size = new Size(35, 20);
            FileToolStripMenuItem.Text = "&File";
            // 
            // NewToolStripMenuItem
            // 
            NewToolStripMenuItem.Name = "NewToolStripMenuItem";
            NewToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            NewToolStripMenuItem.Size = new Size(185, 22);
            NewToolStripMenuItem.Text = "&New";
            NewToolStripMenuItem.Click += NewToolStripMenuItem_Click;
            // 
            // OpenToolStripMenuItem
            // 
            OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            OpenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            OpenToolStripMenuItem.Size = new Size(185, 22);
            OpenToolStripMenuItem.Text = "&Open";
            OpenToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            // 
            // SaveToolStripMenuItem
            // 
            SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            SaveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            SaveToolStripMenuItem.Size = new Size(185, 22);
            SaveToolStripMenuItem.Text = "&Save";
            SaveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            // 
            // SaveAsToolStripMenuItem
            // 
            SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            SaveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            SaveAsToolStripMenuItem.Size = new Size(185, 22);
            SaveAsToolStripMenuItem.Text = "&Save As...";
            SaveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;
            // 
            // ExitToolStripMenuItem
            // 
            ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            ExitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            ExitToolStripMenuItem.Size = new Size(185, 22);
            ExitToolStripMenuItem.Text = "&Exit";
            ExitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            // 
            // EditToolStripMenuItem
            // 
            EditToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { AddFileToolStripMenuItem, SetEncryptionToolStripMenuItem, OptimizeCompressionToolStripMenuItem, CaseInsensitiveModeToolStripMenuItem });
            EditToolStripMenuItem.Enabled = false;
            EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            EditToolStripMenuItem.Size = new Size(37, 20);
            EditToolStripMenuItem.Text = "&Edit";
            // 
            // AddFileToolStripMenuItem
            // 
            AddFileToolStripMenuItem.Name = "AddFileToolStripMenuItem";
            AddFileToolStripMenuItem.ShortcutKeyDisplayString = "Ins";
            AddFileToolStripMenuItem.Size = new Size(180, 22);
            AddFileToolStripMenuItem.Text = "&Add File...";
            AddFileToolStripMenuItem.Click += AddFileToolStripMenuItem_Click;
            // 
            // SetEncryptionToolStripMenuItem
            // 
            SetEncryptionToolStripMenuItem.Name = "SetEncryptionToolStripMenuItem";
            SetEncryptionToolStripMenuItem.Size = new Size(180, 22);
            SetEncryptionToolStripMenuItem.Text = "Set &Encryption";
            SetEncryptionToolStripMenuItem.Click += SetEncryptionToolStripMenuItem_Click;
            // 
            // OptimizeCompressionToolStripMenuItem
            // 
            OptimizeCompressionToolStripMenuItem.Name = "OptimizeCompressionToolStripMenuItem";
            OptimizeCompressionToolStripMenuItem.Size = new Size(180, 22);
            OptimizeCompressionToolStripMenuItem.Text = "Optimize &Compression";
            OptimizeCompressionToolStripMenuItem.Click += OptimizeCompressionToolStripMenuItem_Click;
            // 
            // CaseInsensitiveModeToolStripMenuItem
            // 
            CaseInsensitiveModeToolStripMenuItem.Name = "CaseInsensitiveModeToolStripMenuItem";
            CaseInsensitiveModeToolStripMenuItem.Size = new Size(180, 22);
            CaseInsensitiveModeToolStripMenuItem.Text = "Case &Insensitive mode";
            CaseInsensitiveModeToolStripMenuItem.Click += CaseInsensitiveModeToolStripMenuItem_Click;
            // 
            // HelpToolStripMenuItem
            // 
            HelpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { OpenWebsiteToolStripMenuItem, DocumentationToolStripMenuItem, AboutToolStripMenuItem });
            HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            HelpToolStripMenuItem.Size = new Size(41, 20);
            HelpToolStripMenuItem.Text = "&Help";
            // 
            // OpenWebsiteToolStripMenuItem
            // 
            OpenWebsiteToolStripMenuItem.Name = "OpenWebsiteToolStripMenuItem";
            OpenWebsiteToolStripMenuItem.Size = new Size(180, 22);
            OpenWebsiteToolStripMenuItem.Text = "&Open Website";
            OpenWebsiteToolStripMenuItem.Click += OpenWebsiteToolStripMenuItem_Click;
            // 
            // DocumentationToolStripMenuItem
            // 
            DocumentationToolStripMenuItem.Name = "DocumentationToolStripMenuItem";
            DocumentationToolStripMenuItem.ShortcutKeys = Keys.F1;
            DocumentationToolStripMenuItem.Size = new Size(180, 22);
            DocumentationToolStripMenuItem.Text = "&Documentation";
            DocumentationToolStripMenuItem.Click += DocumentationToolStripMenuItem_Click;
            // 
            // AboutToolStripMenuItem
            // 
            AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            AboutToolStripMenuItem.Size = new Size(180, 22);
            AboutToolStripMenuItem.Text = "&About";
            AboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // LvContents
            // 
            LvContents.AllowDrop = true;
            LvContents.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LvContents.Columns.AddRange(new ColumnHeader[] { ChFileName, ChSize, ChAttributes, ChPreview });
            LvContents.ContextMenuStrip = CmsList;
            LvContents.FullRowSelect = true;
            LvContents.LabelEdit = true;
            LvContents.Location = new Point(12, 39);
            LvContents.Name = "LvContents";
            LvContents.Size = new Size(600, 390);
            LvContents.TabIndex = 1;
            LvContents.UseCompatibleStateImageBehavior = false;
            LvContents.View = View.Details;
            LvContents.AfterLabelEdit += LvContents_AfterLabelEdit;
            LvContents.ItemDrag += LvContents_ItemDrag;
            LvContents.SelectedIndexChanged += LvContents_SelectedIndexChanged;
            LvContents.DragDrop += FileDragDrop;
            LvContents.DragEnter += FileDragEnter;
            LvContents.KeyDown += LvContents_KeyDown;
            // 
            // ChFileName
            // 
            ChFileName.Text = "File name";
            ChFileName.Width = 120;
            // 
            // ChSize
            // 
            ChSize.Text = "Size";
            ChSize.Width = 100;
            // 
            // ChAttributes
            // 
            ChAttributes.Text = "Attributes";
            ChAttributes.Width = 100;
            // 
            // ChPreview
            // 
            ChPreview.Text = "Data preview";
            ChPreview.Width = 200;
            // 
            // CmsList
            // 
            CmsList.Items.AddRange(new ToolStripItem[] { SaveFileToDiskToolStripMenuItem, RenameToolStripMenuItem, EnableCompressionToolStripMenuItem, DeleteFileToolStripMenuItem });
            CmsList.Name = "CmsList";
            CmsList.Size = new Size(184, 92);
            // 
            // SaveFileToDiskToolStripMenuItem
            // 
            SaveFileToDiskToolStripMenuItem.Name = "SaveFileToDiskToolStripMenuItem";
            SaveFileToDiskToolStripMenuItem.ShortcutKeyDisplayString = "Enter";
            SaveFileToDiskToolStripMenuItem.Size = new Size(183, 22);
            SaveFileToDiskToolStripMenuItem.Text = "&Save file to disk";
            SaveFileToDiskToolStripMenuItem.Click += SaveFileToDiskToolStripMenuItem_Click;
            // 
            // RenameToolStripMenuItem
            // 
            RenameToolStripMenuItem.Name = "RenameToolStripMenuItem";
            RenameToolStripMenuItem.ShortcutKeyDisplayString = "F2";
            RenameToolStripMenuItem.Size = new Size(183, 22);
            RenameToolStripMenuItem.Text = "Rename";
            RenameToolStripMenuItem.Click += RenameToolStripMenuItem_Click;
            // 
            // EnableCompressionToolStripMenuItem
            // 
            EnableCompressionToolStripMenuItem.Name = "EnableCompressionToolStripMenuItem";
            EnableCompressionToolStripMenuItem.ShortcutKeyDisplayString = "C";
            EnableCompressionToolStripMenuItem.Size = new Size(183, 22);
            EnableCompressionToolStripMenuItem.Text = "Enable compression";
            EnableCompressionToolStripMenuItem.Click += EnableCompressionToolStripMenuItem_Click;
            // 
            // DeleteFileToolStripMenuItem
            // 
            DeleteFileToolStripMenuItem.Name = "DeleteFileToolStripMenuItem";
            DeleteFileToolStripMenuItem.ShortcutKeyDisplayString = "Del";
            DeleteFileToolStripMenuItem.Size = new Size(183, 22);
            DeleteFileToolStripMenuItem.Text = "Delete";
            DeleteFileToolStripMenuItem.Click += DeleteFileToolStripMenuItem_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(624, 441);
            Controls.Add(LvContents);
            Controls.Add(MainMenu);
            MainMenuStrip = MainMenu;
            Name = "MainWindow";
            Text = "TinyFS Manager";
            FormClosing += MainWindow_FormClosing;
            FormClosed += MainWindow_FormClosed;
            MainMenu.ResumeLayout(false);
            MainMenu.PerformLayout();
            CmsList.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog OFD;
        private SaveFileDialog SFD;
        private MenuStrip MainMenu;
        private ToolStripMenuItem FileToolStripMenuItem;
        private ToolStripMenuItem NewToolStripMenuItem;
        private ToolStripMenuItem OpenToolStripMenuItem;
        private ToolStripMenuItem SaveToolStripMenuItem;
        private ToolStripMenuItem SaveAsToolStripMenuItem;
        private ToolStripMenuItem ExitToolStripMenuItem;
        private ToolStripMenuItem EditToolStripMenuItem;
        private ToolStripMenuItem AddFileToolStripMenuItem;
        private ToolStripMenuItem SetEncryptionToolStripMenuItem;
        private ToolStripMenuItem OptimizeCompressionToolStripMenuItem;
        private ToolStripMenuItem CaseInsensitiveModeToolStripMenuItem;
        private ListView LvContents;
        private ColumnHeader ChFileName;
        private ColumnHeader ChSize;
        private ColumnHeader ChAttributes;
        private ColumnHeader ChPreview;
        private ContextMenuStrip CmsList;
        private ToolStripMenuItem SaveFileToDiskToolStripMenuItem;
        private ToolStripMenuItem RenameToolStripMenuItem;
        private ToolStripMenuItem EnableCompressionToolStripMenuItem;
        private ToolStripMenuItem DeleteFileToolStripMenuItem;
        private ToolStripMenuItem HelpToolStripMenuItem;
        private ToolStripMenuItem OpenWebsiteToolStripMenuItem;
        private ToolStripMenuItem DocumentationToolStripMenuItem;
        private ToolStripMenuItem AboutToolStripMenuItem;
    }
}