namespace UssdBuilder.Models
{
    /// <summary>
    /// A standard ussd request.
    /// </summary>
    /// <param name="SessionId">user unique reference per session</param>
    /// <param name="PhoneNumber">user phone number</param>
    /// <param name="ServiceCode">dialed ussd short code</param>
    /// <param name="Text">user input</param>
    public record UssdRequest(string SessionId, string PhoneNumber, string ServiceCode, string Text);
    
    public class UssdResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class UssdScreen
    {
        public UssdScreen Prev { get; set; }
        public string Task { get; set; }
        public string Prompt { get; set; }
        public string Input { get; set; }
        public string Code { get; set; }
    }

    /// <summary>
    /// A route map that passes control to <see cref='Goto'/> if all conditions are met.
    /// </summary>
    public class UssdRoute
    {
        /// <summary>
        /// The ussd code that this route belongs to.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// The previous ussd screen or handler.
        /// </summary>
        public string Prev { get; set; }
        /// <summary>
        /// A condition route matcher for a <see cref='UssdRequest'/> input.
        /// If specified, this route will only work if the condition is met.
        /// </summary>
        public Func<UssdScreen, UssdRequest, bool> Regx { get; set; }
        /// <summary>
        /// The next ussd screen or handler to go to.
        /// </summary>
        public string Goto { get; set; }
    }
}