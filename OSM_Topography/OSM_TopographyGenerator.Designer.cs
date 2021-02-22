namespace OSM_Topography
{
    partial class OSM_TopographyGenerator
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
            this.runButton = new System.Windows.Forms.Button();
            this.heightScalerTextBox = new System.Windows.Forms.TextBox();
            this.closestXTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.saveLocTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.runButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runButton.Location = new System.Drawing.Point(16, 126);
            this.runButton.Margin = new System.Windows.Forms.Padding(2);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(381, 76);
            this.runButton.TabIndex = 0;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // heightScalerTextBox
            // 
            this.heightScalerTextBox.Location = new System.Drawing.Point(205, 53);
            this.heightScalerTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.heightScalerTextBox.Name = "heightScalerTextBox";
            this.heightScalerTextBox.Size = new System.Drawing.Size(68, 20);
            this.heightScalerTextBox.TabIndex = 1;
            this.heightScalerTextBox.Text = "2000";
            // 
            // closestXTextBox
            // 
            this.closestXTextBox.Location = new System.Drawing.Point(205, 86);
            this.closestXTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.closestXTextBox.Name = "closestXTextBox";
            this.closestXTextBox.Size = new System.Drawing.Size(68, 20);
            this.closestXTextBox.TabIndex = 2;
            this.closestXTextBox.Text = "48";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 56);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Z Scaling Factor (exaggerates height):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 89);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Closest X (impacts accuracy/runtime):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 20);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Save Location";
            // 
            // saveLocTextBox
            // 
            this.saveLocTextBox.Location = new System.Drawing.Point(92, 17);
            this.saveLocTextBox.Name = "saveLocTextBox";
            this.saveLocTextBox.Size = new System.Drawing.Size(213, 20);
            this.saveLocTextBox.TabIndex = 6;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(310, 13);
            this.browseButton.Margin = new System.Windows.Forms.Padding(2);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(87, 26);
            this.browseButton.TabIndex = 7;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // OSM_TopographyGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(421, 228);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.saveLocTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closestXTextBox);
            this.Controls.Add(this.heightScalerTextBox);
            this.Controls.Add(this.runButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OSM_TopographyGenerator";
            this.Opacity = 0.88D;
            this.ShowIcon = false;
            this.Text = "OSM Topography Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.TextBox heightScalerTextBox;
        private System.Windows.Forms.TextBox closestXTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox saveLocTextBox;
        private System.Windows.Forms.Button browseButton;
    }
}

