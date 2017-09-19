namespace TokenManager.dialog
{
    partial class UpdateProfile
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateProfile));
            this.header = new System.Windows.Forms.Panel();
            this.title = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.bunifuImageButton2 = new Bunifu.Framework.UI.BunifuImageButton();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.bunifuDragControl1 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.cancelBtn = new Bunifu.Framework.UI.BunifuFlatButton();
            this.okBtn = new Bunifu.Framework.UI.BunifuFlatButton();
            this.bunifuCustomLabel2 = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.bunifuCustomLabel3 = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.EmailError = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.PhoneError = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.txt = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.PinError = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.EmailTextBox = new Bunifu.Framework.UI.BunifuMetroTextbox();
            this.PhoneTextBox = new Bunifu.Framework.UI.BunifuMetroTextbox();
            this.PinTxt = new Bunifu.Framework.UI.BunifuMetroTextbox();
            this.bunifuDragControl2 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // header
            // 
            this.header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(143)))), ((int)(((byte)(204)))));
            this.header.Controls.Add(this.title);
            this.header.Controls.Add(this.bunifuImageButton2);
            this.header.Controls.Add(this.bunifuImageButton1);
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Location = new System.Drawing.Point(0, 0);
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(397, 45);
            this.header.TabIndex = 0;
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.ForeColor = System.Drawing.Color.White;
            this.title.Location = new System.Drawing.Point(41, 13);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(183, 18);
            this.title.TabIndex = 2;
            this.title.Text = "Cập nhật thông tin cá nhân";
            // 
            // bunifuImageButton2
            // 
            this.bunifuImageButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bunifuImageButton2.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bunifuImageButton2.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton2.Image")));
            this.bunifuImageButton2.ImageActive = null;
            this.bunifuImageButton2.Location = new System.Drawing.Point(0, 0);
            this.bunifuImageButton2.Name = "bunifuImageButton2";
            this.bunifuImageButton2.Size = new System.Drawing.Size(45, 45);
            this.bunifuImageButton2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.bunifuImageButton2.TabIndex = 1;
            this.bunifuImageButton2.TabStop = false;
            this.bunifuImageButton2.Zoom = 0;
            this.bunifuImageButton2.Click += new System.EventHandler(this.bunifuImageButton1_Click);
            // 
            // bunifuImageButton1
            // 
            this.bunifuImageButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bunifuImageButton1.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bunifuImageButton1.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.Image")));
            this.bunifuImageButton1.ImageActive = null;
            this.bunifuImageButton1.Location = new System.Drawing.Point(352, 0);
            this.bunifuImageButton1.Name = "bunifuImageButton1";
            this.bunifuImageButton1.Size = new System.Drawing.Size(45, 45);
            this.bunifuImageButton1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.bunifuImageButton1.TabIndex = 1;
            this.bunifuImageButton1.TabStop = false;
            this.bunifuImageButton1.Zoom = 0;
            this.bunifuImageButton1.Click += new System.EventHandler(this.bunifuImageButton1_Click);
            this.bunifuImageButton1.MouseEnter += new System.EventHandler(this.bunifuImageButton1_MouseEnter);
            this.bunifuImageButton1.MouseLeave += new System.EventHandler(this.bunifuImageButton1_MouseLeave);
            // 
            // bunifuDragControl1
            // 
            this.bunifuDragControl1.Fixed = true;
            this.bunifuDragControl1.Horizontal = true;
            this.bunifuDragControl1.TargetControl = this.header;
            this.bunifuDragControl1.Vertical = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.cancelBtn);
            this.panel1.Controls.Add(this.okBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 346);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(397, 60);
            this.panel1.TabIndex = 1;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.BackColor = System.Drawing.Color.White;
            this.cancelBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cancelBtn.BorderRadius = 0;
            this.cancelBtn.ButtonText = "Close";
            this.cancelBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelBtn.DisabledColor = System.Drawing.Color.Gray;
            this.cancelBtn.ForeColor = System.Drawing.Color.DimGray;
            this.cancelBtn.Iconcolor = System.Drawing.Color.Transparent;
            this.cancelBtn.Iconimage = ((System.Drawing.Image)(resources.GetObject("cancelBtn.Iconimage")));
            this.cancelBtn.Iconimage_right = null;
            this.cancelBtn.Iconimage_right_Selected = null;
            this.cancelBtn.Iconimage_Selected = null;
            this.cancelBtn.IconMarginLeft = 0;
            this.cancelBtn.IconMarginRight = 0;
            this.cancelBtn.IconRightVisible = true;
            this.cancelBtn.IconRightZoom = 0D;
            this.cancelBtn.IconVisible = false;
            this.cancelBtn.IconZoom = 90D;
            this.cancelBtn.IsTab = false;
            this.cancelBtn.Location = new System.Drawing.Point(294, 15);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Normalcolor = System.Drawing.Color.White;
            this.cancelBtn.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.cancelBtn.OnHoverTextColor = System.Drawing.Color.DimGray;
            this.cancelBtn.selected = false;
            this.cancelBtn.Size = new System.Drawing.Size(90, 30);
            this.cancelBtn.TabIndex = 0;
            this.cancelBtn.Text = "Close";
            this.cancelBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cancelBtn.Textcolor = System.Drawing.Color.DimGray;
            this.cancelBtn.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // okBtn
            // 
            this.okBtn.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.okBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.okBtn.BorderRadius = 0;
            this.okBtn.ButtonText = "Cập nhật";
            this.okBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.okBtn.DisabledColor = System.Drawing.Color.Gray;
            this.okBtn.Iconcolor = System.Drawing.Color.Transparent;
            this.okBtn.Iconimage = ((System.Drawing.Image)(resources.GetObject("okBtn.Iconimage")));
            this.okBtn.Iconimage_right = null;
            this.okBtn.Iconimage_right_Selected = null;
            this.okBtn.Iconimage_Selected = null;
            this.okBtn.IconMarginLeft = 0;
            this.okBtn.IconMarginRight = 0;
            this.okBtn.IconRightVisible = true;
            this.okBtn.IconRightZoom = 0D;
            this.okBtn.IconVisible = false;
            this.okBtn.IconZoom = 90D;
            this.okBtn.IsTab = false;
            this.okBtn.Location = new System.Drawing.Point(194, 15);
            this.okBtn.Name = "okBtn";
            this.okBtn.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.okBtn.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(109)))), ((int)(((byte)(246)))));
            this.okBtn.OnHoverTextColor = System.Drawing.Color.White;
            this.okBtn.selected = false;
            this.okBtn.Size = new System.Drawing.Size(90, 30);
            this.okBtn.TabIndex = 13;
            this.okBtn.Text = "Cập nhật";
            this.okBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.okBtn.Textcolor = System.Drawing.Color.White;
            this.okBtn.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // bunifuCustomLabel2
            // 
            this.bunifuCustomLabel2.AutoSize = true;
            this.bunifuCustomLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuCustomLabel2.ForeColor = System.Drawing.Color.DimGray;
            this.bunifuCustomLabel2.Location = new System.Drawing.Point(30, 73);
            this.bunifuCustomLabel2.Name = "bunifuCustomLabel2";
            this.bunifuCustomLabel2.Size = new System.Drawing.Size(45, 16);
            this.bunifuCustomLabel2.TabIndex = 2;
            this.bunifuCustomLabel2.Text = "Email:";
            // 
            // bunifuCustomLabel3
            // 
            this.bunifuCustomLabel3.AutoSize = true;
            this.bunifuCustomLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bunifuCustomLabel3.ForeColor = System.Drawing.Color.DimGray;
            this.bunifuCustomLabel3.Location = new System.Drawing.Point(30, 159);
            this.bunifuCustomLabel3.Name = "bunifuCustomLabel3";
            this.bunifuCustomLabel3.Size = new System.Drawing.Size(70, 16);
            this.bunifuCustomLabel3.TabIndex = 2;
            this.bunifuCustomLabel3.Text = "Điện thoại:";
            // 
            // EmailError
            // 
            this.EmailError.AutoSize = true;
            this.EmailError.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmailError.ForeColor = System.Drawing.Color.Red;
            this.EmailError.Location = new System.Drawing.Point(30, 136);
            this.EmailError.Name = "EmailError";
            this.EmailError.Size = new System.Drawing.Size(0, 17);
            this.EmailError.TabIndex = 2;
            // 
            // PhoneError
            // 
            this.PhoneError.AutoSize = true;
            this.PhoneError.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PhoneError.ForeColor = System.Drawing.Color.Red;
            this.PhoneError.Location = new System.Drawing.Point(30, 224);
            this.PhoneError.Name = "PhoneError";
            this.PhoneError.Size = new System.Drawing.Size(0, 17);
            this.PhoneError.TabIndex = 2;
            // 
            // txt
            // 
            this.txt.AutoSize = true;
            this.txt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt.ForeColor = System.Drawing.Color.DimGray;
            this.txt.Location = new System.Drawing.Point(30, 249);
            this.txt.Name = "txt";
            this.txt.Size = new System.Drawing.Size(91, 16);
            this.txt.TabIndex = 2;
            this.txt.Text = "Mã PIN token:";
            // 
            // PinError
            // 
            this.PinError.AutoSize = true;
            this.PinError.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PinError.ForeColor = System.Drawing.Color.Red;
            this.PinError.Location = new System.Drawing.Point(30, 314);
            this.PinError.Name = "PinError";
            this.PinError.Size = new System.Drawing.Size(0, 17);
            this.PinError.TabIndex = 2;
            // 
            // EmailTextBox
            // 
            this.EmailTextBox.BorderColorFocused = System.Drawing.Color.DimGray;
            this.EmailTextBox.BorderColorIdle = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.EmailTextBox.BorderColorMouseHover = System.Drawing.Color.DimGray;
            this.EmailTextBox.BorderThickness = 2;
            this.EmailTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.EmailTextBox.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.EmailTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.EmailTextBox.isPassword = false;
            this.EmailTextBox.Location = new System.Drawing.Point(33, 92);
            this.EmailTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.EmailTextBox.Name = "EmailTextBox";
            this.EmailTextBox.Size = new System.Drawing.Size(329, 40);
            this.EmailTextBox.TabIndex = 10;
            this.EmailTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.EmailTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EmailTextBox_KeyDown);
            // 
            // PhoneTextBox
            // 
            this.PhoneTextBox.BorderColorFocused = System.Drawing.Color.DimGray;
            this.PhoneTextBox.BorderColorIdle = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PhoneTextBox.BorderColorMouseHover = System.Drawing.Color.DimGray;
            this.PhoneTextBox.BorderThickness = 2;
            this.PhoneTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.PhoneTextBox.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.PhoneTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PhoneTextBox.isPassword = false;
            this.PhoneTextBox.Location = new System.Drawing.Point(33, 180);
            this.PhoneTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.PhoneTextBox.Name = "PhoneTextBox";
            this.PhoneTextBox.Size = new System.Drawing.Size(329, 40);
            this.PhoneTextBox.TabIndex = 11;
            this.PhoneTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.PhoneTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PhoneTextBox_KeyDown);
            // 
            // PinTxt
            // 
            this.PinTxt.BorderColorFocused = System.Drawing.Color.DimGray;
            this.PinTxt.BorderColorIdle = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PinTxt.BorderColorMouseHover = System.Drawing.Color.DimGray;
            this.PinTxt.BorderThickness = 2;
            this.PinTxt.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.PinTxt.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.PinTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PinTxt.isPassword = true;
            this.PinTxt.Location = new System.Drawing.Point(33, 270);
            this.PinTxt.Margin = new System.Windows.Forms.Padding(4);
            this.PinTxt.Name = "PinTxt";
            this.PinTxt.Size = new System.Drawing.Size(329, 40);
            this.PinTxt.TabIndex = 12;
            this.PinTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.PinTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PinTxt_KeyDown);
            // 
            // bunifuDragControl2
            // 
            this.bunifuDragControl2.Fixed = true;
            this.bunifuDragControl2.Horizontal = true;
            this.bunifuDragControl2.TargetControl = this.title;
            this.bunifuDragControl2.Vertical = true;
            // 
            // UpdateProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(397, 406);
            this.Controls.Add(this.PinTxt);
            this.Controls.Add(this.PhoneTextBox);
            this.Controls.Add(this.EmailTextBox);
            this.Controls.Add(this.txt);
            this.Controls.Add(this.bunifuCustomLabel3);
            this.Controls.Add(this.PinError);
            this.Controls.Add(this.PhoneError);
            this.Controls.Add(this.EmailError);
            this.Controls.Add(this.bunifuCustomLabel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "UpdateProfile";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UpdateProfile";
            this.header.ResumeLayout(false);
            this.header.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel header;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl1;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
        private Bunifu.Framework.UI.BunifuCustomLabel title;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton2;
        private System.Windows.Forms.Panel panel1;
        private Bunifu.Framework.UI.BunifuFlatButton okBtn;
        private Bunifu.Framework.UI.BunifuFlatButton cancelBtn;
        private Bunifu.Framework.UI.BunifuCustomLabel bunifuCustomLabel2;
        private Bunifu.Framework.UI.BunifuCustomLabel bunifuCustomLabel3;
        private Bunifu.Framework.UI.BunifuCustomLabel EmailError;
        private Bunifu.Framework.UI.BunifuCustomLabel PhoneError;
        private Bunifu.Framework.UI.BunifuCustomLabel txt;
        private Bunifu.Framework.UI.BunifuCustomLabel PinError;
        private Bunifu.Framework.UI.BunifuMetroTextbox PinTxt;
        private Bunifu.Framework.UI.BunifuMetroTextbox PhoneTextBox;
        private Bunifu.Framework.UI.BunifuMetroTextbox EmailTextBox;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl2;
    }
}