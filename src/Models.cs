namespace UssdBuilder.Models
{
    /// <summary>
    /// Ussd request interface.
    /// </summary> 
    public interface IUssdRequest
    {
        /// <summary>
        /// Unique identifier for a ussd session.
        /// You may change json property name.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Phone number of the ussd dialer.
        /// You may change json property name.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The code dialed.
        /// You may change json property name.
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// User text input.
        /// You may change json property name.
        /// </summary>
        public string Text { get; set; }
    }

    /// <summary>
    /// Default implementation of <see cref='IUssdRequest'/>.
    /// </summary>
    public class UssdRequest : IUssdRequest
    {
        public string SessionId { get; set; }
        public string PhoneNumber { get; set; }
        public string ServiceCode { get; set; }
        public string Text { get; set; }
    }

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
        /// A conditional route matcher for an <see cref='IUssdRequest'/>.
        /// If specified, this route will only work if the condition is met.
        /// </summary>
        public Func<UssdScreen, IUssdRequest, bool> Regx { get; set; }
        /// <summary>
        /// The next ussd screen or handler to go to.
        /// </summary>
        public string Goto { get; set; }
    }

    
    public class UssdServerOption
    {
        /// <summary>
        /// Enables ussd input split. Default is true 
        /// </summary>
        public bool EnableInputSplit { get; set; }
        /// <summary>
        /// Ussd input split separators. Default is ['*', '#'] 
        /// </summary>
        public char[] InputSplitSeparators { get; set; }
    }
}