﻿namespace eStoreClient.Constants
{
    public static class Endpoints
    {
        private static string BaseUri = "http://localhost:5000/api";

        public static string Login = $"{BaseUri}/Members/login";
        public static string Authenticate = $"{BaseUri}/Members/authenticate";
        public static string Authorize = $"{BaseUri}/Members/authorize";

        public static string Members = $"{BaseUri}/Members";
    }
}