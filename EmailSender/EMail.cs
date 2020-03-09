namespace EmailSender
{
    public class EMail : IEmail
    {
        public string FromAddress { get; set ; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
