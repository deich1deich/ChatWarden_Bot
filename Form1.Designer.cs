namespace PeaceDaBoll
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            Chat_TextBox = new TextBox();
            button1 = new Button();
            TextMessage_TextBox = new TextBox();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            button2 = new Button();
            label1 = new Label();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            развернутьToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // Chat_TextBox
            // 
            Chat_TextBox.Location = new Point(30, 25);
            Chat_TextBox.Multiline = true;
            Chat_TextBox.Name = "Chat_TextBox";
            Chat_TextBox.ScrollBars = ScrollBars.Both;
            Chat_TextBox.Size = new Size(556, 425);
            Chat_TextBox.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(488, 456);
            button1.Name = "button1";
            button1.Size = new Size(98, 88);
            button1.TabIndex = 1;
            button1.Text = "Отправить";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnSend_Click;
            // 
            // TextMessage_TextBox
            // 
            TextMessage_TextBox.Location = new Point(30, 456);
            TextMessage_TextBox.Multiline = true;
            TextMessage_TextBox.Name = "TextMessage_TextBox";
            TextMessage_TextBox.Size = new Size(438, 88);
            TextMessage_TextBox.TabIndex = 2;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(30, 559);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(125, 27);
            textBox1.TabIndex = 3;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(606, 25);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(462, 207);
            textBox2.TabIndex = 4;
            textBox2.Text = "Для поддержки в развитии проекта: \r\nСБЕР 4274 3200 5645 0680 \r\nВсе полученные средства уйдут на развитие проекта.";
            // 
            // button2
            // 
            button2.Location = new Point(952, 237);
            button2.Name = "button2";
            button2.Size = new Size(117, 52);
            button2.TabIndex = 5;
            button2.Text = "Применить";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(606, 253);
            label1.Name = "label1";
            label1.Size = new Size(149, 20);
            label1.TabIndex = 6;
            label1.Text = "Прошедшее время: ";
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { развернутьToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(211, 56);
            // 
            // развернутьToolStripMenuItem
            // 
            развернутьToolStripMenuItem.Name = "развернутьToolStripMenuItem";
            развернутьToolStripMenuItem.Size = new Size(210, 24);
            развернутьToolStripMenuItem.Text = "Развернуть";
            развернутьToolStripMenuItem.Click += развернутьToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1081, 612);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(TextMessage_TextBox);
            Controls.Add(button1);
            Controls.Add(Chat_TextBox);
            Name = "Form1";
            Text = "Form1";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Resize += Form1_Resize;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox Chat_TextBox;
        private Button button1;
        private TextBox TextMessage_TextBox;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button button2;
        private Label label1;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem развернутьToolStripMenuItem;
    }
}
