//using Microsoft.AspNetCore.Http;
//using ModularSystem.Core.Cryptography;
//using ModularSystem.Core.Security;

//namespace ModularSystem.Web;

//public interface ISessionManager
//{
//    /// <summary>
//    /// Defines if <see cref="IIdentity"/> should be serialized into outgoing <see cref="HttpRequest"/><br/>
//    /// using the <see cref="Decode(HttpResponse)"/> method.
//    /// </summary>
//    bool EnableHttpForwarding { get; }
//    /// <summary>
//    /// Defines the authentication strategy used by the web application. Examples are HTTP header Cookies, HTTP Barear tokens, HTTP query params, etc...
//    /// </summary>
//    /// <param name="context"></param>
//    /// <returns></returns>
//    Task<ISession?> GetSessionAsync(HttpContext context);

//    /// <summary>
//    /// Defines cryptography for algorithm for all the <see cref="EFSession"/> instances.
//    /// </summary>
//    /// <param name="key"></param>
//    /// <returns></returns>
//    IEncrypter GetDataEncrypter(byte[] key);

//    /// <summary>
//    /// Defines encoding strategy to serialize the <see cref="IIdentity"/> into the <see cref="HttpRequest"/>.
//    /// </summary>
//    /// <param name="session"></param>
//    /// <param name="request"></param>
//    /// <returns></returns>
//    void Encode(IIdentity session, HttpRequest request);

//    /// <summary>
//    /// Defines decoding strategy to deserialize the <see cref="IIdentity"/> from the <see cref="HttpRequest"/>.
//    /// </summary>
//    /// <param name="response"></param>
//    /// <returns></returns>
//    IIdentity? Decode(HttpResponse response);
//}