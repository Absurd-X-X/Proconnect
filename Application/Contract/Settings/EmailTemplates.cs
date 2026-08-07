using Newtonsoft.Json.Linq;

namespace Application.Contract.Settings
{

    public static class EmailTemplates
    {
        private const string PrimaryColor = "#2563EB";
        private const string BackgroundColor = "#F3F6FB";
        private const string CardColor = "#FFFFFF";
        private const string TextColor = "#374151";
        private const string MutedColor = "#6B7280";
        private const string DangerColor = "#DC2626";



        private static string BaseTemplate(
            string title,
            string content,
            string? buttonText = null,
            string? buttonUrl = null)
        {
            var button = string.Empty;

            if (!string.IsNullOrWhiteSpace(buttonUrl))
            {
                button = $@"
            <div style='text-align:center;margin:40px 0;'>
                <a href='{buttonUrl}'
                    style='
                        background:{PrimaryColor};
                        color:white;
                        text-decoration:none;
                        padding:15px 35px;
                        border-radius:8px;
                        display:inline-block;
                        font-size:15px;
                        font-weight:bold;
                        font-family:Segoe UI,sans-serif;'>
                    {buttonText}
                </a>
            </div>";
            }

            return $@"
            <!DOCTYPE html>

            <html>

            <head>

            <meta charset='UTF-8'/>

            <meta name='viewport'
                    content='width=device-width, initial-scale=1.0'/>

            </head>

            <body style='
            background:{BackgroundColor};
            margin:0;
            padding:30px;
            font-family:Segoe UI,Arial,sans-serif;'>

            <table
            width='100%'
            cellpadding='0'
            cellspacing='0'>

            <tr>

            <td align='center'>

            <table
            width='650'
            cellpadding='0'
            cellspacing='0'
            style='
            background:{CardColor};
            border-radius:12px;
            overflow:hidden;
            box-shadow:0 5px 18px rgba(0,0,0,.08);'>

            <tr>

            <td
            style='
            background:{PrimaryColor};
            padding:35px;
            text-align:center;'>

            <h1
            style='
            color:white;
            margin:0;
            font-size:34px;'>

            ProConnect

            </h1>

            <p
            style='
            color:#DCE8FF;
            margin-top:10px;
            font-size:15px;'>

            Connect • Network • Grow

            </p>

            </td>

            </tr>

            <tr>

            <td
            style='
            padding:45px;
            color:{TextColor};
            font-size:15px;
            line-height:1.8;'>

            <h2 style='margin-top:0;'>

            {title}

            </h2>

            {content}

            {button}

            </td>

            </tr>

            <tr>

            <td
            style='
            background:#F9FAFB;
            padding:25px;
            text-align:center;
            font-size:13px;
            color:{MutedColor};'>

            <p style='margin:0;'>

            Need help?

            </p>

            <p>

            support@proconnect.com

            </p>

            <p>

            © {DateTime.UtcNow.Year}
            ProConnect

            </p>

            </td>

            </tr>

            </table>

            </td>

            </tr>

            </table>

            </body>

            </html>";
        }


        private static string VerificationCode(string token)
        {
            return $@"
            <div style='text-align:center;margin:35px 0;'>

            <div style='
            display:inline-block;
            background:{PrimaryColor};
            padding:18px 45px;
            border-radius:10px;
            font-size:34px;
            font-weight:bold;
            color:white;
            letter-spacing:10px;'>

            {token}

            </div>

            </div>";
        }

        private static string Warning(string message)
        {
            return $@"
            <p style='
            text-align:center;
            color:{DangerColor};
            font-weight:bold;'>

            ⚠ {message}

            </p>";

        }

        public static string VerificationEmail(
    string username,
    string email,
    string token,
    string frontendUrl)
        {

            var baseUrl = frontendUrl.TrimEnd('/');
            var verificationLink =
                $"{baseUrl}/verify-email.html?email={Uri.EscapeDataString(email)}&token={token}";

            return BaseTemplate(
                "Verify Your ProConnect Account",
                $@"
        <p>Hello <strong>{username}</strong>,</p>
        <p>Thank you for joining ProConnect. Please verify your email address to activate your account.</p>
        <p>Click the button below to complete your verification.</p>
        <p style='font-size:13px;color:#6B7280;'>This verification link expires shortly.</p>
        ",
                "Verify Account",
                verificationLink
            );
        }

        public static string ForgotPasswordEmail(
        string fullName,
        string token,
        string appUrl)
        {
            return BaseTemplate(

                "Reset Your Password",

                $@"

                <p>

                Hello <strong>{fullName}</strong>,

                </p>

                <p>

                We received a request to reset
                your password.

                </p>

                {VerificationCode(token)}

                {Warning("This reset code expires in 5 minutes.")}

                <p>

                If you didn't request this,
                please ignore this email.

                </p>

                ",

                "Open ProConnect",

                appUrl);
        }

        public static string WelcomeEmail(
        string fullName,
        string email,
        string appUrl)
        {


            var baseUrl = appUrl.TrimEnd('/');
            var verificationLink =
                $"{baseUrl}/choose-account-type.html?email={Uri.EscapeDataString(email)}";
            return BaseTemplate(

                "Welcome to ProConnect 🎉",

                $@"

                <p>

                Hello <strong>{fullName}</strong>,

                </p>

                <p>

                Your email has been verified successfully.

                </p>

                <p>

                You're now ready to:

                </p>

                <ul>

                <li>Create your professional profile</li>

                <li>Build your network</li>

                <li>Connect with recruiters</li>

                <li>Apply for jobs</li>

                <li>Share professional updates</li>

                </ul>

                <p>

                Welcome to the ProConnect community!

                </p>

                ",

                "Complete Your Profile",

                verificationLink);
        }

        public static string ConnectionRequestEmail(
    string receiverName,
    string senderName,
    string appUrl)
        {
            return BaseTemplate(

                "New Connection Request 🤝",

                $@"

                <p>
                Hello <strong>{receiverName}</strong>,
                </p>

                <p>
                <strong>{senderName}</strong> wants to connect with you on ProConnect.
                </p>

                <p>
                Grow your professional network by connecting with people,
                companies, and opportunities.
                </p>

                ",

                "View Connection Request",

                appUrl);
        }

        public static string ConnectionAcceptedEmail(
            string userName,
            string acceptedBy,
            string appUrl)
        {
            return BaseTemplate(

                "Connection Accepted 🎉",

                $@"

                <p>
                Hello <strong>{userName}</strong>,
                </p>

                <p>

                <strong>{acceptedBy}</strong>
                accepted your connection request.

                </p>

                <p>

                You can now view their profile,
                send messages, and grow your network.

                </p>

                ",

                "View Profile",

                appUrl);
        }

        public static string NewMessageEmail(
    string receiverName,
    string senderName,
    string preview,
    string appUrl)
        {
            return BaseTemplate(

                "You Have A New Message 💬",

                $@"

                <p>
                Hello <strong>{receiverName}</strong>,
                </p>

                <p>

                <strong>{senderName}</strong>
                sent you a message.

                </p>


                <div style='
                background:#F3F4F6;
                padding:20px;
                border-radius:10px;
                margin:25px 0;'>

                {preview}

                </div>


                <p>

                Log in to ProConnect to continue the conversation.

                </p>

                ",

                "Open Messages",

                appUrl);
        }

        public static string CompanyInvitationEmail(
    string userName,
    string companyName,
    string appUrl)
        {
            return BaseTemplate(

                "Company Invitation 🏢",

                $@"

                <p>
                Hello <strong>{userName}</strong>,
                </p>

                <p>

                You have been invited to join:

                </p>


                <h3 style='text-align:center;'>

                {companyName}

                </h3>


                <p>

                Accept the invitation to become part of
                the company's recruitment team on ProConnect.

                </p>

                ",

                "View Invitation",

                appUrl);
        }

        public static string JobApplicationEmail(
    string applicantName,
    string jobTitle,
    string companyName,
    string appUrl)
        {
            return BaseTemplate(

                "Application Submitted Successfully ✅",

                $@"

                <p>

                Hello <strong>{applicantName}</strong>,

                </p>


                <p>

                Your application for:

                </p>


                <h3 style='text-align:center;'>

                {jobTitle}

                </h3>


                <p>

                at <strong>{companyName}</strong>
                has been submitted successfully.

                </p>


                <p>

                The company will review your application
                and contact you if selected.

                </p>

                ",

                "View Application",

                appUrl);
        }

        public static string ApplicationStatusEmail(
    string applicantName,
    string jobTitle,
    string status,
    string appUrl)
        {
            return BaseTemplate(

                "Application Status Update 📌",

                $@"

                <p>

                Hello <strong>{applicantName}</strong>,

                </p>


                <p>

                Your application for:

                </p>


                <h3 style='text-align:center;'>

                {jobTitle}

                </h3>


                <p>

                Current status:

                </p>


                <h2 style='text-align:center;color:#2563EB;'>

                {status}

                </h2>


                <p>

                Thank you for using ProConnect.

                </p>

                ",

                "View Application",

                appUrl);
        }
    }
}

