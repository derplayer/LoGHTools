namespace LoGHTools
{
    partial class MainForm
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
            this.button_Decompress = new System.Windows.Forms.Button();
            this.button_DecompressBatch = new System.Windows.Forms.Button();
            this.button_Pack = new System.Windows.Forms.Button();
            this.button_PackBatch = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_Decompress
            // 
            this.button_Decompress.Location = new System.Drawing.Point(12, 12);
            this.button_Decompress.Name = "button_Decompress";
            this.button_Decompress.Size = new System.Drawing.Size(250, 51);
            this.button_Decompress.TabIndex = 0;
            this.button_Decompress.Text = "Decompress";
            this.button_Decompress.UseVisualStyleBackColor = true;
            this.button_Decompress.Click += new System.EventHandler(this.button_Decompress_Click);
            // 
            // button_DecompressBatch
            // 
            this.button_DecompressBatch.Location = new System.Drawing.Point(268, 13);
            this.button_DecompressBatch.Name = "button_DecompressBatch";
            this.button_DecompressBatch.Size = new System.Drawing.Size(250, 50);
            this.button_DecompressBatch.TabIndex = 1;
            this.button_DecompressBatch.Text = "Decompress (Recrusive)";
            this.button_DecompressBatch.UseVisualStyleBackColor = true;
            // 
            // button_Pack
            // 
            this.button_Pack.Location = new System.Drawing.Point(12, 89);
            this.button_Pack.Name = "button_Pack";
            this.button_Pack.Size = new System.Drawing.Size(250, 48);
            this.button_Pack.TabIndex = 2;
            this.button_Pack.Text = "Pack";
            this.button_Pack.UseVisualStyleBackColor = true;
            this.button_Pack.Click += new System.EventHandler(this.button_Pack_Click);
            // 
            // button_PackBatch
            // 
            this.button_PackBatch.Location = new System.Drawing.Point(268, 89);
            this.button_PackBatch.Name = "button_PackBatch";
            this.button_PackBatch.Size = new System.Drawing.Size(250, 48);
            this.button_PackBatch.TabIndex = 3;
            this.button_PackBatch.Text = "Pack (Recrusive)";
            this.button_PackBatch.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 450);
            this.Controls.Add(this.button_PackBatch);
            this.Controls.Add(this.button_Pack);
            this.Controls.Add(this.button_DecompressBatch);
            this.Controls.Add(this.button_Decompress);
            this.Name = "MainForm";
            this.Text = "LoGHTools";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Decompress;
        private System.Windows.Forms.Button button_DecompressBatch;
        private System.Windows.Forms.Button button_Pack;
        private System.Windows.Forms.Button button_PackBatch;
    }
}

