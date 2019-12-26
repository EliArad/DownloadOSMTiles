namespace DownloadOSMTiles
{
    partial class Form1
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
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.cmbZoom = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCreateLon = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbZoom2 = new System.Windows.Forms.ComboBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCreateName = new System.Windows.Forms.TextBox();
            this.txtCreateLat = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblLon = new System.Windows.Forms.Label();
            this.lblLat = new System.Windows.Forms.Label();
            this.lblTileY = new System.Windows.Forms.Label();
            this.lblTileX = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnCreateFromRegion = new System.Windows.Forms.Button();
            this.txtStartX = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtStartY = new System.Windows.Forms.TextBox();
            this.txtDownloadCount = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lblPixelY = new System.Windows.Forms.Label();
            this.lblPixelX = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.cmbDrawShape = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showXYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showBorderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblMouseXY = new System.Windows.Forms.Label();
            this.txtLocationName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.snapshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapControl1 = new DownloadOSMTiles.MapControl();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(18, 190);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(189, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Create JSON DB";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(18, 231);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(189, 23);
            this.button4.TabIndex = 3;
            this.button4.Text = "Load JSON DB";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // cmbZoom
            // 
            this.cmbZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbZoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZoom.FormattingEnabled = true;
            this.cmbZoom.Location = new System.Drawing.Point(67, 754);
            this.cmbZoom.Name = "cmbZoom";
            this.cmbZoom.Size = new System.Drawing.Size(56, 21);
            this.cmbZoom.TabIndex = 5;
            this.cmbZoom.SelectedIndexChanged += new System.EventHandler(this.cmbZoom_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 757);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Zoom";
            // 
            // txtCreateLon
            // 
            this.txtCreateLon.Location = new System.Drawing.Point(71, 71);
            this.txtCreateLon.Name = "txtCreateLon";
            this.txtCreateLon.Size = new System.Drawing.Size(67, 20);
            this.txtCreateLon.TabIndex = 7;
            this.txtCreateLon.Text = "34.4";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbZoom2);
            this.groupBox1.Controls.Add(this.btnGo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtCreateName);
            this.groupBox1.Controls.Add(this.txtCreateLat);
            this.groupBox1.Controls.Add(this.txtCreateLon);
            this.groupBox1.Location = new System.Drawing.Point(18, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(210, 163);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Zoom";
            // 
            // cmbZoom2
            // 
            this.cmbZoom2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbZoom2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZoom2.FormattingEnabled = true;
            this.cmbZoom2.Location = new System.Drawing.Point(68, 106);
            this.cmbZoom2.Name = "cmbZoom2";
            this.cmbZoom2.Size = new System.Drawing.Size(70, 21);
            this.cmbZoom2.TabIndex = 16;
            this.cmbZoom2.SelectedIndexChanged += new System.EventHandler(this.cmbZoom2_SelectedIndexChanged);
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(144, 45);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(59, 44);
            this.btnGo.TabIndex = 15;
            this.btnGo.Text = "GO";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Lon";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Lat";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Name";
            // 
            // txtCreateName
            // 
            this.txtCreateName.Location = new System.Drawing.Point(71, 19);
            this.txtCreateName.Name = "txtCreateName";
            this.txtCreateName.Size = new System.Drawing.Size(67, 20);
            this.txtCreateName.TabIndex = 9;
            this.txtCreateName.Text = "israel";
            // 
            // txtCreateLat
            // 
            this.txtCreateLat.Location = new System.Drawing.Point(71, 45);
            this.txtCreateLat.Name = "txtCreateLat";
            this.txtCreateLat.Size = new System.Drawing.Size(67, 20);
            this.txtCreateLat.TabIndex = 8;
            this.txtCreateLat.Text = "33.4";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(33, 328);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "lat";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 358);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Lon";
            // 
            // lblLon
            // 
            this.lblLon.AutoSize = true;
            this.lblLon.Location = new System.Drawing.Point(71, 358);
            this.lblLon.Name = "lblLon";
            this.lblLon.Size = new System.Drawing.Size(13, 13);
            this.lblLon.TabIndex = 12;
            this.lblLon.Text = "0";
            // 
            // lblLat
            // 
            this.lblLat.AutoSize = true;
            this.lblLat.Location = new System.Drawing.Point(71, 328);
            this.lblLat.Name = "lblLat";
            this.lblLat.Size = new System.Drawing.Size(13, 13);
            this.lblLat.TabIndex = 11;
            this.lblLat.Text = "0";
            // 
            // lblTileY
            // 
            this.lblTileY.AutoSize = true;
            this.lblTileY.Location = new System.Drawing.Point(71, 419);
            this.lblTileY.Name = "lblTileY";
            this.lblTileY.Size = new System.Drawing.Size(13, 13);
            this.lblTileY.TabIndex = 16;
            this.lblTileY.Text = "0";
            // 
            // lblTileX
            // 
            this.lblTileX.AutoSize = true;
            this.lblTileX.Location = new System.Drawing.Point(71, 389);
            this.lblTileX.Name = "lblTileX";
            this.lblTileX.Size = new System.Drawing.Size(13, 13);
            this.lblTileX.TabIndex = 15;
            this.lblTileX.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(33, 419);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Y";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(33, 389);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(14, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "X";
            // 
            // btnCreateFromRegion
            // 
            this.btnCreateFromRegion.Location = new System.Drawing.Point(12, 577);
            this.btnCreateFromRegion.Name = "btnCreateFromRegion";
            this.btnCreateFromRegion.Size = new System.Drawing.Size(189, 23);
            this.btnCreateFromRegion.TabIndex = 15;
            this.btnCreateFromRegion.Text = "Create For Region";
            this.btnCreateFromRegion.UseVisualStyleBackColor = true;
            this.btnCreateFromRegion.Click += new System.EventHandler(this.btnCreateFromRegion_Click);
            // 
            // txtStartX
            // 
            this.txtStartX.Location = new System.Drawing.Point(12, 551);
            this.txtStartX.Name = "txtStartX";
            this.txtStartX.Size = new System.Drawing.Size(44, 20);
            this.txtStartX.TabIndex = 15;
            this.txtStartX.Text = "304";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 535);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Start X";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(62, 535);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(39, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Start Y";
            // 
            // txtStartY
            // 
            this.txtStartY.Location = new System.Drawing.Point(62, 551);
            this.txtStartY.Name = "txtStartY";
            this.txtStartY.Size = new System.Drawing.Size(44, 20);
            this.txtStartY.TabIndex = 18;
            this.txtStartY.Text = "205";
            // 
            // txtDownloadCount
            // 
            this.txtDownloadCount.Location = new System.Drawing.Point(112, 551);
            this.txtDownloadCount.Name = "txtDownloadCount";
            this.txtDownloadCount.Size = new System.Drawing.Size(44, 20);
            this.txtDownloadCount.TabIndex = 20;
            this.txtDownloadCount.Text = "10";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(109, 535);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Count";
            // 
            // lblPixelY
            // 
            this.lblPixelY.AutoSize = true;
            this.lblPixelY.Location = new System.Drawing.Point(71, 478);
            this.lblPixelY.Name = "lblPixelY";
            this.lblPixelY.Size = new System.Drawing.Size(13, 13);
            this.lblPixelY.TabIndex = 25;
            this.lblPixelY.Text = "0";
            // 
            // lblPixelX
            // 
            this.lblPixelX.AutoSize = true;
            this.lblPixelX.Location = new System.Drawing.Point(71, 448);
            this.lblPixelX.Name = "lblPixelX";
            this.lblPixelX.Size = new System.Drawing.Size(13, 13);
            this.lblPixelX.TabIndex = 24;
            this.lblPixelX.Text = "0";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(33, 478);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(35, 13);
            this.label15.TabIndex = 23;
            this.label15.Text = "pixelY";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(33, 448);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(35, 13);
            this.label16.TabIndex = 22;
            this.label16.Text = "pixelX";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(18, 651);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(129, 23);
            this.button5.TabIndex = 26;
            this.button5.Text = "Create Single Zoom";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 609);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(19, 13);
            this.label13.TabIndex = 27;
            this.label13.Text = "....";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(15, 680);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(108, 28);
            this.button6.TabIndex = 16;
            this.button6.Text = "Stop Download";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button7.Location = new System.Drawing.Point(12, 720);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 28;
            this.button7.Text = "Clear";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // cmbDrawShape
            // 
            this.cmbDrawShape.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbDrawShape.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDrawShape.FormattingEnabled = true;
            this.cmbDrawShape.Items.AddRange(new object[] {
            "None",
            "Line",
            "Circle",
            "Rectangle",
            "Triangle"});
            this.cmbDrawShape.Location = new System.Drawing.Point(104, 720);
            this.cmbDrawShape.Name = "cmbDrawShape";
            this.cmbDrawShape.Size = new System.Drawing.Size(79, 21);
            this.cmbDrawShape.TabIndex = 29;
            this.cmbDrawShape.SelectedIndexChanged += new System.EventHandler(this.cmbDrawShape_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.mapToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1615, 24);
            this.menuStrip1.TabIndex = 30;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.snapshotToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // mapToolStripMenuItem
            // 
            this.mapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showXYToolStripMenuItem,
            this.showBorderToolStripMenuItem});
            this.mapToolStripMenuItem.Name = "mapToolStripMenuItem";
            this.mapToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.mapToolStripMenuItem.Text = "Map";
            // 
            // showXYToolStripMenuItem
            // 
            this.showXYToolStripMenuItem.Name = "showXYToolStripMenuItem";
            this.showXYToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.showXYToolStripMenuItem.Text = "Show XY";
            this.showXYToolStripMenuItem.Click += new System.EventHandler(this.showXYToolStripMenuItem_Click);
            // 
            // showBorderToolStripMenuItem
            // 
            this.showBorderToolStripMenuItem.Name = "showBorderToolStripMenuItem";
            this.showBorderToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.showBorderToolStripMenuItem.Text = "ShowBorder";
            this.showBorderToolStripMenuItem.Click += new System.EventHandler(this.showBorderToolStripMenuItem_Click);
            // 
            // lblMouseXY
            // 
            this.lblMouseXY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMouseXY.AutoSize = true;
            this.lblMouseXY.Location = new System.Drawing.Point(170, 762);
            this.lblMouseXY.Name = "lblMouseXY";
            this.lblMouseXY.Size = new System.Drawing.Size(13, 13);
            this.lblMouseXY.TabIndex = 31;
            this.lblMouseXY.Text = "..";
            // 
            // txtLocationName
            // 
            this.txtLocationName.Location = new System.Drawing.Point(18, 288);
            this.txtLocationName.Name = "txtLocationName";
            this.txtLocationName.Size = new System.Drawing.Size(100, 20);
            this.txtLocationName.TabIndex = 33;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(18, 269);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(86, 13);
            this.label14.TabIndex = 34;
            this.label14.Text = "Location to save";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(124, 286);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(50, 23);
            this.button2.TabIndex = 35;
            this.button2.Text = "Load";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // snapshotToolStripMenuItem
            // 
            this.snapshotToolStripMenuItem.Name = "snapshotToolStripMenuItem";
            this.snapshotToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.snapshotToolStripMenuItem.Text = "Snapshot";
            this.snapshotToolStripMenuItem.Click += new System.EventHandler(this.snapshotToolStripMenuItem_Click);
            // 
            // mapControl1
            // 
            this.mapControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapControl1.BackColor = System.Drawing.Color.LightSkyBlue;
            this.mapControl1.Location = new System.Drawing.Point(234, 3);
            this.mapControl1.Name = "mapControl1";
            this.mapControl1.Size = new System.Drawing.Size(1369, 780);
            this.mapControl1.TabIndex = 4;
            this.mapControl1.DragEnter += new System.Windows.Forms.DragEventHandler(this.mapControl1_DragEnter);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1615, 784);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtLocationName);
            this.Controls.Add(this.lblMouseXY);
            this.Controls.Add(this.cmbDrawShape);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.lblPixelY);
            this.Controls.Add(this.lblPixelX);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtDownloadCount);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtStartY);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtStartX);
            this.Controls.Add(this.btnCreateFromRegion);
            this.Controls.Add(this.lblTileY);
            this.Controls.Add(this.lblTileX);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.lblLon);
            this.Controls.Add(this.lblLat);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbZoom);
            this.Controls.Add(this.mapControl1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Eli\'s Map Control";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private MapControl mapControl1;
        private System.Windows.Forms.ComboBox cmbZoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCreateLon;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtCreateName;
        private System.Windows.Forms.TextBox txtCreateLat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblLon;
        private System.Windows.Forms.Label lblLat;
        private System.Windows.Forms.Label lblTileY;
        private System.Windows.Forms.Label lblTileX;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnCreateFromRegion;
        private System.Windows.Forms.TextBox txtStartX;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtStartY;
        private System.Windows.Forms.TextBox txtDownloadCount;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblPixelY;
        private System.Windows.Forms.Label lblPixelX;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ComboBox cmbDrawShape;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showXYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showBorderToolStripMenuItem;
        private System.Windows.Forms.Label lblMouseXY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbZoom2;
        private System.Windows.Forms.TextBox txtLocationName;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripMenuItem snapshotToolStripMenuItem;
    }
}

