namespace PAS.API.Configurations;

public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
    public bool UseDefaultCredentials { get; set; } = false;
    public int TimeoutMilliseconds { get; set; } = 30000;
    public string TemplatesPath { get; set; } = "Templates/Email";
    public bool Enabled { get; set; } = true;
    public string BccAddress { get; set; } = string.Empty;
    public string TestRecipient { get; set; } = string.Empty;
    public string SubjectPrefix { get; set; } = "[ECX PAS] ";
    public EmailTemplates Templates { get; set; } = new();
}

public class EmailTemplates
{
    public string Welcome { get; set; } = "Welcome.html";
    public string PasswordReset { get; set; } = "PasswordReset.html";
    public string ServiceRequestCreated { get; set; } = "ServiceRequestCreated.html";
    public string ServiceRequestApproved { get; set; } = "ServiceRequestApproved.html";
    public string ServiceRequestRejected { get; set; } = "ServiceRequestRejected.html";
    public string ServiceRequestFulfilled { get; set; } = "ServiceRequestFulfilled.html";
    public string PropertyAssigned { get; set; } = "PropertyAssigned.html";
    public string PropertyReturned { get; set; } = "PropertyReturned.html";
    public string LowStockAlert { get; set; } = "LowStockAlert.html";
    public string InspectionScheduled { get; set; } = "InspectionScheduled.html";
    public string DisposalApproved { get; set; } = "DisposalApproved.html";
    public string TransferNotification { get; set; } = "TransferNotification.html";
    public string AnnualInventoryReminder { get; set; } = "AnnualInventoryReminder.html";
}