namespace AppDeckAssistant
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("fghfgh");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("bgfbgfb");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("fffff");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("hjkhjkjhk");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panelBackground = new System.Windows.Forms.Panel();
            this.lytMain = new System.Windows.Forms.TableLayoutPanel();
            this.picBanner = new System.Windows.Forms.PictureBox();
            this.listGames = new MetroFramework.Controls.MetroListView();
            this.mainColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lytModify = new System.Windows.Forms.TableLayoutPanel();
            this.labelInfo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolBar1 = new AppDeckAssistant.ToolBar();
            this.btnRemove = new AppDeckAssistant.CustomButton();
            this.btnImport = new AppDeckAssistant.CustomButton();
            this.windowBar1 = new AppDeckAssistant.WindowBar();
            this.panelBackground.SuspendLayout();
            this.lytMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBanner)).BeginInit();
            this.lytModify.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBackground
            // 
            this.panelBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBackground.Controls.Add(this.lytMain);
            this.panelBackground.Location = new System.Drawing.Point(0, 55);
            this.panelBackground.Margin = new System.Windows.Forms.Padding(0);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Padding = new System.Windows.Forms.Padding(8);
            this.panelBackground.Size = new System.Drawing.Size(350, 453);
            this.panelBackground.TabIndex = 100;
            // 
            // lytMain
            // 
            this.lytMain.ColumnCount = 1;
            this.lytMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lytMain.Controls.Add(this.picBanner, 0, 0);
            this.lytMain.Controls.Add(this.listGames, 0, 2);
            this.lytMain.Controls.Add(this.lytModify, 0, 3);
            this.lytMain.Controls.Add(this.label1, 0, 1);
            this.lytMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lytMain.Location = new System.Drawing.Point(8, 8);
            this.lytMain.Margin = new System.Windows.Forms.Padding(0);
            this.lytMain.Name = "lytMain";
            this.lytMain.RowCount = 4;
            this.lytMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.50247F));
            this.lytMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.lytMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 69.49754F));
            this.lytMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.lytMain.Size = new System.Drawing.Size(334, 437);
            this.lytMain.TabIndex = 3;
            // 
            // picBanner
            // 
            this.picBanner.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picBanner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBanner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBanner.Location = new System.Drawing.Point(0, 0);
            this.picBanner.Margin = new System.Windows.Forms.Padding(0);
            this.picBanner.Name = "picBanner";
            this.picBanner.Size = new System.Drawing.Size(334, 118);
            this.picBanner.TabIndex = 0;
            this.picBanner.TabStop = false;
            this.picBanner.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picBanner_MouseDown);
            this.picBanner.MouseEnter += new System.EventHandler(this.picBanner_MouseEnter);
            this.picBanner.MouseLeave += new System.EventHandler(this.picBanner_MouseLeave);
            // 
            // listGames
            // 
            this.listGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mainColumn});
            this.listGames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listGames.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.listGames.FullRowSelect = true;
            this.listGames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listGames.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4});
            this.listGames.Location = new System.Drawing.Point(0, 138);
            this.listGames.Margin = new System.Windows.Forms.Padding(0);
            this.listGames.MultiSelect = false;
            this.listGames.Name = "listGames";
            this.listGames.OwnerDraw = true;
            this.listGames.Size = new System.Drawing.Size(334, 268);
            this.listGames.TabIndex = 3;
            this.listGames.UseCompatibleStateImageBehavior = false;
            this.listGames.UseSelectable = true;
            this.listGames.View = System.Windows.Forms.View.Details;
            this.listGames.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listGames_ItemSelectionChanged);
            // 
            // lytModify
            // 
            this.lytModify.ColumnCount = 2;
            this.lytModify.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.lytModify.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.lytModify.Controls.Add(this.btnRemove, 1, 0);
            this.lytModify.Controls.Add(this.btnImport, 0, 0);
            this.lytModify.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lytModify.Location = new System.Drawing.Point(0, 406);
            this.lytModify.Margin = new System.Windows.Forms.Padding(0);
            this.lytModify.Name = "lytModify";
            this.lytModify.RowCount = 1;
            this.lytModify.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lytModify.Size = new System.Drawing.Size(334, 31);
            this.lytModify.TabIndex = 2;
            // 
            // labelInfo
            // 
            this.labelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelInfo.AutoSize = true;
            this.labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInfo.Location = new System.Drawing.Point(6, 513);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(35, 13);
            this.labelInfo.TabIndex = 101;
            this.labelInfo.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 118);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(334, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Click on the banner to change it";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolBar1
            // 
            this.toolBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.toolBar1.DarkMode = true;
            this.toolBar1.FixedPos = new System.Drawing.Point(0, 30);
            this.toolBar1.FixExtraWidth = false;
            this.toolBar1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.toolBar1.Location = new System.Drawing.Point(0, 30);
            this.toolBar1.Margin = new System.Windows.Forms.Padding(0);
            this.toolBar1.MetroStyle = MetroFramework.MetroColorStyle.Blue;
            this.toolBar1.MetroTheme = MetroFramework.MetroThemeStyle.Dark;
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.Owner = this;
            this.toolBar1.Size = new System.Drawing.Size(350, 25);
            this.toolBar1.TabIndex = 100;
            this.toolBar1.TabStop = false;
            // 
            // btnRemove
            // 
            this.btnRemove.ButtonBackColor = System.Drawing.Color.White;
            this.btnRemove.ButtonBackgroundImage = null;
            this.btnRemove.ButtonBackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRemove.ButtonBorderClickable = false;
            this.btnRemove.ButtonBorderColor = System.Drawing.Color.Black;
            this.btnRemove.ButtonBorderHighlightColor = System.Drawing.Color.Empty;
            this.btnRemove.ButtonBorderWidth = new System.Windows.Forms.Padding(1);
            this.btnRemove.ButtonFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnRemove.ButtonForeColor = System.Drawing.Color.Black;
            this.btnRemove.ButtonHighlight = false;
            this.btnRemove.ButtonHighlightBackColor = System.Drawing.Color.Empty;
            this.btnRemove.ButtonHighlightForeColor = System.Drawing.Color.Empty;
            this.btnRemove.ButtonText = "Remove";
            this.btnRemove.ButtonTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnRemove.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemove.Location = new System.Drawing.Point(167, 0);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(0);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(167, 31);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRemove_MouseDown);
            // 
            // btnImport
            // 
            this.btnImport.ButtonBackColor = System.Drawing.Color.White;
            this.btnImport.ButtonBackgroundImage = null;
            this.btnImport.ButtonBackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnImport.ButtonBorderClickable = false;
            this.btnImport.ButtonBorderColor = System.Drawing.Color.Black;
            this.btnImport.ButtonBorderHighlightColor = System.Drawing.Color.Empty;
            this.btnImport.ButtonBorderWidth = new System.Windows.Forms.Padding(1);
            this.btnImport.ButtonFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnImport.ButtonForeColor = System.Drawing.Color.Black;
            this.btnImport.ButtonHighlight = false;
            this.btnImport.ButtonHighlightBackColor = System.Drawing.Color.Empty;
            this.btnImport.ButtonHighlightForeColor = System.Drawing.Color.Empty;
            this.btnImport.ButtonText = "Import";
            this.btnImport.ButtonTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnImport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImport.Location = new System.Drawing.Point(0, 0);
            this.btnImport.Margin = new System.Windows.Forms.Padding(0);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(167, 31);
            this.btnImport.TabIndex = 0;
            this.btnImport.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnImport_MouseDown);
            // 
            // windowBar1
            // 
            this.windowBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.windowBar1.CloseButton = true;
            this.windowBar1.DarkMode = true;
            this.windowBar1.FixedPos = new System.Drawing.Point(0, 0);
            this.windowBar1.FixExtraWidth = false;
            this.windowBar1.Location = new System.Drawing.Point(0, 0);
            this.windowBar1.Margin = new System.Windows.Forms.Padding(0);
            this.windowBar1.MaximizeButton = true;
            this.windowBar1.MetroStyle = MetroFramework.MetroColorStyle.Blue;
            this.windowBar1.MetroTheme = MetroFramework.MetroThemeStyle.Light;
            this.windowBar1.MinimizeButton = true;
            this.windowBar1.Name = "windowBar1";
            this.windowBar1.Owner = this;
            this.windowBar1.ShowIcon = true;
            this.windowBar1.Size = new System.Drawing.Size(350, 30);
            this.windowBar1.TabIndex = 100;
            this.windowBar1.TabStop = false;
            this.windowBar1.Title = "Title";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 530);
            this.ControlBox = false;
            this.Controls.Add(this.toolBar1);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.panelBackground);
            this.Controls.Add(this.windowBar1);
            this.DisplayHeader = false;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Movable = false;
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(0, 30, 0, 22);
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.panelBackground.ResumeLayout(false);
            this.lytMain.ResumeLayout(false);
            this.lytMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBanner)).EndInit();
            this.lytModify.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private WindowBar windowBar1;
        private ToolBar toolBar1;
        private System.Windows.Forms.Panel panelBackground;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.TableLayoutPanel lytMain;
        private CustomButton btnImport;
        private MetroFramework.Controls.MetroListView listGames;
        private System.Windows.Forms.ColumnHeader mainColumn;
        private System.Windows.Forms.PictureBox picBanner;
        private System.Windows.Forms.TableLayoutPanel lytModify;
        private CustomButton btnRemove;
        private System.Windows.Forms.Label label1;
    }
}

