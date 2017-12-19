namespace KnotThatFast.CustomControllers
{
    partial class MovablePoint
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
            this.SuspendLayout();
            // 
            // MovablePoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.Name = "MovablePoint";
            this.Size = new System.Drawing.Size(7, 7);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MovablePoint_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MovablePoint_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MovablePoint_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
