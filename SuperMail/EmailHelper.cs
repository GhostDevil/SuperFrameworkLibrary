using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SuperFramework.SuperMail
{
    /// <summary>
    /// 电子邮件辅助类
    /// </summary>
    public class EmailHelper
    {
        #region 发送电子邮件
        /// <summary>
        /// 发送电子邮件,所有SMTP配置信息均在config配置文件中system.net节设置.
        /// </summary>
        /// <param name="receiveEmail">接收电子邮件的地址</param>
        /// <param name="msgSubject">电子邮件的标题</param>
        /// <param name="msgBody">电子邮件的正文</param>
        /// <param name="IsEnableSSL">是否开启SSL</param>
        public static bool SendEmail(string receiveEmail, string msgSubject, string msgBody, bool IsEnableSSL)
        {
            //创建电子邮件对象
            MailMessage email = new MailMessage();
            //设置接收人的电子邮件地址
            email.To.Add(receiveEmail);
            //设置邮件的标题
            email.Subject = msgSubject;
            //设置邮件的正文
            email.Body = msgBody;
            //设置邮件为HTML格式
            email.IsBodyHtml = true;
            //创建SMTP客户端，将自动从配置文件中获取SMTP服务器信息
            SmtpClient smtp = new SmtpClient();
            //开启SSL
            smtp.EnableSsl = IsEnableSSL;
            try
            {
                //发送电子邮件
                smtp.Send(email);
                return true;
            }
            catch(Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="toUser">接收人</param>
        /// <param name="subject">主题</param>
        /// <param name="content">邮件内容</param>
        /// <param name="emailUser">发送人</param>
        /// <param name="emailPwd">邮箱密码</param>
        /// <param name="emailService">指定 smtp 服务器地址</param>
        /// <param name="name">昵称</param>
        public void SendEmail(string toUser, string subject, string content,string emailUser,string emailPwd,string emailService,string name)
        {

            string MailFrom = emailUser;
            string MailFromPassword = emailPwd;
            string MailServer = emailService;
            SmtpClient smtp = new SmtpClient() { DeliveryMethod = SmtpDeliveryMethod.Network /*将smtp的出站方式设为 Network*/, EnableSsl = false /*smtp服务器是否启用SSL加密*/, Host = MailServer /*指定 smtp 服务器地址*/, Port = 25, UseDefaultCredentials = true, Credentials = new NetworkCredential(MailFrom, MailFromPassword) }; //实例化一个SmtpClient

            MailAddress mAddress = new MailAddress(MailFrom, name);


            MailMessage mm = new MailMessage() { Priority = MailPriority.Normal, /* mm.From = new MailAddress(MailFrom);*/From = mAddress }; //实例化一个邮件类
            mm.To.Add(toUser);
            mm.Subject = subject;
            mm.SubjectEncoding = Encoding.GetEncoding(936);
            mm.IsBodyHtml = true;
            mm.BodyEncoding = Encoding.GetEncoding(936);
            mm.Body = content;
            smtp.Send(mm);
        }
        #endregion

    }
}
