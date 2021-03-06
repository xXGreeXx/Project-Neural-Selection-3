﻿namespace Project_Neural_Selection_3
{
    partial class Game
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Game));
            this.canvas = new System.Windows.Forms.PictureBox();
            this.renderTimer = new System.Windows.Forms.Timer(this.components);
            this.physicsTimer = new System.Windows.Forms.Timer(this.components);
            this.creatureStatsCanvas = new System.Windows.Forms.PictureBox();
            this.speedBar = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.creatureStatsCanvas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedBar)).BeginInit();
            this.SuspendLayout();
            // 
            // canvas
            // 
            this.canvas.BackColor = System.Drawing.Color.SlateGray;
            this.canvas.Location = new System.Drawing.Point(0, 0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(1066, 743);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.canvas_Paint);
            this.canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseDown);
            this.canvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseUp);
            // 
            // renderTimer
            // 
            this.renderTimer.Interval = 16;
            this.renderTimer.Tick += new System.EventHandler(this.renderTimer_Tick);
            // 
            // physicsTimer
            // 
            this.physicsTimer.Tick += new System.EventHandler(this.physicsTimer_Tick);
            // 
            // creatureStatsCanvas
            // 
            this.creatureStatsCanvas.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.creatureStatsCanvas.Location = new System.Drawing.Point(1062, 0);
            this.creatureStatsCanvas.Name = "creatureStatsCanvas";
            this.creatureStatsCanvas.Size = new System.Drawing.Size(272, 743);
            this.creatureStatsCanvas.TabIndex = 1;
            this.creatureStatsCanvas.TabStop = false;
            this.creatureStatsCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.creatureStatsCanvas_Paint);
            // 
            // speedBar
            // 
            this.speedBar.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.speedBar.Location = new System.Drawing.Point(1062, 0);
            this.speedBar.Maximum = 100;
            this.speedBar.Minimum = 1;
            this.speedBar.Name = "speedBar";
            this.speedBar.Size = new System.Drawing.Size(272, 45);
            this.speedBar.TabIndex = 2;
            this.speedBar.Value = 20;
            // 
            // Game
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1332, 743);
            this.Controls.Add(this.speedBar);
            this.Controls.Add(this.creatureStatsCanvas);
            this.Controls.Add(this.canvas);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Game";
            this.Text = "Project: Neural Selection 3";
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.creatureStatsCanvas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.Timer renderTimer;
        private System.Windows.Forms.Timer physicsTimer;
        private System.Windows.Forms.PictureBox creatureStatsCanvas;
        private System.Windows.Forms.TrackBar speedBar;
    }
}

