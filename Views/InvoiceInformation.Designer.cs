namespace Transportation_Management_System
{
    partial class InvoiceInformation
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
            this.label1 = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.order = new System.Windows.Forms.Label();
            this.clientName = new System.Windows.Forms.Label();
            this.origin = new System.Windows.Forms.Label();
            this.destination = new System.Windows.Forms.Label();
            this.totalAmount = new System.Windows.Forms.Label();
            this.totalDistance = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(220, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(321, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Omnicorp Shipping Handling Transport Management";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(560, 402);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(660, 402);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 2;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // order
            // 
            this.order.Location = new System.Drawing.Point(248, 68);
            this.order.Name = "order";
            this.order.Size = new System.Drawing.Size(273, 58);
            this.order.TabIndex = 3;
            // 
            // clientName
            // 
            this.clientName.Location = new System.Drawing.Point(248, 135);
            this.clientName.Name = "clientName";
            this.clientName.Size = new System.Drawing.Size(273, 55);
            this.clientName.TabIndex = 4;
            // 
            // origin
            // 
            this.origin.Location = new System.Drawing.Point(248, 201);
            this.origin.Name = "origin";
            this.origin.Size = new System.Drawing.Size(273, 55);
            this.origin.TabIndex = 5;
            // 
            // destination
            // 
            this.destination.Location = new System.Drawing.Point(248, 256);
            this.destination.Name = "destination";
            this.destination.Size = new System.Drawing.Size(273, 55);
            this.destination.TabIndex = 6;
            // 
            // totalAmount
            // 
            this.totalAmount.Location = new System.Drawing.Point(248, 330);
            this.totalAmount.Name = "totalAmount";
            this.totalAmount.Size = new System.Drawing.Size(273, 55);
            this.totalAmount.TabIndex = 7;
            // 
            // totalDistance
            // 
            this.totalDistance.Location = new System.Drawing.Point(248, 386);
            this.totalDistance.Name = "totalDistance";
            this.totalDistance.Size = new System.Drawing.Size(273, 55);
            this.totalDistance.TabIndex = 8;
            // 
            // InvoiceInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.totalDistance);
            this.Controls.Add(this.totalAmount);
            this.Controls.Add(this.destination);
            this.Controls.Add(this.origin);
            this.Controls.Add(this.clientName);
            this.Controls.Add(this.order);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.label1);
            this.Name = "InvoiceInformation";
            this.Text = "InvoiceInformation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Label order;
        private System.Windows.Forms.Label clientName;
        private System.Windows.Forms.Label origin;
        private System.Windows.Forms.Label destination;
        private System.Windows.Forms.Label totalAmount;
        private System.Windows.Forms.Label totalDistance;
    }
}