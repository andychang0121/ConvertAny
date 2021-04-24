﻿using ConvertAny.Web.Models;

namespace ConvertAny.Web.Helper
{
    public static class RequestDataExtension
    {
        public static bool IsPortsait(this RequestData requestData)
            => requestData.Height > requestData.Width;

        public static bool ValidRequestData(this RequestData requestData)
            => !string.IsNullOrEmpty(requestData.Base64)
               && !string.IsNullOrEmpty(requestData.FileName);
    }
}