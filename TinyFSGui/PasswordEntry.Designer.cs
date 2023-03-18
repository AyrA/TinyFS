namespace TinyFSGui
{
    partial class PasswordEntry
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BtnOk = new Button();
            BtnCancel = new Button();
            TbPassword = new TextBox();
            TbKey = new TextBox();
            RbPassword = new RadioButton();
            RbKey = new RadioButton();
            SuspendLayout();
            // 
            // BtnOk
            // 
            BtnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnOk.Location = new Point(165, 120);
            BtnOk.Name = "BtnOk";
            BtnOk.Size = new Size(75, 23);
            BtnOk.TabIndex = 3;
            BtnOk.Text = "&OK";
            BtnOk.UseVisualStyleBackColor = true;
            BtnOk.Click += BtnOk_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnCancel.Location = new Point(246, 120);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(75, 23);
            BtnCancel.TabIndex = 4;
            BtnCancel.Text = "&Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            // 
            // TbPassword
            // 
            TbPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TbPassword.Location = new Point(12, 35);
            TbPassword.Name = "TbPassword";
            TbPassword.Size = new Size(309, 20);
            TbPassword.TabIndex = 0;
            TbPassword.UseSystemPasswordChar = true;
            // 
            // TbKey
            // 
            TbKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TbKey.Enabled = false;
            TbKey.Location = new Point(12, 84);
            TbKey.Name = "TbKey";
            TbKey.Size = new Size(309, 20);
            TbKey.TabIndex = 2;
            TbKey.UseSystemPasswordChar = true;
            // 
            // RbPassword
            // 
            RbPassword.AutoSize = true;
            RbPassword.Checked = true;
            RbPassword.Location = new Point(12, 12);
            RbPassword.Name = "RbPassword";
            RbPassword.Size = new Size(71, 17);
            RbPassword.TabIndex = 5;
            RbPassword.TabStop = true;
            RbPassword.Text = "Password";
            RbPassword.UseVisualStyleBackColor = true;
            RbPassword.CheckedChanged += RbPassword_CheckedChanged;
            // 
            // RbKey
            // 
            RbKey.AutoSize = true;
            RbKey.Location = new Point(12, 61);
            RbKey.Name = "RbKey";
            RbKey.Size = new Size(112, 17);
            RbKey.TabIndex = 1;
            RbKey.Text = "Raw key (Base64)";
            RbKey.UseVisualStyleBackColor = true;
            // 
            // PasswordEntry
            // 
            AcceptButton = BtnOk;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = BtnCancel;
            ClientSize = new Size(333, 155);
            Controls.Add(RbKey);
            Controls.Add(RbPassword);
            Controls.Add(TbKey);
            Controls.Add(TbPassword);
            Controls.Add(BtnCancel);
            Controls.Add(BtnOk);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PasswordEntry";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Encryption password";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnOk;
        private Button BtnCancel;
        private TextBox TbPassword;
        private TextBox TbKey;
        private RadioButton RbPassword;
        private RadioButton RbKey;
    }
}