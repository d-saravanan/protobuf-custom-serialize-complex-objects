using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CPSample.Models
{
    /// <summary>
    /// 
    /// </summary>
    [ProtoContract]
    public class AuthCodeDetail
    {
        /// <summary>
        /// Gets or sets the response mode.
        /// </summary>
        /// <value>
        /// The response mode.
        /// </value>
        [ProtoMember(1)] public string ResponseMode { get; set; }
        /// <summary>
        /// Gets or sets the principal.
        /// </summary>
        /// <value>
        /// The principal.
        /// </value>
        [ProtoMember(2)] public ClaimsPrincipal Principal { get; set; }
    }

    /// <summary>
    /// The custom principal
    /// </summary>
    [ProtoContract]
    public partial class CustomPrincipal
    {
        /// <summary>
        /// Gets or sets the type of the authentication.
        /// </summary>
        /// <value>
        /// The type of the authentication.
        /// </value>
        [ProtoMember(1)] public string AuthenticationType { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
        /// </value>
        [ProtoMember(2)] public bool IsAuthenticated { get; set; }
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        [ProtoMember(3)] public string Label { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [ProtoMember(4)] public string Name { get; set; }
        /// <summary>
        /// Gets or sets the type of the name claim.
        /// </summary>
        /// <value>
        /// The type of the name claim.
        /// </value>
        [ProtoMember(5)] public string NameClaimType { get; set; }
        /// <summary>
        /// Gets or sets the type of the role claim.
        /// </summary>
        /// <value>
        /// The type of the role claim.
        /// </value>
        [ProtoMember(6)] public string RoleClaimType { get; set; }
        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        /// <value>
        /// The claims.
        /// </value>
        [ProtoMember(7)] public Dictionary<string, Tuple<string, string>> Claims { get; set; }
    }

    public partial class CustomPrincipal
    {
        /// <summary>
        /// Performs an implicit conversion from <see cref="ClaimsPrincipal"/> to <see cref="CustomPrincipal"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator CustomPrincipal(ClaimsPrincipal other)
        {
            var cp = new CustomPrincipal();

            if (other == null) { return cp; }

            var identity = other?.Identity as ClaimsIdentity;

            if (identity == null) { return null; }

            cp.AuthenticationType = identity.Name;
            cp.IsAuthenticated = identity.IsAuthenticated;
            cp.Label = identity.Label;
            cp.Name = identity.Name;
            cp.NameClaimType = identity.NameClaimType;
            cp.RoleClaimType = identity.RoleClaimType;
            cp.Claims = new Dictionary<string, Tuple<string, string>>();

            foreach (var claim in other.Claims)
            {
                cp.Claims.Add(claim.Type, new Tuple<string, string>(claim.Value, claim.ValueType));
            }

            return cp;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="CustomPrincipal"/> to <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ClaimsPrincipal(CustomPrincipal other)
        {
            var claimsForIdentity = new List<Claim>();
            if (other.Claims != null)
                claimsForIdentity = other.Claims.Select(x => { return ReConstructClaim(x); }).ToList();

            var claimsIdentity = new ClaimsIdentity(claimsForIdentity, other.AuthenticationType);
            return new ClaimsPrincipal(claimsIdentity);
        }

        /// <summary>
        /// Res the construct claim.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        private static Claim ReConstructClaim(KeyValuePair<string, Tuple<string, string>> x)
        {
            return (string.IsNullOrEmpty(x.Value.Item2) ? new Claim(x.Key, x.Value.Item1) : new Claim(x.Key, x.Value.Item1, x.Value.Item2));
        }
    }
}