using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices.Protocols;
using System.Net;

namespace POC_LDAP.Controllers
{
    [ApiController]
    [Route("api/auth/login")]

    public class LdapLoginController : ControllerBase
    {
        private readonly IConfiguration _config;

        public LdapLoginController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [HttpPost]
        [Route("")]
        public bool Login([FromBody] Login body)
            
        {
            var domain = _config.GetValue<string>("MySettings:domain");
            var host = _config.GetValue<int>("MySettings:host");
            var server = _config.GetValue<string>("MySettings:server");
            try
            {
                LdapDirectoryIdentifier identifier = new LdapDirectoryIdentifier(server, host);
                System.DirectoryServices.Protocols.LdapConnection ldapConnection =
                    new System.DirectoryServices.Protocols.LdapConnection(identifier);
                ldapConnection.AuthType = AuthType.Basic;
                ldapConnection.SessionOptions.ProtocolVersion = 3;
                NetworkCredential credential = new NetworkCredential($"uid={body.username},{domain}",
                    body.password);
                ldapConnection.Bind(credential);
                ldapConnection.Dispose();
                return true;
            }
            catch (LdapException e)
            {
                Console.WriteLine("\r\nLOGIN FAILED:\r\n\t" + e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("\r\nEXCEPTION OCURRED:\r\n\t" + e.GetType() + ":" + e.Message);
                return false;
            }
        }
    }
}