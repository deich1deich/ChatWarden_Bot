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
            SuspendLayout();
            // 
            // Chat_TextBox
            // 
            Chat_TextBox.Location = new Point(26, 19);
            Chat_TextBox.Margin = new Padding(3, 2, 3, 2);
            Chat_TextBox.Multiline = true;
            Chat_TextBox.Name = "Chat_TextBox";
            Chat_TextBox.ScrollBars = ScrollBars.Both;
            Chat_TextBox.Size = new Size(487, 320);
            Chat_TextBox.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(427, 342);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(86, 66);
            button1.TabIndex = 1;
            button1.Text = "Отправить";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnSend_Click;
            // 
            // TextMessage_TextBox
            // 
            TextMessage_TextBox.Location = new Point(26, 342);
            TextMessage_TextBox.Margin = new Padding(3, 2, 3, 2);
            TextMessage_TextBox.Multiline = true;
            TextMessage_TextBox.Name = "TextMessage_TextBox";
            TextMessage_TextBox.Size = new Size(384, 67);
            TextMessage_TextBox.TabIndex = 2;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(26, 419);
            textBox1.Margin = new Padding(3, 2, 3, 2);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(110, 23);
            textBox1.TabIndex = 3;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(530, 19);
            textBox2.Margin = new Padding(3, 2, 3, 2);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(405, 156);
            textBox2.TabIndex = 4;
            textBox2.Text = "Для поддержки в развитии проекта: \r\nСБЕР 4274 3200 5645 0680 \r\nВсе полученные средства уйдут на развитие проекта.";
            // 
            // button2
            // 
            button2.Location = new Point(833, 178);
            button2.Margin = new Padding(3, 2, 3, 2);
            button2.Name = "button2";
            button2.Size = new Size(102, 39);
            button2.TabIndex = 5;
            button2.Text = "Применить";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(530, 190);
            label1.Name = "label1";
            label1.Size = new Size(119, 15);
            label1.TabIndex = 6;
            label1.Text = "Прошедшее время: ";
            label1.Click += label1_Click;
            // 
            // notifyIcon1
            // 
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(946, 459);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(TextMessage_TextBox);
            Controls.Add(button1);
            Controls.Add(Chat_TextBox);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Form1";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Resize += Form1_Resize;
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
    }
}
