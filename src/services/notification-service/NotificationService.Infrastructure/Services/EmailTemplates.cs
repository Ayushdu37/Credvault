namespace NotificationService.Infrastructure.Services
{
    /// <summary>
    /// Generates premium, dark-themed HTML email templates for CredVault notifications.
    /// All templates are inline-styled for maximum email client compatibility.
    /// </summary>
    public static class EmailTemplates
    {
        private static string WrapInLayout(string innerContent)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>CredVault Notification</title>
</head>
<body style=""margin:0;padding:0;background-color:#0f0f0f;font-family:'Segoe UI',Roboto,'Helvetica Neue',Arial,sans-serif;"">
  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#0f0f0f;padding:40px 0;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" width=""600"" cellpadding=""0"" cellspacing=""0"" style=""max-width:600px;width:100%;background-color:#1a1a1a;border-radius:16px;overflow:hidden;border:1px solid #2a2a2a;"">
          
          <!-- Header -->
          <tr>
            <td style=""background:linear-gradient(135deg,#dc2626 0%,#991b1b 100%);padding:32px 40px;text-align:center;"">
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td align=""center"">
                    <div style=""font-size:28px;font-weight:800;color:#ffffff;letter-spacing:-0.5px;"">
                      🔐 CredVault
                    </div>
                    <div style=""font-size:13px;color:rgba(255,255,255,0.8);margin-top:4px;letter-spacing:1.5px;text-transform:uppercase;"">
                      Secure Financial Platform
                    </div>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          
          <!-- Body -->
          <tr>
            <td style=""padding:40px;"">
              {innerContent}
            </td>
          </tr>
          
          <!-- Footer -->
          <tr>
            <td style=""padding:24px 40px;background-color:#141414;border-top:1px solid #2a2a2a;"">
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td align=""center"">
                    <p style=""margin:0 0 8px;font-size:12px;color:#666;line-height:1.6;"">
                      This is an automated notification from CredVault.
                    </p>
                    <p style=""margin:0;font-size:11px;color:#444;line-height:1.6;"">
                      © {DateTime.UtcNow.Year} CredVault • All rights reserved
                    </p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }

        /// <summary>
        /// Generates a premium payment success email.
        /// </summary>
        public static string PaymentSuccess(decimal amount, Guid paymentId)
        {
            var inner = $@"
              <!-- Status Badge -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td align=""center"" style=""padding-bottom:28px;"">
                    <div style=""display:inline-block;background-color:rgba(34,197,94,0.12);border:1px solid rgba(34,197,94,0.3);border-radius:50px;padding:10px 24px;"">
                      <span style=""font-size:14px;font-weight:600;color:#22c55e;letter-spacing:0.5px;"">
                        ✅ PAYMENT SUCCESSFUL
                      </span>
                    </div>
                  </td>
                </tr>
              </table>

              <!-- Amount -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom:28px;"">
                <tr>
                  <td align=""center"">
                    <p style=""margin:0 0 8px;font-size:13px;color:#888;text-transform:uppercase;letter-spacing:1px;"">Amount Paid</p>
                    <p style=""margin:0;font-size:42px;font-weight:800;color:#ffffff;letter-spacing:-1px;"">₹{amount:N2}</p>
                  </td>
                </tr>
              </table>

              <!-- Details Card -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#222;border-radius:12px;border:1px solid #333;margin-bottom:28px;"">
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #333;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#888;"">Payment ID</td>
                        <td align=""right"" style=""font-size:13px;color:#ccc;font-family:monospace;"">{paymentId}</td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #333;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#888;"">Status</td>
                        <td align=""right"">
                          <span style=""font-size:13px;color:#22c55e;font-weight:600;"">● Completed</span>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#888;"">Date</td>
                        <td align=""right"" style=""font-size:13px;color:#ccc;"">{DateTime.UtcNow:MMMM dd, yyyy · hh:mm tt} UTC</td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>

              <!-- Message -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td style=""font-size:14px;color:#aaa;line-height:1.7;text-align:center;"">
                    Your payment has been processed and confirmed. The transaction details are available in your CredVault dashboard.
                  </td>
                </tr>
              </table>";

            return WrapInLayout(inner);
        }

        /// <summary>
        /// Generates a premium payment failure email.
        /// </summary>
        public static string PaymentFailed(decimal amount, Guid paymentId, string reason)
        {
            var inner = $@"
              <!-- Status Badge -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td align=""center"" style=""padding-bottom:28px;"">
                    <div style=""display:inline-block;background-color:rgba(239,68,68,0.12);border:1px solid rgba(239,68,68,0.3);border-radius:50px;padding:10px 24px;"">
                      <span style=""font-size:14px;font-weight:600;color:#ef4444;letter-spacing:0.5px;"">
                        ❌ PAYMENT FAILED
                      </span>
                    </div>
                  </td>
                </tr>
              </table>

              <!-- Amount -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom:28px;"">
                <tr>
                  <td align=""center"">
                    <p style=""margin:0 0 8px;font-size:13px;color:#888;text-transform:uppercase;letter-spacing:1px;"">Attempted Amount</p>
                    <p style=""margin:0;font-size:42px;font-weight:800;color:#ffffff;letter-spacing:-1px;"">₹{amount:N2}</p>
                  </td>
                </tr>
              </table>

              <!-- Details Card -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#222;border-radius:12px;border:1px solid #333;margin-bottom:28px;"">
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #333;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#888;"">Payment ID</td>
                        <td align=""right"" style=""font-size:13px;color:#ccc;font-family:monospace;"">{paymentId}</td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #333;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#888;"">Status</td>
                        <td align=""right"">
                          <span style=""font-size:13px;color:#ef4444;font-weight:600;"">● Failed</span>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #333;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#888;"">Reason</td>
                        <td align=""right"" style=""font-size:13px;color:#f87171;"">{reason}</td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#888;"">Date</td>
                        <td align=""right"" style=""font-size:13px;color:#ccc;"">{DateTime.UtcNow:MMMM dd, yyyy · hh:mm tt} UTC</td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>

              <!-- Warning Box -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom:24px;"">
                <tr>
                  <td style=""background-color:rgba(239,68,68,0.08);border:1px solid rgba(239,68,68,0.2);border-radius:10px;padding:16px 20px;"">
                    <p style=""margin:0;font-size:13px;color:#f87171;line-height:1.6;"">
                      ⚠️ Your payment could not be processed. Please verify your payment details and try again from your CredVault dashboard.
                    </p>
                  </td>
                </tr>
              </table>

              <!-- Message -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td style=""font-size:14px;color:#aaa;line-height:1.7;text-align:center;"">
                    If this issue persists, please contact our support team for assistance.
                  </td>
                </tr>
              </table>";

            return WrapInLayout(inner);
        }

        /// <summary>
        /// Generates a premium OTP verification email.
        /// </summary>
        public static string OTPEmail(string otpCode, string purposeLabel)
        {
            var inner = $@"
              <!-- Status Badge -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td align=""center"" style=""padding-bottom:28px;"">
                    <div style=""display:inline-block;background-color:rgba(220,38,38,0.12);border:1px solid rgba(220,38,38,0.3);border-radius:50px;padding:10px 24px;"">
                      <span style=""font-size:14px;font-weight:600;color:#dc2626;letter-spacing:0.5px;"">
                        🔐 {purposeLabel.ToUpper()}
                      </span>
                    </div>
                  </td>
                </tr>
              </table>

              <!-- Message -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom:28px;"">
                <tr>
                  <td align=""center"">
                    <p style=""margin:0 0 8px;font-size:15px;color:#ccc;line-height:1.7;"">
                      Use the following code to complete your {purposeLabel.ToLower()}. This code will expire in <strong style=""color:#fff;"">10 minutes</strong>.
                    </p>
                  </td>
                </tr>
              </table>

              <!-- OTP Code Box -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom:28px;"">
                <tr>
                  <td align=""center"">
                    <div style=""background-color:#222;border:2px dashed #dc2626;border-radius:16px;padding:28px 40px;display:inline-block;"">
                      <span style=""font-size:48px;font-weight:800;color:#ffffff;letter-spacing:12px;font-family:'Courier New',monospace;"">{otpCode}</span>
                    </div>
                  </td>
                </tr>
              </table>

              <!-- Details Card -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#222;border-radius:12px;border:1px solid #333;margin-bottom:28px;"">
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #333;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#888;"">Purpose</td>
                        <td align=""right"" style=""font-size:13px;color:#ccc;font-weight:600;"">{purposeLabel}</td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #333;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#888;"">Valid For</td>
                        <td align=""right"" style=""font-size:13px;color:#f87171;font-weight:600;"">10 Minutes</td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#888;"">Requested At</td>
                        <td align=""right"" style=""font-size:13px;color:#ccc;"">{DateTime.UtcNow:MMMM dd, yyyy · hh:mm tt} UTC</td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>

              <!-- Security Warning -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td style=""background-color:rgba(239,68,68,0.08);border:1px solid rgba(239,68,68,0.2);border-radius:10px;padding:16px 20px;"">
                    <p style=""margin:0;font-size:12px;color:#f87171;line-height:1.6;"">
                      ⚠️ If you did not request this code, please ignore this email. Never share your OTP with anyone. CredVault will never ask for your OTP via phone or chat.
                    </p>
                  </td>
                </tr>
              </table>";

            return WrapInLayout(inner);
        }

        /// <summary>
        /// Generates a premium welcome email for newly registered users.
        /// </summary>
        public static string WelcomeEmail(string fullName)
        {
            var inner = $@"
              <!-- Welcome Badge -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td align=""center"" style=""padding-bottom:28px;"">
                    <div style=""display:inline-block;background-color:rgba(34,197,94,0.12);border:1px solid rgba(34,197,94,0.3);border-radius:50px;padding:10px 24px;"">
                      <span style=""font-size:14px;font-weight:600;color:#22c55e;letter-spacing:0.5px;"">
                        🎉 ACCOUNT CREATED SUCCESSFULLY
                      </span>
                    </div>
                  </td>
                </tr>
              </table>

              <!-- Greeting -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom:28px;"">
                <tr>
                  <td align=""center"">
                    <p style=""margin:0 0 6px;font-size:28px;font-weight:800;color:#ffffff;"">
                      Welcome, {fullName}!
                    </p>
                    <p style=""margin:0;font-size:15px;color:#aaa;line-height:1.7;"">
                      Your CredVault account has been created. You're now part of a secure financial platform built for control and peace of mind.
                    </p>
                  </td>
                </tr>
              </table>

              <!-- What's Next Card -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#222;border-radius:12px;border:1px solid #333;margin-bottom:28px;"">
                <tr>
                  <td style=""padding:24px;border-bottom:1px solid #333;"">
                    <p style=""margin:0 0 4px;font-size:16px;font-weight:700;color:#ffffff;"">📋 What's Next?</p>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #333;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#ccc;line-height:1.6;"">
                          <span style=""color:#dc2626;font-weight:700;margin-right:8px;"">1.</span>
                          <strong style=""color:#fff;"">Verify your email</strong> — check for the OTP we sent
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #333;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#ccc;line-height:1.6;"">
                          <span style=""color:#dc2626;font-weight:700;margin-right:8px;"">2.</span>
                          <strong style=""color:#fff;"">Link your cards</strong> — start managing your finances
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""font-size:13px;color:#ccc;line-height:1.6;"">
                          <span style=""color:#dc2626;font-weight:700;margin-right:8px;"">3.</span>
                          <strong style=""color:#fff;"">Set up notifications</strong> — stay on top of every transaction
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>

              <!-- Footer Message -->
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td style=""font-size:14px;color:#aaa;line-height:1.7;text-align:center;"">
                    We're glad to have you. If you have any questions, our support team is always here to help.
                  </td>
                </tr>
              </table>";

            return WrapInLayout(inner);
        }
    }
}

