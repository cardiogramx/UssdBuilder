// using System.Text.Json.Serialization;
// using UssdBuilder.Models;

// namespace SampleApplication.Models
// {
//     /// <summary>
//     /// My custom <see cref='IUssdRequest'/> implementation.
//     /// </summary>
//     public class CustomRequest : IUssdRequest
//     {
//         [JsonPropertyName("session-property-name-if-different")]
//         public string SessionId { get; set; }

//         [JsonPropertyName("phone-property-name-if-different")]
//         public string PhoneNumber { get; set; }

//         [JsonPropertyName("code-property-name-if-different")]
//         public string ServiceCode { get; set; }
        
//         [JsonPropertyName("text-property-name-if-different")]
//         public string Text { get; set; }

//         // You may add other custom params that are specific to your ussd operator or provider
//         public string CustomParam1 { get; set; } //some custom param 1
//         public string CustomParam2 { get; set; } //some custom param 2
//         public string CustomParam3 { get; set; } //some custom param 3
//     }
// }