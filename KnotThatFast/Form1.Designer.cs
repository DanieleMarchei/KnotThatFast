namespace KnotThatFast
{
    partial class Form1
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

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.saveImg_btn = new System.Windows.Forms.Button();
            this.step_btn = new System.Windows.Forms.Button();
            this.solve_btn = new System.Windows.Forms.Button();
            this.clear_btn = new System.Windows.Forms.Button();
            this.close_btn = new System.Windows.Forms.Button();
            this.knotCanvas = new KnotThatFast.CustomControllers.KnotCanvas();
            this.SuspendLayout();
            // 
            // saveImg_btn
            // 
            this.saveImg_btn.Location = new System.Drawing.Point(12, 12);
            this.saveImg_btn.Name = "saveImg_btn";
            this.saveImg_btn.Size = new System.Drawing.Size(75, 23);
            this.saveImg_btn.TabIndex = 0;
            this.saveImg_btn.Text = "Save Img";
            this.saveImg_btn.UseVisualStyleBackColor = true;
            this.saveImg_btn.Click += new System.EventHandler(this.saveImg_btn_Click);
            // 
            // step_btn
            // 
            this.step_btn.Location = new System.Drawing.Point(313, 12);
            this.step_btn.Name = "step_btn";
            this.step_btn.Size = new System.Drawing.Size(78, 23);
            this.step_btn.TabIndex = 1;
            this.step_btn.Text = "Step";
            this.step_btn.UseVisualStyleBackColor = true;
            // 
            // solve_btn
            // 
            this.solve_btn.Location = new System.Drawing.Point(394, 12);
            this.solve_btn.Name = "solve_btn";
            this.solve_btn.Size = new System.Drawing.Size(78, 23);
            this.solve_btn.TabIndex = 2;
            this.solve_btn.Text = "Solve";
            this.solve_btn.UseVisualStyleBackColor = true;
            // 
            // clear_btn
            // 
            this.clear_btn.Location = new System.Drawing.Point(145, 12);
            this.clear_btn.Name = "clear_btn";
            this.clear_btn.Size = new System.Drawing.Size(78, 23);
            this.clear_btn.TabIndex = 4;
            this.clear_btn.Text = "Clear";
            this.clear_btn.UseVisualStyleBackColor = true;
            this.clear_btn.Click += new System.EventHandler(this.clear_btn_Click);
            // 
            // close_btn
            // 
            this.close_btn.Location = new System.Drawing.Point(229, 12);
            this.close_btn.Name = "close_btn";
            this.close_btn.Size = new System.Drawing.Size(78, 23);
            this.close_btn.TabIndex = 6;
            this.close_btn.Text = "Close knot";
            this.close_btn.UseVisualStyleBackColor = true;
            this.close_btn.Click += new System.EventHandler(this.close_btn_Click);
            // 
            // knotCanvas
            // 
            this.knotCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.knotCanvas.BackColor = System.Drawing.SystemColors.Control;
            this.knotCanvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.knotCanvas.Location = new System.Drawing.Point(12, 41);
            this.knotCanvas.Name = "knotCanvas";
            this.knotCanvas.Size = new System.Drawing.Size(600, 368);
            this.knotCanvas.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 421);
            this.Controls.Add(this.knotCanvas);
            this.Controls.Add(this.close_btn);
            this.Controls.Add(this.clear_btn);
            this.Controls.Add(this.solve_btn);
            this.Controls.Add(this.step_btn);
            this.Controls.Add(this.saveImg_btn);
            this.MinimumSize = new System.Drawing.Size(640, 460);
            this.Name = "Form1";
            this.Text = "Knot That Fast";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button saveImg_btn;
        private System.Windows.Forms.Button step_btn;
        private System.Windows.Forms.Button solve_btn;
        private System.Windows.Forms.Button clear_btn;
        private System.Windows.Forms.Button close_btn;
        private CustomControllers.KnotCanvas knotCanvas;
    }
}

