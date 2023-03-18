namespace TinyFSGui
{
    partial class AboutWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutWindow));
            TablePanel = new TableLayoutPanel();
            LogoPictureBox = new PictureBox();
            LabelProductName = new Label();
            LabelVersion = new Label();
            LabelCopyright = new Label();
            LabelCompanyName = new Label();
            TextBoxDescription = new TextBox();
            OkButton = new Button();
            TablePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LogoPictureBox).BeginInit();
            SuspendLayout();
            // 
            // TablePanel
            // 
            TablePanel.ColumnCount = 2;
            TablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            TablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67F));
            TablePanel.Controls.Add(LogoPictureBox, 0, 0);
            TablePanel.Controls.Add(LabelProductName, 1, 0);
            TablePanel.Controls.Add(LabelVersion, 1, 1);
            TablePanel.Controls.Add(LabelCopyright, 1, 2);
            TablePanel.Controls.Add(LabelCompanyName, 1, 3);
            TablePanel.Controls.Add(TextBoxDescription, 1, 4);
            TablePanel.Controls.Add(OkButton, 1, 5);
            TablePanel.Dock = DockStyle.Fill;
            TablePanel.Location = new Point(9, 9);
            TablePanel.Name = "TablePanel";
            TablePanel.RowCount = 6;
            TablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            TablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            TablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            TablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            TablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            TablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            TablePanel.Size = new Size(417, 265);
            TablePanel.TabIndex = 0;
            // 
            // LogoPictureBox
            // 
            LogoPictureBox.Dock = DockStyle.Fill;
            LogoPictureBox.Image = (Image)resources.GetObject("LogoPictureBox.Image");
            LogoPictureBox.Location = new Point(3, 3);
            LogoPictureBox.Name = "LogoPictureBox";
            TablePanel.SetRowSpan(LogoPictureBox, 6);
            LogoPictureBox.Size = new Size(131, 259);
            LogoPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            LogoPictureBox.TabIndex = 12;
            LogoPictureBox.TabStop = false;
            // 
            // LabelProductName
            // 
            LabelProductName.Dock = DockStyle.Fill;
            LabelProductName.Location = new Point(143, 0);
            LabelProductName.Margin = new Padding(6, 0, 3, 0);
            LabelProductName.MaximumSize = new Size(0, 17);
            LabelProductName.Name = "LabelProductName";
            LabelProductName.Size = new Size(271, 17);
            LabelProductName.TabIndex = 19;
            LabelProductName.Text = "Product Name";
            LabelProductName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // LabelVersion
            // 
            LabelVersion.Dock = DockStyle.Fill;
            LabelVersion.Location = new Point(143, 26);
            LabelVersion.Margin = new Padding(6, 0, 3, 0);
            LabelVersion.MaximumSize = new Size(0, 17);
            LabelVersion.Name = "LabelVersion";
            LabelVersion.Size = new Size(271, 17);
            LabelVersion.TabIndex = 0;
            LabelVersion.Text = "Version";
            LabelVersion.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // LabelCopyright
            // 
            LabelCopyright.Dock = DockStyle.Fill;
            LabelCopyright.Location = new Point(143, 52);
            LabelCopyright.Margin = new Padding(6, 0, 3, 0);
            LabelCopyright.MaximumSize = new Size(0, 17);
            LabelCopyright.Name = "LabelCopyright";
            LabelCopyright.Size = new Size(271, 17);
            LabelCopyright.TabIndex = 21;
            LabelCopyright.Text = "Copyright";
            LabelCopyright.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // LabelCompanyName
            // 
            LabelCompanyName.Dock = DockStyle.Fill;
            LabelCompanyName.Location = new Point(143, 78);
            LabelCompanyName.Margin = new Padding(6, 0, 3, 0);
            LabelCompanyName.MaximumSize = new Size(0, 17);
            LabelCompanyName.Name = "LabelCompanyName";
            LabelCompanyName.Size = new Size(271, 17);
            LabelCompanyName.TabIndex = 22;
            LabelCompanyName.Text = "Company Name";
            LabelCompanyName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // TextBoxDescription
            // 
            TextBoxDescription.Dock = DockStyle.Fill;
            TextBoxDescription.Location = new Point(143, 107);
            TextBoxDescription.Margin = new Padding(6, 3, 3, 3);
            TextBoxDescription.Multiline = true;
            TextBoxDescription.Name = "TextBoxDescription";
            TextBoxDescription.ReadOnly = true;
            TextBoxDescription.ScrollBars = ScrollBars.Both;
            TextBoxDescription.Size = new Size(271, 126);
            TextBoxDescription.TabIndex = 23;
            TextBoxDescription.TabStop = false;
            TextBoxDescription.Text = "Description";
            // 
            // OkButton
            // 
            OkButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OkButton.DialogResult = DialogResult.Cancel;
            OkButton.Location = new Point(339, 239);
            OkButton.Name = "OkButton";
            OkButton.Size = new Size(75, 23);
            OkButton.TabIndex = 24;
            OkButton.Text = "&OK";
            // 
            // AboutWindow
            // 
            AcceptButton = OkButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(435, 283);
            Controls.Add(TablePanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutWindow";
            Padding = new Padding(9);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "AboutWindow";
            TablePanel.ResumeLayout(false);
            TablePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)LogoPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TablePanel;
        private System.Windows.Forms.PictureBox LogoPictureBox;
        private System.Windows.Forms.Label LabelProductName;
        private System.Windows.Forms.Label LabelVersion;
        private System.Windows.Forms.Label LabelCopyright;
        private System.Windows.Forms.Label LabelCompanyName;
        private System.Windows.Forms.TextBox TextBoxDescription;
        private System.Windows.Forms.Button OkButton;
    }
}
