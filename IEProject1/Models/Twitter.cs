using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Extensions;

namespace IEProject1.Models
{
    public class Twitter
    {
        public string GetRequestToken(string key,string secret, string callBackUrl)
        {
            var client = new RestClient("https://api.twitter.com");

            client.Authenticator = OAuth1Authenticator.ForRequestToken(
                key,
                secret,
                callBackUrl
                );

            var request = new RestRequest("/oauth/request_token",Method.POST);
            var response = client.Execute(request);

            var qs = HttpUtility.ParseQueryString(response.Content);
            string oauthToken = qs["oauth_token"];
            string oauthTokenSecret = qs["oauth_token_secret"];

            request = new RestRequest("oauth/authorize?oauth_token=" + oauthToken);

            string url = client.BuildUri(request).ToString();

            return url;
        }

    }
}