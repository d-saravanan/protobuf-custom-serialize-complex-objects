using CPSample.Models;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CPSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        static Dictionary<string, string> _loginCombos = new Dictionary<string, string>
        {
            {"admin","admin" },
            {"john","john" },
            {"smith","smith" },
            {"ryan","ryan" },
            {"borg","borg" },
        };

        public ActionResult Logon(LoginModel model)
        {
            if (_loginCombos.ContainsKey(model.Name) && _loginCombos[model.Name].Equals(model.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,model.Name),
                    new Claim(ClaimTypes.Upn,model.Name),
                    new Claim(ClaimTypes.NameIdentifier,Guid.NewGuid().ToString())
                };
                var id = new ClaimsIdentity(claims, "forms");
                var cp = new ClaimsPrincipal(id);

                var ac = new AuthCodeDetail()
                {
                    ResponseMode = "AwesomeResposne",
                    Principal = cp
                };

                if (!RuntimeTypeModel.Default.IsDefined(typeof(ClaimsPrincipal)))
                    RuntimeTypeModel.Default.Add(typeof(ClaimsPrincipal), false)
                        .SetSurrogate(typeof(CustomPrincipal));

                var strAcd = ProtobufSerializer<AuthCodeDetail>.SerializeToString(ac);
                GenericStaticCache<string>.Add2Cache("cp-cache", strAcd);

                Request.GetOwinContext().Authentication.SignOut("forms");
                Request.GetOwinContext().Authentication.SignIn(id);

                return RedirectToAction("index");
            }

            ModelState.AddModelError("", "failed authentiation");
            return View("Login");
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult HitMe(string data)
        {
            StringBuilder message = TryReadFromtheSerializedData();
            System.Diagnostics.Trace.WriteLine(message);

            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            StringBuilder message = TryReadFromtheSerializedData();
            ViewBag.Message = message.ToString();

            return View();
        }

        private static StringBuilder TryReadFromtheSerializedData()
        {
            StringBuilder message = new StringBuilder();

            var acd = ProtobufSerializer<AuthCodeDetail>.Deserialize(GenericStaticCache<string>.Get("cp-cache"));
            message.AppendLine(acd.ResponseMode);
            foreach (var claim in acd.Principal.Claims)
            {
                message.AppendLine($"{claim.Type} : {claim.Value} - {claim.ValueType}");
            }

            return message;
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    /// <summary>
    /// The protobuf Serializer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProtobufSerializer<T>
    {
        /// <summary>
        /// Serializes the specified object data.
        /// </summary>
        /// <param name="objDataToSerialize">The object data to serialize.</param>
        /// <returns></returns>
        public static byte[] Serialize(T objDataToSerialize)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, objDataToSerialize);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Serializes to base64 encoded string.
        /// </summary>
        /// <param name="objDataToSerialize">The object data to serialize.</param>
        /// <returns></returns>
        public static string SerializeToString(T objDataToSerialize)
        {
            var serializedBytes = Serialize(objDataToSerialize);
            return Convert.ToBase64String(serializedBytes);
        }

        /// <summary>
        /// Deserializes the specified serialized data in bytes.
        /// </summary>
        /// <param name="serializedDataInBytes">The serialized data in bytes.</param>
        /// <returns></returns>
        public static T Deserialize(byte[] serializedDataInBytes)
        {
            using (var deserializableDataStream = new MemoryStream(serializedDataInBytes))
            {
                return Serializer.Deserialize<T>(deserializableDataStream);
            }
        }

        /// <summary>
        /// Deserializes the specified serialized base64 string.
        /// </summary>
        /// <param name="serializedDataAsBase64String">The serialized data as base64 string.</param>
        /// <returns></returns>
        public static T Deserialize(string serializedDataAsBase64String)
        {
            var serializedContents = Convert.FromBase64String(serializedDataAsBase64String);
            return Deserialize(serializedContents);
        }
    }
}