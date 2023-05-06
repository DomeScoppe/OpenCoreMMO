﻿using System.Net;

namespace NeoServer.Web.Shared.Exceptions;

public class NeoException : Exception
{
    #region properties

    public HttpStatusCode HttpStatusCode { get; }

    public object CustomData { get; }

    #endregion

    #region constructors

    public NeoException(string message = "", HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError, object customData = null) : base(message)
    {
        HttpStatusCode = httpStatusCode;
        CustomData = customData;
    }

    #endregion
}
