namespace KnotThatFast.CustomControllers
{
    partial class KnotCanvas
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.canvas_pic = new System.Windows.Forms.PictureBox();
            this.gaussCode_lbl = new System.Windows.Forms.Label();
            this.gaussCode_txt = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.canvas_pic)).BeginInit();
            this.SuspendLayout();
            // 
            // canvas_pic
            // 
            this.canvas_pic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.canvas_pic.BackColor = System.Drawing.Color.White;
            this.canvas_pic.Location = new System.Drawing.Point(-1, -1);
            this.canvas_pic.Name = "canvas_pic";
            this.canvas_pic.Size = new System.Drawing.Size(331, 215);
            this.canvas_pic.TabIndex = 0;
            this.canvas_pic.TabStop = false;
            this.canvas_pic.MouseClick += new System.Windows.Forms.MouseEventHandler(this.canvas_pic_MouseClick);
            // 
            // gaussCode_lbl
            // 
            this.gaussCode_lbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gaussCode_lbl.AutoSize = true;
            this.gaussCode_lbl.Location = new System.Drawing.Point(3, 224);
            this.gaussCode_lbl.Name = "gaussCode_lbl";
            this.gaussCode_lbl.Size = new System.Drawing.Size(65, 13);
            this.gaussCode_lbl.TabIndex = 1;
            this.gaussCode_lbl.Text = "Gauss Code";
            // 
            // gaussCode_txt
            // 
            this.gaussCode_txt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gaussCode_txt.Location = new System.Drawing.Point(70, 221);
            this.gaussCode_txt.Name = "gaussCode_txt";
            this.gaussCode_txt.ReadOnly = true;
            this.gaussCode_txt.Size = new System.Drawing.Size(245, 20);
            this.gaussCode_txt.TabIndex = 2;
            // 
            // KnotCanvas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.gaussCode_txt);
            this.Controls.Add(this.gaussCode_lbl);
            this.Controls.Add(this.canvas_pic);
            this.Name = "KnotCanvas";
            this.Size = new System.Drawing.Size(329, 247);
            this.Load += new System.EventHandler(this.KnotCanvas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.canvas_pic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox canvas_pic;
        private System.Windows.Forms.Label gaussCode_lbl;
        private System.Windows.Forms.TextBox gaussCode_txt;
    }
}
