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
            this.SuspendLayout();
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(177, 308);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(510, 130);
            this.runButton.TabIndex = 0;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // heightScalerTextBox
            // 
            this.heightScalerTextBox.Location = new System.Drawing.Point(555, 170);
            this.heightScalerTextBox.Name = "heightScalerTextBox";
            this.heightScalerTextBox.Size = new System.Drawing.Size(100, 26);
            this.heightScalerTextBox.TabIndex = 1;
            this.heightScalerTextBox.Text = "2000";
            // 
            // closestXTextBox
            // 
            this.closestXTextBox.Location = new System.Drawing.Point(555, 225);
            this.closestXTextBox.Name = "closestXTextBox";
            this.closestXTextBox.Size = new System.Drawing.Size(100, 26);
            this.closestXTextBox.TabIndex = 2;
            this.closestXTextBox.Text = "48";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(411, 173);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Z Scaling Factor:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(459, 225);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Closest X:";
            // 
            // OSM_TopographyGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 567);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closestXTextBox);
            this.Controls.Add(this.heightScalerTextBox);
            this.Controls.Add(this.runButton);
            this.Name = "OSM_TopographyGenerator";
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
    }
}

